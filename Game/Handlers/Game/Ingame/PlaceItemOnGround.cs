using System;
using Game.Enums;
using Game.Networking;

namespace Game.Handlers.Game.Ingame
{
    internal class PlaceItemOnGround : GameDataHandler
    {
        protected override void Handle()
        {
            if (Room.State != RoomState.Playing)
                Player.User.Disconnect();

            if (!Player.IsAlive)
                return;
          
             string _weapon = GetString(27);

             if (Player.User.Inventory.Itemlist.Contains(_weapon))
             {
                 if (Room.Mode == Mode.Free_For_All || Player.ItemsPlanted < 7)
                 {
                     Player.ItemsPlanted++;
                     Objects.GroundItems Item = new Objects.GroundItems(_weapon, Player);
                     Room.ItemsOnGround.Add(Item);
                     Set(4, Room.ItemsOnGround.Count - 1);
                     respond = true;
                 }
             }
             else
                 Player.User.Disconnect();
        }
    }
}