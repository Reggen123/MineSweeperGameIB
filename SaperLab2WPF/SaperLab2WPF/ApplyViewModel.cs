using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SaperLab2WPF
{
    public class ApplyViewModel: INotifyPropertyChanged, IObserver
    {
        private ObservableCollection<CellApplyViewModel> cells;
        private List<CellApplyViewModel> mines;
        private GameManager gameManager;
        private ImageSource facerestart;

        private int minefieldborderwidth;
        private int minefieldborderlength;

        private int windowborderwidth;
        private int windowborderlength;

        private DispatcherTimer Timer;
        private DispatcherTimer AnimTimer;
        public ApplyViewModel()
        {
            if (Record.LoadGame("..\\Save") != null)
            {
                gameManager = Record.LoadGame("..\\Save");
                gameManager.AddObserver(this, false);
                StartGame();
                startTimer();
            }
            else
            {
                gameManager = new GameManager();
                gameManager.AddObserver(this, false);
                gameManager.Restart();
            }
            OnPropertyChanged("MineFieldRows");
            OnPropertyChanged("MineFieldCols");
        }

        public ObservableCollection<CellApplyViewModel> Cells
        {
            get
            {
                return this.cells;
            }
        }


        public bool IsHintEnabled
        {
            get
            {
                return gameManager.IsHintEnabled;
            }
        }

        public bool IsHistoryWorking
        {
            get
            {
                return gameManager.IsHistoryWorking;
            }
        }

        public bool? IsCtrlDown
        {
            get
            {
                if (gameManager.State != GameState.Started)
                    return null;
                else
                    return gameManager.IsCtrlDown;
            }
            set
            {
                if (gameManager.State == GameState.Started)
                    gameManager.IsCtrlDown = (bool)value;
            }
        }

        public ImageSource FaceRestartImage
        {
            get
            {
                return facerestart;

            }
            set
            {
                facerestart = value;
            }
        }

        public int MineFieldRows
        {
            get
            {
                return GameManager.singleton.Cols;
            }
            set
            {

            }
        }

        public int MineFieldCols
        {
            get
            {
                return GameManager.singleton.Rows;
            }
            set
            {

            }
        }

        public int WindowBorderWidth
        {
            get
            {
                return windowborderwidth;
            }
            set
            {
                windowborderwidth = value;
                OnPropertyChanged("WindowBorderWidth");
            }

        }

        public int WindowBorderLength
        {
            get
            {
                return windowborderlength;
            }
            set
            {
                windowborderlength = value;
                OnPropertyChanged("WindowBorderLength");
            }

        }

        public int MineFieldBorderWidth
        {
            get
            {
                return minefieldborderwidth;
            }
            set
            {
                minefieldborderwidth = value;
                OnPropertyChanged("MineFieldBorderWidth");
            }
        }

        public int MineFieldBorderLength
        {
            get
            {
                return minefieldborderlength;
            }
            set
            {
                minefieldborderlength = value;
                OnPropertyChanged("MineFieldBorderLength");
            }
        }

        public ImageSource Hundreds
        {
            get
            {
                return ImageContainer.ByteToImage((byte[])Resource1.ResourceManager.GetObject($"_{((int)gameManager.Time / 100) % 10}time"));

            }
        }

        public ImageSource Dozens
        {
            get
            {
                return ImageContainer.ByteToImage((byte[])Resource1.ResourceManager.GetObject($"_{((int)gameManager.Time / 10) % 10}time"));
            }
        }

        public ImageSource Units
        {
            get
            {
                return ImageContainer.ByteToImage((byte[])Resource1.ResourceManager.GetObject($"_{gameManager.Time % 10}time"));
            }

        }
    

        public ImageSource HundredsMinesLeft
        {
            get
            {
                return ImageContainer.ByteToImage((byte[])Resource1.ResourceManager.GetObject($"_{((int)(GameManager.singleton.FlagsLeft / 100)) % 10}time"));
            }
        }

        public ImageSource DozensMinesLeft
        {
            get
            {
                return ImageContainer.ByteToImage((byte[])Resource1.ResourceManager.GetObject($"_{((int)(GameManager.singleton.FlagsLeft / 10)) % 10}time"));
            }
        }

        public ImageSource UnitsMinesLeft
        {
            get
            {
                return ImageContainer.ByteToImage((byte[])Resource1.ResourceManager.GetObject($"_{(GameManager.singleton.FlagsLeft) % 10}time"));            
            }
        }


        private void FaceImageChange(int faceid)
        {
            switch(faceid)
            {
                case 1:
                    {
                        FaceRestartImage = ImageContainer.ByteToImage((byte[])Resource1.ResourceManager.GetObject("facefear"));
                        break;
                    }
                case 2:
                    {
                        FaceRestartImage = ImageContainer.ByteToImage((byte[])Resource1.ResourceManager.GetObject("facejoy"));
                        break;
                    }
                case 3:
                    {
                        FaceRestartImage = ImageContainer.ByteToImage((byte[])Resource1.ResourceManager.GetObject("faceded"));
                        break;
                    }
                case 4:
                    {
                        FaceRestartImage = ImageContainer.ByteToImage((byte[])Resource1.ResourceManager.GetObject("facecool"));
                        break;
                    }
            }
            OnPropertyChanged("FaceRestartImage");
        }
        public void FirstCellOpened(int x, int y)
        {
            gameManager.GetGameField(x, y);
            mines.Clear();
            for (int i = 0; i < GameManager.singleton.Rows; i++)
            {
                for (int j = 0; j < GameManager.singleton.Cols; j++)
                {
                    CellApplyViewModel cell = cells[i * GameManager.singleton.Cols + j];
                    if (i == x && j == y)
                        cell.IsOpened = true;
                    if (gameManager.Cells[i, j].IsMine)
                        mines.Add(cell);
                }
            }
            startTimer();
            OnPropertyChanged("Cells");
        }

        public void Win()
        {
            FaceImageChange(4);
        }

        public void Lose()
        {
            //DetonateMines();
            startTimerAnim();
        }

        private void StartGame()
        {
            cells = new ObservableCollection<CellApplyViewModel>();
            GC.Collect();
            mines = new List<CellApplyViewModel>();
            GC.Collect();
            for (int i = 0; i < GameManager.singleton.Rows; i++)
            {
                for (int j = 0; j < GameManager.singleton.Cols; j++)
                {
                    CellApplyViewModel cell = new CellApplyViewModel(GameManager.singleton.Cells[i, j]);
                    cells.Add(cell);
                    GameManager.singleton.AddObserver(cell, true);
                    if (GameManager.singleton.Cells[i, j].IsMine)
                        mines.Add(cell);
                }
            }
            MineFieldBorderWidth = MineFieldRows * 40;
            MineFieldBorderLength = MineFieldCols * 40;
            WindowBorderWidth += minefieldborderwidth-500;
            WindowBorderLength += minefieldborderlength-450;
            FaceImageChange(2);
            OnPropertyChanged("HundredsMinesLeft");
            OnPropertyChanged("DozensMinesLeft");
            OnPropertyChanged("UnitsMinesLeft");
            OnPropertyChanged("Hundreds");
            OnPropertyChanged("Dozens");
            OnPropertyChanged("Units");
            OnPropertyChanged("Cells");
            OnPropertyChanged("MineFieldRows");
            OnPropertyChanged("MineFieldCols");
            OnPropertyChanged("IsHistoryWorking");
            OnPropertyChanged("IsHintEnabled");
        }

        private void EndGame()
        {
            endtimer();
            OnPropertyChanged("CellsFlagged");
            OnPropertyChanged("Cells");
        }

        private void RestartGame()
        {
            if(gameManager.State == GameState.Started)
            {
                EndGame();
            }
            StartGame();
        }


        //Timers:
        private void startTimer()
        {
            Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
        }

        private void endtimer()
        {
            if (Timer == null)
                return;
            Timer.Tick -= new EventHandler(Timer_Tick);
            Timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            gameManager.Time++;
            OnPropertyChanged("Hundreds");
            OnPropertyChanged("Dozens");
            OnPropertyChanged("Units");
        }

        private void startTimerAnim()
        {
            AnimTimer = new DispatcherTimer();
            AnimTimer.Tick += new EventHandler(Timer_TickAnim);
            AnimTimer.Interval = new TimeSpan(0, 0, 0, 0, 25);
            AnimTimer.Start();
        }

        private void endtimerAnim()
        {
            if (AnimTimer == null)
                return;
            AnimTimer.Tick -= new EventHandler(Timer_TickAnim);
            AnimTimer.Stop();
        }

        private void Timer_TickAnim(object sender, EventArgs e)
        {
            bool allminesdetonated = false;
            foreach(var c in mines)
            {
                if (c.CurrentAnimationFrame > 11)
                    continue;
                c.AnimTick();
                if(c.CurrentAnimationFrame < 12)
                    allminesdetonated = true;
            }
            if (!allminesdetonated)
            {
                endtimerAnim();
            }
        }


        //RelayCommands:

        RelayCommand? restartCommand;
        public RelayCommand RestartCommand
        {
            get
            {
                return restartCommand ??
                  (restartCommand = new RelayCommand(obj =>
                  {
                      GameManager.singleton.Restart();
                  }));
            }
        }

        RelayCommand? openSettingsCommand;
        public RelayCommand OpenSettingsCommand
        {
            get
            {
                return openSettingsCommand ??
                  (openSettingsCommand = new RelayCommand(obj =>
                  {
                      WindowSettings window = new WindowSettings();
                      window.Show();
                  }));
            }
        }

        RelayCommand? hintCommand;
        public RelayCommand HintCommand
        {
            get
            {
                return hintCommand ??
                  (hintCommand = new RelayCommand(obj =>
                  {
                      gameManager.Hint();
                  }));
            }
        }

        RelayCommand? cancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                return cancelCommand ??
                  (cancelCommand = new RelayCommand(obj =>
                  {
                      gameManager.UseHistory();
                  }));
            }
        }

        RelayCommand? openStatisticsCommand;
        public RelayCommand OpenStatisticsCommand
        {
            get
            {
                return openStatisticsCommand ??
                  (openStatisticsCommand = new RelayCommand(obj =>
                  {
                      WindowStatistics window = new WindowStatistics();
                      window.Show();
                  }));
            }
        }

        RelayCommand? ctrlDownCommand;
        public RelayCommand CtrlDownCommand
        {
            get
            {
                return ctrlDownCommand ??
                  (ctrlDownCommand = new RelayCommand(obj =>
                  {
                      if (IsCtrlDown != null)
                          IsCtrlDown = !IsCtrlDown;
                  }));
            }
        }

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
            OnPropertyChanged("HundredsMinesLeft");
            OnPropertyChanged("DozensMinesLeft");
            OnPropertyChanged("UnitsMinesLeft");
        }

        public void UpdateCPointed(int count)
        {
            FaceImageChange(count);
        }

        public void UpdateLose()
        {
            Lose();
            EndGame();
        }

        public void UpdateWin()
        {
            Win();
            EndGame();
        }

        public void UpdateRestart()
        {
            RestartGame();
        }

        public void UpdateFirstCellOpened(int x, int y)
        {
            FirstCellOpened(x, y);
        }

        public void UpdateCellOpened(int x, int y)
        {
        }

        public void UpdateCellClosed(int x, int y)
        {
        }

        public void UpdateCellOpenedForced(int x, int y)
        {
        }

        public void UpdateCQuestioned()
        {
        }
    }

    public class RelayCommand : ICommand
    {
        Action<object?> execute;
        Func<object?, bool>? canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
        public bool CanExecute(object? parameter)
        {
            return canExecute == null || canExecute(parameter);
        }
        public void Execute(object? parameter)
        {
            execute(parameter);
        }
    }
}
