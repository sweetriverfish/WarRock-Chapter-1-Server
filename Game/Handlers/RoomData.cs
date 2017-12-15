using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Handlers {
    class RoomData : Networking.PacketHandler {
        protected override void Process(Entities.User u) {
            if (u.Authorized && u.Room != null) {
                // [0] = ROOM SLOT
                // [1] = ROOM ID
                byte roomSlot = GetByte(0);
                if (roomSlot < u.Room.MaximumPlayers || u.SpectatorId != -1) {
                    uint roomId = GetuInt(1);
                    if (roomId == u.Room.ID) {
                        byte unknown = GetByte(2); // Seems to be 2 or 0?
                        ushort subType = GetUShort(3);
                        // HANDLE PACKET IN A SEPERATED CLASS //
                        Networking.GameDataHandler handler = Managers.PacketManager.Instance.GetHandler(subType);
                        if (handler != null) {
                            try {
                                handler.Process(u, this.InPacket);
                            } catch { /* error? */ }
                        } else {
                            Log.Instance.WriteBoth("UNKNOWN SUBPACKET :: " + this.InPacket.fullPacket);
                        }

                    } else {
                        u.Disconnect(); // Wrong room targeted - Cheating?
                    }
                } else {
                    u.Disconnect(); // Room slot over maximum players - Cheating?
                }
            }
        }

    
    }
}
