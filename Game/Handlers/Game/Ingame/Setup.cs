using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Handlers.Game {
    class Setup : Networking.GameDataHandler {
        protected override void Handle() {
            if (Room.State == Enums.RoomState.Playing) {
                type = Enums.GameSubs.StartMission;

                Set(2, 3); // ?
                Set(3, 500); // ?
                Set(4, 0);
                Set(5, 1); // ? Team??

                if (!Room.Running) {
                    Room.Running = true;
                    System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(Room.Process));
                    t.Start();
                }

                selfTarget = true;
                respond = true;
            } else {
                //Player.User.Disconnect(); // Can't? We are in the lobby?
            }
        }
    }
}
