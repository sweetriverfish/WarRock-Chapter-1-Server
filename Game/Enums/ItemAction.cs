namespace Game.Enums {
    public enum ItemAction : ushort {
        BuyItem = 0x456,    // Item has been purchased!
        UseItem = 0x457,    // Item use successful.
        RemoveItem = 0x458  // An expired term item has been removed.
    }
}
