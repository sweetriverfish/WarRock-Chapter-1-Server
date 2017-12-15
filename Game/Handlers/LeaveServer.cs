using System;
using System.Collections;

namespace Game.Handlers
{
    class LeaveServer : Networking.PacketHandler
    {
        protected override void Process(Entities.User u)
        {
            if (!u.Authorized)
                return;

            u.Send(new Packets.LeaveServer());
            ServerLogger.Instance.Append(ServerLogger.AlertLevel.Information, String.Concat("Player ", u.Displayname, " logged OFF"));

        }
    }
}
