using System;
namespace Game.Packets {
    class ChangeChannel : Core.Networking.OutPacket {
        public ChangeChannel(Game.Enums.ChannelType channelType)
            : base((ushort)Enums.Packets.ChannelSelection) {
            Append(Core.Constants.Error_OK);
            Append((sbyte)channelType);
        }
    }
}
