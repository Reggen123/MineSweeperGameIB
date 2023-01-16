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
    public class Cell
    {
        private int? minescloseby;
        private bool ismine;
        private bool isopened;
        private bool isquestioned;
        private bool isflagged;

        public int x;
        public int y;

        public int CurrentAnimation = 0;
        private int CurrentAnimationTimeStart = 0;
        private bool AnimationStarted = false;


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
                GameManager.singleton.NotifyObserversCellOpenedForced(x, y);
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
                GameManager.singleton.NotifyObserversCQuestioned();
            }
        }
        public void OpenNearBy(int xl, int yl)
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

        public void FlagIt()
        {
            if (this.isopened)
                return;
            if (IsFlagged)
            {
                if (GameManager.singleton.IsQuestionsEnabled)
                    IsQuestioned = true;
                IsFlagged = false;
            }
            else if (IsQuestioned)
                IsQuestioned = false;
            else
                IsFlagged = true;
            if (GameManager.singleton.FirstCellOpened)
            {
                GameManager.singleton.SaveHistory();
                GameManager.singleton.SaveGame();
            }
        }
        public void AccordIt()
        {
            if (GameManager.singleton.IsCtrlDown)
                OpenAccord();
        }
        public void OpenIt()
        {
            GameManager.singleton.SaveHistory();
            GameManager.singleton.SaveGame();
        }
        public void PointIt()
        {
            if (!this.isopened && GameManager.singleton.State == GameState.Started)
                GameManager.singleton.CellPoint(true);
        }
        public void StopPointIt()
        {
            if (GameManager.singleton.State == GameState.Started)
                GameManager.singleton.CellPoint(false);
        }

        public void OpenAsMine()
        {
            if(ismine)
                isopened = true;
        }


    }
}
