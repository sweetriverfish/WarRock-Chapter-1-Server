/* Apr-2007
New Game Mode: Conquest

Conquest is an objective, team-based battle where teams fight for control of a map by capturing key points throughout the map.
At the beginning of a Conquest game, both teams start with a base score which ticks down every second. The only way to slow down the clock is to capture bases and defend them with your life. Teamwork is the key in this mode, as teams will need to work together to advance and overtake their opponents’ key strategic points.
Creating a Conquest room requires a Premium account. All players are able to join public Conquest games created by Premium members.
Note: At the start of the match, all bases are open for capture, with the exception of the enemy’s main base flag. This flag becomes available for capture after three minutes. An in-game timer has been added to display this information. 
 */


using System;
using System.Linq;
using Game.Enums;


namespace Game.Modes
{
    class Conquest : Objects.GameMode
    {
        private readonly object _syncObject;

        //Global variables
        private ushort[] players;
        private ushort _derbTicks;
        private ushort _niuTicks;
        private int _derbFlags;
        private int _niuFlags;
        private long _timeCount;
        private long _mainFlagProtection;
        private int _derbSlowDown;
        private int _niuSlowDown;

        public Conquest()
            :base(3, "Conquest")
        {
            _syncObject = new object();
            players = new ushort[] { 0, 0 };
        }

        public override void Initilize(Entities.Room room)
        {
            base.Initilize(room);
            _derbTicks = _niuTicks = 999;
            _derbFlags = _niuFlags = 1;
            _timeCount = Room.DownTick = 3600000;
            _mainFlagProtection = 300000;
            _derbSlowDown = 0;
            _niuSlowDown = 0;
            Initilized = true;
            FreezeTick = false;

          
        }

        public override Team Winner()
        {
            if(_niuFlags == 0 || _derbFlags == 0) //game ended because of flags
            {
                if (_niuFlags > _derbFlags)
                    return Team.NIU;

                if (_derbFlags > _niuFlags)
                    return Team.Derbaran;

                if (_derbFlags == _niuFlags) //TOOOOOO ODD
                    return Team.None;
            }

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

            if (players[0] <= 0)
                this.Room.EndGame(Team.NIU);

            if (players[1] <= 0)
               this.Room.EndGame(Team.Derbaran);

            if (_timeCount - Room.DownTick == 1000)
            {
                UpdateTicks();
                _timeCount = Room.DownTick;
            }

            if (Room.UpTick == _mainFlagProtection) //main flag can be captured now
                 Array.Clear(Room.SpawnFlags, 0, Room.SpawnFlags.Length);

        }

        private void UpdateTicks()
        {

            if (_niuSlowDown == 0)
            {
                _niuTicks -=2;
                _niuSlowDown = _niuFlags;
            }
            else
                _niuSlowDown--;


            if (_derbSlowDown == 0)
            {
                _derbTicks -=2;
                _derbSlowDown = _derbFlags;
            }
            else
                _derbSlowDown--;
          
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
           if(_flagState == -1) //the flag was neutral, add flag to the capturer team
           {
               if (Player.Team == Team.Derbaran)
                   _derbFlags++;
               else
                   _niuFlags++;
           }
           else    //flag stolen from the other team
           {
               if (Player.Team == Team.Derbaran)
                   _niuFlags--;
               else
                   _derbFlags--;
           }

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

            if (_derbFlags <= 0 || _niuFlags <= 0)
                return true;

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
