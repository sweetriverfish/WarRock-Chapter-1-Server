using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Handlers.Game {
    class ToggleUserlimit : Networking.GameDataHandler {
        protected override void Handle() {

            if (Room.State == Enums.RoomState.Waiting) {
                if (Room.Master == Player.Id) {

                    if (!Room.UserLimit)
                        Set(2, 1);
                    else
                        Set(2, 0);
                    Room.UserLimit = !Room.UserLimit;
                    respond = true;
                    updateLobby = true; //dark: necessary?
                   
                } else {
                    Player.User.Disconnect(); // Cheating! You are not the game master!
                }
            } else {
                Player.User.Disconnect(); // Changing packets! Cheating!
            }
        }
    }
}
