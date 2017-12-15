using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Handlers.Game {
    class ChangeSubMode : Networking.GameDataHandler {
        protected override void Handle() {
            if (Room.State == Enums.RoomState.Waiting) {
                if (Room.Master == Player.Id) {
                    byte targetSetting = GetByte(2);
                    byte highestSetting = 4;

                    if (Room.Channel > Enums.ChannelType.CQC)
                        highestSetting = 5;

                    if (targetSetting > highestSetting) {
                        targetSetting = 0;
                    }

                    Room.Setting = targetSetting;
                    Set(2, Room.Setting);

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
