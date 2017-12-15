using System;
using System.Collections.Generic;

namespace Game.Handlers
{
    public sealed class LeaveVehicle : Networking.PacketHandler
    {
        protected override void Process(Entities.User u)
        {
            if (!u.Authorized || u.Room == null)
                return;

            if (u.Room.State == Enums.RoomState.Playing)
            {
              //  u.Send(new Packets.LeaveVehicle(GetByte(0), GetString(8)));
              //  Log.Instance.WriteDev("Do smth about me");
            }
            else
                u.Disconnect();
        }
    }
}
