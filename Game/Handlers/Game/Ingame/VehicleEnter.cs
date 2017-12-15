using System;
using Game.Enums;
using Game.Networking;

namespace Game.Handlers.Game.Ingame
{
   public sealed class VehicleEnter : Networking.GameDataHandler
    {
       protected override void Handle()
       {
           //Checking the player status
           if (Room.State != RoomState.Playing || (Room.Channel != ChannelType.Urban_Ops && Room.Channel != ChannelType.Battle_Group))
               Player.User.Disconnect(); //cheating

           if (!Player.IsAlive || Player.InVehicle)
               return;

           //Getting the DATA & checking it is registered
           byte _vehicleId = GetByte(2);

           for(byte i = 0; i < Room.Vehicles.Count; i++)
           {
               if (Room.Vehicles[i].MapID == _vehicleId)
               {
                   if (Room.Vehicles[i].Team == Player.Team || Room.Vehicles[i].Team == Team.None)
                   {
                       int _seat = Room.Vehicles[i].EnterVehicle(Player);

                       if (_seat == -1)
                           return;
                       else
                       {
                           Player.InVehicle = true;
                           Player.VehicleId = _vehicleId;
                           Player.VehicleSeatId = Room.Vehicles[i].Seats[_seat].ID;
                           if (Room.Vehicles[i].Team == Team.None)
                               Room.Vehicles[i].SetTeam(Player.Team);

                           Set(2, Room.Vehicles[i].MapID);
                           Set(3, Room.Vehicles[i].Seats[_seat].ID);
                           Set(4, Room.Vehicles[i].Health);
                           Set(5, Room.Vehicles[i].MaxHealth);
                           Set(6, Room.Vehicles[i].Seats[_seat].Weapon1CurrentAmmo);
                           Set(7, Room.Vehicles[i].Seats[_seat].Weapon1CurrentMag);
                           Set(8, Room.Vehicles[i].Seats[_seat].Weapon2CurrentAmmo);
                           Set(9, Room.Vehicles[i].Seats[_seat].Weapon2CurrentMag);
                           Set(22, "$");
                           respond = true;

                       }

                   }
                   break;
               }

           }
           
       }
    }

}
