/*
MIT License

Copyright (c) 2019 steel9

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

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
        public Direction? Direction_ { get; set; } = null;

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
