using System;
using System.Linq;
using System.Collections;

namespace Game.Handlers
{
    class RoomKick : Networking.PacketHandler
    {
        protected override void Process(Entities.User u)
        {
            if (u.Authorized)
            {
                if (u.Room != null)
                {
                    Entities.Room r = u.Room;
                    byte _target = GetByte(0);
                    foreach (Entities.Player Player in r.Players.Values)
                    {
                        if (r.Master == Player.Id && Player.User == u)
                            KickPlayer(r, _target);
                    }


                }
            }
        }

        private void KickPlayer(Entities.Room Room, int _target)
        {
            foreach (Entities.Player Target in Room.Players.Values)
            {
                if (Target.Id == _target)
                    Target.User.Send(new Packets.RoomKick(_target));
            }
        }

    }

}
