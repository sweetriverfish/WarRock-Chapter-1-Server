using Core.Enums.Internal;

namespace Authorization.Packets.Internal {
    class PlayerAuthorization : Core.Networking.OutPacket {

        public PlayerAuthorization()
            : base((ushort)Core.Enums.InternalPackets.PlayerAuthorization, Core.Constants.xOrKeyServerSend) {
            
        }

        public PlayerAuthorization(Entities.Session session)
            : base((ushort)Core.Enums.InternalPackets.PlayerAuthorization, Core.Constants.xOrKeyServerSend) {
                Append((ushort)Core.Enums.Internal.PlayerAuthorizationErrorCodes.Login);
                Append(session.SessionID);
                Append(session.ID);
                Append(session.Name);
                Append(session.Displayname);
                Append(session.AccessLevel);
        }

        public PlayerAuthorization(PlayerAuthorizationErrorCodes errorCode, uint targetId)
            : base((ushort)Core.Enums.InternalPackets.PlayerAuthorization, Core.Constants.xOrKeyServerSend) {
                Append((ushort)errorCode);
                Append(targetId);
        }
    }
}
