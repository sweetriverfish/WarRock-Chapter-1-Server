using System;
using System.Collections;

namespace Game.Handlers {
    class RequestServerTime : Networking.PacketHandler {
        protected override void Process(Entities.User u) {
            //throw new NotImplementedException();
            int versionId = GetInt(1);
            string MACAdress = GetString(2);

            if (versionId == 3) {
                if (MACAdress.Length == 12) 
                { 
                u.Send(new Packets.ServerTime());
                } else {
                    u.Send(new Packets.Authorization(Packets.Authorization.ErrorCodes.NormalProcedure));
                }
            }
            else {
                u.Send(new Packets.ServerTime(Packets.ServerTime.ErrorCodes.DiffrentClientVersion));
            }
        }
    }
}
