using System;
using System.Collections.Generic;
using System.Text;

namespace ArlottiL_SudokuAppClient
{
    public abstract class Sudoku_Action
    {
        public int Row { get; }
        public int Column { get; }
        public Sudoku_Action(int row, int column)
        {
            this.Row = row;
            this.Column = column;
        }

    }
}
