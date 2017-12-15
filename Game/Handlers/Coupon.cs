using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Game.Handlers
{
    class Coupons : Networking.PacketHandler
    {
        protected override void Process(Entities.User u)
        {
            if (u.Authorized)
            {
                string _inputCode = GetString(0);
                //let´s check if any coupon matches the input
                if (Managers.CouponManager.Instance.isValidCouponCode(_inputCode))
                {
                    Objects.Coupon Coupon = Managers.CouponManager.Instance.getCoupon(_inputCode);

                    if (Coupon.Uses > 0)
                    {
                        u.Money += Coupon.DinarReward;

                        byte _usesLeft = Coupon.Uses;
                        _usesLeft--;

                        Managers.CouponManager.Instance.UpdateCouponUses(Coupon.Index, _usesLeft);

                        Databases.Game.AsyncQuery("UPDATE coupons SET uses=" + _usesLeft + "  WHERE id=" + Coupon.Index + "; UPDATE user_details SET money=" + u.Money + " WHERE id=" + u.ID + ";");
                        ServerLogger.Instance.Append(String.Concat("Player ", u.Displayname, " used coupon ", _inputCode));
                        u.Send(new Packets.Coupon(0, u.Money));
                    }
                    else //already registered :(
                        u.Send(new Packets.Coupon(-1, 0));
                    
                }
                else
                {//throw here invalid code
                }
            }
            else
                u.Disconnect();
            


        }
    }
}
