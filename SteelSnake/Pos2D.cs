using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelSnake
{
    public class Pos2D
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Direction? Direction_ { get; set; }

        public Pos2D(int x, int y, Direction? direction_ = null)
        {
            X = x;
            Y = y;
            if (direction_ != null)
            {
                Direction_ = direction_;
            }
        }

        public enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }

        public bool Equals(Pos2D pos, bool compareDirection = false)
        {
            if (pos == null)
            {
                return false;
            }
            else if (this.X == pos.X && this.Y == pos.Y && (!compareDirection || this.Direction_ == pos.Direction_))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Pos2D Add(int x, int y)
        {
            return new Pos2D(this.X + x, this.Y + y);
        }
    }
}
