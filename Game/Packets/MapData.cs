namespace Game.Packets {
    class MapData : Core.Networking.OutPacket {
        public MapData(Entities.Room r)
            : base((ushort)Enums.Packets.MapData) {
                Append(Core.Constants.Error_OK);

                sbyte[] flags = r.Flags;
                Append(flags.Length);
                for (byte i = 0; i < flags.Length; i++) 
                    Append(flags[i]);
                Append(900); // ?

                lock (r._playerLock) {
                    Append(r.Players.Count);
                    foreach (Entities.Player p in r.Players.Values) {
                        Append(p.Id); // Slot
                        Append(-1); // ?
                        Append(p.Kills); // Kills
                        Append(p.Deaths); // Deaths
                        Append((byte)p.Class); // Class
                        Append(p.Health);
                        Append(-1); // Vehicle ID
                        Append(-1); // Vehicle Seat
                    }
                }

            // TODO: Moved Vehicles :)
                Append(0);
        }
    }
}
