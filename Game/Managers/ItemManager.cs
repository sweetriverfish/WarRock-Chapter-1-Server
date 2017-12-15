using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game.Objects.Items;

using MySql.Data.MySqlClient;

namespace Game.Managers {
    class ItemManager {

        public ConcurrentDictionary<string, ItemData> Items { get; private set; }

        public ItemManager() {
            Items = null;
        }

        public bool Load() {

            ConcurrentDictionary<string, ItemData> temp = new ConcurrentDictionary<string, ItemData>();

            try {
                MySqlCommand cmd = new MySqlCommand("SELECT details.id, details.code, details.active, details.name, shop.buyable, shop.req_level, shop.req_premium, shop.add_dinar, shop.cost, equipment.class1, equipment.class2, equipment.class3, equipment.class4, equipment.class5, target_info.power, target_info.personal, target_info.surface, target_info.ship, target_info.air FROM item_weapons AS details LEFT JOIN item_shop shop ON shop.code=details.code LEFT JOIN item_equipment equipment on equipment.code=details.code LEFT JOIN item_target_info target_info on target_info.code=details.code", Databases.Game.connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows) {
                    while (reader.Read()) {

                        // BASIC INFO //
                        int id = reader.GetInt32("id");
                        string code = reader.GetString("code");
                        bool isActive = reader.GetBoolean("active");
                        string name = reader.GetString("name");

                        // SHOP INFO //
                        bool isBuyable = reader.GetBoolean("buyable");
                        byte requiredlevel = reader.GetByte("req_level");
                        bool requiresPremium = reader.GetBoolean("req_premium");
                        string add_dinar = reader.GetString("add_dinar");
                        string cost = reader.GetString("cost");

                        // EQUIPMENT //
                        string class1 = reader.GetString("class1");
                        string class2 = reader.GetString("class2");
                        string class3 = reader.GetString("class3");
                        string class4 = reader.GetString("class4");
                        string class5 = reader.GetString("class5");

                        string[] equipment = { class1, class2, class3, class4, class5 };

                        // WEAPON DATA //
                        short power = reader.GetInt16("power");
                        string personal = reader.GetString("personal");
                        string surface = reader.GetString("surface");
                        string ship = reader.GetString("ship");
                        string air = reader.GetString("air");

                        string[] weaponPower = { personal, surface, ship, air }; 

                        temp.TryAdd(code.ToUpper(), new Weapon((uint)id, code, name, isActive,
                            new ShopData(isBuyable, requiredlevel, requiresPremium, add_dinar, cost),
                            new WeaponData(equipment, power, weaponPower)));
                    }
                }

                reader.Close();
                cmd = new MySqlCommand("SELECT details.id, details.code, details.active, details.name, shop.buyable, shop.req_level, shop.req_premium, shop.add_dinar, shop.cost FROM item_extra AS details LEFT JOIN item_shop shop ON shop.code=details.code", Databases.Game.connection);
                reader = cmd.ExecuteReader();
                if (reader.HasRows) {
                    while (reader.Read()) {

                        // BASIC INFO //
                        int id = reader.GetInt32("id");
                        string code = reader.GetString("code");
                        bool isActive = reader.GetBoolean("active");
                        string name = reader.GetString("name");

                        // SHOP INFO //
                        bool isBuyable = reader.GetBoolean("buyable");
                        byte requiredlevel = reader.GetByte("req_level");
                        bool requiresPremium = reader.GetBoolean("req_premium");
                        string add_dinar = reader.GetString("add_dinar");
                        string cost = reader.GetString("cost");

                        temp.TryAdd(code.ToUpper(), new ItemData((uint)id, code, name, isActive,
                            new ShopData(isBuyable, requiredlevel, requiresPremium, add_dinar, cost)));
                    }
                }

                reader.Close();

                this.Items = temp;

                return true;
            } catch {

            }

            return false;
        }


        // OLD

        public bool Import() {

            Core.Files.Bin.TypedObject itemData = null;
            Core.Files.Bin.TypedObject branchData = null;

            Core.Files.BinReader itemFile = new Core.Files.BinReader("D:\\Warrock-Dev\\Data\\items.bin");
            Core.Files.BinReader branchFile = new Core.Files.BinReader("D:\\Warrock-Dev\\Data\\branch.bin");

            if (itemFile != null) {
                if (itemFile.Read() && branchFile.Read()) {
                    itemData = itemFile.Data;
                    branchData = branchFile.Data;

                    Core.Files.Bin.TypedObject itemList = itemData.GetTO("item data");

                    Core.Files.Bin.TypedObject weaponList = itemList.GetTO("weapon");
                    Core.Files.Bin.TypedObject extraList = itemList.GetTO("etc");

                    for (byte i = 1; i <= weaponList.Count; i++) {
                        Core.Files.Bin.TypedObject weapon = weaponList.GetTO((int)i);

                        string query = "";
                        byte id = i;
                        bool active = weapon.GetTO("basic_info").GetString("active").ToLower() == "true";
                        string name = weapon.GetTO("basic_info").GetString("english");
                        string code = weapon.GetTO("basic_info").GetString("code");

                        query = string.Concat("INSERT INTO item_weapons (`id`, `code`, `active`, `name`) VALUES ('", id-1, "', '", code.ToUpper(), "', '", active ? 1 : 0, "', '", name, "');");

                        if (weapon.ContainsKey("target_info")) {

                            int power = weapon.GetTO("ability_info").GetInt("power");

                            string personal = weapon.GetTO("target_info").GetString("personal");
                            string surface = weapon.GetTO("target_info").GetString("surface");
                            string ship = weapon.GetTO("target_info").GetString("ship");
                            string air = weapon.GetTO("target_info").GetString("air");

                            query += string.Concat("INSERT INTO item_target_info (`code`, `power`, `personal`, `surface`, `ship`, `air`) VALUES ('", code.ToUpper(), "', '", power, "', '", personal, "', '", surface, "', '", ship, "', '", air, "');");
                        }

                        if (weapon.ContainsKey("buy_info")) {

                            bool buyable = weapon.GetTO("buy_info").GetString("buyable").ToLower() == "true";
                            bool reqPremium = weapon.GetTO("buy_info").GetString("req_bp") != "0";

                            int reqLevel = weapon.GetTO("buy_info").GetInt("req_level");
                            string add_dinar = weapon.GetTO("buy_info").GetString("add_dinar");
                            string cost = weapon.GetTO("buy_info").GetString("cost");

                            if (add_dinar == "0")
                                add_dinar = "-1,-1,-1,-1,-1";


                            if (cost == "0")
                                cost = "-1,-1,-1,-1,-1";



                            query += string.Concat("INSERT INTO item_shop (`code`, `buyable`, `req_level`, `req_premium`, `add_dinar`, `cost`) VALUES ('", code.ToUpper(), "', '", buyable ? 1 : 0, "', '", reqLevel, "', '", reqPremium ? 1 : 0, "', '", add_dinar, "', '", cost, "');");
                        }

                        // SCAN EQUIPMENT //

                        string equipmentCode = code.ToUpper().Substring(1, 1);
                        int[] engeneer = new int[7], medic = new int[7], sniper = new int[7], assault = new int[7], heavy = new int[7];

                        for (byte j = 0; j < 7; j++) {
                            string key = string.Concat(j, "slotcode");

                            string[] classes = { "engineer", "medic", "patrol", "assult", "antitank" };

                            foreach (string sClass in classes) {
                                string classKeys = branchData.GetTO(sClass).GetTO("slot info").GetString(key);

                                int val = classKeys.Contains(equipmentCode) == true ? 1 : 0;

                                switch (sClass) {
                                    case "engineer": {
                                            engeneer[j] = val;
                                            break;
                                        }
                                    case "medic": {
                                            medic[j] = val;
                                            break;
                                        }
                                    case "patrol": {
                                            sniper[j] = val;
                                            break;
                                        }
                                    case "assult": {
                                            assault[j] = val;
                                            break;
                                        }
                                    case "antitank": {
                                            heavy[j] = val;
                                            break;
                                        }
                                }
                            }

                        }

                        string sEngeneer = string.Join(",", engeneer);
                        string sMedic = string.Join(",", medic);
                        string sSniper = string.Join(",", sniper);
                        string sAssault = string.Join(",", assault);
                        string sHeavy = string.Join(",", heavy);


                        query += string.Concat("INSERT INTO item_equipment (`code`, `class1`, `class2`, `class3`, `class4`, `class5`) VALUES ('", code.ToUpper(), "', '", sEngeneer, "', '", sMedic, "', '", sSniper, "', '", sAssault, "', '", sHeavy, "');");
                        Databases.Game.Query(query);
                    }

                    for (byte i = 1; i <= extraList.Count; i++) {
                        Core.Files.Bin.TypedObject extra = extraList.GetTO((int)i);

                        string query = "";
                        byte id = i;
                        bool active = extra.GetTO("basic_info").GetString("active").ToLower() == "true";
                        string name = extra.GetTO("basic_info").GetString("english");
                        string code = extra.GetTO("basic_info").GetString("code");

                        query = string.Concat("INSERT INTO item_extra (`id`, `code`, `active`, `name`) VALUES ('", id-1, "', '", code.ToUpper(), "', '", active ? 1 : 0, "', '", name, "');");

                        if (extra.ContainsKey("buy_info")) {

                            bool buyable = extra.GetTO("buy_info").GetString("buyable").ToLower() == "true";
                            bool reqPremium = extra.GetTO("buy_info").GetString("req_bp") != "0";

                            int reqLevel = extra.GetTO("buy_info").GetInt("req_level");
                            string add_dinar = extra.GetTO("buy_info").GetString("add_dinar");
                            string cost = extra.GetTO("buy_info").GetString("cost");

                            if (add_dinar == "0")
                                add_dinar = "-1,-1,-1,-1,-1";


                            if (cost == "0")
                                cost = "-1,-1,-1,-1,-1";

                            query += string.Concat("INSERT INTO item_shop (`code`, `buyable`, `req_level`, `req_premium`, `add_dinar`, `cost`) VALUES ('", code.ToUpper(), "', '", buyable ? 1 : 0, "', '", reqLevel, "', '", reqPremium ? 1 : 0, "', '", add_dinar, "', '", cost, "');");
                        }
                        
                        Databases.Game.Query(query);
                    }

                    return true;
                }
            }

            return false;
        }

        private static ItemManager instance;
        public static ItemManager Instance { get { if (instance == null) { instance = new ItemManager(); } return instance; } }
    }
}
