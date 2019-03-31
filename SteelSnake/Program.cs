/*
MIT License

Copyright (c) 2019 steel9apps

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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SteelSnake
{
    class Program
    {
        const int MOVE_DELAY = 70;
        const int EXTENSION_TIMES = 15;
        const int POINTS_APPLE = 1;

        static bool runGame = true;
        static bool input = true;

        static bool gameOver = false;

        static Thread gamePhysicsThread = null;
        static Thread renderingThread = null;
        static Thread inputThread = null;

        static Pos2D[] snakePositions;
        static Pos2D.Direction snakeDirection;

        static int Score { get; set; } = 0;

        static Pos2D applePos;

        static int fieldArea => Console.WindowWidth * Console.WindowHeight;

        static List<Pos2D> writtenConsolePositions = new List<Pos2D>();

        static int fps = 144;

        static void Main(string[] args)
        {
            inputThread = new Thread(() =>
            {
                while (input)
                {
                    var input = Console.ReadKey(true);
                    OnInput(input);
                }
            });

            gamePhysicsThread = new Thread(() =>
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                while (runGame)
                {
                    var moveMsElapsed = stopWatch.ElapsedMilliseconds;
                    if (moveMsElapsed >= MOVE_DELAY)
                    {
                        stopWatch.Stop();
                        System.Diagnostics.Debug.Print("Time elapsed since last move (ms): " + moveMsElapsed.ToString());
                        Move();
                        stopWatch.Restart();
                    }
                }
            });

            renderingThread = new Thread(() =>
            {
                var stopWatch = new Stopwatch();
                while (runGame)
                {
                    stopWatch.Restart();

                    Paint();

                    stopWatch.Stop();
                    var msDiff = stopWatch.ElapsedMilliseconds;
                    System.Diagnostics.Debug.Print("Paint including wait for movement took (ms): " + msDiff.ToString());
                    var fpsSleepMs = 1000 / fps;
                    var sleepMs = Convert.ToInt32(Math.Round((double)(fpsSleepMs - msDiff)));
                    if (sleepMs > 0)
                    {
                        Thread.Sleep(sleepMs);
                    }
                }
            });

            snakePositions = new Pos2D[1];
            snakeDirection = Pos2D.Direction.Right;
            snakePositions[0] = new Pos2D(FieldX() / 2, FieldY() / 2);
            ExtendSnake(EXTENSION_TIMES - 1);

            GenerateApple();

            Console.CursorVisible = false;

            inputThread.Start();
            gamePhysicsThread.Start();
            renderingThread.Start();

            renderingThread.Join();

            string msg1 = "Game over! Score: " + Score.ToString();
            const string msg2 = "Press ENTER to exit";

            if (gameOver)
            {
                Console.SetCursorPosition(Console.WindowWidth / 2 - msg1.Length / 2, Console.WindowHeight / 2);
                Console.Write(msg1);
            }
            Console.Write("\r\n\r\n");
            Console.SetCursorPosition(Console.WindowWidth / 2 - msg2.Length / 2, Console.WindowHeight / 2 + 1);
            Console.WriteLine(msg2);
            inputThread.Join();
        }

        static int FieldX()
        {
            return Console.WindowWidth;
        }

        static int FieldY()
        {
            return fieldArea / FieldX();
        }

        static void GenerateApple()
        {
            var random = new Random();
            applePos = new Pos2D(random.Next(0, FieldX()), random.Next(0, FieldY()));
        }

        static void OnInput(ConsoleKeyInfo args)
        {
            switch (args.Key)
            {
                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    if (snakeDirection != Pos2D.Direction.Right && snakePositions[snakePositions.Length - 1].Direction_ != Pos2D.Direction.Right)
                        snakeDirection = Pos2D.Direction.Left;
                    break;

                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    if (snakeDirection != Pos2D.Direction.Left && snakePositions[snakePositions.Length - 1].Direction_ != Pos2D.Direction.Left)
                        snakeDirection = Pos2D.Direction.Right;
                    break;

                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    if (snakeDirection != Pos2D.Direction.Down && snakePositions[snakePositions.Length - 1].Direction_ != Pos2D.Direction.Down)
                        snakeDirection = Pos2D.Direction.Up;
                    break;

                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    if (snakeDirection != Pos2D.Direction.Up && snakePositions[snakePositions.Length - 1].Direction_ != Pos2D.Direction.Up)
                        snakeDirection = Pos2D.Direction.Down;
                    break;

                case ConsoleKey.Enter:
                    if (!runGame)
                    {
                        input = false;
                    }
                    break;
            }
        }

        static void ExtendSnake(int times = 1)
        {
            var initLength = snakePositions.Length;
            Array.Resize(ref snakePositions, initLength + times);
            for (int i = initLength; i < snakePositions.Length; ++i)
            {
                switch (snakeDirection)
                {
                    case Pos2D.Direction.Left:
                        {
                            var pos = new Pos2D(snakePositions[i - 1].X - 1, snakePositions[i - 1].Y);
                            if (pos.X >= 0)
                            {
                                snakePositions[i] = pos;
                            }
                            else
                            {
                                snakePositions[i] = new Pos2D(FieldX() + pos.X, pos.Y);
                            }
                            break;
                        }

                    case Pos2D.Direction.Right:
                        {
                            var pos = new Pos2D(snakePositions[i - 1].X + 1, snakePositions[i - 1].Y);
                            if (pos.X < FieldX())
                            {
                                snakePositions[i] = pos;
                            }
                            else
                            {
                                snakePositions[i] = new Pos2D(pos.X - FieldX(), pos.Y);
                            }
                            break;
                        }

                    case Pos2D.Direction.Up:
                        {
                            var pos = new Pos2D(snakePositions[i - 1].X, snakePositions[i - 1].Y - 1);
                            if (pos.Y >= 0)
                            {
                                snakePositions[i] = pos;
                            }
                            else
                            {
                                snakePositions[i] = new Pos2D(pos.X, FieldY() + pos.Y);
                            }
                            break;
                        }

                    case Pos2D.Direction.Down:
                        {
                            var pos = new Pos2D(snakePositions[i - 1].X, snakePositions[i - 1].Y + 1);
                            if (pos.Y < FieldY())
                            {
                                snakePositions[i] = pos;
                            }
                            else
                            {
                                snakePositions[i] = new Pos2D(pos.X, pos.Y - FieldY());
                            }
                            break;
                        }
                }
            }
        }

        static void AddPoints(int points)
        {
            Score += points;
        }

        static void GameOver()
        {
            gameOver = true;
            runGame = false;
        }

        static void CollisionCheck()
        {
            for (int i = 0; i < snakePositions.Length; ++i)
            {
                var snakePos = snakePositions[i];
                for (int j = 0; j < snakePositions.Length; ++j)
                {
                    if (i != j && snakePositions[i].Equals(snakePositions[j]))
                    {
                        //snake collision
                        GameOver();
                    }
                }

                if (snakePos.Equals(applePos))
                {
                    AddPoints(POINTS_APPLE);
                    ExtendSnake(EXTENSION_TIMES);
                    GenerateApple();
                }
            }
        }

        static void Move()
        {
            for (int i = 0; i < snakePositions.Length - 1; ++i)
            {
                snakePositions[i] = snakePositions[i + 1];
            }

            switch (snakeDirection)
            {
                case Pos2D.Direction.Left:
                    {
                        var pos = snakePositions[snakePositions.Length - 1].Add(-1, 0);
                        if (pos.X >= 0)
                        {
                            snakePositions[snakePositions.Length - 1] = pos;
                        }
                        else
                        {
                            snakePositions[snakePositions.Length - 1] = new Pos2D(FieldX() + pos.X, pos.Y);
                        }
                        break;
                    }

                case Pos2D.Direction.Right:
                    {
                        var pos = snakePositions[snakePositions.Length - 1].Add(1, 0);
                        if (pos.X < FieldX())
                        {
                            snakePositions[snakePositions.Length - 1] = pos;
                        }
                        else
                        {
                            snakePositions[snakePositions.Length - 1] = new Pos2D(pos.X - FieldX(), pos.Y);
                        }
                        break;
                    }

                case Pos2D.Direction.Up:
                    {
                        var pos = snakePositions[snakePositions.Length - 1].Add(0, -1);
                        if (pos.Y >= 0)
                        {
                            snakePositions[snakePositions.Length - 1] = pos;
                        }
                        else
                        {
                            snakePositions[snakePositions.Length - 1] = new Pos2D(pos.X, FieldY() + pos.Y);
                        }
                        break;
                    }

                case Pos2D.Direction.Down:
                    {
                        var pos = snakePositions[snakePositions.Length - 1].Add(0, 1);
                        if (pos.Y < FieldY())
                        {
                            snakePositions[snakePositions.Length - 1] = pos;
                        }
                        else
                        {
                            snakePositions[snakePositions.Length - 1] = new Pos2D(pos.X, pos.Y - FieldY());
                        }
                        break;
                    }
            }
            snakePositions[snakePositions.Length - 1].Direction_ = snakeDirection;

            CollisionCheck();
        }

        static int lastFieldX = FieldX();
        static int lastFieldY = FieldY();

        static void Paint()
        {
            var resized = lastFieldX != FieldX() || lastFieldY != FieldY();

            if (resized)
            {
                writtenConsolePositions.Clear();
                lastFieldX = FieldX();
                lastFieldY = FieldY();

                Console.CursorVisible = false;
                for (int y = 0; y < Console.WindowHeight; ++y)
                {
                    for (int x = 0; x < Console.WindowWidth; ++x)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(" \b\b");
                    }
                }

                if (applePos.X >= FieldX() || applePos.Y >= FieldY())
                {
                    // apple is outside field boundaries, generate a new
                    GenerateApple();
                }
            }

            var correctConsolePositions = new List<Pos2D>();
            foreach (var snakePos in snakePositions)
            {
                if (snakePos != null)
                {
                    correctConsolePositions.Add(snakePos);
                    if (writtenConsolePositions.Contains(snakePos))
                    {
                        continue;
                    }

                    Console.SetCursorPosition(snakePos.X, snakePos.Y);
                    Console.Write("*");
                    writtenConsolePositions.Add(snakePos);
                }
            }

            correctConsolePositions.Add(applePos);
            if (!writtenConsolePositions.Contains(applePos))
            {
                Console.SetCursorPosition(applePos.X, applePos.Y);
                Console.Write("Q");
                writtenConsolePositions.Add(applePos);
            }

            var writtenConsolePositionsToRemove = new List<Pos2D>();
            foreach (var writtenPos in writtenConsolePositions)
            {
                var correct = false;
                foreach (var correctConsolePos in correctConsolePositions)
                {
                    if (correctConsolePos.Equals(writtenPos))
                    {
                        correct = true;
                        break;
                    }
                }

                if (!correct)
                {
                    Console.SetCursorPosition(writtenPos.X, writtenPos.Y);
                    Console.Write(" \b\b");
                    writtenConsolePositionsToRemove.Add(writtenPos);
                }
            }

            foreach (var toRemove in writtenConsolePositionsToRemove)
            {
                writtenConsolePositions.Remove(toRemove);
            }
        }
    }
}
