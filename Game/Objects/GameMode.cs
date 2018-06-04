using System.Linq;
using Game.Enums;

namespace Game.Objects
{
    public abstract class GameMode
    {
        #region basic GameMode
        public byte Id { get; private set; }
        public string Name { get; private set; }
        public Entities.Room Room { get; private set; }
        public bool Initilized { get; protected set; }
        public bool FreezeTick { get; protected set; }

        public GameMode(byte id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.Initilized = false;
            this.FreezeTick = false;
        }

        public virtual void Initilize(Entities.Room room)
        {
            this.Room = room;
        }
        #endregion 

        #region Core Functions
        // Core functions //
        public abstract bool IsGoalReached();
        public abstract void Process();

        public virtual byte SpawnSlot()
        {
            return 0;
        }
        #endregion 

        #region ScoreBoard
        public abstract byte CurrentRoundTeamA();
        public abstract byte CurrentRoundTeamB();
        public abstract ushort ScoreboardA();
        public abstract ushort ScoreboardB();
        #endregion 

        #region GameMode Events
        public abstract Enums.Team Winner();
        public virtual void HandleExplosives(string[] blocks, Entities.Player p) //overriden by Explosive GameMode
        {
            byte _playerSlotId;
            byte _isExploding;
            byte _actionType;
            byte _itemId;

            byte.TryParse(blocks[0], out _playerSlotId);
            byte.TryParse(blocks[1], out _isExploding);
            byte.TryParse(blocks[2], out _actionType);
            byte.TryParse(blocks[3], out _itemId);

            if (_actionType == 2) //planting TMAs, no bombs in TDM or Conquest
            {
                if (_isExploding == 0)
                {
                    if (p.Class != Classes.Heavy)
                        p.User.Disconnect();
                }

                Room.Send((new Packets.Explosives(blocks)).BuildEncrypted());

            }
        }
        public abstract void OnFlagCapture(Entities.Player Player, sbyte _flagState);
        public abstract void OnPlayerSuicide(Entities.Player Player);
        protected abstract void OnDeath(Entities.Player killer, Entities.Player target);
        #endregion 

        #region OnDamageEvent

        public virtual void OnDamage(Networking.GameDataHandler handler)
        {

            bool _isPlayer = handler.GetBool(2);
            byte _targetId = handler.GetByte(3);
            bool _isRadiusWeapon = handler.GetBool(10);
            uint _boneId = handler.GetuInt(11);
            string _weaponCode = handler.GetString(22).ToUpper();

            //Radius is used when _isradiusWeapon is true. Values range from 0 to 100
            uint _radius = _boneId;

            Entities.Player Victim = null;
            Entities.Player Attacker = handler.Player;

            try { handler.Room.Players.TryGetValue(_targetId, out Victim); }
            catch { Victim = null; }

            if (Victim != null)
            {
                if (_isPlayer)
                {
                    Objects.Items.ItemData ItemData = null;

                    try { ItemData = Managers.ItemManager.Instance.Items.Values.Where(n => n.Code == _weaponCode).First(); }
                    catch { ItemData = null; }

                    if (ItemData != null && ItemData.IsWeapon)
                    {
                        Objects.Items.Weapon Weapon = (Objects.Items.Weapon)ItemData;
                        Objects.Inventory.Item PlayerItem = Attacker.User.Inventory.Equipment.Get(handler.Player.Class, _weaponCode);

                        if (PlayerItem != null) //player have the wep
                        {
                            if (CanInFlictDamageTo(Attacker, Victim))
                            {

                                uint _realBoneId = _boneId - handler.Player.User.SessionID;
                                bool _isHeadShot = false;
                                short _damageTaken = 0;
                                double _hitboxMultiplier = 1.0;
                                Enums.Hitbox Location;
                                try { Location = (Hitbox)_realBoneId; }
                                catch { Location = Hitbox.TorsoLimbs; }

                                if (Location == Hitbox.HeadNeck && !Weapon.Code.Contains("DA") && !Weapon.Code.Contains("DJ") && !Weapon.Code.Contains("DK"))
                                    _isHeadShot = true;

                                #region Explanation to the DJ03 case
                                //RPG-7 vanilla behaviour requires some tweaks... our reverse-engineered damage calc. is not consistent
                                //with vanilla in-game experience suggesting some more shit is going on
                                //server side
                                //SO... most DJ-DK weapons have reduced personal hitMult. in the DB... consistent with gamefiles and damage calc.
                                //but RPG-7
                                #endregion

                                if (!_isRadiusWeapon)
                                {
                                    if (Weapon.Code == "DJ03") 
                                        _damageTaken = 1000;
                                    else
                                    {
                                        _hitboxMultiplier = GetHitBoxMultiplier(Location);
                                        _damageTaken = DamageCalculator(Weapon.Power, Weapon.PowerPersonal[0], _hitboxMultiplier, 100);
                                    }  
                                }
                                else
                                    _damageTaken = DamageCalculator(Weapon.Power, Weapon.PowerPersonal[0], 1.0, _radius);

                                DamagePlayer(handler, Attacker, Victim, _damageTaken, _isHeadShot);
                            }
                        }
                      //  else
                      //      Attacker.User.Disconnect(); ---> Commented out because of client bug with knifes
                    }          
                }
                else //a vehicle attacked our poor player :(
                    OnVehicleAttackToPlayer(handler, Attacker, Victim);
            }
        }
    

                
                
        #endregion 

        #region OnVehicleAttackToPlayer Event
        private void OnVehicleAttackToPlayer(Networking.GameDataHandler handler, Entities.Player Attacker, Entities.Player Victim)
        {
            Entities.Vehicle AttackerVehicle = null;
            string _vehicleId = handler.GetString(22).ToUpper();

            try { AttackerVehicle = handler.Room.Vehicles[Attacker.VehicleId]; }
            catch { AttackerVehicle = null; }

             if(AttackerVehicle != null)
             {
                 if(CanInFlictDamageTo(Attacker, Victim) && IsOperative(AttackerVehicle))
                 {
                     if(_vehicleId == AttackerVehicle.Code)
                      {
                          Objects.VehicleSeat Seat = AttackerVehicle.Seats[Attacker.VehicleSeatId];

                          if (Seat.Weapons.Count > 0)
                          {
                              byte _weaponSlot = handler.GetByte(9); //0 = main CT, 1 = subCT
                              Objects.VehicleWeapon Weapon = null;
                              Seat.Weapons.TryGetValue((VehicleWeaponType)_weaponSlot, out Weapon);

                              if (Weapon != null)
                              {
                                  //Most vehicles uses Radius even for machineguns... yeah odd
                                  short _damageTaken = 0;
                                  double _hitboxMultiplier = 1.0;
                                  bool _useRadius = handler.GetBool(10);
                                  uint _boneId = handler.GetuInt(11);
                                  uint _radius = _boneId;
                                  uint _realBoneId = _boneId - handler.Player.User.SessionID;
                                  
                                  //just to make sure...
                                  
                                   if(!_useRadius)
                                   {
                                       Enums.Hitbox Location;
                                       try { Location = (Hitbox)_realBoneId; }
                                       catch { Location = Hitbox.TorsoLimbs; }

                                       _hitboxMultiplier = GetHitBoxMultiplier(Location);
                                       _radius = 100;
                                                             
                                   }
                                   _damageTaken = DamageCalculator((short)Weapon.Damage, Weapon.HitBox[(byte)VehWeapDamType.Personal], _radius);
                                   DamagePlayer(handler, Attacker, Victim, _damageTaken, false);
                              }
                          }
                      }
                 }
             }
             else //Artillery Attack?
             {
                 Objects.VehicleWeapon Artillery = null;

                 Artillery = Managers.VehicleManager.Instance.GetWeaponBy(handler.GetString(22).ToUpper());

                 if (Artillery != null && isValidArtilleryAttack(Attacker))
                 {
                     uint _radius = handler.GetuInt(11);
                     short _damageTaken = DamageCalculator((short)Artillery.Damage, Artillery.HitBox[(byte)VehWeapDamType.Personal], _radius);
                     DamagePlayer(handler, Attacker, Victim, _damageTaken, false);
                 }
                     
                 else
                     Attacker.User.Disconnect();                
             }
        }
        #endregion 

        #region OnObjectDamageEvent
          public virtual void OnObjectDamage(Networking.GameDataHandler handler)
        {
          
            bool _isPlayer = handler.GetBool(2);
            byte  _objectDamagedId = handler.GetByte(3);
            bool _isSubWeapon = handler.GetBool(9);
            bool _isRadiusWeapon = handler.GetBool(10);
            uint _radius = handler.GetuInt(11); //vehicles don´t seem to have a hitbox

            string _weaponCode = handler.GetString(22).ToUpper();    
            short _damageTaken = 0;

            Entities.Player Attacker = handler.Player;
            Entities.Vehicle VehicleDamaged = null;
            Objects.Items.Weapon Weapon = null;

            try { VehicleDamaged = Room.Vehicles[_objectDamagedId]; }

              catch{VehicleDamaged = null;
                Log.Instance.WriteError("Unknown damaged object with ID: " + _objectDamagedId.ToString());
              }

              if(VehicleDamaged != null && CanInFlictDamageTo(Attacker, VehicleDamaged))
              {
                  if(_isPlayer)
                  {
                      try {  Weapon = (Objects.Items.Weapon)Managers.ItemManager.Instance.Items.Values.Where(n => n.Code == _weaponCode).First(); }
                      catch{ Weapon = null; }
                             
                      if(Weapon != null)
                      {

                          short _weaponMultiplier = 100;
                          switch (VehicleDamaged.VehicleClass)
                          {
                              case VehicleType.Surface:
                                  _weaponMultiplier = Weapon.PowerSurface[0];
                                  break;
                              case VehicleType.Air:
                                  _weaponMultiplier = Weapon.PowerAir[0];
                                  break;
                              case VehicleType.Ship:
                                  _weaponMultiplier = Weapon.PowerShip[0];
                                  break;
                              default:
                                  _weaponMultiplier = Weapon.PowerPersonal[0];
                                  break;
                          }

                          _damageTaken = DamageCalculator(Weapon.Power, _weaponMultiplier, _radius);
                      }

                  }
                  else //Vehicle attaking other vehicle
                  {
                    if (Attacker.VehicleId != -1)
                        {
                            Entities.Vehicle AttackerVehicle = Room.Vehicles[Attacker.VehicleId];

                            if (_weaponCode == AttackerVehicle.Code) //dont trust the client
                            {
                                Objects.VehicleSeat Seat = AttackerVehicle.Seats[AttackerVehicle.GetSeatOf(Attacker)];
                                Objects.VehicleWeapon VehWeapon = null;

                                if (_isSubWeapon) //he is shooting with the subCT        
                                    Seat.Weapons.TryGetValue(VehicleWeaponType.Sub, out VehWeapon);                              
                                else
                                    Seat.Weapons.TryGetValue(VehicleWeaponType.Main, out VehWeapon);

                                if (VehWeapon != null)
                                   _damageTaken = DamageCalculator((short)VehWeapon.Damage, VehWeapon.HitBox[(byte)VehicleDamaged.VehicleClass], _radius);
                                
                            }
                        }
                        else //maybe it´s an artillery attack
                        {
                            if (!isValidArtilleryAttack(Attacker))
                                Attacker.User.Disconnect();

                            Objects.VehicleWeapon Artillery = Managers.VehicleManager.Instance.GetWeaponBy(_weaponCode);
                           
                             if(Artillery != null)
                                 _damageTaken = DamageCalculator((short)Artillery.Damage, Artillery.HitBox[(byte)VehicleDamaged.VehicleClass], _radius);              
                             else
                                 Log.Instance.WriteError("Couldn´t find artillery weapon " + _weaponCode + " in the Manager");
                                                           
                        }
                    
                  }

                  DamageVehicle(handler, Attacker, VehicleDamaged, _damageTaken);
              }
        }
        #endregion

        #region Auxiliary Methods

        /// <summary>
        /// Returns true if both entities are valid for attacking one another; returns false otherwise
        /// </summary>
        /// <param name="Attacker"></param>
        /// <param name="Victim"></param>
        /// <returns>True/false</returns>
        private bool CanInFlictDamageTo(Entities.Player Attacker, Entities.Player Victim)
        {
            bool _result = false;

            if (Victim.SpawnProtection == 0)
            {
                if (Attacker.IsAlive && Attacker.Health > 0)
                {
                    if (Victim.IsAlive && Victim.Health > 0)
                    {
                        //FF check
                        if (Attacker.Team != Victim.Team || Room.FriendlyFire || Room.Mode == Mode.Free_For_All)
                            _result = true;
                    }
                }
            }
            
            return _result;
        }

        private bool CanInFlictDamageTo(Entities.Player Attacker, Entities.Vehicle Vehicle)
        {
            bool _result = false;

           if(IsOperative(Vehicle) && Attacker.Health > 0 && Attacker.IsAlive)
           {
               if (Attacker.Team != Vehicle.Team || Room.FriendlyFire || Room.Mode == Mode.Free_For_All)
                   _result = true;
           }
            return _result;
        }

        private bool isFriendlyFire(Entities.Player Attacker, Entities.Player Victim)
        {
            if (Room.FriendlyFire && Attacker.Team == Victim.Team && Room.Mode != Mode.Free_For_All)
                return true;

            return false;
        }

        private bool isFriendlyFire(Entities.Player Attacker, Entities.Vehicle Victim)
        {
            if (Room.FriendlyFire && Attacker.Team == Victim.Team && Room.Mode != Mode.Free_For_All)
                return true;

            return false;
        }
        private void DamagePlayer(Networking.GameDataHandler handler, Entities.Player Attacker, Entities.Player Victim, short _damageTaken, bool _isHeadShot)
        {
            ushort _previousHealth = Victim.Health;
            short _remainingHealth = (short)((short)Victim.Health - _damageTaken);

            if (_remainingHealth > 0)
                Victim.Health -= (ushort)_damageTaken;
            else
            {
                Victim.Health = 0;

                handler.type = GameSubs.PlayerDeath;
                
                 if(_isHeadShot)
                     handler.Set(17, "99.000"); //headshot icon

                if (isFriendlyFire(Attacker, Victim))
                 {
                     Attacker.AddFakeDeath();
                     Attacker.SubtractKill();
                     Victim.IsAlive = false; //no more fake methods ;)
                     ServerLogger.Instance.Append(ServerLogger.AlertLevel.Gaming, string.Concat(Attacker.User.Displayname, " killed his teammate ", Victim.User.Displayname));
                 }
                 else
                 {
                     Victim.AddDeaths();
                     Attacker.AddKill(_isHeadShot);
                     OnDeath(Attacker, Victim);
                 }
                ServerLogger.Instance.Append(ServerLogger.AlertLevel.Gaming, string.Concat(Attacker.User.Displayname, " killed ", Victim.User.Displayname));
            }

            handler.Set(12, Victim.Health);
            handler.Set(13, _previousHealth);
            handler.respond = true;       
        }

        private void DamageVehicle(Networking.GameDataHandler handler, Entities.Player Attacker, Entities.Vehicle DamagedVehicle, short _damageTaken)
        {

            DamagedVehicle.Damage((ushort)_damageTaken);

             if(DamagedVehicle.IsAlive) //vehicle is still ok
             {
                 handler.type = GameSubs.ObjectDamage;
                 handler.Set(12, DamagedVehicle.Health);
                 handler.Set(13, _damageTaken);
                 handler.respond = true;
             }
             else //vehicle has been destroyed
             {
                 if (DamagedVehicle.Team != Team.None)
                 {
                     //Creating the death packet from the original buffer
                     foreach (Objects.VehicleSeat Seat in DamagedVehicle.Seats)
                     {
                         Entities.Player Victim = Seat.UsedBy;

                         if (Victim != null && Victim.IsAlive)
                         {
                             Networking.GameDataHandler DeathHandler = Managers.PacketManager.Instance.GetHandler((ushort)GameSubs.PlayerDeath);
                             if (DeathHandler != null)
                             {
                                 try
                                 {
                                     DeathHandler.Process(Victim.User, handler.GetIncPacket());

                                     if(!isFriendlyFire(Attacker, Victim))
                                     {
                                         Victim.AddDeaths();
                                         Attacker.AddKill(false);
                                     }
                                     else
                                     {
                                         Attacker.SubtractKill();
                                         Attacker.AddFakeDeath();
                                     }                                   
                                     OnDeath(Attacker, Victim);
                                 }
                                 catch { ServerLogger.Instance.Append(ServerLogger.AlertLevel.ServerError, string.Concat("Could not kill player ", Victim.User.Displayname, " on vehicle ", DamagedVehicle.Code)); }
                             }
                         }
                     }
                 }

                 //Destroying veh
                 handler.type = GameSubs.ObjectDestroy;
                 handler.Set(12, DamagedVehicle.Health);
                 handler.Set(13, _damageTaken);
                 handler.Set(14, 15);
                 handler.respond = true;

                 if(!isFriendlyFire(Attacker, DamagedVehicle))
                 {
                       Attacker.VehiclesDestroyed++;
                       Attacker.User.VehiclesDestroyed++;
                 }
                 ServerLogger.Instance.Append(ServerLogger.AlertLevel.Gaming, string.Concat(Attacker.User.Displayname, " destroyed a vehicle: ", DamagedVehicle.Code));
             }

        }

        private bool IsOperative(Entities.Vehicle Vehicle)
        {
            if (Vehicle.Health > 0 && Vehicle.IsAlive)
                return true;
            return false;
        }

        /// <summary>
        /// Check if the player can execute an artillery Attacker
        /// </summary>
        /// <param name="Attacker"></param>
        /// <param name="_weaponCode"></param>
        /// <returns>True if the player is allowed to perform the attack; false otherwise</returns>
        private bool isValidArtilleryAttack(Entities.Player Attacker)
        {
            if (Attacker.User.Inventory.Itemlist.Contains("DX01"))
                return true;

            return false;
        }

        private short DamageCalculator(short _weaponPower, short _weaponDamageModifier, double _hitBoxModifier, uint _radius)
        {
            short _damageTaken = 0;

            double _weaponDamageMultiplier = (double)_weaponDamageModifier / 100;
            double _radiusDamageMultiplier = (double)_radius / 100;
            double _finalMultiplier = _weaponDamageMultiplier * _radiusDamageMultiplier * _hitBoxModifier;

            _damageTaken = (short)System.Math.Round(_finalMultiplier * _weaponPower);
            return _damageTaken;
        }

        private short DamageCalculator(short _weaponPower, byte _vehicleWeaponDamageModifier, uint _radius)
        {
            short _damageTaken = 0;

            double _weaponDamageMultiplier = (double)_vehicleWeaponDamageModifier / 100;
            double _radiusDamageMultiplier = (double)_radius / 100;
            double _finalMultiplier = _weaponDamageMultiplier * _radiusDamageMultiplier;

            _damageTaken = (short)System.Math.Round(_finalMultiplier * _weaponPower);
            return _damageTaken;
        }

        private short DamageCalculator(short _weaponPower, short _WeaponEfficiencyAgainstVehicle, uint _radius)
        {
            short _damageTaken = 0;

            double _weaponDamageMultiplier = (double)_WeaponEfficiencyAgainstVehicle / 100;
            double _radiusDamageMultiplier = (double)_radius / 100;
            double _finalMultiplier = _weaponDamageMultiplier * _radiusDamageMultiplier;

            _damageTaken = (short)System.Math.Round(_finalMultiplier * _weaponPower);
            return _damageTaken;
        }

        private double GetHitBoxMultiplier(Enums.Hitbox Location)
        {
            double _hitboxMultiplier = 1.0;

            switch (Location)
            {
                case Hitbox.HeadNeck:
                    _hitboxMultiplier = GameConfig.HeadMultiplier;
                    break;
                case Hitbox.TorsoLimbs:
                    _hitboxMultiplier = GameConfig.TorsoMultiplier;
                    break;
                case Hitbox.FeetHands:
                    _hitboxMultiplier = GameConfig.LowerLimbsMultiplier;
                    break;
                case Hitbox.SniperBone:
                    _hitboxMultiplier = GameConfig.SniperBoneMultiplier;
                    break;
                default:
                    _hitboxMultiplier = GameConfig.TorsoMultiplier;
                    break;
            }

            return _hitboxMultiplier; 
        }
        #endregion 
    }
}