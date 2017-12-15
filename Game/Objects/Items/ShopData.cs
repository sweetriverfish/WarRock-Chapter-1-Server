using System;

namespace Game.Objects.Items {
    public struct ShopData {

        public bool IsBuyable;
        public byte RequiredLevel;
        public bool RequiresPremium;
        public int[] AddDinar;
        public int[] Cost;

        public ShopData(bool isBuyable, byte requiredLevel, bool requiresPremium, string add_dinar, string cost) {

            this.IsBuyable = isBuyable;
            this.RequiredLevel = requiredLevel;
            this.RequiresPremium = requiresPremium;
            
            int i = 0;
            string[] strSplit = add_dinar.Split(',');
            this.AddDinar = new int[strSplit.Length];
            foreach (string text in strSplit) {
                int.TryParse(text, out AddDinar[i]); ++i;
            }

            i = 0;
            strSplit = cost.Split(',');
            this.Cost = new int[strSplit.Length];
            foreach (string text in strSplit) {
                int.TryParse(text, out Cost[i]); ++i;
            }

            if (this.AddDinar.Length < 5) 
                Array.Resize(ref this.AddDinar, 5);

            if (this.Cost.Length < 5)
                Array.Resize(ref this.Cost, 5);
        }
    }
}
