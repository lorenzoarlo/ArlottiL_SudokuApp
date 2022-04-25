using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ArlottiL_SudokuApp
{
    public class Sudoku_Cell
    {
        public ShapedRectangle_View View { get; set; } = new ShapedRectangle_View();

        public event EventHandler ViewClickedEvent;

        public bool IsCellReadonly = false;

        private int _value = 0;
        public int Value { 
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
                this.View.TextContent = (this._value == 0) ? " " : _value.ToString();
            }

        }

        public int Row { get; }

        public int Column { get; }

        public int Region { get { return SudokuBoard_View.GetRegionIndex(this.Row, this.Column); } }

        public Label[] CandidateLabels { get; set; }


        public Sudoku_Cell(int row, int column, int value)
        {
            this.Row = row;
            this.Column = column;
            this.Value = value;
            View.ClickedAction = new Func<ShapedRectangle_View, Task>(sender =>
            {
                this.ViewClickedEvent?.Invoke(this, EventArgs.Empty);
                return Task.CompletedTask;
            });
            this.IsCellReadonly = (value != 0);
        }



    }
}
