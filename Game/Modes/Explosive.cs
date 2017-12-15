using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game.Enums;

namespace Game.Modes {
    public class Explosive : Objects.GameMode {

        private byte limitRound;
        private byte currentRound;
        private byte[] teamRounds;

        private ushort[] playersAlive;

        private bool roundActive = false;
        private sbyte bombSide = -1;
        private bool bombPlanted = false;
        private bool bombDefused = false;

        private DateTime roundEnd;

        private readonly object _syncObject;

        public Explosive()
            : base(0, "Explosive") {
            _syncObject = new object();
            currentRound = 0;
            teamRounds = new byte[] { 0, 0 };
            playersAlive = new ushort[] {0,0};
        }

        public override void Initilize(Entities.Room room) {
            base.Initilize(room);
            // Validate based on the room setting here.
            currentRound = 0;
            teamRounds = new byte[] { 0, 0 };
            limitRound = Constants.RoundLimits[Room.Setting];

            Initilized = true;
            PrepareRound(true);
            roundActive = true;

        }

        public override Team Winner() {
            if (teamRounds[(byte)Team.Derbaran] > teamRounds[(byte)Team.NIU])
                return Team.Derbaran;
            else
                return Team.NIU;
        }

        private bool RoundRunning() {
            if (playersAlive[(byte)Team.Derbaran] == 0 && !bombPlanted) return false;   // Deb : DEAD + NO BOMB
            if (playersAlive[(byte)Team.NIU] == 0) return false; // NIU : DEAD
            if (bombPlanted && bombDefused) return false; // Bomb = Defused
            if (Room.DownTick <= 0) return false;

            return true;
        }


        public Team WinningTeam() {
            return ((bombPlanted && bombDefused) || (!bombPlanted && playersAlive[(byte)Team.Derbaran] == 0) || (!bombPlanted && Room.DownTick <= 0)) ? Team.NIU : Team.Derbaran;
        }

        public override void Process() {
            if (!roundActive && PrepareRound(false)) {
                roundActive = true;
            }

            if (roundActive) {
                playersAlive[(byte)Team.Derbaran] = (ushort)Room.Players.Select(p => p.Value).Where(p => p.IsAlive && p.Health > 0 && p.Team == Team.Derbaran).Count();
                playersAlive[(byte)Team.NIU] = (ushort)Room.Players.Select(p => p.Value).Where(p => p.IsAlive && p.Health > 0 && p.Team == Team.NIU).Count();

                if (!RoundRunning()) {
                    Team winner = WinningTeam();
                    if (teamRounds[(byte)winner] < limitRound) {
                        EndRound(winner);
                    } else {
                        Room.EndGame(WinningTeam());
                    }
                
                }
                
            }
        }

        private bool PrepareRound(bool first) {

            TimeSpan tsDiff = DateTime.Now - roundEnd;

            if (tsDiff.TotalMilliseconds >= 5000) {
                FreezeTick = false;

                if (!first)
                    currentRound++;

                Room.ItemsOnGround.Clear();

                if (Room.Players.Values.All(p => !p.RoundWait)) {

                    Room.Players.Values.ToList().ForEach(p => p.RoundStart());

                    Room.UpTick = 0;
                    Room.DownTick = GameConfig.ExplosiveRoundTime;
                    bombPlanted = false;
                    bombDefused = false;
                    bombSide = -1;

                    playersAlive[(byte)Team.Derbaran] = (ushort)Room.Players.Select(p => p.Value).Where(p => p.IsAlive && p.Health > 0 && p.Team == Team.Derbaran).Count();
                    playersAlive[(byte)Team.NIU] = (ushort)Room.Players.Select(p => p.Value).Where(p => p.IsAlive && p.Health > 0 && p.Team == Team.NIU).Count();

                    if (!first) {
                        if (currentRound > 0) {
                            if (playersAlive[0] == 0 || playersAlive[1] == 0) {
                                Team winning = Team.None;

                                if (playersAlive[0] > 0)
                                    winning = Team.Derbaran;
                                else
                                    winning = Team.NIU;

                                Room.EndGame(winning);
                                return false;
                            }
                            Room.Send((new Packets.Mission(Room)).BuildEncrypted());
                            Room.Send((new Packets.StartRound(Room)).BuildEncrypted());
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        protected override void OnDeath(Entities.Player killer, Entities.Player target) {
            //throw new System.NotImplementedException();
        }


        private void EndRound(Team winning) {
            roundActive = false;
            roundEnd = DateTime.Now;
            FreezeTick = true;

            foreach (Entities.Player p in Room.Players.Values)
                p.EndRound();

            if (winning != Team.None) {
                teamRounds[(byte)winning]++;
                Room.Send((new Packets.EndRound(Room, winning)).BuildEncrypted());
            }
        }

        public override bool IsGoalReached() {
            return (teamRounds[0] == limitRound || teamRounds[1] == limitRound);
        }

        private void ProcessKill() {

        }

        public override void HandleExplosives(string[] blocks, Entities.Player p) {
            if (p.IsAlive && p.Health > 0) {
                sbyte byteType = -1;
                try {
                    sbyte.TryParse(blocks[2], out byteType);
                } catch { byteType = -1; }

                switch(byteType) { // TODO: Validate item id.
                    case 0: {

                        if (p.Team == Team.Derbaran) {
                            if (!bombPlanted) {
                                // STATICS //
                                p.BombsPlanted += 1;
                                p.User.BombsPlanted += 1;

                                bombPlanted = true;
                                bombDefused = false;
                                bombSide = sbyte.Parse(blocks[4]);
                                Room.DownTick = GameConfig.BombTime;
                                Room.Send((new Packets.Explosives(blocks)).BuildEncrypted());
                            }
                        } else if (p.Team == Team.NIU) {
                            if (bombPlanted) {
                                // STATICS //
                                p.BombsDefused += 1;
                                p.User.BombsDefused += 1;

                                bombDefused = true;
                                Room.Send((new Packets.Explosives(blocks)).BuildEncrypted());
                            }
                        }
                        
                        break;
                    }
                    case 1: {
                        break;
                    }
                    case 2: //it´s a heavy trooper TMA
                    {
                        if(p.Class == Classes.Heavy)
                            Room.Send((new Packets.Explosives(blocks)).BuildEncrypted());
                        break;
                    }

                    default: {
                        p.User.Disconnect(); // Cheating?
                        break;
                        }
                }
            }
        }

        public override byte CurrentRoundTeamA() {
            return teamRounds[0];
        }

        public override byte CurrentRoundTeamB() {
            return teamRounds[1];
        }

        public override ushort ScoreboardA() {
            return playersAlive[0];
        }

        public override ushort ScoreboardB() {
            return playersAlive[1];
        }

        public override void OnFlagCapture(Entities.Player Player, sbyte _flagState)
        {
            throw new NotImplementedException();
        }

        public override void OnPlayerSuicide(Entities.Player Player)
        {
            throw new NotImplementedException();
        }
    }
}
