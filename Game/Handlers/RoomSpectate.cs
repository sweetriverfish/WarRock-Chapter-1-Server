using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Handlers
{
    class RoomSpectate : Networking.PacketHandler
    {
       
        protected override void Process(Entities.User u)
        {
            if (u.Authorized)
            {
                if (u.Room == null)
                {
                    Objects.Channel channel = Managers.ChannelManager.Instance.Get(u.Channel);

                    uint roomId = GetuInt(1);
                    string roomPassword = GetString(2);

                    if (channel.Rooms.ContainsKey(roomId))
                    {
                        Entities.Room r = null;
                        if (channel.Rooms.TryGetValue(roomId, out r))
                        {
                            if (!r.AddSpectator(u))
                                u.Send(new Packets.RoomSpectate(Packets.RoomSpectate.ErrorCodes.MaxSpectators));
                        }
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
