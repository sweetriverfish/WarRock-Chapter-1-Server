using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Objects
{
    public class Coupon
    {

        public byte Index { get; private set; }
        public string Code { get; private set; }
        public uint DinarReward { get; private set; }
        public byte Uses { get; set; }


        public Coupon(byte _index, string _code, byte _uses, uint _dinarReward)
        {
            Index = _index;
            Code = _code;
            DinarReward = _dinarReward;
            Uses = _uses;
        }

    }
}
