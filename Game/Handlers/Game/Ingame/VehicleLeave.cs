using System;
using Game.Enums;
using Game.Networking;

namespace Game.Handlers.Game.Ingame
{
    //NOTE: THIS PACKET IS ALSO SENT WHEN THE PLAYER IS KILLED ON A VEHICLE
    public sealed class VehicleLeave : Networking.GameDataHandler
    {
        protected override void Handle()
        {
            //Checking the player status
            if (Room.State != RoomState.Playing || (Room.Channel != ChannelType.Urban_Ops && Room.Channel != ChannelType.Battle_Group))
                Player.User.Disconnect(); //cheating


            byte _vehicleMapID = GetByte(2);
            uint _mainCTAmmo = GetuInt(4);
            byte _mainCTMag = GetByte(5);
            uint _subCTAmmo = GetuInt(6);
            byte _subCTMag = GetByte(7);

            for (byte i = 0; i < Room.Vehicles.Count; i++)
            {
                if (Room.Vehicles[i].MapID == _vehicleMapID)
                {
                    sbyte _pseat = Room.Vehicles[i].GetSeatOf(Player);

                     if(_pseat == -1)
                     {
                         Log.Instance.WriteError("CANT FIND SEAT OF PLAYER");
                         return;
                     }
                     else
                     {
                         byte _seat = (byte)_pseat;
                             
                    Room.Vehicles[i].EmptySeat(_seat);
                    Player.InVehicle = false;
                    Player.VehicleId = -1;
                    Player.VehicleSeatId = _seat;

                    Room.Vehicles[i].Seats[_seat].Weapon1CurrentAmmo = _mainCTAmmo;
                    Room.Vehicles[i].Seats[_seat].Weapon1CurrentMag=   _mainCTMag;
                    Room.Vehicles[i].Seats[_seat].Weapon2CurrentAmmo = _subCTAmmo;
                    Room.Vehicles[i].Seats[_seat].Weapon2CurrentMag =  _subCTMag;

                    Set(2, _vehicleMapID);
                    Set(3, _seat);
                    Set(4, _mainCTAmmo);
                    Set(5, _mainCTMag);
                    Set(6, _subCTAmmo);
                    Set(7, _subCTMag);
                    Set(8, 0);
                    Set(9, 0); //prueba a cambiar el nº
                    respond = true;
                    break;
                     }
                }

            }
        }
    }
}
