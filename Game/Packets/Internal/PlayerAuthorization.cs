using Core.Enums.Internal;

namespace Game.Packets.Internal {
    class PlayerAuthorization : Core.Networking.OutPacket {
        public PlayerAuthorization(uint sessionId, uint id, string name)
            : base((ushort)Core.Enums.InternalPackets.PlayerAuthorization, Core.Constants.xOrKeyServerReceive) {
                Append((ushort)PlayerAuthorizationErrorCodes.Login);
                Append(sessionId);
                Append(id);
                Append(name);
        }
        public PlayerAuthorization(uint sessionId, uint id, string name, byte _accessLevel)
            : base((ushort)Core.Enums.InternalPackets.PlayerAuthorization, Core.Constants.xOrKeyServerReceive)
        {
            Append((ushort)PlayerAuthorizationErrorCodes.Login);
            Append(sessionId);
            Append(id);
            Append(name);
            Append(_accessLevel);
        }

        public PlayerAuthorization(uint id)
            : base((ushort)Core.Enums.InternalPackets.PlayerAuthorization, Core.Constants.xOrKeyServerReceive) {
            Append((ushort)PlayerAuthorizationErrorCodes.Logout);
            Append(id);
        }

        public PlayerAuthorization(Entities.User u)
            : base((ushort)Core.Enums.InternalPackets.PlayerAuthorization, Core.Constants.xOrKeyServerReceive) {
            Append((ushort)PlayerAuthorizationErrorCodes.Update);
            Append(u.ID);

            
        }
    }
}
