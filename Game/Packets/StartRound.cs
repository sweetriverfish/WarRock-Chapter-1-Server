using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Packets {
    class StartRound : Core.Networking.OutPacket {
        public StartRound(Entities.Room r)
            : base((ushort)Enums.Packets.GamePacket) {
            Append(1);
            Append(-1);
            Append(r.ID);
            Append(1);
            Append(5); // TYPE
            Append(0);
            Append(r.CurrentGameMode.CurrentRoundTeamA());
            Append(r.CurrentGameMode.CurrentRoundTeamB());
            Append(1);
            Append(0);
            Append(5);
            Append(0);
            Append(0);
            Append(-1);
            Append(0);
            Append(0);
            Append(800000);
            Append(-184460);
            Append(800000);
            Append(1200000);
            Append(-900000);
            Append(1200000);
            Append("0.0000");
            Append("0.0000");
            Append("0.0000");
            Append("0.0000");
            Append("0.0000");
            Append("0.0000");
            Append(0);
            Append(0);
            Append("DS05");
        }
    }
}
