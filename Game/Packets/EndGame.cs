using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Packets {
    class EndGame : Core.Networking.OutPacket {
        public EndGame(Entities.Room r, Entities.Player[] players, Enums.Team winningTeam)
            : base((ushort)Enums.Packets.EndGame) {

                int[] teamKills = new int[] { 0, 0 };
                int[] teamDeaths = new int[] { 0, 0 };

                teamKills[(byte)Enums.Team.Derbaran] = players.Where(n => n.Team == Enums.Team.Derbaran).Sum(n => n.Kills);
                teamKills[(byte)Enums.Team.NIU] = players.Where(n => n.Team == Enums.Team.NIU).Sum(n => n.Kills);
                teamDeaths[(byte)Enums.Team.Derbaran] = players.Where(n => n.Team == Enums.Team.Derbaran).Sum(n => n.Deaths);
                teamDeaths[(byte)Enums.Team.NIU] = players.Where(n => n.Team == Enums.Team.NIU).Sum(n => n.Deaths);

            Append(Core.Constants.Error_OK);

            if (r.CurrentGameMode != null) //random crash
            {
                Append(r.CurrentGameMode.CurrentRoundTeamA()); //DERBARAN ROUNDS
                Append(r.CurrentGameMode.CurrentRoundTeamB()); //NIU ROUNDS
            }
            else
            {
                Fill(2, 0);
                Log.Instance.WriteDev("FFA Crash stopped"); //TODO
            }
                
       
            Append(teamKills[(byte)Enums.Team.Derbaran]);
            Append(teamDeaths[(byte)Enums.Team.Derbaran]);
            Append(teamKills[(byte)Enums.Team.NIU]);
            Append(teamDeaths[(byte)Enums.Team.NIU]);
            Append(0);
            Append(0);
            Append(players.Length);
                for (byte i = 0; i < players.Length; i++) {
                    Entities.Player plr = players[i];
                    Append(plr.Id); // Slot
                    Append(plr.Kills); // Kills
                    Append(plr.Deaths); // Deaths
                    Append(plr.Flags); //Flags
                    Append(plr.Points); // Points 
                    Append(plr.MoneyEarned); // Dinar earned
                    Append(plr.XPEarned); // Exp earned
                    Append(plr.User.XP); // Player Exp
                }

                Append(r.Master);
        }
    }
}
