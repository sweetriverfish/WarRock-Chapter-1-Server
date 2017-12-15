using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using Game.Enums;
using Game.Objects;
using MySql.Data.MySqlClient;


namespace Game.Managers
{
    class VehicleManager
    {
        public ConcurrentDictionary<string, Entities.Vehicle> Vehicles;  
        public ConcurrentDictionary<string, Objects.VehicleWeapon> VehicleWeapons;
        List<Objects.VehicleSeat> VehicleSeats = new List<Objects.VehicleSeat>();
        public List<string> VehicleTable = new List<string>();

        public VehicleManager()
        {

        }
        public bool Load()
        {
            if(LoadVehicleWeapons() && LoadVehicleSeats() && LoadVehicles() && LoadMapVehicles())          
                return true;

           return false;
        }


        #region MYSQL queues
        private bool LoadVehicleWeapons()
        {
            MySqlCommand Queue1 = new MySqlCommand("SELECT code, damage, hitbox from vehicleweapons", Databases.Game.connection);
            MySqlDataReader Reader1 = Queue1.ExecuteReader();
            ConcurrentDictionary<string, Objects.VehicleWeapon> tempWeapons = new ConcurrentDictionary<string,VehicleWeapon>();

            bool _result = false;

            try
            {
                if (Reader1.HasRows)
                {
                    while (Reader1.Read())
                    {
                        string _wepCode = Reader1.GetString("code");
                        uint _damage = Reader1.GetUInt16("damage");
                        string _preHitBox = Reader1.GetString("hitbox");
                        tempWeapons.TryAdd(_wepCode, new Objects.VehicleWeapon(_wepCode, _damage, GetHitBox(_preHitBox)));
                    }

                    Reader1.Close();
                    VehicleWeapons = tempWeapons;
                    _result = true;
                }
                else
                    Log.Instance.WriteError("Vehicle weapons table is empty");

            }
            catch
            {
                Log.Instance.WriteError("Couldnt load vehicle weapons table");
            }
            return _result;
        }

        private bool LoadVehicleSeats()
        {
            
             List<Objects.VehicleSeat> tempVehSeat = new List<Objects.VehicleSeat>();
            //Vehicle seat queue
            MySqlCommand Queue = new MySqlCommand("SELECT vehiclecode, seatid, seatcode, mainweapon, secondweapon, mainweaponclip, secondweaponclip, mainweaponmagazine, secondweaponmagazine from vehicleseats", Databases.Game.connection);
            MySqlDataReader Reader = Queue.ExecuteReader();
            bool _result = false;
            try
            {
                if (Reader.HasRows)
                {
                    while (Reader.Read())
                    {
                        string _vehicleCode = Reader.GetString("vehiclecode");
                        byte _seatId = Reader.GetByte("seatid");
                        string _seatCode = Reader.GetString("seatcode");
                        string _mainWeapon = Reader.GetString("mainweapon");
                        string _secondWeapon = Reader.GetString("secondweapon");
                        uint _mainWeaponAmmo = Reader.GetUInt16("mainweaponclip");
                        uint _secondWeaponAmmo = Reader.GetUInt16("secondweaponclip");
                        byte _mainWeaponMagazine = Reader.GetByte("mainweaponmagazine");
                        byte _secondWeaponMagazine = Reader.GetByte("secondweaponmagazine");

                        if(_seatCode != "")
                        {
                            Objects.VehicleSeat Seat = new Objects.VehicleSeat(_seatId, _seatCode, _vehicleCode, _mainWeaponAmmo, _secondWeaponAmmo,
                                _mainWeaponMagazine, _secondWeaponMagazine);

                            Seat.AddWeapon(VehicleWeaponType.Main, GetWeaponBy(_mainWeapon));
                            Seat.AddWeapon(VehicleWeaponType.Sub, GetWeaponBy(_secondWeapon));

                            tempVehSeat.Add(Seat);
                        }
                    }
                    Reader.Close();
                    VehicleSeats = tempVehSeat;
                    _result = true;
                }
                else
                    Log.Instance.WriteError("Vehicle seats table is empty");
            }
            catch
            {
                Log.Instance.WriteError("Couldn´t load  vehicle seats table");
            }
   
            return _result;
        }

        private bool LoadVehicles()
        {
            bool _result = false;
           ConcurrentDictionary<string, Entities.Vehicle> tempVeh = new ConcurrentDictionary<string,Entities.Vehicle>();
           MySqlCommand Queue2 = new MySqlCommand("SELECT code, seats, health, spawninterval, type from vehicletypes", Databases.Game.connection);
           MySqlDataReader Reader2 = Queue2.ExecuteReader();

           try
           {
               if (Reader2.HasRows)
               {
                   while (Reader2.Read())
                   {
                       string _code = Reader2.GetString("code");
                       byte _seats = Reader2.GetByte("seats");
                       uint _health = Reader2.GetUInt32("health");
                       uint _spawnInterval = Reader2.GetUInt32("spawninterval");
                       byte _type = Reader2.GetByte("type");

                       Entities.Vehicle Vehicle = new Entities.Vehicle((Enums.VehicleType)_type,_code, _seats, _health, _spawnInterval, GetSeatsFor(_code));
                       tempVeh.TryAdd(_code, Vehicle);
                   }
                   Reader2.Close();
                   Vehicles = tempVeh;
                   _result = true;
               }
               else
                   Log.Instance.WriteError("Vehicle types table is empty");
           }
           catch
           {
               Log.Instance.WriteError("Couldn´t load vehicle types table");
           }

           return _result;

        }

         private bool LoadMapVehicles()
         {
            MySqlCommand Queue3 = new MySqlCommand("SELECT MapID, VehicleMapID, VehicleCode from map_vehicles", Databases.Game.connection);
            MySqlDataReader Reader3 = Queue3.ExecuteReader();
             bool _result = false;

             try
             {
                 if(Reader3.HasRows)
                 {
                     while(Reader3.Read())
                     {
                          byte _mapID = Reader3.GetByte("MapID");
                          byte _vehicleMapID = Reader3.GetByte("VehicleMapID");
                         string _vehicleCode = Reader3.GetString("VehicleCode");
                         string _vehicleStamp = _mapID.ToString() + "-" + _vehicleMapID.ToString() + "-" + _vehicleCode;
                         VehicleTable.Add(_vehicleStamp);
                     }
                     _result = true;
                     Reader3.Close();
                 }
                 else
                     Log.Instance.WriteError("Map vehicles is empty");
             }
             catch
             {
                 Log.Instance.WriteError("Couldnt load map vehicles table");
             }

             return _result;
         }
           

          

        #endregion

        #region vehicles methods
        private byte[] GetHitBox(string _input)
        {
            byte[] HitBox = new byte[4];

            string[] _multipliers = _input.Split(','); //we get something like 100,80,30,20...

            for (byte i = 0; i < HitBox.Length; i++)
            {
               byte _mult;
               if (Byte.TryParse(_multipliers[i], out _mult))
                   HitBox[i] = _mult;
            }
                return HitBox;
        }

        public Objects.VehicleWeapon GetWeaponBy(string _code)
        {
            Objects.VehicleWeapon Weapon = null;
            VehicleWeapons.TryGetValue(_code, out Weapon);
            return Weapon;
        }

        public Entities.Vehicle GetVehicleBy(string _code)
        {
            Entities.Vehicle Vehicle = null;
            Vehicles.TryGetValue(_code, out Vehicle);
            return Vehicle;
        }
        public List<Objects.VehicleSeat> GetSeatsFor(string _vehicleCode)
        {
            List<Objects.VehicleSeat> Seats = new List<Objects.VehicleSeat>();

            foreach (Objects.VehicleSeat pSeat in VehicleSeats)
            {
                if (pSeat.VehicleCode == _vehicleCode)
                    Seats.Add(pSeat);
            }
            Seats.OrderBy(x => x.ID).ToList();
            return Seats;
        }

        #endregion

        #region RoomSupport Methods

        public List<Entities.Vehicle> BuildArray(byte _mapId, byte _vehicleCount)
        {
            List<Entities.Vehicle> VehicleList = new List<Entities.Vehicle>();

            foreach (string _stamp in VehicleTable)
            {
                byte _pMapId;
                string[] _stampParts = _stamp.Split('-');
                byte.TryParse(_stampParts[0], out _pMapId);

                if (_pMapId == _mapId)
                {               
                    //0 = mapId, 1 = vehicleMapID; 2 = vehicleCode
                    try
                    {
                        Entities.Vehicle Sheet = GetVehicleBy(_stampParts[2]);

                        Entities.Vehicle Vehicle = new Entities.Vehicle(Sheet.VehicleClass, Convert.ToByte(_stampParts[1]), Sheet.Code, Sheet.SeatCount, Sheet.MaxHealth, Sheet.SpawnInterval);

                        VehicleList.Add(Vehicle);
                    }
                    catch
                    {
                        Log.Instance.WriteError("Warning: Could not generate sheet vehicle " + _stampParts + " from vehicle types table... Vehicle not found!!!!");
                    }
            
                }
            }

            VehicleList.OrderBy(x => x.MapID);
            return VehicleList;
        }


        #endregion

        private static VehicleManager instance;
        public static VehicleManager Instance { get { if (instance == null) { instance = new VehicleManager(); } return instance; } }
    }
}
