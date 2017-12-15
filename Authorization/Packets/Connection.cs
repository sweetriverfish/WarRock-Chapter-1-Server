namespace Authorization.Packets {
    class Connection : Core.Networking.OutPacket {
        public Connection()
            : base((ushort)Enums.Packets.Connection) {
                Append(new System.Random().Next(111111111, 999999999));
                Append(77);
        }

    }
}
