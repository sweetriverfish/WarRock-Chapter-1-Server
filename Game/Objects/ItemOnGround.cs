using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Objects
{
    public class GroundItems
    {
        public string ItemCode { get; private set; }
        public bool isUsed { get; set; }
        public Entities.Player Owner { get; private set; }
        public GroundItems(string _code, Entities.Player Player)
        {
            ItemCode = _code;
            isUsed = false;
            Owner = Player;
        }
    }
}
