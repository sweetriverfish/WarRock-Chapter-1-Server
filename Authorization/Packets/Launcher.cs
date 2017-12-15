namespace Authorization.Packets {
    class Launcher : Core.Networking.OutPacket {
        public Launcher()
            : base((ushort)Enums.Packets.Launcher) {
                Append(AuthConfig.Format); // Format
                Append(AuthConfig.Launcher); // Launcher Version
                Append(AuthConfig.Updater); // Updater Version
                Append(AuthConfig.Client); // Client Version
                Append(AuthConfig.Sub); // Sub Version
                Append(AuthConfig.Option); // Option
                Append(AuthConfig.URL); // URL
        }
    }
}
