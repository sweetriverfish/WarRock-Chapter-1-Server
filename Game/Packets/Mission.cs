using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Packets {
    class Mission : Core.Networking.OutPacket {
        public Mission(Entities.Room r)
            : base(30000) {
            Append(1); // Success
            Append(-1); // ??
            Append(r.ID); // Room ID
            Append(2);
            Append(403); // Sub id?
            Append(0);
            Append(1);
            Append(3);
            Append(363);
            Append(0);
            Append(1);
            Append(0);
            Append(0);
            Append(0);
            Append(0);
        }
    }
}
