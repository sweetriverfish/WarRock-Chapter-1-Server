using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Packets {
    class Itemshop: Core.Networking.OutPacket {

        public enum ErrorCodes : uint {
            PremiumOnly = 98010,        // Available to Premium users only.
            GoldPremiumOnly = 98020,    // Available to Gold users only.
            Slot5IsFree = 98030,        // 5th Slot is free for Gold user.
            InvalidItem = 97010,        // Item is no longer valid
            Slot5Required = 97012,      // You must purchase 5th slot first.
            Slot5RequiredTime = 97015,  // Insufficient slot time.
            CannotBeBougth = 97020,     // Item cannot be bought
            NotEnoughDinar = 97040,     // Insufficient balance
            LevelUnsuitable = 97050,    // Your level is unsuitable
            LevelRequirement = 97060,   // You do not meet the level requirements\n to purchase this weapon.
            InventoryFull = 97070,      // Your inventory is full
            ExceededLeasePeriod = 97080,// Cannot purchase. You have exceeded maximum lease period.
            CannotPurchaseTwice = 97090 // You cannot purchase the item twice.
        }

        public Itemshop(Entities.User u)
            : base((ushort)Enums.Packets.Itemshop) {
                Append((byte)Core.Constants.Error_OK);
                Append((ushort)Enums.ItemAction.BuyItem);
                Append(-1);
                Append(3);
                Append(u.Inventory.Items.Count);
                Append(u.Inventory.Itemlist);
                Append(u.Money);
                Append(u.Inventory.SlotState);
        }

        public Itemshop(ErrorCodes errorCode)
            : base((ushort)Enums.Packets.Itemshop) {
                Append((uint)errorCode);
        }
    }
}
