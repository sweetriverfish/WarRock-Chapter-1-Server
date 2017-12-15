using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Packets {
    class Scoreboard : Core.Networking.OutPacket {
        public Scoreboard(Entities.Room r)
            : base((ushort)Enums.Packets.Scoreboard) {

                Append(Core.Constants.Error_OK);

                if (r.Mode == Enums.Mode.Explosive) {
                    Append(r.CurrentGameMode.CurrentRoundTeamA());
                    Append(r.CurrentGameMode.CurrentRoundTeamB());
                } else {
                    Append(0);
                    Append(0);
                }

                Append(r.CurrentGameMode.ScoreboardA());
                Append(r.CurrentGameMode.ScoreboardB());

                Entities.Player[] players = r.Players.Values.Where(p => p != null).ToArray();
                Append(players.Length);
                foreach (Entities.Player p in players) {
                    Append(p.Id);
                    Append(p.Kills);
                    Append(p.Deaths);
                    Append(p.Flags);
                    Append(p.Assists + p.Points);
                    Append(0);
                }

        }
    }
}
