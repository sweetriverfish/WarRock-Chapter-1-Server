namespace Authorization.Packets
{
    class Launcher : Core.Networking.OutPacket
    {
        public Launcher()
            : base((ushort)Enums.Packets.Launcher)
        {
            Append(0); // Format
            Append(0); // Launcher Version
            Append(0); // Updater Version
            Append(0); // Client Version
            Append(0); // Sub Version
            Append(0); // Option
            Append("http://"); // URL
        }
    }
}
