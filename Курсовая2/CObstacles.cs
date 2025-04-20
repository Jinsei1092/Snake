using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class CObstacles
    {
        public List<CPoint> Wall = new List<CPoint>();

        public CObstacles(int x, int y)
        {
            Wall.Add(new CPoint(x, y));
        }

        public void CountUp(int x, int y)
        {
            Wall.Add(new CPoint(x, y)); // Добавляем еще одно препятствие
        }
    }
}
