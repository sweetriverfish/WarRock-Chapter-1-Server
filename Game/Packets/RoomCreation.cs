namespace Game.Packets {
    class RoomCreation : Core.Networking.OutPacket {
        public RoomCreation(Enums.RoomCreationErrors errorCode)
            : base((ushort)Enums.Packets.RoomCreation) {
                Append((uint)errorCode);
        }

        public RoomCreation(Entities.Room r)
            : base((ushort)Enums.Packets.RoomCreation) {
            Append(Core.Constants.Error_OK);
            Append(0);
            addRoomInfo(r);
        }

        private void addRoomInfo(Entities.Room r) {
            // ROOM INFORMATION //
            Append(r.ID);                           // Room Id
            Append(1);                              // ?
            Append((byte)r.State);                  // Room state: 1 = Waiting | 2 = Playing
            Append(r.Master);                       // The slot index of the room master.
            Append(r.Displayname);                  // The name of the room that will displayed in the roomlist.
            Append(r.HasPassword);                  // Tells the client if the room has a password.
            Append(r.MaximumPlayers);               // The maximum player count that is allowed in the room.
            Append(r.Players.Count);                // The current player count that is currently in the room.
            Append(r.Map);                          // The current map that is selected.
            Append((r.Mode == 0) ? r.Setting : 0);  // CQC Rounds
            Append((r.Mode > 0) ? r.Setting : 0);   // FFA & TDM Kills
            Append(0);                              // Time left?
            Append((byte)r.Mode);                   // The current game mode of the room.
            Append(4);                              // ?
            Append(r.IsJoinable);                   // This is the join state of the room. 0 = Unjoinable | 1 = Joinable
            Append(0);                              // ?
            Append(r.Supermaster);                  // Does the room have the supermaster buf enabled? 0 = NO | 1 = YES
            Append(r.Type);                              // Room Type, Unused in chapter 1.
            Append(r.LevelLimit);                   // This is the value of the level limit.
            Append(r.FriendlyFire);                  // Indicates if the room is a premium only room. 0 = NO | 1 = YES
            Append(r.EnableVoteKick);               // Indicates if the vote kick function is enabled. 0 = NO | 1 = YES
            Append(r.AutoStart);                    // Indicates if the auto start function is enabled. 0 = NO | 1 = YES
            Append(0);                              // This was a number of average ping (before patch G1-17).
            Append(r.PingLimit);                    // This is the value of the ping limit. 1 = GREEN , 2 = YELLOW, 3 = ALL.

            // CLAN BLOCK
            Append(r.isClanWar);                 // Is clan war? -1 = NO  >0 = YES
            
        }

    }
}
