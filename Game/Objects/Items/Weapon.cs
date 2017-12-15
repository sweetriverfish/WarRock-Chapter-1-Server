using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Objects.Items {
    class Weapon : ItemData{

        public bool[,] CanEquip { get; private set; }
        public short[] PowerPersonal { get; private set; }
        public short[] PowerSurface { get; private set; }
        public short[] PowerShip { get; private set; }
        public short[] PowerAir { get; private set; }

        public short Power { get; set; }

        public Weapon(uint dbId, string code, string name, bool active, ShopData shop, WeaponData weaponData)
            : base(dbId, code, name, active, shop) {
                this.IsWeapon = true;

                this.Power = weaponData.Power;
                this.PowerPersonal = weaponData.Powers[0];
                this.PowerSurface = weaponData.Powers[1];
                this.PowerShip = weaponData.Powers[2];
                this.PowerAir = weaponData.Powers[3];

                this.CanEquip = weaponData.Equipstate;

                weaponData = null;
        }

        public bool CanEquip6Th {
            get {
                for (byte i = 0; i < (byte)Enums.Classes.COUNT; i++) {
                    if (CanEquip[i, 5]) return true;
                }
                return false;
            }
        }
    }
}
