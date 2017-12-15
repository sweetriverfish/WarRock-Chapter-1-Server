namespace Game.Packets {
    class LevelUp : Core.Networking.OutPacket {
        public LevelUp(Entities.User u, uint dinarEarned)
            : base((ushort)Enums.Packets.LevelUp) {
            Append(u.RoomSlot);
            Append(0);
            Append(Core.LevelCalculator.GetLevelforExp(u.XP));
            Append(u.XP);
            Append(dinarEarned);
        }
    }
}
