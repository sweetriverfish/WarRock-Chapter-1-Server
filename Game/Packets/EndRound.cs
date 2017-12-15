using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Packets {
    class EndRound : Core.Networking.OutPacket {
        public EndRound(Entities.Room r, Enums.Team winningTeam)
            : base((ushort)Enums.Packets.GamePacket) {
            Append(1);
            Append(-1);
            Append(r.ID);
            Append(1);
            Append(6); // TYPE
            Append(0); // ?
            Append(1); // ?
            Append((byte)winningTeam); // Winning Team ID
            Append(r.CurrentGameMode.CurrentRoundTeamA());
            Append(r.CurrentGameMode.CurrentRoundTeamB());
            Append(5); // Remaining Rounds?
            Append(0);
            Append(92);
            Append(-1);
            Append(0);
            Append(0);
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
