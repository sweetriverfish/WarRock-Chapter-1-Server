using System;
using Core.Enums.Internal;
namespace Game.Handlers.Internal {
    class Authorization : Networking.PacketHandler {
        protected override void Process(Networking.ServerClient s) {
            ushort errorCode = GetUShort(0);
            if (s.Authorized) return; // Ignore other packets.
            if (Enum.IsDefined(typeof(AuthorizationErrorCodes), errorCode)) {
                AuthorizationErrorCodes enumErrorCode = (AuthorizationErrorCodes)errorCode;
                switch (enumErrorCode) {

                    case AuthorizationErrorCodes.OK: {
                            s.OnAuthorize(GetByte(1));
                            Console.Title = "AlterEmu - Game Server: " + Config.SERVER_NAME;

                            if (!s.IsFirstConnect) {
                                // We disconnected, sync the server!
                            }

                            break;
                        }

                    case AuthorizationErrorCodes.InvalidKey: {
                            Log.Instance.WriteLine("Error while authorizing: the authorization key didn't match.");
                            s.Disconnect(true);
                            break;
                        }

                    case AuthorizationErrorCodes.Duplicate: {
                            Log.Instance.WriteLine("Error while authorizing: a server with the same ip address is already online.");
                            s.Disconnect(true);
                            break;
                        }

                    case AuthorizationErrorCodes.MaxServersReached: {
                            Log.Instance.WriteLine("Error while authorizing: maximum amount of servers reached.");
                            s.Disconnect(true);
                            break;
                        }

                    case AuthorizationErrorCodes.NameAlreadyUsed: {
                            Log.Instance.WriteLine("Error while authorizing: the server name is already in use.");
                            s.Disconnect(true);
                            break;
                        }

                    default: {
                            Log.Instance.WriteLine(string.Concat("An unknown(", errorCode.ToString("x2"), ") error occured while authorizing the server."));
                            s.Disconnect(true);
                            break;
                        }

                }
            } else {
                // Unknown error
                Log.Instance.WriteLine(string.Concat("An unknown(", errorCode.ToString("x2"), ") error occured while authorizing the server."));
                s.Disconnect(true);
            }
        }
    }
}
