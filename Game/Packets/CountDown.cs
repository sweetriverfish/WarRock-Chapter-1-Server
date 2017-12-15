using Game.Enums;

namespace Game.Packets {
    class CountDown : Core.Networking.OutPacket {

        public CountDown(uint seconds)
            : base((ushort)Enums.Packets.GameCountDown) {
                Append(0);
        }
    }
}
