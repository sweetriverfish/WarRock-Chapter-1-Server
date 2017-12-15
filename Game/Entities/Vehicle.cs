using System;
using System.Collections.Generic;
using Game.Enums;

namespace Game.Entities
{
   public sealed class Vehicle
    {
       public byte MapID { get; private set; } //map Id
       public string Code { get; private set; }
       public uint Health { get; private set; }
       public uint MaxHealth { get; private set; }
       public List<Objects.VehicleSeat> Seats { get; private set; }
       public bool IsSpawned { get; private set; }
       public bool IsAlive { get; private set; }
       public byte SeatCount { get; private set; }
       public uint SpawnInterval { get; private set; }
       public uint BrokenFor { get; set; }
       public Enums.Team Team { get; private set; }
       public Enums.VehicleType VehicleClass { get; private set; }

       public Vehicle(VehicleType _type, string _code, byte _seatCount, uint _maxhealth, uint _spawnInterval)
       {
           MapID = 0;
           VehicleClass = _type;
           Code = _code;
           Health = MaxHealth = _maxhealth;
           Seats = new List<Objects.VehicleSeat>();
           IsSpawned = true; //by default, veh are spawned when room starts
           IsAlive = true;
           SeatCount = _seatCount;
           SpawnInterval = _spawnInterval;
           Team = Enums.Team.None;
           BrokenFor = 0;
       }

       public Vehicle(VehicleType _type, string _code, byte _seatCount, uint _maxhealth, uint _spawnInterval, List<Objects.VehicleSeat> _seats)
       {
           MapID = 0;
           VehicleClass = _type;
           Code = _code;
           Health = MaxHealth = _maxhealth;
           Seats =  _seats;
           IsSpawned = true; //by default, veh are spawned when room starts
           IsAlive = true;
           SeatCount = _seatCount;
           SpawnInterval = _spawnInterval;
           Team = Enums.Team.None;
           BrokenFor = 0;
       }

       public Vehicle(VehicleType _type, byte _mapId, string _code, byte _seatCount, uint _maxhealth, uint _spawnInterval)
       {
           MapID = _mapId;
           VehicleClass = _type;
           Code = _code;
           Health = MaxHealth = _maxhealth;
           Seats = new List<Objects.VehicleSeat>();
           IsSpawned = true; //by default, veh are spawned when room starts
           IsAlive = true;
           SeatCount = _seatCount;
           SpawnInterval = _spawnInterval;
           Team = Enums.Team.None;
           BrokenFor = 0;
       }


       public bool AddSeat(Objects.VehicleSeat Seat)
       {
           if(Seat.VehicleCode == Code && !Seats.Contains(Seat))
           {
               Seats.Add(Seat);
               return true;
           }
           return false;        
       }
       public void Reset()
       {
           Health = MaxHealth;
           IsSpawned = true;
           IsAlive = true;
           Team = Enums.Team.None;
           BrokenFor = 0;

           for (byte i = 0; i < Seats.Count; i++)
           {
               Seats[i].EmptySeat();
               Seats[i].ResetWeapons();
           }
                    
       }


       private void Destroy()
       {
           IsAlive = false;
           Health = 0;
           IsSpawned = false;
           BrokenFor = 0;
       }

       public void Damage(ushort _damage)
       {
           if (_damage >= Health)
               Destroy();
           else
               Health -= _damage;
       }

       public void Repair(uint _repairAmount)
       {
           Health += _repairAmount;
           if (Health > MaxHealth)
               Health = MaxHealth;
       }
       public void SetID(byte _id)
       {
           MapID = _id;
       }
       public void SetTeam(Enums.Team team)
       {
           Team = team;
       }

       public void EmptySeat(byte _targetSeat)
       {
          if(Seats[_targetSeat] != null)
           Seats[_targetSeat].EmptySeat();

           //if the last player leaves, set the team to none
          foreach (Objects.VehicleSeat Seat in Seats)
          {
              if (Seat.UsedBy != null)
                  break;
             SetTeam(Team.None);
          }

       }

       public int EnterVehicle(Entities.Player Player)
       {
           for (byte i = 0; i < SeatCount; i++ )
           {
               if(Seats[i].UsedBy == null)
               {
                   Seats[i].EnterSeat(Player);
                   return Seats[i].ID;
               }
           }
               return -1;
       }

       public bool ChangeSeat(byte _targetSeat, byte _oldSeat, Entities.Player NewOwner)
       {
           if (Seats[_oldSeat] != null && Seats[_targetSeat] != null && Seats[_targetSeat].UsedBy == null)
           {
               Seats[_oldSeat].EmptySeat();
               Seats[_targetSeat].EnterSeat(NewOwner);
               return true;
           }
           return false;
       }

       public sbyte GetSeatOf(Entities.Player Player)
       {
           sbyte _seat = -1;

           foreach(Objects.VehicleSeat Seat in Seats)
           {
               if (Seat.UsedBy == Player)
                   _seat = (sbyte)Seat.ID;
           }

           return _seat;
       }
    }
}
