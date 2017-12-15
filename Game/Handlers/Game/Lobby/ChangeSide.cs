using System;

namespace Game.Handlers.Game {
    class ChangeSide : Networking.GameDataHandler {
        protected override void Handle() {
            if (Room.State == Enums.RoomState.Waiting) {
                TimeSpan tsDiff = DateTime.Now.Subtract(Player.LastSwitch);
                if (tsDiff.TotalMilliseconds >= 200) {
                    byte targetTeam = GetByte(2);
                    if (targetTeam < 2) {
                        if ((byte)Player.Team != targetTeam) {
                            if (Room.SwitchSide(Player)) {
                                Set(2, (byte)Player.Team);
                                Set(3, Player.Id);
                                Set(4, Room.Master);
                                respond = true;
                            } else {
                              //  Player.User.Disconnect();
                            }
                        }
                    } else {
                        Player.User.Disconnect(); // Cheating?
                    }
                }
            } else {
                Player.User.Disconnect(); // Changing packets! Cheating!
            }
        }
    }
}
