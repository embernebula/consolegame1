﻿using Tetris.Events;
using Tetris.Inits;
using Tetris.Tetrimino_;

namespace Tetris.Tasks
{
    internal static class Controller
    {
        public static void _(CancellationToken token)
        {
            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }

            var CKey = Console.ReadKey(true);
            switch (CKey.Key)
            {
                case ConsoleKey.LeftArrow:
                    MoveLeft(token);
                    break;
                case ConsoleKey.RightArrow:
                    MoveRight(token);
                    break;
                case ConsoleKey.DownArrow:
                    MoveDown(token);
                    break;
                case ConsoleKey.R:
                    Rotate(token);
                    break;
                default:
                    _(Game.Cts!.Token);
                    break;
            }
        }

        private static void MoveLeft(CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
            if (GameLoop.RunningTetriminoInstance != null && GameLoop.RunningTetriminoInstance.IsActive)
            {
                if (!HasCollided(GameLoop.RunningTetriminoInstance.Shape!, Game.Board!, 0, -1))
                {
                    GameLoop.EraseTetriminoFromBoard(GameLoop.RunningTetriminoInstance, Game.Board!);
                    GameLoop.RunningTetriminoInstance.X -= 1;
                    if (IsOutOfBound(GameLoop.RunningTetriminoInstance.Shape!, Game.Board!))
                    {
                        GameLoop.RunningTetriminoInstance.X += 1;
                    }
                    GameLoop.DrawTetriminoOnBoard(GameLoop.RunningTetriminoInstance, Game.Board!);
                    Printer.Print(Game.Board!, true);
                    _(Game.Cts!.Token);
                }
                else
                {
                    _(Game.Cts!.Token);
                }
            }
        }

        private static void MoveRight(CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
            if (GameLoop.RunningTetriminoInstance != null && GameLoop.RunningTetriminoInstance.IsActive)
            {
                if (!HasCollided(GameLoop.RunningTetriminoInstance.Shape!, Game.Board!, 0, 1))
                {
                    GameLoop.EraseTetriminoFromBoard(GameLoop.RunningTetriminoInstance, Game.Board!);
                    GameLoop.RunningTetriminoInstance.X += 1;
                    if (IsOutOfBound(GameLoop.RunningTetriminoInstance.Shape!, Game.Board!))
                    {
                        GameLoop.RunningTetriminoInstance.X -= 1;
                    }
                    GameLoop.DrawTetriminoOnBoard(GameLoop.RunningTetriminoInstance, Game.Board!);
                    Printer.Print(Game.Board!, true);
                    _(Game.Cts!.Token);
                }
                else
                {
                    _(Game.Cts!.Token);
                }
            }
        }
        private static void MoveDown(CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
            if (GameLoop.RunningTetriminoInstance != null)
            {
                if (GameLoop.RunningTetriminoInstance.IsActive)
                {
                    if (!HasCollided(GameLoop.RunningTetriminoInstance.Shape!, Game.Board!, 1, 0))
                    {
                        Game.Score += 2;
                        Game.UpdateScoreDisplay();
                        Printer.Print(Game.Board!, printscore: true);
                        GameLoop.EraseTetriminoFromBoard(GameLoop.RunningTetriminoInstance, Game.Board!);
                        GameLoop.RunningTetriminoInstance.Y += 1;
                        if (IsOutOfBound(GameLoop.RunningTetriminoInstance.Shape!, Game.Board!))
                        {
                            GameLoop.RunningTetriminoInstance.Y -= 1;
                        }
                        GameLoop.DrawTetriminoOnBoard(GameLoop.RunningTetriminoInstance, Game.Board!);
                        Printer.Print(Game.Board!, true);
                        _(Game.Cts!.Token);
                    }
                    else
                    {
                        TetriminoManager.CycleComplete();
                        GameLoop.EraseTetriminoFromBoard(GameLoop.RunningTetriminoInstance, Game.Board!);
                        GameLoop.RunningTetriminoInstance.Y += 1;
                        GameLoop.DrawTetriminoOnBoard(GameLoop.RunningTetriminoInstance, Game.Board!);
                        Printer.Print(Game.Board!, true);
                        _(Game.Cts!.Token);
                    }
                }
            }
        }

        public static void Rotate(CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
            if (GameLoop.RunningTetriminoInstance != null)
            {
                if (GameLoop.RunningTetriminoInstance.IsActive)
                {
                    //int Rows = GameLoop.RunningTetriminoInstance.Shape!.GetLength(0);
                    //int Columns = GameLoop.RunningTetriminoInstance.Shape.GetLength(1);
                    Stack<int[,]> Stack = new();
                    //int[,] RotatedTetrimino = new int[Columns, Rows];
                    int N = GameLoop.RunningTetriminoInstance.Shape!.GetLength(0);
                    int[,] RotatedTetrimino = new int[N, N];

                    for (int oldY = 0; oldY < N; oldY++)
                    {
                        for (int oldX = 0; oldX < N; oldX++)
                        {
                            int newY = N - 1 - oldX;
                            int newX = oldY;
                            RotatedTetrimino[newX, newY] = GameLoop.RunningTetriminoInstance.Shape[oldX, oldY];
                        }
                    }
                    Stack.Push(GameLoop.RunningTetriminoInstance.Shape);
                    Stack.Push(RotatedTetrimino);
                    if (IsOutOfBound(RotatedTetrimino, Game.Board!) || HasCollided(RotatedTetrimino, Game.Board!, 0, 0))
                    {
                        Stack.Pop();
                    }
                    GameLoop.EraseTetriminoFromBoard(GameLoop.RunningTetriminoInstance, Game.Board!);
                    GameLoop.RunningTetriminoInstance.Shape = Stack.Peek();
                    GameLoop.DrawTetriminoOnBoard(GameLoop.RunningTetriminoInstance, Game.Board!);
                    Printer.Print(Game.Board!, true);
                    _(Game.Cts!.Token);
                }
            }
        }

        public static bool IsOutOfBound(int[,] tetrimino, string[,] board)
        {
            for (int row = 0; row < tetrimino.GetLength(0); row++)
            {
                for (int col = 0; col < tetrimino.GetLength(1); col++)
                {
                    if (tetrimino[row, col] == 1)
                    {
                        int Col = GameLoop.RunningTetriminoInstance!.X + col;
                        if (Col < 0 || Col >= board.GetLength(1))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool HasCollided(int[,] tetrimino, string[,] board, int rowOffset, int colOffset)
        {
            string[,] boardCopy = (string[,])board.Clone();
            GameLoop.EraseTetriminoFromBoard(GameLoop.RunningTetriminoInstance!, boardCopy);

            for (int row = 0; row < tetrimino.GetLength(0); row++)
            {
                for (int col = 0; col < tetrimino.GetLength(1); col++)
                {
                    if (tetrimino[row, col] != 0)
                    {
                        int boardRow = GameLoop.RunningTetriminoInstance!.Y + row + rowOffset;
                        int boardCol = GameLoop.RunningTetriminoInstance!.X + col + colOffset;

                        if (boardRow < 0 || boardRow >= boardCopy.GetLength(0) || boardCol < 0 || boardCol >= boardCopy.GetLength(1))
                        {
                            return true;
                        }
                        else if (boardCopy[boardRow, boardCol] == Game.TetriminoASCII)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }





    }
}