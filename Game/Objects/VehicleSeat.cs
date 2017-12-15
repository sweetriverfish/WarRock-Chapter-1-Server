using System;
using System.Collections.Concurrent;
using Game.Enums;

namespace Game.Objects
{
    public sealed class VehicleSeat 
    {
        public byte ID { get; private set; }
        public string VehicleCode { get; private set; }
        public string SeatCode { get; private set; }
        public Entities.Player UsedBy { get; private set; }
        public ConcurrentDictionary<VehicleWeaponType, Objects.VehicleWeapon> Weapons { get; private set; }

        public uint Weapon1Ammo { get; private set; }
        public uint Weapon2Ammo { get; private set; }
        public byte Weapon1Mag { get; private set; }
        public byte Weapon2Mag { get; private set; }
        public uint Weapon1CurrentAmmo { get; set; }
        public uint Weapon2CurrentAmmo { get; set; }
        public byte Weapon1CurrentMag { get;  set; }
        public byte Weapon2CurrentMag { get;  set; }

        public VehicleSeat(byte _id, string _seatCode, string _vehicleCode, uint _wep1am, uint _wep2am, byte _wep1mag, byte _wep2mag)
        {
            UsedBy = null;

            ID = _id;
            SeatCode = _seatCode;
            VehicleCode = _vehicleCode;
            Weapon1CurrentAmmo = Weapon1Ammo = _wep1am;
            Weapon2CurrentAmmo = Weapon2Ammo = _wep2am;
            Weapon1CurrentMag = Weapon1Mag = _wep1mag;
            Weapon2CurrentMag = Weapon2Mag = _wep2mag;
            Weapons = new ConcurrentDictionary<VehicleWeaponType, VehicleWeapon>();
           
        }

        public VehicleSeat(byte _id, string _seatCode, string _vehicleCode, uint _wep1am, uint _wep2am, byte _wep1mag, byte _wep2mag, ConcurrentDictionary<VehicleWeaponType, VehicleWeapon> _weapons)
        {
            UsedBy = null;

            ID = _id;
            SeatCode = _seatCode;
            VehicleCode = _vehicleCode;
            Weapon1CurrentAmmo = Weapon1Ammo = _wep1am;
            Weapon2CurrentAmmo = Weapon2Ammo = _wep2am;
            Weapon1CurrentMag = Weapon1Mag = _wep1mag;
            Weapon2CurrentMag = Weapon2Mag = _wep2mag;
            Weapons = _weapons; 

        }

        public void EmptySeat()
        {
            UsedBy = null;
        }

        public void EnterSeat(Entities.Player NewOwner)
        {        
                UsedBy = NewOwner;       
        }

        public void AddWeapon(VehicleWeaponType Type, Objects.VehicleWeapon Weapon)
        {
           if(Weapon != null)
            Weapons.TryAdd(Type, Weapon);
        }

        public void ResetWeapons()
        {
            Weapon1CurrentAmmo = Weapon1Ammo;
            Weapon2CurrentAmmo = Weapon2Ammo;
            Weapon1CurrentMag = Weapon1Mag;
            Weapon2CurrentMag = Weapon2Mag;
        }


    }


}
