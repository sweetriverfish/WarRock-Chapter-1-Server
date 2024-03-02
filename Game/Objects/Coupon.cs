﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Game.Objects
{
    public class Coupon
    {

        public byte Index { get; private set; }
        public string Code { get; private set; }
        public uint DinarReward { get; private set; }
        public int Uses { get; set; }
        public string ItemReward { get; set; }


        public Coupon(byte _index, string _code, int _uses, uint _dinarReward, string _itemReward)
        {
            Index = _index;
            Code = _code;
            DinarReward = _dinarReward;
            Uses = _uses;
            ItemReward = _itemReward;
        }

    }
}
