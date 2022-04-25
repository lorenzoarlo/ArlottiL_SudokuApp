﻿using System.Collections.Generic;

namespace ArlottiL_SudokuApp
{
    public class Sudoku_ValueAction : Sudoku_Action
    {
        public int Value { get; }

        public Stack<Sudoku_CandidateAction> CandidatesModifed { get; set; } = new Stack<Sudoku_CandidateAction>();

        public Sudoku_ValueAction(int row, int column, int value) : base(row, column)
        {
            this.Value = value;
        }

    }
}