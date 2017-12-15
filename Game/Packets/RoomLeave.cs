namespace Game.Packets {
    class RoomLeave : Core.Networking.OutPacket {
        public RoomLeave(Entities.User u, byte oldSlot,  Entities.Room r)
            : base((ushort)Enums.Packets.RoomLeave) {
            Append(Core.Constants.Error_OK);
            Append(u.SessionID); // SessionID
            Append(oldSlot); // Position in Room
            Append(r.Players.Count); // Remaining player count
            Append(r.Master); // Master Slot
            Append(u.XP); // XP
            Append(u.Money); // Dinar
        }
    }
}
