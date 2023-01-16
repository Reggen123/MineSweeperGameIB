using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;

namespace SaperLab2WPF
{
    public class CellApplyViewModel: INotifyPropertyChanged, IObserver
    {
        public Cell ThisCell;
        public int CurrentAnimation = 0;
        private int CurrentAnimationTimeStart = 0;
        private bool AnimationStarted = false;


        public CellApplyViewModel(Cell cell)
        {
            ThisCell = cell;
        }

        public int CurrentAnimationFrame
        {
            get
            {
                if (CurrentAnimation - CurrentAnimationTimeStart < 0)
                    return 0;
                else if (CurrentAnimation - CurrentAnimationTimeStart > 12)
                    return 12;
                else
                    return CurrentAnimation - CurrentAnimationTimeStart;
            }
        }

        public bool IsOpened
        {
            get
            {
                return ThisCell.IsOpened;
            }
            set
            {
                ThisCell.IsOpened = value;
                OnPropertyChanged("IsOpened");
                OnPropertyChanged("CurrentImage");
                OnPropertyChanged("BgColor");
            }
        }
        public bool IsFlagged
        {
            get
            {
                return ThisCell.IsFlagged;
            }
            set
            {
                ThisCell.IsFlagged = value;
            }
        }
        public bool IsQuestioned
        {
            get
            {
                return ThisCell.IsQuestioned;
            }
            set
            {
                ThisCell.IsQuestioned = value;
            }
        }
        public string BgColor
        {
            get
            {
                if (ThisCell.IsMine && ThisCell.IsOpened)
                    return "Red";
                else if (ThisCell.IsOpened)
                    return "LightGray";
                else
                    return "White";
            }
        }
        public ImageSource CurrentImage
        {
            get
            {
                if (ThisCell.IsFlagged)
                    return ImageContainer.ByteToImage((byte[])Resource1.ResourceManager.GetObject("flagpixel"));
                else if (ThisCell.IsQuestioned)
                    return ImageContainer.ByteToImage((byte[])Resource1.ResourceManager.GetObject("QuestionMark"));
                else if (ThisCell.IsOpened && ThisCell.IsMine)
                    return ImageContainer.ByteToImage((byte[])Resource1.ResourceManager.GetObject($"mineanim{CurrentAnimationFrame}"));
                else if (ThisCell.IsOpened && ThisCell.MinesCloseBy != null && ThisCell.MinesCloseBy != 0)
                    return ImageContainer.ByteToImage((byte[])Resource1.ResourceManager.GetObject($"_{ThisCell.MinesCloseBy}pixel"));
                else
                    return null;

            }
        }

        public void Detonate(int waittime)
        {
            if (AnimationStarted)
                return;
            AnimationStarted = true;
            CurrentAnimationTimeStart = waittime;
        }

        public void AnimTick()
        {
            if (!AnimationStarted)
                return;
            CurrentAnimation++;
            OnPropertyChanged("CurrentImage");
        }

        //RelayCommands

        RelayCommand? flagCommand;
        public RelayCommand FlagCommand
        {
            get
            {
                return flagCommand ??
                  (flagCommand = new RelayCommand(obj =>
                  {
                      ThisCell.FlagIt();
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
                      ThisCell.AccordIt();
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
                      ThisCell.OpenIt();
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
                      ThisCell.PointIt();
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
                      ThisCell.StopPointIt();
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
            OnPropertyChanged("IsFlagged");
            OnPropertyChanged("IsQuestioned");
            OnPropertyChanged("CurrentImage");
        }

        public void UpdateCFlagged(int count)
        {
            OnPropertyChanged("IsFlagged");
            OnPropertyChanged("IsQuestioned");
            OnPropertyChanged("CurrentImage");
        }

        public void UpdateCPointed(int count)
        {
        }

        public void UpdateLose()
        {
            ThisCell.OpenAsMine();
            Detonate(new Random().Next(0, 5));
            OnPropertyChanged("IsOpened");
            OnPropertyChanged("CurrentImage");
            OnPropertyChanged("BgColor");
        }

        public void UpdateWin()
        {
            OnPropertyChanged("IsOpened");
            OnPropertyChanged("CurrentImage");
            OnPropertyChanged("BgColor");
        }

        public void UpdateRestart()
        {
        }

        public void UpdateFirstCellOpened(int x, int y)
        {
        }

        public void UpdateCellOpened(int x, int y)
        {
            OnPropertyChanged("IsOpened");
            OnPropertyChanged("CurrentImage");
            OnPropertyChanged("BgColor");
            ThisCell.OpenNearBy(x, y);
        }

        public void UpdateCellClosed(int x, int y)
        {
        }

        public void UpdateCellOpenedForced(int x, int y)
        {
            OnPropertyChanged("IsOpened");
            OnPropertyChanged("CurrentImage");
            OnPropertyChanged("IsFlagged");
            OnPropertyChanged("IsQuestioned");
            OnPropertyChanged("BgColor");
        }

        public void UpdateCQuestioned()
        {
            OnPropertyChanged("CurrentImage");
            OnPropertyChanged("IsQuestioned");
            OnPropertyChanged("BgColor");
        }
    }
}
