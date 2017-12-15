namespace Game.Packets {
    class Explosives : Core.Networking.OutPacket {
        public Explosives(string[] blocks)
            : base((ushort)Enums.Packets.Explosives) {
                Append(Core.Constants.Error_OK);
                for (byte i = 0; i < blocks.Length; i++)
                    Append(blocks[i]);
                Append(0);
        }
    }
}
