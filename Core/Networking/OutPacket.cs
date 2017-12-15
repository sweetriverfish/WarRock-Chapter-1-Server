using System;
using System.Text;

namespace Core.Networking {
    public abstract class OutPacket {
        private byte xOrKey = 0x96; // Server -> Client
        private StringBuilder builder;

        public OutPacket(ushort packetId, byte xOrKey) {
            this.xOrKey = xOrKey;
            builder = new StringBuilder();
            Append(Environment.TickCount);
            Append(packetId);
        }

        public OutPacket(ushort packetId)
        : this(packetId, Constants.xOrKeySend){
            // Handled by the other constructor.
        }

        public void Fill(byte count, bool data) {
            for (byte i = 0; i < count; i++) {
                Append(data);
            }
        }

        public void Fill(byte count, double data) {
            for (byte i = 0; i < count; i++) {
                Append(data);
            }
        }

        public void Fill(byte count, int data) {
            for (byte i = 0; i < count; i++) {
                Append(data);
            }
        }

        public void Fill(byte count, uint data) {
            for (byte i = 0; i < count; i++) {
                Append(data);
            }
        }

        public void Fill(byte count, long data) {
            for (byte i = 0; i < count; i++) {
                Append(data);
            }
        }

        public void Fill(byte count, ulong data) {
            for (byte i = 0; i < count; i++) {
                Append(data);
            }
        }

        public void Fill(byte count, byte data) {
            for (byte i = 0; i < count; i++) {
                Append(data);
            }
        }

        public void Fill(byte count, sbyte data) {
            for (byte i = 0; i < count; i++) {
                Append(data);
            }
        }

        public void Fill(byte count, string data) {
            for (byte i = 0; i < count; i++) {
                Append(data);
            }
        }

        public void Append(bool data) {
            builder.Append(data ? 1 : 0);
            builder.Append(" ");
        }

        public void Append(double data) {
            builder.Append(data);
            builder.Append(" ");
        }

        public void Append(int data) {
            builder.Append(data);
            builder.Append(" ");
        }

        public void Append(uint data) {
            builder.Append(data);
            builder.Append(" ");
        }

        public void Append(long data) {
            builder.Append(data);
            builder.Append(" ");
        }

        public void Append(ulong data) {
            builder.Append(data);
            builder.Append(" ");
        }

        public void Append(byte data) {
            builder.Append(data);
            builder.Append(" ");
        }

        public void Append(sbyte data) {
            builder.Append(data);
            builder.Append(" ");
        }

        public void Append(string data) {
            builder.Append(data.Replace(' ', (char)(0x1D)));
            builder.Append(" ");
        }

        public string Build() {
            string strOutput = builder.ToString();
        //    Console.WriteLine("OUT :: " + strOutput);
            return string.Concat(strOutput, (char)(0x0A));
        }

        public byte[] BuildEncrypted() {
            byte[] buffer = Encoding.UTF8.GetBytes(this.Build());

            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = (byte)(buffer[i] ^ xOrKey);

            return buffer;
        }
    }
}
