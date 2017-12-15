using System;
using Core.Enums.Internal;

namespace Game.Handlers.Internal {
    class PlayerAuthorization : Networking.PacketHandler {
        protected override void Process(Networking.ServerClient s) {
            ushort errorCode = GetUShort(0);

            if (Enum.IsDefined(typeof(PlayerAuthorizationErrorCodes), errorCode)) {
                PlayerAuthorizationErrorCodes enumErrorCode = (PlayerAuthorizationErrorCodes)errorCode;
                uint targetId = GetuInt(1);

                switch (enumErrorCode) {

                    // A new player logs in.
                    case PlayerAuthorizationErrorCodes.Login: {
                            Entities.User u = Managers.UserManager.Instance.Get(targetId);
                            if (u != null) {
                                uint userId = GetuInt(2);
                                string username = GetString(3);
                                string displayname = GetString(4);
                                byte _accessLevel = GetByte(5);
                                u.OnAuthorize(userId, username, displayname, _accessLevel);
                            }
                            break;
                        }

                    // Update the information of a player.
                    case PlayerAuthorizationErrorCodes.Update: {
                            break;
                        }

                    // A player logs out of the server.
                    case PlayerAuthorizationErrorCodes.Logout: {
                            break;
                        }

                    case PlayerAuthorizationErrorCodes.InvalidSession: {
                            Entities.User u = Managers.UserManager.Instance.Get(targetId);
                            if (u != null) {
                                if (!u.Authorized)
                                    u.Send(new Packets.Authorization(Packets.Authorization.ErrorCodes.NormalProcedure));
                                u.Disconnect();
                            }
                            break;
                        }

                    case PlayerAuthorizationErrorCodes.IvalidMatch: {
                            Entities.User u = Managers.UserManager.Instance.Get(targetId);
                            if (u != null) {
                                u.Send(new Packets.Authorization(Packets.Authorization.ErrorCodes.NormalProcedure));
                                u.Disconnect();
                            }
                            break;
                        }

                    case PlayerAuthorizationErrorCodes.SessionAlreadyActivated: {
                            Entities.User u = Managers.UserManager.Instance.Get(targetId);
                            if (u != null) {
                                u.Send(new Packets.Authorization(Packets.Authorization.ErrorCodes.NormalProcedure));
                                u.Disconnect();
                            }
                            break;
                        }

                    default: {
                            // Unused.
                            break;
                        }
                }
            } else {
                Log.Instance.WriteLine(string.Concat("Unknown PlayerAuthorization error: ", errorCode));
            }
        }
    }
}

