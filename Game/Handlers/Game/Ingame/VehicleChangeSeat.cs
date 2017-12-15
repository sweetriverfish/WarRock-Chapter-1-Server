using System;
using Game.Enums;
using Game.Networking;

namespace Game.Handlers.Game.Ingame
{
    public sealed class VehicleChangeSeat : Networking.GameDataHandler
    {
        protected override void Handle()
        {
            //Checking the player status
            if (Room.State != RoomState.Playing || (Room.Channel != ChannelType.Urban_Ops && Room.Channel != ChannelType.Battle_Group))
                Player.User.Disconnect(); //cheating

            if (!Player.IsAlive || !Player.InVehicle)
                return;

            //30000 0 0 2 201 0 0 vehicleMapID, NewSeat, MainCT, MainCTMmag, SubCT, SubCTMag, 0, 0

            byte _vehicleMapID = GetByte(2);
            byte _newSeat = GetByte(3);
            uint _mainCTAmmo = GetuInt(4);
            byte _mainCTMag = GetByte(5);
            uint _subCTAmmo = GetuInt(6);
            byte _subCTMag = GetByte(7);

            for(byte i = 0; i <Room.Vehicles.Count; i++)
            {
                if(Room.Vehicles[i].MapID == _vehicleMapID)
                {
                    sbyte _oldseat = Room.Vehicles[i].GetSeatOf(Player);

                    if (_oldseat == -1) //should disconnect ya
                        return;
      
                    byte _oldSeat = (byte)_oldseat;

                    if(Room.Vehicles[i].ChangeSeat(_newSeat, _oldSeat, Player))
                    {
                         Room.Vehicles[i].Seats[_oldSeat].Weapon1CurrentAmmo = _mainCTAmmo;
                         Room.Vehicles[i].Seats[_oldSeat].Weapon1CurrentMag = _mainCTMag;
                         Room.Vehicles[i].Seats[_oldSeat].Weapon2CurrentAmmo = _subCTAmmo;
                         Room.Vehicles[i].Seats[_oldSeat].Weapon2CurrentMag = _subCTMag;

                         Player.VehicleSeatId = _newSeat;
                         Set(2, _vehicleMapID);
                         Set(3, _newSeat);
                         Set(4, _oldSeat);
                         Set(5,  Room.Vehicles[i].Seats[_newSeat].Weapon1CurrentAmmo);
                         Set(6,  Room.Vehicles[i].Seats[_newSeat].Weapon1CurrentMag);
                         Set(7,  Room.Vehicles[i].Seats[_newSeat].Weapon2CurrentAmmo);
                         Set(8,  Room.Vehicles[i].Seats[_newSeat].Weapon2CurrentMag);
                         respond = true;
                    }
                    break;
                }
            }


        }
    }
}
