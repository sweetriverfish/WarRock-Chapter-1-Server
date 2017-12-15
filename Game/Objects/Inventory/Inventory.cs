using System;
using System.Linq;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Game.Objects.Inventory {
    public class Inventory {
        public const byte MAX_ITEMS = 32;

        private readonly Entities.User User;

        public readonly ConcurrentDictionary<sbyte, Item> Items;
        public readonly List<string> Retails;
        public readonly ArrayList ExpiredItems;

        private readonly SortedSet<sbyte> _openSlots;

        public readonly Equipment Equipment;

        public string SlotState;
        public string Itemlist;

        public Inventory(Entities.User user) {
            this.User = user;
            this.Equipment = new Equipment(user);
            this.Items = new ConcurrentDictionary<sbyte, Item>();
            this.Retails = new List<string>();
            this._openSlots = new SortedSet<sbyte>();
            this.ExpiredItems = new ArrayList();
            this.Itemlist = "";

            Reset();
        }

        public void Load() {

            string fetchQuery = string.Concat("SELECT id, code, startdate, sum(length) AS len, retail FROM user_inventory WHERE owner=", this.User.ID, " AND expired=0 AND deleted=0 GROUP BY code ORDER BY id ASC");

            MySqlCommand cmd;
            MySqlDataReader result;

            try {
                cmd = new MySqlCommand(fetchQuery, Databases.Game.connection);
                result = cmd.ExecuteReader();
            } catch { User.Disconnect(); return; }


            uint nowTimeStamp = (uint)DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime()).TotalSeconds;
            string expireQueries = "";

            if (result != null) {
                if (result.HasRows) {
                    while (result.Read()) {

                        if (Items.Count >= MAX_ITEMS)
                            break;

                        uint dbId = result.GetUInt32("id");
                        string weaponCode = result.GetString("code");
                        uint startDate = result.GetUInt32("startdate");
                        uint length = result.GetUInt32("len");
                        uint expireDate = startDate + length;
                        bool isRetail = result.GetBoolean("retail");
    
                        if (nowTimeStamp < expireDate ||  isRetail) {
                            sbyte openSlot = _openSlots.First();
                            if (openSlot >= 0) {

                                Item item = new Item(openSlot, dbId, weaponCode, expireDate);
                                Items.TryAdd(openSlot, item);
                                _openSlots.Remove(openSlot);

                                if (isRetail)
                                    Retails.Add(weaponCode);
                            }                            
                        } else {
                            ExpiredItems.Add(weaponCode.ToUpper()); // This item is expired!
                            expireQueries += string.Concat("UPDATE user_inventory SET expired='1' WHERE code='", weaponCode.ToUpper(), "' AND owner='", this.User.ID, "';");
                        }

                    }
                }
                result.Close();
            }

            if (expireQueries.Length > 0) {
                Databases.Game.Query(expireQueries); // Run all the querries at once
            }

            result = Databases.Game.Select(
                new string[] { "class1", "class2", "class3", "class4", "class5" },
                "user_equipment",
                new Dictionary<string, object>()
                            {
                                { "owner", User.ID }
                            }
                );

            if (result != null) {
                if (result.HasRows) {
                    if (result.Read()) {
                        for (byte i = 0; i < 5; i++) {
                            string equipment = result.GetString(i);
                            ReadEquipment((Enums.Classes)i, equipment);
                        }
                    }
                } else {
                    ReadEquipment((Enums.Classes)0, "DA02,DB01,DF01,DR01,^,^,^,^");
                    ReadEquipment((Enums.Classes)1, "DA02,DB01,DF01,DQ01,^,^,^,^");
                    ReadEquipment((Enums.Classes)2, "DA02,DB01,DG05,DN01,^,^,^,^");
                    ReadEquipment((Enums.Classes)3, "DA02,DB01,DC02,DN01,^,^,^,^");
                    ReadEquipment((Enums.Classes)4, "DA02,DB01,DJ01,DL01,^,^,^,^");

                    try {
                        if (!result.IsClosed)
                            result.Close();
                    } catch { }

                    Databases.Game.Query(string.Concat("INSERT INTO user_equipment (`owner`, `class1`, `class2`, `class3`, `class4`, `class5`) VALUES ('", User.ID ,"', 'DA02,DB01,DF01,DR01,^,^,^,^', 'DA02,DB01,DF01,DQ01,^,^,^,^', 'DA02,DB01,DG05,DN01,^,^,^,^', 'DA02,DB01,DC02,DN01,^,^,^,^', 'DA02,DB01,DJ01,DL01,^,^,^,^');")); // Insert into database.
                }
                try {
                    if (!result.IsClosed)
                        result.Close();
                } catch { }
            } else {
                throw new Exception("Fatal error occured while loading the equipment of " + User.Displayname + ".");
            }

            Rebuild(); // Rebuild everything.
        }

        private bool ContainsItem(string weaponCode) {
            return (Items.Select(n => n.Value).Where(n => n.ItemCode == weaponCode.ToUpper()).Count() > 0);
        }

        public bool Add(Item item) {
            sbyte openSlot = 0;

            try {
                openSlot = _openSlots.First();
            } catch { openSlot = -1; }

            if (openSlot >= 0) {
                item.Slot = openSlot;
                Items.TryAdd(openSlot, item);
                _openSlots.Remove(openSlot);
                return true;
            }

            return false;
        }

        public Item Get(string weaponCode) {
            try {
                return Items.Select(n => n.Value).Where(n => n.ItemCode == weaponCode.ToUpper()).First();
            } catch { return null; }
        }

        public void Remove(Item item) {
            Item itm = null;

            if (Items.ContainsKey(item.Slot))
                Items.TryRemove(item.Slot, out itm);

            if (!_openSlots.Contains(item.Slot))
                _openSlots.Add(item.Slot);
        }

        public void Remove(string weaponCode) {
            var items = Items.Select(n => n.Value).Where(n => n.ItemCode == weaponCode.ToUpper());

            foreach (Item item in items) {
                Item itm;
                if (Items.ContainsKey(item.Slot))
                    Items.TryRemove(item.Slot, out itm);

                if (!_openSlots.Contains(item.Slot))
                    _openSlots.Add(item.Slot);
            }
        }

        private void Reset() {
            Items.Clear();
            _openSlots.Clear();
            ExpiredItems.Clear();

            SlotState = "F,F,F,F";

            for (sbyte i = 0; i < MAX_ITEMS; i++) {

                if (i == 0)
                    Itemlist = "^";
                else
                    Itemlist += ",^";

                _openSlots.Add(i);
            }
        }

        public void Rebuild() {
            RebuildItemList();
            RebuildSlotState();
            Equipment.BuildInternal();
            Equipment.Build();
        }

        private void RebuildSlotState() {
            string[] strSlots = { "F", "F", "F", "F" };

            if (User.Premium > Enums.Premium.Free2Play) { 
                strSlots[0] = "T"; // Enable 5Th Slot 
            }

            var _6ThSlotEnabled = Managers.ItemManager.Instance.Items.Select(n => n.Value).Where(n => n.IsWeapon).OfType<Objects.Items.Weapon>().Where(n => n.CanEquip6Th).ToArray() ;
            var _6ThSlotItems = _6ThSlotEnabled.Where(n => ContainsItem(n.Code)).Count();
            if (ContainsItem("CA02") || _6ThSlotItems > 0) {
                strSlots[1] = "T";
            }

            if (ContainsItem("CA03")) {
                strSlots[2] = "T";
            }
            if (ContainsItem("CA04"))
            {
                strSlots[3] = "T";
            }

            SlotState = string.Join(",", strSlots);
        }

        private void RebuildItemList() {
            var itemList = Items.Values.Take(MAX_ITEMS).OrderBy(i => i.Slot);

            Itemlist = ""; // Output

            //B33-3-0-13070522-0-0-0-0-0-9999-9999,CB08-2-0-13052022-0-5-0-0-0-9999-9999,CC02-3-0-13070522-0-0-0-0-0-9999-9999,DS10-3-0-13061821-0-0-0-0-0-9999-9999
            //split('-')[0] = code, 

            foreach (Item i in itemList) {
                string output = i.ItemCode.ToUpper() + "-" + i.Type + "-0-" + System.String.Format("{0:yyMMddHH}", i.ExpireDate) + "-" + i.Amount;
                if (Itemlist.Length == 0) {
                    this.Itemlist = output;
                } else {
                    this.Itemlist += "," + output;
                }
            }

            for (byte i = (byte)itemList.Count(); i < MAX_ITEMS; i++) {
                if (i == 0) {
                    this.Itemlist = "^";
                } else {
                    this.Itemlist += ",^";
                }
            }
        }

        // TODO: Re-do the equipment system.
        private void ReadEquipment(Enums.Classes targetClass, string equipment) {
            if (targetClass >= 0 && targetClass <= Enums.Classes.Heavy) {
                string[] weaponCodes = equipment.Split(',');

                for (byte i = 0; i < Equipment.MAX_SLOTS; i++) {
                    if (weaponCodes.Length <= i) {
                        Equipment.Add(targetClass, i, null);
                    } else {
                        Item item = Get(weaponCodes[i]);
                        if (item != null) {
                            Equipment.Add(targetClass, i, item);
                        } else {
                            for (byte j = 0; j < DEFAULT_ITEMS.Length; j++) {
                                if (weaponCodes[i].ToUpper() == DEFAULT_ITEMS[j].ToUpper()) {
                                    Equipment.Add(targetClass, i, new Item(-1, 0, DEFAULT_ITEMS[j], 0));
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static string[] DEFAULT_ITEMS = new string[] { "DA02",
                                                            "DB01",
                                                            "DF01",
                                                            "DG05",
                                                            "DC02",
                                                            "DJ01",
                                                            "DR01",
                                                            "DQ01",
                                                            "DN01",
                                                            "DL01"
                                                            };

    }
}
