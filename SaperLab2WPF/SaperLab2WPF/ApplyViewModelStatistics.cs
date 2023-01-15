using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace SaperLab2WPF
{
    public class ApplyViewModelStatistics : INotifyPropertyChanged
    {
        private int gamesplayed;
        private int gameswon;
        private int gameslost;
        private int cellsopened;

        private ObservableCollection<string> scores;
        public ApplyViewModelStatistics()
        {
            string[] score = Record.ReadAllScore("..\\netcoreapp3.1\\Saves");
            scores = new ObservableCollection<string>();
            for (int i = 0; i < score.Length; i++)
            {
                scores.Add(score[i]);
            }
            OverallRecordInstance record = Record.ReadOverAllScore("..\\netcoreapp3.1\\Resources");
            gamesplayed = record.GamesPlayed;
            gameswon = record.GamesWon;
            gameslost = record.GamesPlayed;
            cellsopened = record.CellsOpen;
            OnPropertyChanged("GamesPlayed");
            OnPropertyChanged("GamesWon");
            OnPropertyChanged("GamesLost");
            OnPropertyChanged("CellsOpened");
            OnPropertyChanged("Scores");
        }


        public int GamesPlayed
        {
            get
            {
                return gamesplayed;
            }
        }

        public int GamesWon
        {
            get
            {
                return gameswon;
            }
        }

        public int GamesLost
        {
            get
            {
                return gameslost;
            }
        }

        public int CellsOpened
        {
            get
            {
                return cellsopened;
            }
        }

        public ObservableCollection<string> Scores
        {
            get
            {
                return scores;
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
