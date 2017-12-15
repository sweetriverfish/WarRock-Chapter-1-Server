namespace Game.Packets {
    class GameData : Core.Networking.OutPacket {
        public GameData(string[] blocks)
            : base((ushort)Enums.Packets.GamePacket) {
                for (byte i = 0; i < blocks.Length; i++)
                    Append(blocks[i]);
        }
    }
}
