namespace Game.Objects.Items {
    public class ItemData {

        public uint dbId { get; private set; }
        public string Code { get; private set; }
        public string Name { get; private set; }
        public bool Active { get; private set; }
        public bool IsWeapon { get; protected set; }
        public ShopData Shop { get; private set; }

        public ItemData(uint dbId, string code, string name, bool active, ShopData shop) {
            this.dbId = dbId;
            this.Code = code;
            this.Name = name;
            this.Active = active;
            this.Shop = shop;
            this.IsWeapon = false;
        }
    }
}
