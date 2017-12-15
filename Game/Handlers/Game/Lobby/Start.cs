using System;
using System.Linq;

namespace Game.Handlers.Game {
    class Start : Networking.GameDataHandler {
        protected override void Handle() {
              if (Player.Id == Room.Master) {
                if (Room.State == Enums.RoomState.Waiting) {
                    lock (Room._playerLock) {
                        if (Room.Players.All(p => p.Value.Ready)) {
                            var debTeam = Room.Players.Select(n => n.Value).Where(n => n.Team == Enums.Team.Derbaran).Count();
                            var niuTeam = Room.Players.Select(n => n.Value).Where(n => n.Team == Enums.Team.NIU).Count();
                            int teamDifference = Math.Abs(debTeam - niuTeam);

                                if (debTeam + niuTeam > 1 || Player.User.AccessLevel >= 3)
                                {
                                    if (teamDifference <= GameConfig.MaxTeamDifference || Room.Mode == Enums.Mode.Free_For_All || Player.User.AccessLevel >=3)
                                    {
                                        respond = updateLobby = true;
                                        type = Enums.GameSubs.StartReply;
                                        Room.State = Enums.RoomState.Playing;
                                        Set(2, Room.Map);
                                        Room.Start();
                                    }
                                    else
                                    {
                                        //string _balanceTeams = Cristina.Core.Cristina.Localization.GetLocMessageFrom("BALANCE_TEAMS_BEFORE");
                                        Cristina.Core.Cristina.Chat.SaytoRoom("Equlibrad equipos antes de empezar", Room);
                                    }
                                        
                                }
                                else
                                {
                                  //  string _waitForAnother = Cristina.Core.Cristina.Localization.GetLocMessageFrom("WAIT_FOR_PLAYER");
                                    Cristina.Core.Cristina.Chat.SayToPlayer("Espera a más jugadores para empezar", Player.User);
                                }                               
                            
                        }
                    }
                }
            } else {
                Player.User.Disconnect(); // Cheating? - Not a master!
            }
        }
    }
}
