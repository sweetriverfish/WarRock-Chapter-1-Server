using System;
using Authorization.Entities;
using Core.Enums.Internal;

namespace Authorization.Handlers.Internal {
    class PlayerAuthorization : Networking.PacketHandler {
        protected override void Process(Entities.Server s) {
            ushort errorCode = GetUShort(0);

            if (Enum.IsDefined(typeof(PlayerAuthorizationErrorCodes), errorCode)) {
                PlayerAuthorizationErrorCodes enumErrorCode = (PlayerAuthorizationErrorCodes)errorCode;
                uint targetId = GetuInt(1);

                switch (enumErrorCode) {

                    // A new player logs in.
                    case PlayerAuthorizationErrorCodes.Login: {

                            Session session = Managers.SessionManager.Instance.Get(targetId);
                            if (session != null) {
                                if (!session.IsActivated) {
                                    session.Activate((byte)s.ID);

                                    s.Send(new Packets.Internal.PlayerAuthorization(session));
                                    ServerLogger.Instance.Append(ServerLogger.AlertLevel.Information, "Player with ID " + targetId.ToString() + " has been authorized");
                                } else {
                                    s.Send(new Packets.Internal.PlayerAuthorization(PlayerAuthorizationErrorCodes.SessionAlreadyActivated, targetId));
                                    ServerLogger.Instance.Append(ServerLogger.AlertLevel.ServerError, "Player with ID " + targetId.ToString() + " was not authorized because the session is already active");
                                }
                            } else {
                                s.Send(new Packets.Internal.PlayerAuthorization(PlayerAuthorizationErrorCodes.InvalidSession, targetId));
                                ServerLogger.Instance.Append(ServerLogger.AlertLevel.ServerError, "Player with ID " + targetId.ToString() + "rejected because the session is NOT invalid");
                            }

                            break;
                        }

                    // Update the information of a player.
                    //TODO: DARK INVESTIGATE THIS
                    case PlayerAuthorizationErrorCodes.Update: {
                            Session session = Managers.SessionManager.Instance.Get(targetId);
                            if (session != null) {
                                /* Packet Structure v2 
                                 * 
                                 * Connection Time
                                 * Ping
                                 * 
                                 * [Sub Type]
                                 *  - 1 - Update Channel
                                 *  - 2 - Update Session Information
                                 *  - 3 - Update Action
                                 *  - 4 - Update Lobby Room information
                                 *  - 5 - Update Ingame Room information
                                 *  
                                 * [Data Blocks]
                                 *  - [1] - Update Channel
                                 *      - Current channel Id
                                 *      - Channel Slot
                                 *      
                                 *  - [2] - Update Session Information
                                 *      - Session - Kills
                                 *      - Session - Deaths
                                 *      - Session - Xp Earned
                                 *      - Session - Dinar Earned
                                 *      - Session - Dinar Spend
                                 *      
                                 *  - [3] - Update Action
                                 *      - Update Type:
                                 *      
                                 *          [1]: Join Room
                                 *               - Room ID
                                 *               - Room Slot
                                 *               - Room Is Master
                                 *               
                                 *          [2]: Leave Room
                                 *              - Room ID
                                 *              - Room Old Slot
                                 *              - Room Was Master?
                                 *                  - New master slot
                                 *                  
                                 *          [3]: Room Start
                                 *              - Team
                                 *              
                                 *          [4]: Room Stop
                                 *              - Kills
                                 *              - Deaths
                                 *              - Flags
                                 *              - Points
                                 *              - Xp Earned
                                 *              - Dinar Earned
                                 *              - xp Bonusses (%-Name:%-Name)
                                 *              - dinar bonusses (%-Name:%-Name)
                                 *      
                                 *  - [4] - Update Lobby Room information
                                 *      - Update Type:
                                 *      
                                 *          [1]: Switch Side
                                 *               - Room ID
                                 *               - Room Slot
                                 *               - Room Is Master
                                 *               
                                 *          [2]:                
                                 *
                                 *  - [5] - Update Ingame Room information
                                 *      - Update Type:
                                 *      
                                 *          [1]: Score Update (Player Kill/Player Death)
                                 *               - Room ID
                                 *               - Room Kills
                                 *               - Room Deaths
                                 *               - Room Flags
                                 *               - Room Points 
                                 *               
                                 *          [2]: 
                                 */


                            } else {
                                // Force a closure of the connection.
                                s.Send(new Packets.Internal.PlayerAuthorization(PlayerAuthorizationErrorCodes.InvalidSession, targetId));
                            }

                            break;
                        }

                    // A player logs out of the server.
                    case PlayerAuthorizationErrorCodes.Logout: {
                            Session session = Managers.SessionManager.Instance.Get(targetId);
                            if (session != null) {
                                if (session.IsActivated) {
                                    session.End();
                                    ServerLogger.Instance.Append(ServerLogger.AlertLevel.Information, "Player with ID " + targetId.ToString() + " has logged out");
                                }
                            }
                            break;
                        }

                    default: {
                            // Unused.
                            break;
                        }
                }
            } else {
                Console.WriteLine(string.Concat("Unknown PlayerAuthorization error: ", errorCode));
            }
        }
    }
}
