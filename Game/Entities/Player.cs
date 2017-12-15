using System;
using Game.Enums;

namespace Game.Entities
{
    public class Player
    {
        public readonly User User;

        public byte Id { get; private set; }
        public Team Team { get; private set; }

        public Classes Class { get; private set; }
        public ushort Health { get; set; }
        public ushort Weapon { get; set; }
        public bool Ready { get; set; }
        public bool IsAlive { get; set; }

        public ushort Kills { get; private set; }
        public ushort Heads { get; private set; }
        public ushort Deaths { get; private set; }
        public ushort Flags { get; private set; }
        public ushort Headshots { get; private set; }
        public ushort Assists { get; private set; }
        public short Points { get; private set; }
        public byte ItemsPlanted { get; set; }

        public byte RoundsPlayed { get; set; }
        public byte BombsPlanted { get; set; }
        public byte BombsDefused { get; set; }
        public byte FlagsTaken { get; set; }
        public byte Wins { get; set; }
        public byte Losses { get; set; }

        public byte VehiclesDestroyed { get; set; }
        public long MoneyEarned { get; private set; }
        public long XPEarned { get; private set; }

        public bool CanSpawn { get; private set; }
        public bool RoundWait { get; private set; }
        public bool InLobby { get; set; }
        public bool InVehicle { get; set; }
        public int VehicleId { get; set; }
        public int VehicleSeatId { get; set; }

        public DateTime LastSwitch { get; private set; }
        public int SpawnProtection { get; set; }

        public Player(byte id, User user, Team team)
        {
            this.Id = id;
            this.User = user;
            this.Team = team;
            this.RoundWait = false;
            this.InLobby = true;
            this.CanSpawn = true;
            this.ItemsPlanted = 0;
            this.InVehicle = false;

            Reset();

            IsAlive = true;
            LastSwitch = DateTime.Now.AddSeconds(-5);
            SpawnProtection = 0;
        }

        public void ToggleReady() {
            Ready = !Ready;
        }

        public void Reset() {
            Class = Classes.Engineer;
            Health = 1000;

            Kills = 0;
            Heads = 0;
            Deaths = 0;
            Flags = 0;
            Headshots = 0;
            Assists = 0;
            Points = 0;
            Weapon = 0;
            ItemsPlanted = 0;

            RoundsPlayed = 0;
            BombsPlanted = 0;
            BombsDefused = 0;
            FlagsTaken = 0;
            Wins = 0;
            Losses = 0;
            VehiclesDestroyed = 0;

            MoneyEarned = 0;
            XPEarned = 0;
            Ready = false;
            IsAlive = true;
            InVehicle = false;
            VehicleSeatId = -1;
            VehicleId = -1;
            SpawnProtection = 3000;
        }

        public void Set(byte newSlot, Team newTeam) {
            this.Id = newSlot;
            this.Team = newTeam;
            this.LastSwitch = DateTime.Now;
            User.SetRoom(User.Room, newSlot);
        }

        public void StartGame() {
            InLobby = false;
            IsAlive = true;
            InVehicle = false;
            VehicleId = -1;
            VehicleSeatId = -1;
        }

        public void EndGame() {
            InLobby = false;
            InVehicle = false;
            VehicleSeatId = -1;
            VehicleId = -1;

            if (User.Room.Master == Id)
                Ready = true;

            #region Old dinar and exp calc.
            /*
            try
            {
                // Pre-check
                bool blnHasDoubleUp = (User.Inventory.Get("CC05") != null);

                if (this.User.Room.Mode == Mode.Explosive)              
                    this.Points *= 2;

                if (this.Points < 0)
                    this.Points = 0;

                // Calculate the xp rates
                double expRate = (User.Room.Supermaster) ? 1.05 : 1.0;
                double[] PremiumBonus = new double[] { 0, 0.2, 0.3, 0.5 };
                expRate += PremiumBonus[(byte)User.Premium];
                expRate += (User.Inventory.Get("CD01") != null) ? 0.3 : 0; // 30% EXP UP
                expRate += (User.Inventory.Get("CD02") != null) ? 0.2 : 0; // 20% EXP UP
                expRate += blnHasDoubleUp ? 0.25 : 0; // DOUBLE UP (25%)

                double dinarRate = (User.Room.Supermaster) ? 1.10 : 1.0;
                dinarRate += blnHasDoubleUp ? 0.25 : 0;

                // Grab the global rates.
                double globalXPRate = GameConfig.ExpRate;
                double globalDinarRate = GameConfig.DinarRate;

                // Calculate the earned XP.
                double xPEarned = 20 + (double)(this.Points) * 4 * expRate;
                xPEarned = xPEarned * globalXPRate; // Apply XP Event.

                // Calculate the earned dinar.
                double dinarEarned = 50 + ((double)this.Points * 3) * dinarRate;
                dinarEarned = dinarEarned * globalDinarRate; // Apply XP Event.

                // Convert the earned value.
                this.XPEarned = (long)Math.Ceiling(xPEarned);
                this.MoneyEarned = (long)Math.Ceiling(dinarEarned);

                // Save the earned exp and dinar
                User.EndGame(this.MoneyEarned, this.XPEarned);
            }
            */
            #endregion

            try
            {
                if (this.Points < 0)
                    this.Points = 0;

                //double the points for CQC
                if (User.Room.Mode == Mode.Explosive)
                    this.Points *= 2;

                double _expRate = 1.0;
                double _dinarRate = 1.0;
                double[] PremiumBonus = new double[] { 0, 0.2, 0.3, 0.5 };

  
                //Supermaster: If you are the supermaster os the room itself: +10% dinar + 5% exp
                //If you aren´t, but the room is SP: +5% exp
                bool _isRoomSuperMaster = (User.Inventory.Get("CC02") != null && User.Room.Master == User.RoomSlot && User.Room.Supermaster);
                bool _isSuperMasterRoom = User.Room.Supermaster;

                //Apply the bonus
                _expRate += PremiumBonus[(byte)User.Premium];
                _expRate += (User.Inventory.Get("CD01") != null) ? 0.3 : 0; // 30% EXP UP
                _expRate += (User.Inventory.Get("CD02") != null) ? 0.2 : 0; // 20% EXP UP
                _expRate += (User.Inventory.Get("CC05") != null) ? 0.25 : 0; // DoubleUP

                _dinarRate += (User.Inventory.Get("CE01") != null) ? 0.2 : 0; // 20% Dinar UP
                _dinarRate += (User.Inventory.Get("CE02") != null) ? 0.3 : 0; // 30% Dinar UP
                _dinarRate += (User.Inventory.Get("CC05") != null) ? 0.25 : 0; // DoubleUP

                if (_isSuperMasterRoom)
                    _expRate += 0.05;

                if (_isRoomSuperMaster)
                    _dinarRate += 0.1;

                //Calculate  the XP and Dinars earned
                double _xpEarned = 20 + (double)(this.Points) * 4 * _expRate; // the x4 and +20 is unknown client stuff
                double _dinarEarned = 50 + ((double)this.Points * 3) * _dinarRate; //the +50 and *3 is unknown client stuff

                //Apply the global Rates
                _xpEarned *= Game.GameConfig.ExpRate;
                _dinarEarned *= Game.GameConfig.DinarRate;

                XPEarned = (long)Math.Ceiling(_xpEarned);
                MoneyEarned = (long)Math.Ceiling(_dinarEarned);

                //save the data!
                User.EndGame(MoneyEarned, XPEarned);
            }
            catch
            {
                ServerLogger.Instance.Append(ServerLogger.AlertLevel.BugWarning, String.Concat("Player ", this.User.Displayname, " xp and dinars could not be calculated."));
            }
        }

        public void BackToLobby() {
            InLobby = true;
        }

        public void EndRound() {
            RoundsPlayed += 1;
            User.RoundsPlayed += 1;

            RoundWait = true;
        }

        public void RoundReady() {
            RoundWait = false;
        }

        public void RoundStart() {
            CanSpawn = true;
            Health = 1000;
            IsAlive = true;
            InVehicle = false;
            VehicleId = -1;
            VehicleSeatId = -1;
            ItemsPlanted = 0;
        }

        public void Send(byte[] buffer) {
            if (User != null)
                User.Send(buffer);
        }

        public void Spawn(Enums.Classes Class) {
            Health = 1000;
            IsAlive = true;
            this.Class = Class;
            ItemsPlanted = 0;
            InVehicle = false;
            VehicleSeatId = -1;
            VehicleId = -1;

            if (User.Room.Mode == Mode.Explosive)
                CanSpawn = false;

            SpawnProtection = 3000;
        }

        public void Suicide() {
            AddDeaths();
            this.Points -= 6; 
        }

        public void AddKill(bool head) { 
            this.Kills += 1;
            this.User.Kills += 1;

            if (head) 
            {
                this.Heads += 1;
                this.User.Headshots += 1;
            }     
                this.Points += 5;             
        }

        public void AddDeaths() {
            this.Deaths += 1;
            this.User.Deaths += 1;
            this.Points += 1;
            this.Health = 0;
            this.IsAlive = false;
            this.InVehicle = false;
            VehicleId = -1;
            VehicleSeatId = -1;
        }

        public void AddFakeDeath()
        {
            this.Deaths++;
            this.User.Deaths++;

            this.Points -= 5;
        }

        public void SubtractKill()
        {
            if (this.Kills > 0)
                this.Kills--;
        }

        public void AddAssists(ushort _assists)
        {
            this.Assists += _assists;
            this.Points += 2;
        }

        public void AddFlags()
        {
            this.Flags++;
            this.User.FlagsTaken++;
            this.Points += 7;
        }
        public short GetPoints() {
            return this.Points;
        }

        public void AddPoints(short _points)
        {
            this.Points += _points;
        }
    }
}
