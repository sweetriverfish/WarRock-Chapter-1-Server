using Game.Enums;

namespace Game.Packets
{
    class Coupon : Core.Networking.OutPacket
    {

        public Coupon(int _success, uint _dinars)
            : base(33024)
        {
            Append(_success);// -2 = invalid | -1 = code alrady registered | 0 = success!
            Append(0); //??
            Append(_dinars); //Dinars total
        }
    }
}
