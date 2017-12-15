using System;
using System.Linq;
using System.Collections;

namespace Game.Handlers {
    class RoomList : Networking.PacketHandler {
        protected override void Process(Entities.User u) {
            if (u.Authorized) {
                if (u.Room == null) {
                    sbyte direction = (sbyte)(GetSByte(0) - GetSByte(2));
                    bool waitingOnly = GetBool(1);

                    if (direction == -1) {
                        if (u.RoomListPage > 0)
                            u.RoomListPage = (byte)(u.RoomListPage - 1);
                        else
                            u.RoomListPage = 0;
                    } else if (direction == 1 && u.RoomListPage < byte.MaxValue) {
                        u.RoomListPage += 1;
                    }

                    var result = Managers.ChannelManager.Instance.Get(u.Channel).Rooms.Select(n => n.Value);

                    if (waitingOnly)
                        result = result.Where(n => n.State == Enums.RoomState.Waiting).OrderByDescending(n => n.ID).Take(8).OrderBy(n => n.ID);
                    else
                        result = result.Where(n => n.ID >= (uint)(8 * u.RoomListPage) && n.ID < (uint)(8 * (u.RoomListPage + 1))).OrderBy(n => n.ID);

                    u.Send(new Packets.RoomList(u.RoomListPage, new ArrayList(result.ToArray())));
                }
            } else {
                u.Disconnect(); // Unauthorized user.
            }
        }
    }
}
