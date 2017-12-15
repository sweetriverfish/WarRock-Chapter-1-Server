using System;
using Game.Enums;
using Game.Networking;

namespace Game.Handlers.Game.Ingame
{
    internal class AmmoRecharge : GameDataHandler
    {
        protected override void Handle()
        {
            if (Room.State == RoomState.Playing)
            {
                if (!Player.IsAlive)
                    return;
                type = GameSubs.AmmoRecharge;
                respond = true;
            }
            else  //cheating!!!! sending a reload packet when not in a room
                Player.User.Disconnect();
        }
    }
}
