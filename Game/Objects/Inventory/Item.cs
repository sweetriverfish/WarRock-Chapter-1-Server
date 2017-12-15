using System;

namespace Game.Objects.Inventory
{
    public class Item
    {
        public sbyte Slot { get; set; }
        public uint DatabaseID { get; private set; }
        public string ItemCode { get; private set; }
        public DateTime ExpireDate { get; set; }
        public sbyte[] Equiped { get; private set; }
        public byte Type { get; private set; }
        public byte Amount { get; private set; }

        public Item(sbyte slot, uint dbId, string itemCode, uint expireDate)
        {
            this.Slot = slot;
            this.DatabaseID = dbId;
            this.ItemCode = itemCode.ToUpper();
            this.Type = (byte)(this.ItemCode.StartsWith("D") ? 1:2);
            this.ExpireDate = new DateTime(1970, 1, 1, 0, 0, 0).ToUniversalTime().AddSeconds(expireDate);
            this.Amount = 0;
            this.Equiped = new sbyte[(byte)Enums.Classes.COUNT];
            for (byte i = 0; i < (byte)Enums.Classes.COUNT; i++) {
                Equiped[i] = -1;
            }
        }
    }
}
