using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace SaperLab2WPF
{
    public class SettingsLoader
    {

        public static void SaveScore(GameDifficulty difficulty, int cols, int rows, bool isquestions, bool isanimations, bool iswinwithoutflagging, bool ishint, bool ishistory)
        {
            if (!Directory.Exists("..\\netcoreapp3.1\\Settings"))
            {
                Directory.CreateDirectory("..\\netcoreapp3.1\\Settings");
            }
            SettingsInstance settings = new SettingsInstance(difficulty, cols, rows, isquestions, isanimations, iswinwithoutflagging, ishint, ishistory);
            string jsonstring = JsonSerializer.Serialize<SettingsInstance>(settings);
            File.WriteAllText($"..\\netcoreapp3.1\\Settings\\UserSettings.json", jsonstring);
        }

        public static SettingsInstance LoadSave()
        {
            if (!Directory.Exists("..\\netcoreapp3.1\\Settings"))
            {
                Directory.CreateDirectory("..\\netcoreapp3.1\\Settings");
            }
            if (!File.Exists("..\\netcoreapp3.1\\Settings\\UserSettings.json"))
                return null;
            string jsonstring = File.ReadAllText("..\\netcoreapp3.1\\Settings\\UserSettings.json");
            SettingsInstance? save = JsonSerializer.Deserialize<SettingsInstance>(jsonstring);
            return save;
        }
    }

    public class SettingsInstance
    {
        public GameDifficulty Difficulty { get; set; }
        public int Cols { get; set; }
        public int Rows { get; set; }
        public bool IsAnim { get; set; }
        public bool IsQuest { get; set; }
        public bool IsWin { get; set; }
        public bool IsHint { get; set; }
        public bool IsHistory { get; set; }

        public SettingsInstance(GameDifficulty difficulty, int cols, int rows,bool isquestions, bool isanimations, bool iswinwithoutflagging, bool ishint, bool ishistory)
        {
            Difficulty = difficulty;
            Cols = cols;
            Rows = rows;
            IsQuest = isquestions;
            IsAnim = isanimations;
            IsWin = iswinwithoutflagging;
            IsHint = ishint;
            IsHistory = ishistory;
        }

        public SettingsInstance()
        {

        }
    }

}
