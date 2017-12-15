using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Handlers.Game.Ingame {
    class PlayerDamage : Networking.GameDataHandler {
        protected override void Handle() {
            if (Room.State == Enums.RoomState.Playing) {
                if (packet.Blocks.Length == 27) {
                    Room.CurrentGameMode.OnDamage(this);
                } else {
                    Player.User.Disconnect();
                }
            }
        }
    }
}
