using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelSnake
{
    public static class Extensions
    {
        public static void Fill(this char[,] array, char value)
        {
            for (int y = 0; y < array.GetLength(1); ++y)
            {
                for (int x = 0; x < array.GetLength(0); ++x)
                {
                    array[x, y] = value;
                }
            }
        }

        public static Pos2D LeftMost(this Pos2D[] array, int y)
        {
            Pos2D leftMost = null;
            foreach (var pos in array)
            {
                if ((leftMost == null && pos.Y == y) || (pos.Y == y && pos.X < leftMost.X))
                {
                    leftMost = pos;
                }
            }

            return leftMost;
        }

        public static Pos2D RightMost(this Pos2D[] array, int y)
        {
            Pos2D rightMost = null;
            foreach (var pos in array)
            {
                if ((rightMost == null && pos.Y == y) || (pos.Y == y && pos.X > rightMost.X))
                {
                    rightMost = pos;
                }
            }

            return rightMost;
        }

        public static Pos2D TopMost(this Pos2D[] array, int x)
        {
            Pos2D topMost = null;
            foreach (var pos in array)
            {
                if ((topMost == null && pos.X == x) || (pos.X == x && pos.Y < topMost.Y))
                {
                    topMost = pos;
                }
            }

            return topMost;
        }

        public static Pos2D DownMost(this Pos2D[] array, int x)
        {
            Pos2D downMost = null;
            foreach (var pos in array)
            {
                if ((downMost == null && pos.X == x) || (pos.X == x && pos.Y > downMost.Y))
                {
                    downMost = pos;
                }
            }

            return downMost;
        }
    }
}
