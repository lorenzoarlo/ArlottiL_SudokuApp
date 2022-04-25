using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ArlottiL_SudokuAppClient
{
    public class Sudoku_Cell
    {

        public int Row { get; }
        public int Column { get; }
        public int Region { get { return Sudoku_Board.GetRegionIndex(Row, Column); } }

        public event EventHandler CellViewTappedEvent;

        private int _value;

        public int Value { 
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
                this.CellView.TextContent = (this._value == 0) ? " " : $"{_value}";
            }
        }

        public ShapedRectangle_View CellView { get; }

        public bool[] Candidates { get; private set; }

        public Label[] CandidatesLabels; 

        public readonly bool Readonly = false;

        public Sudoku_Cell(ShapedRectangle_View view, Label[] candidatesLabels, int row, int column, int value = 0)
        {
            this.CellView = view;
            view.OnTapEvent = new Func<ShapedRectangle_View, Task>(sender =>
            {
                this.CellViewTappedEvent.Invoke(this, EventArgs.Empty);
                return Task.CompletedTask;
            });
            this.CandidatesLabels = candidatesLabels;
            this.Candidates = new bool[Sudoku_Board.BOARD_DIMENSION];
            this.Row = row;
            this.Column = column;
            this.Value = value;
            this.Readonly = value != 0;
            this.CellView.FontColor = (Color)((this.Readonly) ? App.Current.Resources["ReadonlyCellTextColor"] : App.Current.Resources["DefaultCellTextColor"]);
        }

        public void SetAllCandidates(bool value)
        {
            for(int i = 0; i < Sudoku_Board.BOARD_DIMENSION; i++) this.SetCandidate(i,value);   
        }

        public void SetCandidate(int index, bool value)
        {
            this.Candidates[index] = value;
            this.CandidatesLabels[index].IsVisible = value;


        }


    }
}
