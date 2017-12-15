using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Handlers.Game {
    public class ChangeMap : Networking.GameDataHandler {
        protected override void Handle() {
            if (Player.Id == Room.Master) {
                if (Room.State == Enums.RoomState.Waiting) {
                    byte targetMap = GetByte(2);
                    if (Managers.MapManager.Instance.MapRotation[((byte)Room.Channel - 1)][(byte)Room.Mode].Contains(targetMap)) {
                        Room.Map = targetMap;
                        updateLobby = respond = true;
                    } else {
                        Log.Instance.WriteLine("The index of the map is:" + targetMap.ToString());
                       // Player.User.Disconnect(); // Cheating? - Sending wrong map in the rotation list?
                    }
                } else {
                    // Cheating or lagging?
                }
            } else {
                Player.User.Disconnect(); // Cheating? - Changing map without being the master?
            }
        }
    }
}
