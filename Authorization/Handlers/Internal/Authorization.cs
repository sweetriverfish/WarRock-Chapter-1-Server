namespace Authorization.Handlers.Internal {
    class Authorization : Networking.PacketHandler {
        protected override void Process(Entities.Server s) {
            uint ErrorCode = GetuInt(0);
            if (ErrorCode == 1) {
                string globalKey = GetString(1);
                string serverName = GetString(2);
                string ipAddress = GetString(3);
                int port = GetInt(4);
                byte type = GetByte(5);

                Core.Enums.ServerTypes enumType = Core.Enums.ServerTypes.Normal;
                if (System.Enum.IsDefined(typeof(Core.Enums.ServerTypes), type)) {
                    enumType = (Core.Enums.ServerTypes)type;
                } else {
                    s.Disconnect(); return;
                }

                byte serverId = Managers.ServerManager.Instance.Add(s, serverName, ipAddress, port, enumType);
                if (serverId > 0) {
                    s.Send(new Packets.Internal.Authorize(serverId));
                    ServerLogger.Instance.Append(ServerLogger.AlertLevel.Information, "Authorized a new server as " + serverId.ToString());
                } else {
                    s.Send(new Packets.Internal.Authorize(Core.Enums.Internal.AuthorizationErrorCodes.MaxServersReached));
                    s.Disconnect();
                    ServerLogger.Instance.Append(ServerLogger.AlertLevel.ServerError, "Rejecting Server " + s.Displayname + ". Limit reached");
                }

            } else {
                s.Disconnect();
            }
        }
    }
}
