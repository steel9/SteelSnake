using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SteelSnake
{
    class Program
    {
        static bool run = true;

        static Thread gameThread = null;
        static Thread inputThread = null;

        //static int snakeX, snakeY;
        static Pos[] snakePositions;
        static Pos applePos;

        static int fieldArea = 120 * 60;

        static int fps = 144;

        static void Main(string[] args)
        {
            inputThread = new Thread(() =>
            {
                while (run)
                {
                    var input = Console.ReadKey(true);
                    OnInput(input);
                }
            });

            gameThread = new Thread(() =>
            {
                while (run)
                {
                    var start = DateTime.Now;

                    Move();
                    Paint();

                    var end = DateTime.Now;
                    var dtDiff = end.Subtract(start);
                    var msDiff = dtDiff.TotalMilliseconds;
                    var fpsSleepMs = 1000 / fps;
                    var sleepMs = Convert.ToInt32(Math.Round(fpsSleepMs - msDiff));
                    if (sleepMs > 0)
                    {
                        Thread.Sleep(sleepMs);
                    }
                }
            });

            snakePositions = new Pos[1];
            snakePositions[0] = new Pos(FieldX() / 2, FieldY() / 2);

            GenerateApple();

            inputThread.Start();
            gameThread.Start();

            gameThread.Join();
        }

        static int FieldX()
        {
            return Console.BufferWidth;
        }

        static int FieldY()
        {
            return fieldArea / FieldX();
        }

        static void GenerateApple()
        {
            var random = new Random();
            applePos = new Pos(random.Next(0, FieldX()), random.Next(0, FieldY()));
        }

        static void OnInput(ConsoleKeyInfo input)
        {

        }

        static void Move()
        {

        }

        static void Paint()
        {
            for (int y = 0; y < FieldY(); ++y)
            {
                for (int x = 0; x < FieldX(); ++x)
                {
                    var pos = new Pos(x, y);
                    if (snakePositions.Contains(pos))
                    {
                        //not getting triggered
                        Console.SetCursorPosition(x, y);
                        Console.Write("*");
                    }
                    else if (applePos == pos)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write("Q");
                    }
                    else
                    {
                        //should read if something is written at pos first
                        Console.SetCursorPosition(x, y);
                        Console.Write("\b \b");
                    }
                }
            }

            Console.SetCursorPosition(0, 0);
        }
    }
}
