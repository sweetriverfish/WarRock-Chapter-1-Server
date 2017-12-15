using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Packets
{
    class LeaveVehicle : Core.Networking.OutPacket
    {
        public LeaveVehicle(byte _vehicleID, string oddString) : base(29969)
        {
            Append(_vehicleID);
            Append(-1);
            Append(-1);
            Append(-1);
            Append(-1);
            Append(-1);
            Append(-1);
            Append(-1);
            Append(oddString);
            Append(-1);
            Append(-1);
            Append(-1);
            Append(-1);
            Append(-1);
            Append(-1);
            Append(-1);
            Append(-1);
            Append(-1);
        }
    }
}
