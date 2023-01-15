using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace SaperLab2WPF
{
    class ApplyViewModelMini: INotifyPropertyChanged
    {
        private string labeltext;
        public ApplyViewModelMini()
        {
            OnPropertyChanged("IsEasyLevelSelected");
            OnPropertyChanged("IsNormalLevelSelected");
            OnPropertyChanged("IsHardLevelSelected");
            OnPropertyChanged("IsSpecialLevelSelected");
            OnPropertyChanged("IsAnimationsWorking");
            OnPropertyChanged("IsWinningWithOnlyCellOpening");
            OnPropertyChanged("IsQuestionsEnabled");
            OnPropertyChanged("IsHintEnabled");
            OnPropertyChanged("IsHistoryWorking");
            OnPropertyChanged("LabelTextY");
            OnPropertyChanged("LabelTextX");
        }

        public bool IsEasyLevelSelected
        {
            get
            {
                if (GameManager.singleton.Difficulty == GameDifficulty.Easy)
                    return true;
                else
                    return false;
            }
            set
            {
                if(value)
                {
                    GameManager.singleton.Difficulty = GameDifficulty.Easy;
                    OnPropertyChanged("IsEasyLevelSelected");
                    OnPropertyChanged("IsNormalLevelSelected");
                    OnPropertyChanged("IsHardLevelSelected");
                    OnPropertyChanged("IsSpecialLevelSelected");
                }
            }
        }

        public bool IsNormalLevelSelected
        {
            get
            {
                if (GameManager.singleton.Difficulty == GameDifficulty.Normal)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value)
                {
                    GameManager.singleton.Difficulty = GameDifficulty.Normal;
                    OnPropertyChanged("IsEasyLevelSelected");
                    OnPropertyChanged("IsNormalLevelSelected");
                    OnPropertyChanged("IsHardLevelSelected");
                    OnPropertyChanged("IsSpecialLevelSelected");
                }
            }
        }

        public bool IsHardLevelSelected
        {
            get
            {
                if (GameManager.singleton.Difficulty == GameDifficulty.Hard)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value)
                {
                    GameManager.singleton.Difficulty = GameDifficulty.Hard;
                    OnPropertyChanged("IsEasyLevelSelected");
                    OnPropertyChanged("IsNormalLevelSelected");
                    OnPropertyChanged("IsHardLevelSelected");
                    OnPropertyChanged("IsSpecialLevelSelected");
                }
            }
        }

        public bool IsSpecialLevelSelected
        {
            get
            {
                if (GameManager.singleton.Difficulty == GameDifficulty.Special)
                    return true;
                else
                    return false;
            }
            set
            {
                if (value)
                {
                    GameManager.singleton.Difficulty = GameDifficulty.Special;
                    OnPropertyChanged("IsEasyLevelSelected");
                    OnPropertyChanged("IsNormalLevelSelected");
                    OnPropertyChanged("IsHardLevelSelected");
                    OnPropertyChanged("IsSpecialLevelSelected");
                }
            }
        }

        public bool IsQuestionsEnabled
        {
            get
            {
                return GameManager.singleton.IsQuestionsEnabled;
            }
            set
            {
                GameManager.singleton.IsQuestionsEnabled = value;
                OnPropertyChanged("IsQuestionsEnabled");
            }
        }

        public bool IsWinningWithOnlyCellOpening
        {
            get
            {
                return GameManager.singleton.WinWithOnlyCellOpening;
            }
            set
            {
                GameManager.singleton.WinWithOnlyCellOpening = value;
                OnPropertyChanged("IsWinningWithOnlyCellOpening");
            }
        }

        public bool IsAnimationsWorking
        {
            get
            {
                return GameManager.singleton.IsAnimationsWork;
            }
            set
            {
                GameManager.singleton.IsAnimationsWork = value;
                OnPropertyChanged("IsAnimationsWorking");
            }
        }

        public string LabelTextX
        {
            get
            {
                return labeltext;
            }
            set
            {
                if (int.TryParse(value, out GameManager.singleton.Rows))
                    labeltext = value;
                OnPropertyChanged("LabelTextX");
            }
        }
        public string LabelTextY
        {
            get
            {
                return labeltext;
            }
            set
            {
                if (int.TryParse(value, out GameManager.singleton.Cols))
                    labeltext = value;
                OnPropertyChanged("LabelTextY");
            }
        }
        public string LabelTextMines
        {
            get
            {
                return labeltext;
            }
            set
            {
                if (int.TryParse(value, out GameManager.singleton.minesCount))
                    labeltext = value;
                OnPropertyChanged("LabelTextMines");
            }
        }

        public bool IsHintEnabled
        {
            get
            {
                return GameManager.singleton.IsHintEnabled;
            }
            set
            {
                GameManager.singleton.IsHintEnabled = value;
                OnPropertyChanged("IsHintEnabled");
            }
        }

        public bool IsHistoryWorking
        {
            get
            {
                return GameManager.singleton.IsHistoryWorking;
            }
            set
            {
                GameManager.singleton.IsHistoryWorking = value;
                OnPropertyChanged("IsHistoryWorking");
            }
        }


        RelayCommand? restartCommand;
        public RelayCommand RestartCommand
        {
            get
            {
                return restartCommand ??
                  (restartCommand = new RelayCommand(obj =>
                  {
                      GameManager.singleton.Restart();
                      SettingsLoader.SaveScore(GameManager.singleton.Difficulty, GameManager.singleton.Cols, GameManager.singleton.Rows, IsQuestionsEnabled, IsAnimationsWorking, IsWinningWithOnlyCellOpening, IsHintEnabled, IsHistoryWorking);
                  }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
