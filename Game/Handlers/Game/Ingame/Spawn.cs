using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Handlers.Game.Ingame {
    class Spawn : Networking.GameDataHandler {
        protected override void Handle() {
            if (Room.State == Enums.RoomState.Playing) { //TODO: Implement more checks :)
                if (Player.CanSpawn) {
                    respond = true;
                    // 8, 9 ,10
                    int spawnSlot = Room.CurrentGameMode.SpawnSlot();
                    Set(7, spawnSlot);
                    Set(8, spawnSlot);
                    Set(9, spawnSlot);
                    Player.Spawn((Enums.Classes)GetByte(3));
                }
            }
        }
    }
}
