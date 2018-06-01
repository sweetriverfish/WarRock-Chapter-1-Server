using System;
using System.Linq;
using System.Text;

using Game.Objects.Items;

namespace Game.Handlers {
    class Itemshop : Networking.PacketHandler {
        private int[] days = { 3, 7, 15, 30 };
        protected override void Process(Entities.User u) {
            ushort actionType = GetUShort(0);

            if (actionType >= (ushort)Enums.ItemAction.BuyItem && actionType <= (ushort)Enums.ItemAction.UseItem) {

                if (actionType == (ushort)Enums.ItemAction.BuyItem) {
                    string itemCode = GetString(1).ToUpper();

                    if (itemCode.Length == 4) {
                        if (Managers.ItemManager.Instance.Items.ContainsKey(itemCode)) {
                            ItemData item = Managers.ItemManager.Instance.Items[itemCode];
                            if (item != null) {
                                uint dbId = GetuInt(2);
                                //if (item.dbId == dbId) {
                                    byte length = GetByte(4);
                                    if (length < 4) {
                                        if (item.Shop.IsBuyable) {

                                            if (u.Inventory.Items.Count < Objects.Inventory.Inventory.MAX_ITEMS) {
                                                if (!item.Shop.RequiresPremium || (item.Shop.RequiresPremium && (byte)u.Premium > (byte)Enums.Premium.Free2Play)) {
                                                    if (item.Shop.RequiredLevel <= Core.LevelCalculator.GetLevelforExp(u.XP)){
                                                    int price = item.Shop.Cost[length];
                                                    if (price >= 0) {
                                                        int moneyCalc = (int)u.Money - price;
                                                        if (moneyCalc >= 0) {

                                                            var invItem = (Objects.Inventory.Item)null;
                                                            try {
                                                                invItem = u.Inventory.Items.Select(n => n.Value).Where(n => n.ItemCode == item.Code).First();
                                                            } catch { invItem = null; }

                                                            uint utcTime = (uint)DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                                                            uint itemLength = (uint)(86400 * days[length]);
                                                            u.Money = (uint)moneyCalc;

                                                            if (invItem != null) { // Has item in inventory.

                                                                //check for a possible retail
                                                                if (u.Inventory.Retails.Contains(item.Code.ToUpper()))
                                                                    u.Send(new Packets.Itemshop(Packets.Itemshop.ErrorCodes.CannotBeBougth));
                                                                else
                                                                {
                                                                    // Extend & insert into db :)
                                                                    Databases.Game.AsyncQuery(string.Concat("INSERT INTO user_inventory (`id`, `owner`, `code`, `startdate`, `length`, `price`, `expired`, `deleted`) VALUES (NULL, '", u.ID, "', '", item.Code.ToUpper(), "', '", utcTime, "', '", itemLength, "', '", price, "', '0', '0'); UPDATE user_details SET money='", u.Money, "' WHERE id = ", u.ID, ";"));
                                                                    invItem.ExpireDate = invItem.ExpireDate.AddSeconds(itemLength);
                                                                    u.Inventory.Rebuild();
                                                                    u.Send(new Packets.Itemshop(u));
                                                                } 
                                                            } else { // No item in invetory
                                                                // Insert & fetch id
                                                                uint itemdbId = 0;
                                                                MySql.Data.MySqlClient.MySqlCommand cmd;

                                                                try {
                                                                    cmd = new MySql.Data.MySqlClient.MySqlCommand(string.Concat("INSERT INTO user_inventory (`id`, `owner`, `code`, `startdate`, `length`, `price`, `expired`, `deleted`) VALUES (NULL, '", u.ID, "', '", item.Code.ToUpper(), "', '", utcTime, "', '", itemLength, "', '", price, "', '0', '0');"), Databases.Game.connection);
                                                                    cmd.ExecuteNonQueryAsync();
                                                                    itemdbId = (uint)cmd.LastInsertedId;
                                                                } catch { itemdbId = 0; }

                                                                if (itemdbId > 0) {
                                                                    Databases.Game.AsyncQuery(string.Concat("UPDATE user_details SET money='", u.Money, "' WHERE id = ", u.ID, ";"));
                                                                    u.Inventory.Add(new Objects.Inventory.Item(-1, itemdbId, item.Code, (utcTime + itemLength)));
                                                                    u.Inventory.Rebuild();
                                                                    u.Send(new Packets.Itemshop(u));
                                                                } else {
                                                                    u.Send(new Packets.Itemshop(Packets.Itemshop.ErrorCodes.CannotBeBougth));
                                                                }
                                                            }

                                                        } else {
                                                            u.Send(new Packets.Itemshop(Packets.Itemshop.ErrorCodes.NotEnoughDinar));
                                                        }
                                                    } else {
                                                        u.Disconnect(); // Item can't be bought for this period. - Cheating?
                                                    }
                                                } else {
                                                    u.Send(new Packets.Itemshop(Packets.Itemshop.ErrorCodes.LevelRequirement));
                                                }
                                                } else {
                                                    u.Send(new Packets.Itemshop(Packets.Itemshop.ErrorCodes.PremiumOnly));
                                                }
                                            } else {
                                                u.Send(new Packets.Itemshop(Packets.Itemshop.ErrorCodes.InventoryFull)); // Inventory is full.
                                            }

                                        } else {
                                            u.Send(new Packets.Itemshop(Packets.Itemshop.ErrorCodes.CannotBeBougth)); // Buying an item that isn't for sale? - Cheating?
                                        }
                                    } else {
                                        u.Disconnect(); // Cheating?
                                    }
                                } else {
                                    u.Disconnect(); // Invalid id for the item - Cheating?
                                }
                            } else {
                                u.Disconnect(); // Server error.
                            }
                        } else {
                            u.Disconnect(); // Code doesn't exist - Cheating?
                        }
                    } else {
                        u.Disconnect(); // Wrong Code - Cheating?
                    }
                } else if (actionType == (ushort)Enums.ItemAction.UseItem) {
                    u.Send(new Packets.Itemshop(Packets.Itemshop.ErrorCodes.InvalidItem)); 
                } else {
                    u.Send(new Packets.Itemshop(Packets.Itemshop.ErrorCodes.InvalidItem)); 
                }
            /*} else {
                u.Disconnect(); // Invalid Action type - Cheating?
            }*/
        }
    }
}
