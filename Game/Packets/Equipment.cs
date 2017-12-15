using Game.Enums;

namespace Game.Packets {
    class Equipment : Core.Networking.OutPacket {

        public enum ErrorCode : uint {
            CannotBePlaced = 0x17AF2,   // Item cannot be placed in this slot
            CannotInSlot = 0x17AF3,     // \nCan't be equipped at the slot.
            BranchService = 0x17B38,    // Item is unsuitable for this branch of the service
            AlreadyEquipped = 0x17B42,  // Item is already equipped..

        }

        public Equipment(ErrorCode errorCode)
            : base((ushort)Enums.Packets.Equipment) {
            Append((uint)errorCode);
        }

        public Equipment(Classes cClass, string equipment)
            : base((ushort)Enums.Packets.Equipment) {
            Append(Core.Constants.Error_OK);
            Append((byte)cClass);
            Append(equipment);
        }
    }
}
