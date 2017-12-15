using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Handlers.Game {
    class ToggleAutostart : Networking.GameDataHandler {
        protected override void Handle() {
            if (Room.State == Enums.RoomState.Waiting) {
                if (Room.Master == Player.Id) {
                    Room.AutoStart = !Room.AutoStart;
                    Set(2, Room.AutoStart);
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
