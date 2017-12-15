using System;
using Game.Enums;

namespace Game.Handlers.Game.Ingame
{
    public sealed class VehicleRepair : Networking.GameDataHandler
    {
        protected override void Handle()
        {
            if (Room.State == RoomState.Playing && (Room.Channel == ChannelType.Urban_Ops || Room.Channel == ChannelType.Battle_Group))
            {
                if (!Player.IsAlive)
                    return;

                byte _targetVehicleId = GetByte(2);
                byte _fromTower = GetByte(3);

                if (_targetVehicleId >= Room.Vehicles.Count)
                    return;

                Entities.Vehicle TargetVehicle = Room.Vehicles[_targetVehicleId];

                if ((TargetVehicle.Team == Team.None || TargetVehicle.Team == Player.Team) && TargetVehicle.Health < TargetVehicle.MaxHealth)
                {
                   double _repairRate;

                   if (_fromTower == 0)
                       _repairRate = GetRepairRate(Player.Weapon);
                   else
                       _repairRate = 0.20;

                     if(_repairRate != 0)
                     {
                         uint _repairAmount = (uint)Math.Round(TargetVehicle.MaxHealth * _repairRate);
                         TargetVehicle.Repair(_repairAmount);
                         Set(3, TargetVehicle.Health);
                         Set(4, TargetVehicle.MaxHealth);
                         respond = true;
                     }
                }
               
            }
            else
                Player.User.Disconnect();
        }

        double GetRepairRate(uint _playerWeapon)
        {
            double _repairRate = 0;

            switch(_playerWeapon)
            {
                case 80: //default spanner
                    _repairRate = 0.10;
                    break;
                case 81: //pipe wrench
                    _repairRate = 0.15;
                    break;

                default:
                    ServerLogger.Instance.Append(ServerLogger.AlertLevel.Unknown, "Cannot find repair item " + _playerWeapon.ToString());
                    break;
            }

            return _repairRate;
        }
    }
}
