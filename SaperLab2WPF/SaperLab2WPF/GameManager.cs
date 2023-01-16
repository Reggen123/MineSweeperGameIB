using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;

namespace SaperLab2WPF
{
    public class GameManager: IObservable
    {
        public int Rows;
        public int Cols;
        public int minesCount;
        private int minesFlagged;
        private int cellsFlagged;
        private int cellsOpened;
        public Cell[,] Cells;
        public List<byte[,]> BoardHistory;
        public bool FirstCellOpened;
        public int BVCount;
        public GameState State;
        private GameDifficulty diffuclty;
        public static GameManager singleton;

        private int time;
        public bool WinWithOnlyCellOpening;
        public bool IsQuestionsEnabled;
        public bool IsAnimationsWork;
        public bool IsHintEnabled;
        public bool IsHistoryWorking;
        public bool IsCtrlDown;

        public List<IObserver> observers;
        public List<IObserver> Tempobservers;

        public GameManager()
        {
            singleton = this;
            observers = new List<IObserver>();
            Tempobservers = new List<IObserver>();
            BoardHistory = new List<byte[,]>();
            SettingsInstance set = SettingsLoader.LoadSave();
            if(set==null)
            {
                Difficulty = GameDifficulty.Easy;
                IsHintEnabled = true;
                IsHistoryWorking = true;
                WinWithOnlyCellOpening = true;
                IsQuestionsEnabled = true;
                IsAnimationsWork = true;
            }
            else
            {
                Difficulty = set.Difficulty;
                IsHintEnabled = set.IsHint;
                IsHistoryWorking = set.IsHistory;
                WinWithOnlyCellOpening = set.IsWin;
                IsQuestionsEnabled = set.IsQuest;
                IsAnimationsWork = set.IsAnim;
            }
        }

        public int CellsFlagged
        {
            get
            {
                return cellsFlagged;
            }
            set
            {
                cellsFlagged = value;
                NotifyObserversCFlagged(cellsFlagged);
            }
        }
        public int MinesFlagged
        {
            get
            {
                return minesFlagged;
            }
            set
            {
                minesFlagged = value;
                NotifyObserversMFlagged(minesFlagged);
                if (minesFlagged == minesCount)
                    Win();
            }
        }
        public bool CanPlaceFlag
        {
            get
            {
                if (cellsFlagged >= minesCount || !FirstCellOpened)
                    return false;
                else
                    return true;
            }
        }
        public int Time
        {
            get
            {
                return this.time;
            }
            set
            {
                if (value < 0)
                    this.time = 0;
                else
                    this.time = value;
            }
        }

        public int FlagsLeft
        {
            get
            {
                if (minesCount - cellsFlagged < 0)
                    return 0;
                else
                    return minesCount - cellsFlagged;
            }
        }

        public GameDifficulty Difficulty
        {
            get
            {
                return diffuclty;
            }
            set
            {
                switch(value)
                {
                    case GameDifficulty.Easy:
                        {
                            Rows = 9;
                            Cols = 9;
                            minesCount = 10;
                            break;
                        }
                    case GameDifficulty.Normal:
                        {
                            Rows = 16;
                            Cols = 16;
                            minesCount = 40;
                            break;
                        }
                    case GameDifficulty.Hard:
                        {
                            Rows = 16;
                            Cols = 30;
                            minesCount = 99;
                            break;
                        }
                }
                diffuclty = value;
            }
        }

        public void CellPoint(bool ispointed)
        {
            if (ispointed)
                NotifyObserversCPointed(1);
            else
                NotifyObserversCPointed(2);
        }

        public void Win()
        {
            if (State != GameState.Started)
                return;
            NotifyObserversWin();
            State = GameState.Won;
            Record.DeleteSave("..\\Save");
            Record.SaveOverAllScore("..\\netcoreapp3.1\\Resources", 1, 1, 0, cellsOpened);
            Record.SaveScore("..\\netcoreapp3.1\\Saves", diffuclty, Time, Rows, Cols, Record.GetScore(BVCount, Time, true));
        }
        public void Lose()
        {
            if (State != GameState.Started)
                return;
            NotifyObserversCPointed(3);
            NotifyObserversLose();
            Record.DeleteSave("..\\Save");
            Record.SaveOverAllScore("..\\netcoreapp3.1\\Resources", 1, 0, 1, cellsOpened);
            Record.SaveScore("..\\netcoreapp3.1\\Saves", diffuclty, Time, Rows, Cols, Record.GetScore(BVCount, Time, false));
            State = GameState.Lost;
            OpenAllMines();
        }

        public void Restart()
        {
            FirstCellOpened = false;
            Cells = null;
            BoardHistory.Clear();
            cellsOpened = 0;
            minesFlagged = 0;
            cellsFlagged = 0;
            IsCtrlDown = false;
            Difficulty = diffuclty;
            State = GameState.Started;
            Time = 0;
            RemoveObservers(true);
            GetBlankField();
            SaveHistory();
            NotifyObserversRestart();
        }

        public void Hint()
        {
            bool Ishintdone = false;
            if(IsQuestionsEnabled)
            {
                foreach (var c in Cells)
                {
                    if (c.IsQuestioned)
                    {
                        if (c.IsMine)
                        {
                            c.IsFlagged = true;
                            c.IsQuestioned = false;
                        }
                        else
                        {
                            c.IsQuestioned = false;
                            c.IsOpened = true;
                        }
                        Ishintdone = true;
                        break;
                    }
                }
            }
            if (Ishintdone)
                return;
            foreach (var c in Cells)
            {
                if (c.IsMine && !c.IsFlagged)
                {
                    c.IsFlagged = true;
                    break;
                }
            }
        }

        public void SaveHistory()
        {
            if (State == GameState.Won || State == GameState.Lost)
                return;
            byte[,] cellsh = new byte[Rows, Cols];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    if (Cells[i, j].IsOpened)
                        cellsh[i, j] = 1;
                    else if (Cells[i, j].IsFlagged)
                        cellsh[i, j] = 2;
                    else if (Cells[i, j].IsQuestioned)
                        cellsh[i, j] = 3;
                    else
                        cellsh[i, j] = 0;

                }
            }
            BoardHistory.Add(cellsh);
        }

        public void SaveGame()
        {
            Record.SaveGame("..\\Save", diffuclty, BoardHistory, Cells, Cols, Rows, minesCount, Time, BVCount);
        }
        public void UseHistory()
        {
            if (State != GameState.Started || BoardHistory.Count<2)
                return;
            CellsFlagged = 0;
            MinesFlagged = 0;
            byte[,] cellsh = BoardHistory[BoardHistory.Count - 2];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    if (cellsh[i, j] == 1)
                        Cells[i, j].IsOpened = true;
                    else if (cellsh[i, j] == 2)
                        Cells[i, j].IsFlagged = true;
                    else if (cellsh[i, j] == 3)
                        Cells[i, j].IsQuestioned = true;
                    else
                    {
                        if(Cells[i, j].IsQuestioned)
                            Cells[i, j].IsQuestioned = false;
                        if (Cells[i, j].IsFlagged)
                        {
                            Cells[i, j].IsFlagged = false;
                            if(Cells[i, j].IsMine)
                                MinesFlagged++;
                            CellsFlagged++;
                        }
                        if(Cells[i, j].IsOpened)
                            Cells[i, j].IsForcedOpened = false;
                    }

                }
            }
            BoardHistory.Remove(BoardHistory.Last());
            SaveGame();
        }

        public void WinWithCellOpenCheck()
        {
            if(cellsOpened == Cells.Length - minesCount && WinWithOnlyCellOpening)
            {
                foreach(var cell in Cells)
                {
                    if (cell.IsMine)
                        cell.IsFlagged = true;
                }
            }
        }
        private void OpenAllMines()
        {
            //foreach(var c in Cells)
            //{
            //    if (c.IsMine)
            //    {
            //        c.IsForcedOpened = true;
            //    }
            //}
        }
        public void FirstCellOpen(int x, int y)
        {
            FirstCellOpened = true;

            NotifyObserversFirstCellOpened(x, y);
        }
        public void CellOpen(int x, int y)
        {
            if (Cells[x, y].IsMine)
            {
                NotifyObserversCellOpened(x, y);
            }
            else
            {
                cellsOpened++;
                WinWithCellOpenCheck();
                NotifyObserversCellOpened(x, y);
            }
        }

        public void CellClose(int x, int y)
        {
            if (Cells[x, y].IsMine)
            {
                NotifyObserversCellClosed(x, y);
            }
            else
            {
                cellsOpened--;
                NotifyObserversCellClosed(x, y);
            }
        }
        public Cell[,] GetBlankField()
        {
            if(Cells != null)
                Array.Clear(Cells, 0, Cells.GetLength(0) * Cells.GetLength(1));
            Cell[,] cells = new Cell[Rows, Cols];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    cells[i, j] = new Cell(null, false, i, j);
                }
            }
            Cells = cells;
            return cells;
        }
        public void GetGameField(int? x = null, int? y = null)
        {
            Random rnd = new Random();
            int rows = Cells.GetLength(0);
            int cols = Cells.GetLength(1);

            int minecount = 0;
            int X = 0;
            int Y = 0;
            while (minecount++ < minesCount)
            {
                do
                {
                    X = rnd.Next(rows);
                    Y = rnd.Next(cols);
                }
                while (Cells[X, Y].IsMine || (X==x && Y==y));

                Cells[X, Y].ChangeCell(null, true, X, Y); // -1 = have mine
            }
            //Расстановка обычных клеток
            for (int i =0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (!Cells[i, j].IsMine)
                    {
                        int nearbymines = 0;
                        if (i > 0 && j > 0 && Cells[i - 1, j - 1] != null && Cells[i - 1, j - 1].IsMine)
                            nearbymines++;
                        if (j > 0 && Cells[i, j - 1] != null && Cells[i, j - 1].IsMine)
                            nearbymines++;
                        if (i + 1 < rows && j > 0 && Cells[i + 1, j - 1] != null && Cells[i + 1, j - 1].IsMine)
                            nearbymines++;
                        if (i + 1 < rows && Cells[i + 1, j] != null && Cells[i + 1, j].IsMine)
                            nearbymines++;
                        if (i + 1 < rows && j + 1 < cols && Cells[i + 1, j + 1] != null && Cells[i + 1, j + 1].IsMine)
                            nearbymines++;
                        if (j + 1 < cols && Cells[i, j + 1] != null && Cells[i, j + 1].IsMine)
                            nearbymines++;
                        if (i > 0 && j + 1 < cols && Cells[i - 1, j + 1] != null && Cells[i - 1, j + 1].IsMine)
                            nearbymines++;
                        if (i > 0 && Cells[i - 1, j] != null && Cells[i - 1, j].IsMine)
                            nearbymines++;
                        //cells[i, j] = new Cell(nearbymines, false, i, j);
                        Cells[i, j].ChangeCell(nearbymines, false, i, j);
                    }
                }
            }
            GetBVCount();
        }
        public void GetBVCount()
        {
            BVCounter bvcounter = new BVCounter(Cells);
            BVCount = bvcounter.Get3BV();
        }

        public void AddObserver(IObserver o, bool istemp)
        {
            if (!istemp)
                observers.Add(o);
            else
                Tempobservers.Add(o);
        }

        public void RemoveObserver(IObserver o, bool istemp)
        {
            if (!istemp)
                observers.Remove(o);
            else
                Tempobservers.Remove(o);
        }

        public void RemoveObservers(bool istemp)
        {
            if (!istemp)
                observers.Clear();
            else
                Tempobservers.Clear();
        }

        public void NotifyObserversMFlagged(int count)
        {
            foreach (IObserver observer in observers)
                observer.UpdateMFlagged(count);
            foreach (IObserver observer in Tempobservers)
                observer.UpdateMFlagged(count);
        }

        public void NotifyObserversCFlagged(int count)
        {
            foreach (IObserver observer in observers)
                observer.UpdateCFlagged(count);
            foreach (IObserver observer in Tempobservers)
                observer.UpdateCFlagged(count);
        }

        public void NotifyObserversCPointed(int count)
        {
            foreach (IObserver observer in observers)
                observer.UpdateCPointed(count);
            foreach (IObserver observer in Tempobservers)
                observer.UpdateCPointed(count);
        }

        public void NotifyObserversLose()
        {
            foreach (IObserver observer in observers)
                observer.UpdateLose();
            foreach (IObserver observer in Tempobservers)
                observer.UpdateLose();
        }

        public void NotifyObserversWin()
        {
            foreach (IObserver observer in observers)
                observer.UpdateWin();
            foreach (IObserver observer in Tempobservers)
                observer.UpdateWin();
        }

        public void NotifyObserversRestart()
        {
            foreach (IObserver observer in observers)
                observer.UpdateRestart();
            foreach (IObserver observer in Tempobservers)
                observer.UpdateRestart();
        }

        public void NotifyObserversFirstCellOpened(int x, int y)
        {
            foreach (IObserver observer in observers)
                observer.UpdateFirstCellOpened(x, y);
            foreach (IObserver observer in Tempobservers)
                observer.UpdateFirstCellOpened(x, y);
        }

        public void NotifyObserversCellOpened(int x, int y)
        {
            foreach (IObserver observer in observers)
                observer.UpdateCellOpened(x, y);
            foreach (IObserver observer in Tempobservers)
                observer.UpdateCellOpened(x, y);
        }

        public void NotifyObserversCellClosed(int x, int y)
        {
            foreach (IObserver observer in observers)
                observer.UpdateCellClosed(x, y);
            foreach (IObserver observer in Tempobservers)
                observer.UpdateCellClosed(x, y);
        }

        public void NotifyObserversCellOpenedForced(int x, int y)
        {
            foreach (IObserver observer in observers)
                observer.UpdateCellOpenedForced(x, y);
            foreach (IObserver observer in Tempobservers)
                observer.UpdateCellOpenedForced(x, y);
        }

        public void NotifyObserversCQuestioned()
        {
            foreach (IObserver observer in observers)
                observer.UpdateCQuestioned();
            foreach (IObserver observer in Tempobservers)
                observer.UpdateCQuestioned();
        }
    }


    public enum GameState
    {
        NotStarted,
        Started,
        Lost,
        Won
    }

    public enum GameDifficulty
    {
        Easy,
        Normal,
        Hard,
        Special
    }
}
