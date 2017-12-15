using System;
using System.Collections;

namespace Game.Handlers
{
    class RoomQuickJoin : Networking.PacketHandler
    {
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
        protected override void Process(Entities.User u)
        {
            if (!u.Authorized)
                return;

            if (u.Room != null)
                u.Disconnect(); //cheating!

            ArrayList ValidRooms = new ArrayList();
            Objects.Channel PlayerChannel;

            Managers.ChannelManager.Instance.channels.TryGetValue(u.Channel, out PlayerChannel);

            if (PlayerChannel == null)
                return;

            foreach (Entities.Room Room in PlayerChannel.Rooms.Values) //TODO PING CHECK, FIXED NO OF PLAYERS
            {
                if (Room.Players.Count < Room.MaximumPlayers) {
                    if(SuitableLevel(Room.LevelLimit, Core.LevelCalculator.GetLevelforExp(u.XP))){
                            if (!Room.HasPassword){
                                ValidRooms.Add(Room);
                            }
                        
                    }
                }
            }

            if (ValidRooms.Count > 0)
            {
                Random Random = new Random();
                Entities.Room SelectedRoom = (Entities.Room)ValidRooms[Random.Next(0, ValidRooms.Count)];
                SelectedRoom.Add(u);
            }
        }
    }
}
