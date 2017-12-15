using System;
using System.Collections;

namespace Game.Packets {
    class UpdateInventory : Core.Networking.OutPacket {
        public UpdateInventory(Entities.User u)
            : base((ushort)Enums.Packets.UpdateInventory) {
                Append(Core.Constants.Error_OK);
                Append(u.Inventory.SlotState); // The slots that are enabled
                // EQUIPMENT //
                Append(u.Inventory.Equipment.ListsInternal[0]);
                Append(u.Inventory.Equipment.ListsInternal[1]);
                Append(u.Inventory.Equipment.ListsInternal[2]);
                Append(u.Inventory.Equipment.ListsInternal[3]);
                Append(u.Inventory.Equipment.ListsInternal[4]);
                // INVENTORY //
                Append(u.Inventory.Itemlist);
                // Expired Items //
                Append(u.Inventory.ExpiredItems.Count);
                foreach (string itemCode in u.Inventory.ExpiredItems) {
                    Append(itemCode.ToUpper());
                }
        }

    }
}
