using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Snake
{
    public class CSnake
    {
        public List<CPoint> Body = new List<CPoint>();
        public CPoint Head => Body[0];

        public CSnake(int x, int y)
        {
            Body.Add(new CPoint(x, y)); // Голова змеи
        }

        public void Move(СDirection direction)
        {
            CPoint newHead = new CPoint(Head.X, Head.Y);

            switch (direction)
            {
                case СDirection.Up:
                    newHead.Y--;
                    break;
                case СDirection.Down:
                    newHead.Y++;
                    break;
                case СDirection.Left:
                    newHead.X--;
                    break;
                case СDirection.Right:
                    newHead.X++;
                    break;
            }

            Body.Insert(0, newHead); // Добавляем новую голову

            if (Body.Count > 1)
            {
                Body.RemoveAt(Body.Count - 1); // Удаляем хвост
            }
        }

        public void Grow()
        {
            Body.Add(new CPoint(Head.X, Head.Y)); // Добавляем еще одно звено к телу
        }

        public bool IsSelfCollision()
        {
            foreach (var bodyPart in Body.Skip(1)) // Пропускаем первую точку (голову)
            {
                if (Head.X == bodyPart.X && Head.Y == bodyPart.Y)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
