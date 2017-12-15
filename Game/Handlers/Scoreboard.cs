using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Handlers {
    class Scoreboard : Networking.PacketHandler {
        protected override void Process(Entities.User u) {
            if (u.Authorized) {
                if (u.Room != null) {
                    if (u.Room.State == Enums.RoomState.Playing) {
                        u.Send(new Packets.Scoreboard(u.Room)); // Send scoreboard :)
                    } else {
                        //u.Disconnect(); // Sending packets in lobby? Cheating! - IGNORE
                    }
                } else {
                    //u.Disconnect(); // Player is not in a room, cheating? - IGNORE
                }
            } else {
                u.Disconnect(); // Player not authorized - cheating?
            }
        }
    }
}
