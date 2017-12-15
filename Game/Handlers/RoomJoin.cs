using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Handlers {
    class RoomJoin : Networking.PacketHandler {
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
                if (u.Room == null) {
                    Objects.Channel channel = Managers.ChannelManager.Instance.Get(u.Channel);

                    uint roomId = GetuInt(0);
                    string roomPassword = GetString(1);

                    if (channel.Rooms.ContainsKey(roomId)) {
                        Entities.Room r = null;
                        if (channel.Rooms.TryGetValue(roomId, out r)) {
                            bool validPassword = true;
                            bool validLevel = true;

                            if (r.HasPassword && r.Password != roomPassword) {
                                validPassword = false;
                            }

                            if (!SuitableLevel(r.LevelLimit, Core.LevelCalculator.GetLevelforExp(u.XP))) {
                                validLevel = false;
                            }

                            if (validPassword)
                            {
                                   if (validLevel)
                                    {
                                        if (!r.Add(u))
                                        {
                                            if (r.UserLimit)
                                                u.Send(new Packets.RoomJoin(Packets.RoomJoin.ErrorCodes.UsersExceeded));
                                            else
                                            u.Send(new Packets.RoomJoin(Packets.RoomJoin.ErrorCodes.RoomIsFull));
                                        }
                                    }
                                    else { u.Send(new Packets.RoomJoin(Packets.RoomJoin.ErrorCodes.BadLevel)); }
                                
                            }
                            else
                            {
                                u.Send(new Packets.RoomJoin(Packets.RoomJoin.ErrorCodes.InvalidPassword));
                            }
                        }
                        else
                        {
                            u.Send(new Packets.RoomJoin(Packets.RoomJoin.ErrorCodes.GenericError));
                        }
                    }
                    else
                    {
                        u.Send(new Packets.RoomJoin(Packets.RoomJoin.ErrorCodes.GenericError));
                    }
                }
            }
            else
            {
                u.Disconnect();
            }
        }
    }
}
