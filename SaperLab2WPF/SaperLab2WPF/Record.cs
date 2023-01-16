using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SaperLab2WPF
{
    public class Record
    {
        public static int GetScore(int bv, int time, bool iswin)
        {
            double score = 0;
            if (time < 1)
                time = 1;
            if (iswin)
                score = (bv / time) * 1000;
            else
                score = (bv / time) * 10;
            return (int)score;
        }

        public static void SaveScore(string folder, GameDifficulty difficulty, int time, int cols, int rows, int score)
        {
            if (!Directory.Exists($"{folder}"))
            {
                Directory.CreateDirectory($"{folder}");
            }
            string date = $"{DateTime.Now.Day};{DateTime.Now.Month};{DateTime.Now.Year}-{DateTime.Now.Hour};{DateTime.Now.Minute};{DateTime.Now.Second}";
            RecordInstance record = new RecordInstance(difficulty, time, cols, rows, score, date);
            string jsonstring = JsonSerializer.Serialize<RecordInstance>(record);
            File.WriteAllText($"{folder}\\UserScore_{date}.json", jsonstring);
        }

        public static string ReadScore(string filename)
        {
            string jsonstring = File.ReadAllText(filename);
            RecordInstance? rec = JsonSerializer.Deserialize<RecordInstance>(jsonstring);
            return rec.ToString();
        }

        public static string[] ReadAllScore(string folder)
        {
            if (!Directory.Exists($"{folder}"))
            {
                Directory.CreateDirectory($"{folder}");
            }
            string[] paths = Directory.EnumerateFiles(folder, ".", SearchOption.AllDirectories)
                .Where(s => Path.GetExtension(s).TrimStart('.').ToLowerInvariant() == "json").ToArray();
            List<string> rec = new List<string>();
            for (int i = 0; i < paths.Length; i++)
            {
                string jsonstring = File.ReadAllText(paths[i]);
                try
                {
                    RecordInstance instance = JsonSerializer.Deserialize<RecordInstance>(jsonstring);
                    rec.Add(instance.ToString());
                }
                catch
                {

                }
            }
            return rec.ToArray();
        }

        public static OverallRecordInstance ReadOverAllScore(string folder)
        {
            if (!Directory.Exists($"{folder}"))
            {
                Directory.CreateDirectory($"{folder}");
            }
            if (!File.Exists($"{folder}\\OverAllScore.json"))
                SaveOverAllScore($"{folder}", 0, 0, 0, 0);
            string jsonstring = File.ReadAllText($"{folder}\\OverAllScore.json");
            OverallRecordInstance? rec = JsonSerializer.Deserialize<OverallRecordInstance>(jsonstring);
            return rec;
        }

        public static void SaveOverAllScore(string folder, int gamesplayed, int gameswon, int gameslost, int cellsopened)
        {
            if (!Directory.Exists($"{folder}"))
            {
                Directory.CreateDirectory($"{folder}");
            }
            OverallRecordInstance recordadd = new OverallRecordInstance(gamesplayed, gameswon, gameslost, cellsopened);
            if (File.Exists($"{folder}\\OverAllScore.json"))
            {
                OverallRecordInstance record = ReadOverAllScore(folder);
                recordadd = new OverallRecordInstance(gamesplayed + record.GamesPlayed, gameswon + record.GamesWon, gameslost + record.GamesLost, cellsopened + record.CellsOpen);
            }
            string jsonstring = JsonSerializer.Serialize<OverallRecordInstance>(recordadd);
            File.WriteAllText($"{folder}\\OverAllScore.json", jsonstring);
        }

        public static void SaveGame(string folder, GameDifficulty difficulty, List<byte[,]> boardhistory, Cell[,] cells, int cols, int rows, int minescount, int timer, int bv)
        {
            byte[] minesboard = new byte[rows * cols];
            for (int i = 0; i <rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (cells[i, j].IsMine)
                        minesboard[(i*cols)+j] = 9;
                    else
                        minesboard[(i * cols) + j] = (byte)cells[i, j].MinesCloseBy;
                }
            }
            List<byte[]> boardhistory1dimension = new List<byte[]>();
            foreach (var b in boardhistory)
            {
                byte[] bnew = new byte[rows * cols];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        bnew[(i * cols) + j] = b[i, j];
                    }
                }
                boardhistory1dimension.Add(bnew);
            }
            GameSave save = new GameSave(difficulty, boardhistory1dimension, minesboard, cols, rows, minescount, timer, bv);
            if(!Directory.Exists($"{folder}"))
            {
                Directory.CreateDirectory($"{folder}");
            }
            if (File.Exists($"{folder}\\Save.json"))
            {
                File.Delete($"{folder}\\Save.json");
            }
            string jsonstring = JsonSerializer.Serialize<GameSave>(save);
            File.WriteAllText($"{folder}\\Save.json", jsonstring);
        }

        public static void DeleteSave(string folder)
        {
            if (File.Exists($"{folder}\\Save.json"))
            {
                File.Delete($"{folder}\\Save.json");
            }
        }
        public static GameManager LoadGame(string folder)
        {
            GameManager gameManager = new GameManager();
            if (!Directory.Exists($"{folder}"))
            {
                Directory.CreateDirectory($"{folder}");
            }
            if (!File.Exists($"{folder}\\Save.json"))
                return null;
            string jsonstring = File.ReadAllText($"{folder}\\Save.json");
            GameSave? save = JsonSerializer.Deserialize<GameSave>(jsonstring);

            foreach (var b in save.BoardHistory)
            {
                byte[,] bnew = new byte[save.XW, save.YL];
                for (int i = 0; i < save.XW; i++)
                {
                    for (int j = 0; j < save.YL; j++)
                    {
                        bnew[i, j] = b[(i * save.YL) + j];
                    }
                }
                gameManager.BoardHistory.Add(bnew);
            }


            gameManager.Difficulty = save.GameDifficulty;
            gameManager.FirstCellOpened = save.BoardHistory.Count > 0;
            gameManager.Rows = save.XW;
            gameManager.Cols = save.YL;
            gameManager.minesCount = save.MinesCount;
            gameManager.Time = save.Timer;
            gameManager.BVCount = save.BV;
            gameManager.GetBlankField();

            byte[,] cellsh = gameManager.BoardHistory.Last();
            for (int i = 0; i < save.XW; i++)
            {
                for (int j = 0; j < save.YL; j++)
                {
                    if (save.MinesBoard[(i * save.YL) + j] < 9)
                        gameManager.Cells[i, j].ChangeCell(save.MinesBoard[(i * save.YL) + j], false, i, j);
                    else
                        gameManager.Cells[i, j].ChangeCell(null, true, i, j);

                    if (cellsh[i, j] == 1)
                    {
                        gameManager.Cells[i, j].IsOpened = true;
                    }
                    else if (cellsh[i, j] == 2)
                        gameManager.Cells[i, j].IsFlagged = true;
                    else if (cellsh[i, j] == 3)
                        gameManager.Cells[i, j].IsQuestioned = true;
                    else
                    {
                        if (gameManager.Cells[i, j].IsQuestioned)
                            gameManager.Cells[i, j].IsQuestioned = false;
                        if (gameManager.Cells[i, j].IsFlagged)
                            gameManager.Cells[i, j].IsFlagged = false;
                        if (gameManager.Cells[i, j].IsOpened)
                            gameManager.Cells[i, j].IsForcedOpened = false;
                    }
                }
            }
            //gameManager.CellsFlagged = cellsflagged;
            //gameManager.MinesFlagged = minesflagged;
            gameManager.State = GameState.Started;
            gameManager.IsCtrlDown = false;
            return gameManager;
        }
    }

    public class RecordInstance
    {
        public string Difficulty { get; set; }
        public int Time { get; set; }
        public int Score { get; set; }
        public int Cols { get; set; }
        public int Rows { get; set; }
        public string Date { get; set; }

        public RecordInstance(GameDifficulty difficulty, int time, int cols, int rows, int score, string date)
        {
            Difficulty = difficulty.ToString();
            Time = time;
            Cols = cols;
            Rows = rows;
            Score = score;
            Date = date;
        }

        public RecordInstance()
        {

        }
        public override string ToString()
        {
            return $"My Record - {Date}; Time: {Time}; Score:{Score}; Difficulty:{Difficulty}; Field:{Cols}X{Rows}";
        }
    }

    public class OverallRecordInstance
    {
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
        public int CellsOpen { get; set; }

        public OverallRecordInstance(int gamesplayed, int gameswon, int gameslost, int cellsopen)
        {
            GamesPlayed = gamesplayed;
            GamesWon = gameswon;
            GamesLost = gameslost;
            CellsOpen = cellsopen;
        }

        public OverallRecordInstance()
        {

        }
    }

    public class GameSave
    {
        public List<byte[]> BoardHistory { get; set; }
        public GameDifficulty GameDifficulty { get; set; }
        public byte[] MinesBoard { get; set; }
        public int XW { get; set; }
        public int YL { get; set; }
        public int MinesCount { get; set; }
        public int BV { get; set; }
        public int Timer { get; set; }

        public GameSave(GameDifficulty difficulty, List<byte[]> boardhistory, byte[] minesboard, int cols, int rows, int minescount, int timer, int bv)
        {
            GameDifficulty = difficulty;
            BoardHistory = boardhistory;
            MinesBoard = minesboard;
            XW = rows;
            YL = cols;
            MinesCount = minescount;
            BV = bv;
            Timer = timer;
        }

        public GameSave()
        { 
        }
    }
}
