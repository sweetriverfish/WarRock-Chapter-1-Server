using System;
using Game.Enums;

namespace Game.Handlers.Game.Ingame
{
    public sealed class Suicide : Networking.GameDataHandler
    {
        protected override void Handle()
        {
            if (Room.State == RoomState.Playing)
            {
                if (!Player.IsAlive || Room.Mode == Mode.Free_For_All || Room.Mode == Mode.Explosive)
                    return;

                    type = GameSubs.Suicide;
                    Player.Health = 0;
                    Player.Suicide();
                    Set(2, Player.User.RoomSlot);
                    Set(3, 0);
                    respond = true;

                    if (Player.Team != Team.None)
                        Room.CurrentGameMode.OnPlayerSuicide(Player);
                
            }
            else
                Player.User.Disconnect();
        }
    }
}
