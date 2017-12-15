using System;
namespace Game.Networking {
    public abstract class PacketHandler {
        public Core.Networking.InPacket InPacket { get; private set;}

        public void Handle(Core.Networking.InPacket inPacket) {
            this.InPacket = inPacket;

            object attachment = inPacket.Attachment;
            if (attachment is Entities.User) {
                this.Process((Entities.User)inPacket.Attachment);
            } else if (attachment is ServerClient) {
                this.Process((ServerClient)inPacket.Attachment);
            } else {
                Log.Instance.WriteLine("Unknown packet attachment!");
            }
        }

        protected virtual void Process(Entities.User u) { }
        protected virtual void Process(Networking.ServerClient s) { }

        protected string GetString(byte index) {
            if (index < InPacket.Blocks.Length) {
                return InPacket.Blocks[index];
            }
            return string.Empty;
        }

        protected bool GetBool(byte index) {
            if (index < InPacket.Blocks.Length) {
                return (byte.Parse(InPacket.Blocks[index]) > 0);
            }
            return false;
        }

        protected int GetInt(byte index) {
            if (index < InPacket.Blocks.Length) {
                return int.Parse(InPacket.Blocks[index]);
            }
            return 0;
        }

        protected uint GetuInt(byte index) {
            if (index < InPacket.Blocks.Length) {
                return uint.Parse(InPacket.Blocks[index]);
            }
            return 0;
        }

        protected byte GetByte(byte index) {
            if (index < InPacket.Blocks.Length) {
                return byte.Parse(InPacket.Blocks[index]);
            }
            return 0;
        }

        protected sbyte GetSByte(byte index) {
            if (index < InPacket.Blocks.Length) {
                return sbyte.Parse(InPacket.Blocks[index]);
            }
            return 0;
        }

        protected short GetShort(byte index) {
            if (index < InPacket.Blocks.Length) {
                return short.Parse(InPacket.Blocks[index]);
            }
            return 0;
        }

        protected ushort GetUShort(byte index) {
            if (index < InPacket.Blocks.Length) {
                return ushort.Parse(InPacket.Blocks[index]);
            }
            return 0;
        }

    }
}
