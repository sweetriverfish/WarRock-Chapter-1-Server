namespace Core.Packets {
    public class Connection : Core.Networking.OutPacket {
        public Connection(byte xOrKey)
            : base((ushort)Enums.Packets.Connection, xOrKey) {
            Append(new System.Random().Next(111111111, 999999999));
            Append(77);
        }
    }
}
