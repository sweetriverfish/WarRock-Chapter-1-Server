using Game.Objects.Inventory;
using Game.Objects.Items;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;



namespace Game.Handlers
{
    class Coupons : Networking.PacketHandler
    {
        protected override void Process(Entities.User u)
        {
            if (u.Authorized)
            {
                string _inputCode = GetString(0);
                // Let´s check if any coupon matches the input
                if (Managers.CouponManager.Instance.isValidCouponCode(_inputCode))
                {
                    Objects.Coupon Coupon = Managers.CouponManager.Instance.getCoupon(_inputCode);

                    Console.WriteLine("coupon uses:"+Coupon.Uses);
                    if (Coupon.Uses != 0)
                    {
                        if (Coupon.DinarReward > 0)
                        {
                            Console.WriteLine("meow1");
                            u.Money += Coupon.DinarReward;

                            Databases.Game.AsyncQuery("UPDATE user_details SET money=" + u.Money + " WHERE id=" + u.ID + ";");
                            u.Send(new Packets.Coupon(0, u.Money));
                        }
                        if (Coupon.ItemReward.Length > 0)
                        {
                            Console.WriteLine("meow2");
                            // Itemreward coupon column format should be ('itemCode-daysDuration,[next item]) ex: 'DF05-30,DF06-30,DF07-30'
                            string[] rewards = Coupon.ItemReward.Split(',');
                            foreach (string reward in rewards)
                            {
                                string[] rewardContent = reward.Split('-');
                                if (rewardContent[0].Length != 4)
                                {
                                    continue;
                                    // todo: log?
                                }
                                if (rewardContent[1].Length > 6)
                                {
                                    continue;
                                    // todo: log?
                                }

                                int rewardDaysLength;
                                if (!int.TryParse(rewardContent[1], out rewardDaysLength))
                                {
                                    continue;
                                    // todo: log?
                                }

                                ItemData item = Managers.ItemManager.Instance.Items[rewardContent[0]];

                                var invItem = (Objects.Inventory.Item)null;
                                try
                                {
                                    invItem = u.Inventory.Items.Select(n => n.Value).Where(n => n.ItemCode == item.Code).First();
                                }
                                catch { invItem = null; }

                                uint utcTime = (uint)DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                                uint itemLength = (uint)(86400 * rewardDaysLength);

                                if (invItem != null)
                                {
                                    // Has item in inventory.
                                    // Check for a possible retail
                                    if (u.Inventory.Retails.Contains(item.Code.ToUpper()))
                                        u.Send(new Packets.Itemshop(Packets.Itemshop.ErrorCodes.CannotBeBougth));
                                    else
                                    {
                                        // Extend & insert into db :)
                                        Databases.Game.AsyncQuery(string.Concat("INSERT INTO user_inventory (`id`, `owner`, `code`, `startdate`, `length`, `price`, `expired`, `deleted`) VALUES (NULL, '", u.ID, "', '", item.Code.ToUpper(), "', '", utcTime, "', '", itemLength, "', '", '0', "', '0', '0');"));
                                        invItem.ExpireDate = invItem.ExpireDate.AddSeconds(itemLength);
                                        u.Inventory.Rebuild();
                                        u.Send(new Packets.Itemshop(u));
                                    }
                                }
                                else
                                {
                                    // No item in invetory
                                    // Insert & fetch id
                                    uint itemdbId = 0;
                                    MySql.Data.MySqlClient.MySqlCommand cmd;
                                    try
                                    {
                                        cmd = new MySql.Data.MySqlClient.MySqlCommand(string.Concat("INSERT INTO user_inventory (`id`, `owner`, `code`, `startdate`, `length`, `price`, `expired`, `deleted`) VALUES (NULL, '", u.ID, "', '", item.Code.ToUpper(), "', '", utcTime, "', '", itemLength, "', '", '0', "', '0', '0');"), Databases.Game.connection);
                                        cmd.ExecuteNonQueryAsync();
                                        itemdbId = (uint)cmd.LastInsertedId;
                                    }
                                    catch { itemdbId = 0; }

                                    if (itemdbId > 0)
                                    {
                                        u.Inventory.Add(new Objects.Inventory.Item(-1, itemdbId, item.Code, (utcTime + itemLength)));
                                        u.Inventory.Rebuild();
                                        u.Send(new Packets.Itemshop(u));
                                    }
                                    else
                                    {
                                        u.Send(new Packets.Itemshop(Packets.Itemshop.ErrorCodes.CannotBeBougth));
                                    }
                                }
                            }
                        }
                        if (Coupon.Uses > 0)
                        {
                            Console.WriteLine("meow3");
                            int _usesLeft = Coupon.Uses;
                            _usesLeft--;

                            Managers.CouponManager.Instance.UpdateCouponUses(Coupon.Index, _usesLeft);

                            Console.WriteLine("UPDATE coupons SET uses=" + _usesLeft + "  WHERE id=" + Coupon.Index + ";");
                            Databases.Game.AsyncQuery("UPDATE coupons SET uses=" + _usesLeft + "  WHERE id=" + Coupon.Index + ";");
                        }
                        ServerLogger.Instance.Append(String.Concat("Player ", u.Displayname, " used coupon ", _inputCode));
                    }
                    else
                    {
                        // Coupon ran out of uses
                        u.Send(new Packets.Coupon(-1, 0));
                    }

                }
                else
                {
                    // Invalid coupon
                    u.Send(new Packets.Coupon(-2, 0));
                }
            }
            else
            {
                u.Disconnect();
            }
        }
    }
}
