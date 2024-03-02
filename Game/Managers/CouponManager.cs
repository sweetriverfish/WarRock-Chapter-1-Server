using System;
using System.Linq;
using System.Collections.Generic;
using MySql.Data.MySqlClient;


namespace Game.Managers
{
    class CouponManager
    {
        List<Objects.Coupon> CouponList = new List<Objects.Coupon>();

        public CouponManager()
        {

        }

        public bool Load()
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("SELECT id, code, uses, dinarreward, itemreward FROM coupons", Databases.Game.connection);
                MySqlDataReader Reader = cmd.ExecuteReader();

                if (Reader.HasRows)
                {
                    while (Reader.Read())
                    {
                        byte _index = Reader.GetByte("id");
                        string _code = Reader.GetString("code");
                        int _uses = Reader.GetInt32("uses");
                        uint _dinarReward = Reader.GetUInt32("dinarreward");
                        string _itemReward = Reader.GetString("itemreward");

                        Objects.Coupon Coupon = new Objects.Coupon(_index, _code, _uses, _dinarReward, _itemReward);
                        UpdateCouponList(Coupon);
                    }
                }

                Reader.Close();

                if (CouponList.Count > 0)
                {
                    byte _activeCoupons = 0;
                    foreach (var Coupon in CouponList)
                    {
                        if (Coupon.Uses > 0)
                            _activeCoupons++;
                    }
                    Log.Instance.WriteLine("Coupon manager found " + _activeCoupons.ToString() + " useable coupons");
                }
                else
                    Log.Instance.WriteError("Coupon list is empty");

                return true;
            }
            catch { }
            return false;
        }


        public List<Objects.Coupon> UpdateCouponList(Objects.Coupon Coupon)
        {
            if (!CouponList.Contains(Coupon))
                CouponList.Add(Coupon);
            return CouponList;
        }

        public bool isValidCouponCode(string _inputCode)
        {
            foreach (var Coupon in CouponList)
                if (Coupon.Code == _inputCode)
                    return true;       
            return false;
        }

        public Objects.Coupon getCoupon(string _inputCode)
        {
            foreach (var Coupon in CouponList)
                if (Coupon.Code == _inputCode)
                    return Coupon; 
            return null;
        }

        public Objects.Coupon getCoupon(byte _index)
        {
            foreach (var Coupon in CouponList)     
                if (Coupon.Index == _index)
                    return Coupon;
            
            return null;
        }

        public void UpdateCouponUses(byte _index, int _currentUses)
        {
            foreach (var Coupon in CouponList)  
                if (Coupon.Index == _index)
                    Coupon.Uses = _currentUses;
        }


        private static CouponManager instance;
        public static CouponManager Instance { get { if (instance == null) { instance = new CouponManager(); } return instance; } }
    }
}
