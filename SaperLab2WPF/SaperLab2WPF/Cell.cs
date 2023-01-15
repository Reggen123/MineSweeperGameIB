using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SaperLab2WPF
{
    public class Cell : INotifyPropertyChanged, IObserver
    {
        private int? minescloseby;
        private bool ismine;
        private bool isopened;
        private bool isquestioned;
        private bool isflagged;

        private int x;
        private int y;

        private int CurrentAnimation = 0;
        private int CurrentAnimationTimeStart = 0;
        private bool AnimationStarted = false;
        private DispatcherTimer Timer;


        public Cell(int? minesclose, bool isthismine, int x, int y)
        {
            this.minescloseby = minesclose;
            this.ismine = isthismine;
            this.isopened = false;
            this.isquestioned = false;
            this.isflagged = false;
            this.x = x;
            this.y = y;
        }

        public void ChangeCell(int? minesclose, bool isthismine, int x, int y)
        {
            this.minescloseby = minesclose;
            this.ismine = isthismine;
            this.isopened = false;
            this.isquestioned = false;
            this.isflagged = false;
            this.x = x;
            this.y = y;
            OnPropertyChanged("MinesCloseBy");
            OnPropertyChanged("Content");
        }

        public int? MinesCloseBy
        {
            get
            {
                return minescloseby;
            }
        }

        public bool IsMine
        {
            get
            {
                return ismine;
            }
        }

        public bool IsOpened
        {
            get
            {
                return this.isopened;
            }
            set
            {
                if (this.isopened  || (GameManager.singleton.State != GameState.Started && GameManager.singleton.State != GameState.NotStarted ) || this.isflagged || this.isquestioned)
                    return;
                if (!GameManager.singleton.FirstCellOpened)
                {
                    GameManager.singleton.FirstCellOpen(x, y);
                    return;
                }
                this.isopened = value;
                OnPropertyChanged("IsOpened");
                OnPropertyChanged("CurrentImage");
                OnPropertyChanged("IsFreeToCheck");
                OnPropertyChanged("IsFlagged");
                OnPropertyChanged("BgColor");
                if (this.ismine && GameManager.singleton.State == GameState.Started)
                {
                    GameManager.singleton.Lose();
                }
                if (!value)
                    GameManager.singleton.CellClose(this.x, this.y);
                if(GameManager.singleton.State == GameState.Started)
                    GameManager.singleton.CellOpen(x, y);
            }
        }

        public bool IsForcedOpened
        {
            get
            {
                return this.isopened;
            }
            set
            {
                this.isopened = value;
                if (!value)
                    GameManager.singleton.CellClose(this.x, this.y);
                OnPropertyChanged("IsOpened");
                OnPropertyChanged("CurrentImage");
                OnPropertyChanged("IsFreeToCheck");
                OnPropertyChanged("IsFlagged");
                OnPropertyChanged("BgColor");
            }
        }
        public bool IsFreeToCheck
        {
            get
            {
                return !this.isopened;
            }
        }

        public string BgColor
        {
            get
            {
                if (this.ismine && this.isopened)
                    return "Red";
                else if(this.isopened)
                    return "LightGray";
                else
                    return "White";
            }
        }
        public bool IsFlagged
        {
            get
            {
                return this.isflagged;
            }
            set
            {
                if (!GameManager.singleton.CanPlaceFlag && !this.isflagged)
                {
                    this.isflagged = false;
                    return;
                }
                this.isflagged = value;
                int cell = 0;
                int mine = 0;
                if(this.isflagged)
                {
                    cell = 1;
                    if(ismine)
                    {
                        mine = 1;
                    }
                }
                else
                {
                    if (ismine)
                    {
                        mine = -1;
                    }
                    cell = -1;
                }
                GameManager.singleton.MinesFlagged += mine;
                GameManager.singleton.CellsFlagged += cell;
                OnPropertyChanged("CurrentImage");
            }
        }

        public bool IsQuestioned
        {
            get
            {
                return this.isquestioned;
            }
            set
            {
                this.isquestioned = value;
                OnPropertyChanged("CurrentImage");
            }
        }

        public ImageSource CurrentImage
        {
            get
            {
                if (CurrentAnimation - CurrentAnimationTimeStart < 0)
                    CurrentAnimation = CurrentAnimationTimeStart;
                if (isflagged)
                    return ImageContainer.ByteToImage((byte[])Resource1.ResourceManager.GetObject("flagpixel"));
                else if (isquestioned)
                    return ImageContainer.ByteToImage((byte[])Resource1.ResourceManager.GetObject("QuestionMark"));
                else if (isopened && ismine)
                    return ImageContainer.ByteToImage((byte[])Resource1.ResourceManager.GetObject($"mineanim{CurrentAnimation - CurrentAnimationTimeStart}"));
                else if (isopened && minescloseby != null && minescloseby != 0)
                    return ImageContainer.ByteToImage((byte[])Resource1.ResourceManager.GetObject($"_{minescloseby}pixel"));
                else
                    return null;

            }
        }

        private void OpenNearBy(int xl, int yl)
        {
            if (xl < GameManager.singleton.Cells.GetLength(0) && yl < GameManager.singleton.Cells.GetLength(1) &&
                x < GameManager.singleton.Cells.GetLength(0) && y < GameManager.singleton.Cells.GetLength(1) &&
                x - xl < 2 && x - xl > -2 && y - yl < 2 && y - yl > -2 &&
                GameManager.singleton.Cells[xl, yl].MinesCloseBy == 0 && this == GameManager.singleton.Cells[x,y] &&
                !this.ismine)
            {
                this.IsOpened = true;
            }
        }

        public void OpenAccord()
        {
            int i = x;
            int j = y;
            int rows = GameManager.singleton.Cells.GetLength(0);
            int cols = GameManager.singleton.Cells.GetLength(1);
            if (i > 0 && j > 0 && !GameManager.singleton.Cells[i - 1, j - 1].isflagged)
                GameManager.singleton.Cells[i - 1, j - 1].IsOpened = true;
            if (j > 0 && !GameManager.singleton.Cells[i, j - 1].isflagged)
                GameManager.singleton.Cells[i, j - 1].IsOpened = true;
            if (i + 1 < rows && j > 0 && !GameManager.singleton.Cells[i + 1, j - 1].isflagged)
                GameManager.singleton.Cells[i + 1, j - 1].IsOpened = true;
            if (i + 1 < rows && !GameManager.singleton.Cells[i + 1, j].isflagged)
                GameManager.singleton.Cells[i + 1, j].IsOpened = true;
            if (i + 1 < rows && j + 1 < cols && !GameManager.singleton.Cells[i + 1, j + 1].isflagged)
                GameManager.singleton.Cells[i + 1, j + 1].IsOpened = true;
            if (j + 1 < cols &&  !GameManager.singleton.Cells[i, j + 1].IsMine)
                GameManager.singleton.Cells[i, j + 1].IsOpened = true;
            if (i > 0 && j + 1 < cols && !GameManager.singleton.Cells[i - 1, j + 1].isflagged)
                GameManager.singleton.Cells[i - 1, j + 1].IsOpened = true;
            if (i > 0 &&  !GameManager.singleton.Cells[i - 1, j].isflagged)
                GameManager.singleton.Cells[i - 1, j].IsOpened = true;
        }



        //RelayCommands:

        RelayCommand? flagCommand;
        public RelayCommand FlagCommand
        {
            get
            {
                return flagCommand ??
                  (flagCommand = new RelayCommand(obj =>
                  {
                      if (this.isopened)
                          return;
                      if(IsFlagged)
                      {
                          if (GameManager.singleton.IsQuestionsEnabled)
                              IsQuestioned = true;
                          IsFlagged = false;
                      }    
                      else if(IsQuestioned)
                          IsQuestioned = false;
                      else
                        IsFlagged = true;
                      if(GameManager.singleton.FirstCellOpened)
                      {
                          GameManager.singleton.SaveHistory();
                          GameManager.singleton.SaveGame();
                      }
                  }));
            }
        }

        RelayCommand? accordCommand;
        public RelayCommand AccordCommand
        {
            get
            {
                return accordCommand ??
                  (accordCommand = new RelayCommand(obj =>
                  {
                      if(GameManager.singleton.IsCtrlDown)
                        OpenAccord();
                  }));
            }
        }

        RelayCommand? openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ??
                  (openCommand = new RelayCommand(obj =>
                  {
                      GameManager.singleton.SaveHistory();
                      GameManager.singleton.SaveGame();
                  }));
            }
        }

        RelayCommand? pointCommand;
        public RelayCommand PointCommand
        {
            get
            {
                return pointCommand ??
                  (pointCommand = new RelayCommand(obj =>
                  {
                      if (!this.isopened &&GameManager.singleton.State == GameState.Started)
                          GameManager.singleton.CellPoint(true);
                  }));
            }
        }

        RelayCommand? stoppointCommand;
        public RelayCommand StopPointCommand
        {
            get
            {
                return stoppointCommand ??
                  (stoppointCommand = new RelayCommand(obj =>
                  {
                      if(GameManager.singleton.State == GameState.Started)
                        GameManager.singleton.CellPoint(false);
                  }));
            }
        }


        //Timer:

        public void Detonate(int waittime)
        {
            if (AnimationStarted)
                return;
            AnimationStarted = true;
            CurrentAnimationTimeStart = waittime;
            startTimer();
        }
        private void startTimer()
        {
            Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(this.Timer_Tick);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 25);
            Timer.Start();
        }

        private void endtimer()
        {
            Timer.Tick -= new EventHandler(this.Timer_Tick);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            CurrentAnimation++;
            if (CurrentAnimation - CurrentAnimationTimeStart < 0)
                return;
            if (CurrentAnimation - CurrentAnimationTimeStart > 11)
            {
                Timer.Stop();
                endtimer();
            }
            OnPropertyChanged("CurrentImage");
        }

        //OnPropeprtChanged:

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public void UpdateMFlagged(int count)
        {
        }

        public void UpdateCFlagged(int count)
        {
        }

        public void UpdateCPointed(int count)
        {
        }

        public void UpdateLose()
        {
        }

        public void UpdateWin()
        {
        }

        public void UpdateRestart()
        {
        }

        public void UpdateFirstCellOpened(int x, int y)
        {
        }

        public void UpdateCellOpened(int x, int y)
        {
            OpenNearBy(x, y);
        }

        public void UpdateCellClosed(int x, int y)
        {
        }
    }
}
