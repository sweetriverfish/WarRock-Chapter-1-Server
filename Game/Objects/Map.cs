using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Objects {
    public class Map {
        public byte Id { get; private set; }
        public string Name { get; private set; }
        public byte[] SpawnFlags { get; private set; }
        public byte FFALimit { get; private set; }
        public byte Flags { get; private set; }
        public Enums.Premium PremiumType { get; private set; }
        public byte VehicleCount { get; private set; }

        public Map(byte id, string name, byte FFACount, byte flags, string flagState, byte premiumType, byte _vehicleCount) {
            this.Id = id;
            this.Name = name;
            this.FFALimit = FFACount;
            this.Flags = flags;
            this.VehicleCount = _vehicleCount;
            string[] flagsState = flagState.Split('-');
            this.SpawnFlags = new byte[] { byte.Parse(flagsState[0]), byte.Parse(flagsState[1]) };
            this.PremiumType = (Enums.Premium)premiumType;
        }

        public byte GetSpawnFlag(Enums.Team team) {
            return this.SpawnFlags[(byte)team];
        }
    }
}
