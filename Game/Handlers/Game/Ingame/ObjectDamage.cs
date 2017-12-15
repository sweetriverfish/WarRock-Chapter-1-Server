using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Handlers.Game.Ingame
{
    class ObjectDamage : Networking.GameDataHandler {
        protected override void Handle()
        {
            if (Room.State == Enums.RoomState.Playing)
            {
                if (packet.Blocks.Length == 27)
                    Room.CurrentGameMode.OnObjectDamage(this);
                else
                    Player.User.Disconnect(); 
            }
        }
    }
}
