using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Handlers.Game {
    class ToggleReady : Networking.GameDataHandler {
        protected override void Handle() {
            if (Room.State == Enums.RoomState.Waiting) {
                if (Room.Master != Player.Id) {
                    Player.ToggleReady();
                    Set(2, Player.Ready);
                    respond = true;
                } else {
                    Player.User.Disconnect(); // Something went wrong?
                }
            } else {
                Player.User.Disconnect(); // Changing packets! Cheating!
            }
        }
    }
}
