using System;
using System.Collections.Generic;
using System.Text;

namespace SaperLab2WPF
{
    public class BVCounter
    {

        private bool[,] cellsmarked;
        private Cell[,] Cells;

        private int CellsRows;
        private int CellsCols;
        private int bvcount;

        public BVCounter(Cell[,] cells)
        {
            Cells = cells;
            bvcount = 0;
            CellsRows = Cells.GetUpperBound(0) + 1;
            CellsCols = Cells.GetUpperBound(1) + 1;
            cellsmarked = new bool[CellsRows, CellsCols];
        }

        public int Get3BV()
        {
            for (int i = 0; i < CellsRows; i++)
            {
                for (int j = 0; j < CellsCols; j++)
                {
                    if (Cells[i, j].MinesCloseBy == 0)
                    {
                        if (!cellsmarked[i, j])
                        {
                            cellsmarked[i, j] = true;
                            bvcount++;
                            FloodFillMark(i, j);
                        }
                    }
                }
            }

            for (int i = 0; i < CellsRows; i++)
            {
                for (int j = 0; j < CellsCols; j++)
                {
                    if (!cellsmarked[i, j] && !Cells[i, j].IsMine)
                    {
                        cellsmarked[i, j] = true;
                        bvcount++;
                    }
                }
            }
            return bvcount;
        }

        public void FloodFillMark(int x, int y)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (!(x + i < 0 || x + i > CellsRows-1 || y + j < 0 || y + j > CellsCols-1 || (i==0 && j==0)))
                    {
                        if(!cellsmarked[x + i, y + j])
                        {
                            cellsmarked[x + i, y + j] = true;
                            if (Cells[x + i, y + j].MinesCloseBy == 0)
                                FloodFillMark(x + i, y + j);
                        }
                    }
                }
            }
        }
    }
}
