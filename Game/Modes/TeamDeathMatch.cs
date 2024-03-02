using System;
using System.Collections.Generic;
using System.Linq;
using Game.Enums;

namespace Game.Modes {
    class TeamDeathMatch : Objects.GameMode {


        private readonly object _syncObject;
        
        //Global variables
        private ushort[] players;
        private ushort _derbTicks;
        private ushort _niuTicks;

        public TeamDeathMatch()
            : base(2, "TDM") {
            _syncObject = new object();
            players = new ushort[] { 0, 0 };
        }

        public override void Initilize(Entities.Room room) 
        {
            base.Initilize(room);

            Room.DownTick = 3600000;
            _derbTicks = _niuTicks = Constants.TDMTickets[Room.Setting];
            Initilized = true;
            FreezeTick = false;
        }

        public override Enums.Team Winner() 
        {
            if (_derbTicks == _niuTicks)
                return Enums.Team.None;
            else
            {
                if (_derbTicks > _niuTicks)
                    return Enums.Team.Derbaran;
                else
                    return Enums.Team.NIU;
            }
        }

        public override void Process() 
        {
            players[(byte)Team.Derbaran] = (ushort)Room.Players.Select(p => p.Value).Where(p => p.Team == Team.Derbaran).Count();
            players[(byte)Team.NIU] = (ushort)Room.Players.Select(p => p.Value).Where(p => p.Team == Team.NIU).Count();

            if (Room.DownTick <= 0)
                Room.EndGame(Winner());

            if (!GameConfig.AllowStartAlone)
            {
                if (players[0] <= 0)
                    this.Room.EndGame(Team.NIU);

                if (players[1] <= 0)
                    this.Room.EndGame(Team.Derbaran);
            }
        }

        protected override void OnDeath(Entities.Player killer, Entities.Player target)
        {
            if (target.Team == Enums.Team.Derbaran)
                _derbTicks--;
            else
                _niuTicks--;
        }

        public override void OnFlagCapture(Entities.Player Player, sbyte _flagState)
        {
            if (Player.Team == Team.Derbaran)
                _niuTicks--;
            else
                _derbTicks--;
        }

        public override void OnPlayerSuicide(Entities.Player Player)
        {
            if (Player.Team == Team.Derbaran)
                _derbTicks--;
            else
                _niuTicks--;
        }

 

        public override bool IsGoalReached() 
        {
            if (_derbTicks <= 0 || _niuTicks <= 0)
                return true;
            else
                return false;
        }

        public override byte CurrentRoundTeamA() {
            return 0;
        }

        public override byte CurrentRoundTeamB() {
            return 0;
        }

        public override ushort ScoreboardA() {
            return _derbTicks;
        }

        public override ushort ScoreboardB() {
            return _niuTicks;
        }
    }
}
