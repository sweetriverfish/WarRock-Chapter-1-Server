using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Files {
    public abstract class BinHandler {
        protected Hashtable htDecryptBuffer;
        protected List<byte> lstbOutputBuffer;

        protected BinHandler() {
            InitializeHexTable();
            lstbOutputBuffer = new List<byte>();
        }

        private void InitializeHexTable() {
            this.htDecryptBuffer = new Hashtable();
            for (int i = 0; i <= 255; i++) {
                this.htDecryptBuffer.Add(i.ToString("X2").PadLeft(2, '0'), System.Convert.ToByte(i ^ 215));
            }
        }
    }
}
