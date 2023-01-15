using System;
using System.Collections.Generic;
using System.Text;

namespace SaperLab2WPF
{
    public interface IObservable
    {
        public void AddObserver(IObserver o, bool itstemp);
        public void RemoveObserver(IObserver o, bool istemp);
        public void RemoveObservers(bool istemp);

        public void NotifyObserversMFlagged(int count);
        public void NotifyObserversCFlagged(int count);
        public void NotifyObserversCPointed(int count);
        public void NotifyObserversLose();
        public void NotifyObserversWin();
        public void NotifyObserversRestart();
        public void NotifyObserversFirstCellOpened(int x, int y);
        public void NotifyObserversCellOpened(int x, int y);
        public void NotifyObserversCellClosed(int x, int y);
    }

    public interface IObserver
    {
        public void UpdateMFlagged(int count);
        public void UpdateCFlagged(int count);
        public void UpdateCPointed(int count);
        public void UpdateLose();
        public void UpdateWin();
        public void UpdateRestart();
        public void UpdateFirstCellOpened(int x, int y);
        public void UpdateCellOpened(int x, int y);
        public void UpdateCellClosed(int x, int y);
    }
}
