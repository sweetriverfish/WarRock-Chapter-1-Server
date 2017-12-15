namespace Game.Packets.Internal {
    class Ping : Core.Networking.OutPacket {
        public Ping()
            : base((ushort)Core.Enums.InternalPackets.Ping, Core.Constants.xOrKeyServerReceive) {
            Append(Core.Constants.Error_OK);
            Append(System.DateTime.Now.Ticks);
            Append(0); // Player count
            Append(0); // Room Count
        }
    }
}
