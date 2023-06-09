﻿using System.Text.Json;
using Tetris.Console_;
using Tetris.Events;

namespace Tetris.Inits
{
    internal class Menu
    {
        public static char[] logo = "TETRIS".ToArray();
        public static char[] play = "[PLAY]".ToArray();
        public static char[] exit = "[EXIT]".ToArray();
        public static int SelectedButton { get; set; } = 0;
        // Information der skal gemmes til harddisk V
        public static int LastScore { get; set; } = 0;
        public static int LastLines { get; set; } = 0;
        public static int HighScore { get; set; } = 0;
        public static int HighLines { get; set; } = 0;
        // ----------------------------------------
        public static int rows = 16;
        public static int cols = 40;
        public static string[,] UI = new string[rows, cols];
        public static void Main()
        {
            SelectedButton = 0;
            Console.Clear();
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Tetris";
            Console.SetWindowSize(90, Game.DisplayRow);
            ConsoleFontSize.SetConsoleFontSize(30);
            Console.CursorVisible = false;
            Initialize();
            ScoreScan();
        }
        // UI initialisation
        public static void Initialize()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    UI[row, col] = Game.BoardASCII;
                }
            }
            for (int col = 0; col < cols; col++)
            {
                UI[rows - 1, col] = Game.TetriminoASCII;
            }

            for (int col = 0; col < logo.Length; col++)
            {
                UI[3, 16 + col] = logo[col].ToString();
            }
            for (int col = 0; col < logo.Length; col++)
            {
                UI[8, 12 + col] = play[col].ToString();
            }
            for (int col = 0; col < logo.Length; col++)
            {
                UI[8, 20 + col] = exit[col].ToString();
            }
        }
        // Indhenter data fra json filen hvis muligt
        public static void ScoreScan()
        {
            try
            {
                string json;

                using (StreamReader sr = new(JSON.jsonPath))
                {
                    json = sr.ReadToEnd();
                }
                
                JsonDocument doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("Highscore", out JsonElement highscorejson))
                {
                    HighScore = highscorejson.GetInt32();
                }
                if (doc.RootElement.TryGetProperty("Highlines", out JsonElement highlinesjson))
                {
                    HighLines = highlinesjson.GetInt32();
                }
                if (doc.RootElement.TryGetProperty("Lastscore", out JsonElement lastscorejson))
                {
                    LastScore = lastscorejson.GetInt32();
                }
                if (doc.RootElement.TryGetProperty("Lastlines", out JsonElement lastlinesjson))
                {
                    LastLines = lastlinesjson.GetInt32();
                }
            }
            catch (Exception) {};
            string highscore = $"HIGHSCORE: {HighScore} POINTS {HighLines} LINES";
            string lastscore = $"LAST SCORE: {LastScore} POINTS {LastLines} LINES";
            for (int col = 0; col < highscore.Length; col++)
            {
                UI[11, 1 + col] = highscore[col].ToString();
            }
            for (int col = 0; col < lastscore.Length; col++)
            {
                UI[13, 1 + col] = lastscore[col].ToString();
            }
            Printer.Print(UI, printmenu: true);
            _();
        }

        // Læser brugerens indtastninger på menuen
        public static void _()
        {
            while (true)
            {
                var CKey = Console.ReadKey(true);
                switch (CKey.Key)
                {
                    case ConsoleKey.LeftArrow:
                        SelectedButton = 0;
                        Printer.Print(UI, printmenu: true);
                        continue;
                    case ConsoleKey.RightArrow:
                        SelectedButton = 1;
                        Printer.Print(UI, printmenu: true);
                        continue;
                    case ConsoleKey.Enter:
                        if (SelectedButton == 0)
                        {
                            Console.Clear();
                            Game.Start();
                        }
                        else
                        {
                            Environment.Exit(0);
                        }
                        break;
                    default:
                        continue;
                }
                break;
            }
        }

    }
}