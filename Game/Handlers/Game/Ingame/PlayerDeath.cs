using System;
using Game.Enums;
using Game.Networking;


//TODO: USE THIS MORE EFFICIENTLY
//THIS HANDLER SHOULD BE CALLED EVERYTIME A PLAYER DIES... 
//TODO: DO SMTH

namespace Game.Handlers.Game.Ingame
{
    internal class PlayerDeath : GameDataHandler
    {
        protected override void Handle()
        {
            if (Room.State == RoomState.Playing)
            {

                type = Enums.GameSubs.PlayerDeath;
                Set(3, Player.Id);
                Set(12, 0);
                Set(13, Player.Health);
                respond = true;
             }
            else 
                Player.User.Disconnect();
        }
    }
}
