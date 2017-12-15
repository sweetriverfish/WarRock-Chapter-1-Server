using System;
using System.Collections.Generic;
using System.Linq;
using Game.Enums;

namespace Game.Handlers.Game.Ingame
{
    sealed class FlagCapture : Networking.GameDataHandler
    {
        protected override void Handle()
        {
            if (Room.State == RoomState.Playing && (Room.Channel == ChannelType.Urban_Ops || Room.Channel == ChannelType.Battle_Group))
            {
                if (!Player.IsAlive || Player.Team == Team.None)
                    return;
                //Flag information: 0 DERB, 1 NIU, -1 WHITE
                //TODO BUILD ENUMERATION FOR THIS :D :D :D
                byte _capturedFlagId = GetByte(2);

                if (Room.SpawnFlags.Contains(_capturedFlagId)) //this array is cleared on conquest 
                    return;

                sbyte _oldFlagState = Room.Flags[_capturedFlagId];
                if (Room.Flags[_capturedFlagId] == -1) //neutral flag
                    Room.Flags[_capturedFlagId] = (sbyte)Player.Team;
                else
                    Room.Flags[_capturedFlagId] = -1;

                Set(2, _capturedFlagId);
                Set(3, Room.Flags[_capturedFlagId]);
                respond = true;

                Player.AddFlags();

                //Calls the event to update gamemode
                Room.CurrentGameMode.OnFlagCapture(Player, _oldFlagState);
            }
            else
                Player.User.Disconnect(); //cheating
        }

    }
}
