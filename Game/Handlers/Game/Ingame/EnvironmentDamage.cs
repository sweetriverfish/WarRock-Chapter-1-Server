using System;
using Game.Enums;
using Game.Networking;

namespace Game.Handlers.Game.Ingame
{
    //It is the damage produced by the world PHYSICS to the Entity

    public sealed class EnvironmentDamage : GameDataHandler
    {
        protected override void Handle()
        {
            
            if (Room.State != RoomState.Playing)
                Player.User.Disconnect();

            //Packet info:
            byte _entityType = GetByte(2); //player = 1, Veh seems to be 0. Other types???
            byte _targetVehicle = GetByte(4);
            ushort _totalDamage = GetUShort(5);
            long _outOfPlayableZone = GetLong(6);


            if (_totalDamage == 0)
                return;

            if (_entityType == 1) //player
            {
                if (!Player.IsAlive)
                    return;

                if (Player.Health > _totalDamage)
                    Player.Health -= _totalDamage;

                else
                {
                    Player.AddDeaths();
                    Player.Health = 0;
                    type = GameSubs.Suicide;
                    Set(2, Player.User.RoomSlot);

                    if (Player.Team != Team.None)
                        Room.CurrentGameMode.OnPlayerSuicide(Player);
                }
                Set(7, _totalDamage);
                Set(8, Player.Health);
                respond = true;

            }
            else
            {
                Entities.Vehicle TargetVehicle = Room.Vehicles[_targetVehicle];

                if (_outOfPlayableZone != 1)
                    TargetVehicle.Damage(_totalDamage);
                else
                    TargetVehicle.Damage((ushort)TargetVehicle.Health);

                 if(!TargetVehicle.IsAlive)
                 {
                     //kill players
                     string[] _buffer = new string[this.packet.Blocks.Length + 5];
             
                     foreach(Objects.VehicleSeat Seat in TargetVehicle.Seats)
                     {
                         Entities.Player Driver = Seat.UsedBy;
                          if(Driver != null)
                          {
                              Driver.AddDeaths();
                              Driver.Health = 0;
                              _buffer[0] = "1";
                              _buffer[1] = Player.Id.ToString();
                              _buffer[2] = Room.ID.ToString();
                              _buffer[3] = this.packet.Blocks[2];
                              _buffer[4] = "157";
                              Array.Copy(this.packet.Blocks, 0, _buffer, 5, this.packet.Blocks.Length);
                              _buffer[2] = Driver.User.RoomSlot.ToString();
                              _buffer[23] = "$";
                             
                            Room.Send(new Packets.GameData(_buffer).BuildEncrypted());
                            Room.CurrentGameMode.OnPlayerSuicide(Driver); //ur team lost points u idiot
                          }
                     }

                     Room.Send(new Packets.VehicleExplode(Room, TargetVehicle).BuildEncrypted());
                 }
                 else
                 {
                     Set(7, _totalDamage);
                     Set(8, TargetVehicle.Health);
                     Set(23, "$");
                     respond = true;

                 }
                        
            }

        }

    }
}
