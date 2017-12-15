using System;
using Game.Enums;
using Game.Networking;

namespace Game.Handlers.Game.Ingame
{
    internal class UseItemsOnGround : GameDataHandler
    {
        protected override void Handle()
        {
            if (Room.State != RoomState.Playing)
                Player.User.Disconnect();

            if (!Player.IsAlive)
                return;

            string _itemCode = GetString(22);
            int _itemId = GetInt(4);

            if (Room.ItemsOnGround[_itemId] == null)
                return;
            else
            {
                Room.ItemsOnGround.Remove(Room.ItemsOnGround[_itemId]);
                switch (_itemCode)
                {
                    case "DS05": //flash mine and ammo box
                    case "DU01":
                        respond = true;
                        break;

                    case "DU02": //land mine

                        if (Player.Health >= 500)
                            Player.Health -= 300;
                        else
                            Player.Health = 200;

                        Set(6, Player.Health);
                        respond = true;
                        break;

                    case "DV01":  //medic box
                        Player.Health += 400;

                        if (Player.Health > 1000)
                            Player.Health = 1000;

                        Set(6, Player.Health);
                        respond = true;
                        break;

                    default:
                        ServerLogger.Instance.Append(ServerLogger.AlertLevel.Cheating, String.Concat("Player ", Player.User.Displayname, " used an unkown item ", _itemCode));
                        respond = false;
                        break;

                }
            }
        }
    }
}
