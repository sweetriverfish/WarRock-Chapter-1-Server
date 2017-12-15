using System;
using Game.Enums;

namespace Game.Handlers.Game.Ingame
{
    public sealed class Artillery: Networking.GameDataHandler
    {
        protected override void Handle()
        {
            if(Room.State == RoomState.Playing)
            {
                if(!Player.IsAlive)
                    return;

                if(Player.User.Inventory.Itemlist.Contains("DX01"))
                {
                        Set(3, 1);
                        respond = true;
                }
            }
            else
                Player.User.Disconnect();
        
      }
   }
}
