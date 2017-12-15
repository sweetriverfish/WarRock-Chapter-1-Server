using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Handlers.Game {
    class ChangeMode : Networking.GameDataHandler {
        protected override void Handle() {
            if (Room.State == Enums.RoomState.Waiting) {
                if (Room.Master == Player.Id) {
                    byte targetMode = GetByte(2);

                    if (targetMode > 3) {
                        targetMode = 0;
                    }

                    Room.Mode = (Enums.Mode)targetMode;
                    if (!Managers.MapManager.Instance.MapRotation[(byte)Room.Channel-1][(byte)Room.Mode].Contains(Room.Map)) {
                        Room.Map = Managers.MapManager.Instance.MapRotation[(byte)Room.Channel-1][(byte)Room.Mode].First();
                    }

                    Set(2, (byte)Room.Mode);
                    Set(3, Room.Map);
                    Set(4, (Room.Mode == Enums.Mode.Explosive) ? Room.Setting : 0);
                    Set(5, (Room.Mode != Enums.Mode.Explosive) ? Room.Setting : 0);

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
