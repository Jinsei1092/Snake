using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class CFood
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public CFood(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
