using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace Game.Objects.Inventory
{
    public class Equipment
    {
        public const byte MAX_CLASSES = 5;
        public const byte MAX_SLOTS = 8;

        private readonly Entities.User User;

        public readonly ConcurrentDictionary<byte, Item> Engeneer;
        public readonly ConcurrentDictionary<byte, Item> Medic;
        public readonly ConcurrentDictionary<byte, Item> Sniper;
        public readonly ConcurrentDictionary<byte, Item> Assault;
        public readonly ConcurrentDictionary<byte, Item> Heavy;

        public string[] Lists { get; private set; }
        public string[] ListsInternal { get; private set; }

        public Equipment(Entities.User u)
        {
            User = u;

            Engeneer = new ConcurrentDictionary<byte, Item>();
            Medic = new ConcurrentDictionary<byte, Item>();
            Sniper = new ConcurrentDictionary<byte, Item>();
            Assault = new ConcurrentDictionary<byte, Item>();
            Heavy = new ConcurrentDictionary<byte, Item>();

            this.Lists = new string[MAX_CLASSES];
            this.ListsInternal = new string[MAX_CLASSES];
        }

        public bool Add(Enums.Classes Class, byte slot, Item item)
        {
            bool isAdded = false;
            Remove(Class, slot);
            switch (Class)
            {
                case Enums.Classes.Engineer:
                    {
                        if (!Engeneer.ContainsKey(slot))
                            isAdded = Engeneer.TryAdd(slot, item);

                        break;
                    }
                case Enums.Classes.Medic:
                    {
                        if (!Medic.ContainsKey(slot))
                            isAdded = Medic.TryAdd(slot, item);

                        break;
                    }
                case Enums.Classes.Sniper:
                    {
                        if (!Sniper.ContainsKey(slot))
                            isAdded = Sniper.TryAdd(slot, item);

                        break;
                    }
                case Enums.Classes.Assault:
                    {
                        if (!Assault.ContainsKey(slot))
                            isAdded = Assault.TryAdd(slot, item);

                        break;
                    }
                case Enums.Classes.Heavy:
                    {
                        if (!Heavy.ContainsKey(slot))
                            isAdded = Heavy.TryAdd(slot, item);

                        break;
                    }
            }

            if (isAdded)
                item.Equiped[(byte)Class] = (sbyte)slot;

            return isAdded;
        }

        public Item Get(Enums.Classes Class, string weaponCode)
        {
            Item output = null;
            switch (Class)
            {
                case Enums.Classes.Engineer:
                    {
                        try
                        {
                            output = Engeneer.Select(n => n.Value).Where(n => n.ItemCode == weaponCode).First();
                        }
                        catch { output = null; }
                        break;
                    }
                case Enums.Classes.Medic:
                    {
                        try
                        {
                            output = Medic.Select(n => n.Value).Where(n => n.ItemCode == weaponCode).First();
                        }
                        catch { output = null; }
                        break;
                    }
                case Enums.Classes.Sniper:
                    {
                        try
                        {
                            output = Sniper.Select(n => n.Value).Where(n => n.ItemCode == weaponCode).First();
                        }
                        catch { output = null; }
                        break;
                    }
                case Enums.Classes.Assault:
                    {
                        try
                        {
                            output = Assault.Select(n => n.Value).Where(n => n.ItemCode == weaponCode).First();
                        }
                        catch { output = null; }
                        break;
                    }
                case Enums.Classes.Heavy:
                    {
                        try
                        {
                            output = Heavy.Select(n => n.Value).Where(n => n.ItemCode == weaponCode).First();
                        }
                        catch { output = null; }
                        break;
                    }
            }

            return output;
        }

        public Item Get(Enums.Classes Class, byte slot)
        {
            Item output = null;
            switch (Class)
            {
                case Enums.Classes.Engineer:
                    {
                        if (Engeneer.ContainsKey(slot))
                            Engeneer.TryGetValue(slot, out output);

                        break;
                    }
                case Enums.Classes.Medic:
                    {
                        if (Medic.ContainsKey(slot))
                            Medic.TryGetValue(slot, out output);

                        break;
                    }
                case Enums.Classes.Sniper:
                    {
                        if (Sniper.ContainsKey(slot))
                            Sniper.TryGetValue(slot, out output);

                        break;
                    }
                case Enums.Classes.Assault:
                    {
                        if (Assault.ContainsKey(slot))
                            Assault.TryGetValue(slot, out output);

                        break;
                    }
                case Enums.Classes.Heavy:
                    {
                        if (Heavy.ContainsKey(slot))
                            Heavy.TryGetValue(slot, out output);

                        break;
                    }
            }

            return output;
        }

        public void Remove(Enums.Classes Class, byte slot)
        {
            Item itm = null;
            switch (Class)
            {
                case Enums.Classes.Engineer:
                    {
                        if (Engeneer.ContainsKey(slot))
                            Engeneer.TryRemove(slot, out itm);

                        break;
                    }
                case Enums.Classes.Medic:
                    {
                        if (Medic.ContainsKey(slot))
                            Medic.TryRemove(slot, out itm);

                        break;
                    }
                case Enums.Classes.Sniper:
                    {
                        if (Sniper.ContainsKey(slot))
                            Sniper.TryRemove(slot, out itm);

                        break;
                    }
                case Enums.Classes.Assault:
                    {
                        if (Assault.ContainsKey(slot))
                            Assault.TryRemove(slot, out itm);

                        break;
                    }
                case Enums.Classes.Heavy:
                    {
                        if (Heavy.ContainsKey(slot))
                            Heavy.TryRemove(slot, out itm);

                        break;
                    }
            }

            if (itm != null)
                itm.Equiped[(byte)Class] = -1;

        }

        public string[] BuildInternal()
        {
            ListsInternal = new string[MAX_CLASSES];

            for (byte i = 0; i < MAX_SLOTS; i++)
            {

                if (Engeneer.ContainsKey(i))
                {
                    Item itm = Engeneer.Select(n => n).Where(n => n.Key == i).Select(n => n.Value).First();
                    ListsInternal[(byte)Enums.Classes.Engineer] += (itm.Slot == -1 ? itm.ItemCode.ToUpper() : String.Format("I{0:000}", itm.Slot)) + ",";
                }
                else
                    ListsInternal[(byte)Enums.Classes.Engineer] += "^,";

                if (Medic.ContainsKey(i))
                {
                    Item itm = Medic.Select(n => n).Where(n => n.Key == i).Select(n => n.Value).First();
                    ListsInternal[(byte)Enums.Classes.Medic] += (itm.Slot == -1 ? itm.ItemCode.ToUpper() : String.Format("I{0:000}", itm.Slot)) + ",";
                }
                else
                    ListsInternal[(byte)Enums.Classes.Medic] += "^,";

                if (Sniper.ContainsKey(i))
                {
                    Item itm = Sniper.Select(n => n).Where(n => n.Key == i).Select(n => n.Value).First();
                    ListsInternal[(byte)Enums.Classes.Sniper] += (itm.Slot == -1 ? itm.ItemCode.ToUpper() : String.Format("I{0:000}", itm.Slot)) + ",";
                }
                else
                    ListsInternal[(byte)Enums.Classes.Sniper] += "^,";

                if (Assault.ContainsKey(i))
                {
                    Item itm = Assault.Select(n => n).Where(n => n.Key == i).Select(n => n.Value).First();
                    ListsInternal[(byte)Enums.Classes.Assault] += (itm.Slot == -1 ? itm.ItemCode.ToUpper() : String.Format("I{0:000}", itm.Slot)) + ",";
                }
                else
                    ListsInternal[(byte)Enums.Classes.Assault] += "^,";

                if (Heavy.ContainsKey(i))
                {
                    Item itm = Heavy.Select(n => n).Where(n => n.Key == i).Select(n => n.Value).First();
                    ListsInternal[(byte)Enums.Classes.Heavy] += (itm.Slot == -1 ? itm.ItemCode.ToUpper() : String.Format("I{0:000}", itm.Slot)) + ",";
                }
                else
                    ListsInternal[(byte)Enums.Classes.Heavy] += "^,";
            }

            for (byte j = 0; j < 5; j++)
            {
                ListsInternal[j] = ListsInternal[j].Remove(ListsInternal[j].Length - 1);
            }

            return ListsInternal;
        }

        public string[] Build()
        {
            this.Lists = new string[MAX_CLASSES];

            for (byte i = 0; i < MAX_SLOTS; i++)
            {

                if (Engeneer.ContainsKey(i))
                    Lists[(byte)Enums.Classes.Engineer] += Engeneer.Select(n => n).Where(n => n.Key == i).Select(n => n.Value.ItemCode.ToUpper()).First() + ",";
                else
                    Lists[(byte)Enums.Classes.Engineer] += "^,";

                if (Medic.ContainsKey(i))
                    Lists[(byte)Enums.Classes.Medic] += Medic.Select(n => n).Where(n => n.Key == i).Select(n => n.Value.ItemCode.ToUpper()).First() + ",";
                else
                    Lists[(byte)Enums.Classes.Medic] += "^,";

                if (Sniper.ContainsKey(i))
                    Lists[(byte)Enums.Classes.Sniper] += Sniper.Select(n => n).Where(n => n.Key == i).Select(n => n.Value.ItemCode.ToUpper()).First() + ",";
                else
                    Lists[(byte)Enums.Classes.Sniper] += "^,";

                if (Assault.ContainsKey(i))
                    Lists[(byte)Enums.Classes.Assault] += Assault.Select(n => n).Where(n => n.Key == i).Select(n => n.Value.ItemCode.ToUpper()).First() + ",";
                else
                    Lists[(byte)Enums.Classes.Assault] += "^,";

                if (Heavy.ContainsKey(i))
                    Lists[(byte)Enums.Classes.Heavy] += Heavy.Select(n => n).Where(n => n.Key == i).Select(n => n.Value.ItemCode.ToUpper()).First() + ",";
                else
                    Lists[(byte)Enums.Classes.Heavy] += "^,";
            }

            for (byte j = 0; j < 5; j++)
            {
                Lists[j] = Lists[j].Remove(Lists[j].Length - 1);
            }

            // SAVE LISTS TO THE DATABASE // // TODO: Implement better query.
            Databases.Game.Query(string.Concat("UPDATE user_equipment SET `class1` =  '", Lists[0], "', `class2` =  '", Lists[1], "', `class3` =  '", Lists[2], "', `class4` =  '", Lists[3], "', `class5` =  '", Lists[4], "' WHERE owner =", User.ID, ";"));

            return Lists;
        }

    }
}
