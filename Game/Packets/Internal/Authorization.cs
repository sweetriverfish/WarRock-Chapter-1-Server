namespace Game.Packets.Internal {
    class Authorization : Core.Networking.OutPacket {
        public Authorization() 
            : base((ushort)Core.Enums.InternalPackets.Authorization, Core.Constants.xOrKeyServerReceive)  {
            Append(Core.Constants.Error_OK);
            Append(Config.SERVER_KEY);
            //Añado espacio para que in game el servidor salga separado del SERVER codificado por WR
            string _name = Config.SERVER_NAME + " ";
            Append(_name);
            Append(Config.SERVER_IP);
            Append((ushort)Core.Enums.Ports.Game);
            Append((byte)Core.Enums.ServerTypes.Normal);
        }
    }
}
