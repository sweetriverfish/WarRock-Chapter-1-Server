using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Handlers {
    class RoomCreation : Networking.PacketHandler {

        public bool SuitableLevel(byte _levelLimit, byte _playerLevel)
        {
            bool validLevel = false;

            switch (_levelLimit)
            {

                case 0: //No level limit
                    validLevel = true;
                    break;

                case 1: //1-10 only

                    if (_playerLevel >= 1 && _playerLevel <= 10)
                        validLevel = true;
                    else
                        validLevel = false;
                    break;

                case 2:     // +11
                    if (_playerLevel >= 11)
                        validLevel = true;
                    else
                        validLevel = false;
                    break;

                case 3:   //+21
                    if (_playerLevel >= 21)
                        validLevel = true;
                    else
                        validLevel = false;
                    break;

                case 4:  // +31
                    if (_playerLevel >= 31)
                        validLevel = true;
                    else
                        validLevel = false;
                    break;

                case 5: //+41
                    if (_playerLevel >= 41)
                        validLevel = true;
                    else
                        validLevel = false;
                    break;

                default:
                    validLevel = false;
                    break;
            }

            return validLevel;
        }

        protected override void Process(Entities.User u) {
            if (u.Authorized) {
                if (u.Room != null) u.Disconnect();

                bool isRoomValid = true;
                // READING OUT THE ROOM DATA //
                string name = GetString(0);
                bool hasPassword = (GetByte(1) > 0);
                string password = GetString(2);
                byte playerCount = GetByte(3);
                byte mapId = GetByte(4); // Ignore this from the client, we will use it server side.
                byte unknown1 = GetByte(5); // Unknown?
                byte unknown2 = GetByte(6); // Unknown?
                byte type = GetByte(7); // type?
                byte levelLimit = GetByte(8); // level limit
                bool friendlyFire = (GetByte(9) > 0); // premium only?
                bool enableVoteKick = (GetByte(10) > 0); // vote kick

                // VALIDATE ROOM NAME //
                if (name.Length == 0 || name.Length > 25) { // Name Length
                    if (name.Length != 27)
                        isRoomValid = false;
                }

                // VALIDATE ROOM PASSWORD //
                if (hasPassword && (password.Length == 0 || password == "NULL")) { // Password Length
                    isRoomValid = false;
                }

                // VALIDATE MAXIMUM PLAYERS //
                byte highestIndex = 0;
                switch (u.Channel) {
                    case Enums.ChannelType.CQC: {
                            highestIndex = 1;
                            break;
                        }
                    case Enums.ChannelType.Urban_Ops: {
                            highestIndex = 3;
                            break;
                        }
                    case Enums.ChannelType.Battle_Group: {
                            highestIndex = 4;
                            break;
                        }
                    default: {
                            highestIndex = 1;
                            break;
                        }
                }

                if (playerCount > highestIndex) {
                    isRoomValid = false;
                }

                if (levelLimit < 0 || levelLimit > 5)
                    isRoomValid = false;

                if (!SuitableLevel(levelLimit, Core.LevelCalculator.GetLevelforExp(u.XP)))
                    isRoomValid = false;
                
                // VALIDATE PREMIUM SETTING //
        //        if (u.Premium == Enums.Premium.Free2Play && premiumOnly) {
         //           isRoomValid = false;
           //     }
                //DARKRAPTOR: PREMIUM ONLY REPLACED FOR FRIENDLYFIRE

                if (!u.Inventory.Itemlist.Contains("CC02") && !enableVoteKick)
                    isRoomValid = false;


                if (isRoomValid) {
                    // FETCH OPEN ID //
                    Objects.Channel channel = Managers.ChannelManager.Instance.Get(u.Channel);
                    int openRoomId = channel.GetOpenRoomID();

                    if (openRoomId >= 0) {
                        Entities.Room room = new Entities.Room(u, (uint)openRoomId, name, hasPassword, password, playerCount, type, levelLimit, friendlyFire, enableVoteKick, false);
                        if (room != null) {
                            // ROOM CREATED SUCCESSFULLY //
                            Managers.ChannelManager.Instance.Get(room.Channel).Add(room);

                            u.Send(new Packets.RoomCreation(room));

                            // SEND THE ROOM UPDATE TO THE LOBBY //
                            byte roomPage = (byte)Math.Floor((decimal)(room.ID / 8));
                            var targetList = Managers.ChannelManager.Instance.Get(room.Channel).Users.Select(n => n.Value).Where(n => n.RoomListPage == roomPage && n.Room == null);
                            if (targetList.Count() > 0) {
                                byte[] outBuffer = new Packets.RoomUpdate(room, false).BuildEncrypted();
                                foreach (Entities.User usr in targetList)
                                    usr.Send(outBuffer);
                            }
                            ServerLogger.Instance.Append(ServerLogger.AlertLevel.Information, String.Concat("Player ", u.Displayname, " created a room"));
                        } else {
                            channel.ForceFreeSlot(openRoomId); // Force the room slot open again.
                            u.Send(new Packets.RoomCreation(Enums.RoomCreationErrors.GenericError));
                        }
                    } else {
                        u.Send(new Packets.RoomCreation(Enums.RoomCreationErrors.MaxiumRoomsExceeded));
                    }
                } else {
                    u.Send(new Packets.RoomCreation(Enums.RoomCreationErrors.GenericError));
                }

            } else {
                u.Disconnect();
            }
        }
    }
}
