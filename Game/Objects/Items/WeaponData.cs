using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Objects.Items {
    class WeaponData {

        public short Power { get; private set; }
        public bool[,] Equipstate { get; private set; }
        public short[][] Powers { get; private set; }

        public WeaponData(string[] equipmentState, short power, string[] powerState) {
            this.Power = power;

            // PARSING EQUIPMENT STATE //
            this.Equipstate = new bool[(byte)Enums.Classes.COUNT, 8];
            for (byte i = 0; i < (byte)Enums.Classes.COUNT; i++) {
                string currEquipState = equipmentState[i];
                string[] strSplit =  currEquipState.Split(',');
                for (byte j = 0; j < strSplit.Length; j++) {
                    this.Equipstate[i,j] = (strSplit[j] == "1");
                }
            }

            // PARSING POWER //
            int x;
            this.Powers = new short[4][];
            for (byte j = 0; j < 4; j++) {
                x = 0;
                string[] strSplit = powerState[j].Split(',');
                this.Powers[j] = new short[strSplit.Length];
                foreach (string text in strSplit) {
                    short.TryParse(text, out this.Powers[j][x]); ++x;
                }
            }
        }
    }
}
