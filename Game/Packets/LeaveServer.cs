using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Packets
{
    class LeaveServer : Core.Networking.OutPacket
    {
        public LeaveServer() : base(24576)
        {
            Append(1);
        }
    }
}
