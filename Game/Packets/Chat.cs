namespace Game.Packets {
    class Chat : Core.Networking.OutPacket {
        public Chat(Entities.User u, Enums.ChatType type, string message, int targetId, string targetName)
            : base((ushort)Enums.Packets.Chat) {
            Append(Core.Constants.Error_OK);
            Append(u.SessionID);
            Append(u.Displayname);
            Append((byte)type);
            Append(targetId);
            Append(targetName);
            Append(message);
        }

        public Chat(string name, Enums.ChatType type, string message, int targetId, string targetName)
            : base((ushort)Enums.Packets.Chat) {
            Append(Core.Constants.Error_OK);
            Append(0);
            Append(name);
            Append((byte)type);
            Append(targetId);
            Append(targetName);
            Append(message);
        }
        public Chat(int _error, string targetName)
            : base((ushort)Enums.Packets.Chat)
        {
            targetName = targetName + " ";
            Append(_error);
            Append(targetName);

        }
        
    }
}
