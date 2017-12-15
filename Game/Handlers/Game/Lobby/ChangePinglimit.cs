using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Handlers.Game {
    class ChangePinglimit : Networking.GameDataHandler {
        protected override void Handle() {
            if (Room.State == Enums.RoomState.Waiting) {
                if (Room.Master == Player.Id) {
                    byte targetPing = GetByte(2);

                    if (targetPing > 2) {
                        targetPing = 0;
                    }

                    Room.PingLimit = targetPing;
                    Set(2, Room.PingLimit);

                    respond = true;
                    updateLobby = true;
                } else {
                    // Cheating! You are not the game master!
                }
            } else {
                Player.User.Disconnect(); // Changing packets! Cheating!
            }
        }
    }
}
