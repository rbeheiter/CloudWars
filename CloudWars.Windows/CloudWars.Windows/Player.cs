using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CloudWars
{
    public class Player
    {
        public string Name { get; set; }
        public List<Unit> Units { get; private set; }
        public Color Color { get; set; }

        public Player()
        {
            Units = new List<Unit>();
        }

    }
}
