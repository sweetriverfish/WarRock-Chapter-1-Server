using System;
using System.Collections;
using System.Linq;
using Game.Enums;

namespace Game.Handlers {
    class ChangeChannel : Networking.PacketHandler {
        protected override void Process(Entities.User u) {
            if (u.Authorized) {
                sbyte target = GetSByte(0);
                if (target >= 0 && target <= Core.Constants.maxChannelsCount) {
                    if (Enum.IsDefined(typeof(ChannelType), target)) {
                        u.SetChannel((ChannelType)target);
                        u.Send(new Packets.ChangeChannel(u.Channel));

                        var result = Managers.ChannelManager.Instance.Get(u.Channel).Rooms.Select(n => n.Value)
                            .Where(n => n.ID >= (uint)(8 * u.RoomListPage) && n.ID < (uint)(8 * (u.RoomListPage + 1))).OrderBy(n => n.ID);

                        u.Send(new Packets.RoomList(u.RoomListPage, new ArrayList(result.ToArray())));

                    } else {
                        u.Disconnect(); // Channel is not defined?
                    }
                } else {
                    u.Disconnect(); // Channel is out of range.
                }
            } else {
                u.Disconnect(); // Unauthorized user.
            }
        }
    }
}
