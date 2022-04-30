using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ArlottiL_SudokuAppClient
{
    public class Sudoku_Cell : INotifyPropertyChanged
    {
        public int Row { get; }
        public int Column { get; }
        public int Region { get => Sudoku_Board.GetRegionIndex(Row, Column); }

        private int _value;
        public int Value {
            get => this._value;
            set
            {
                this._value = value;
                this.OnPropertyChanged(nameof(TextValue));
            }

        }
        public string TextValue => (this.Value == 0) ? "" : $"{this.Value}";

        private bool[] _candidates;
        public bool[] Candidates => (bool[])this._candidates.Clone();


        // ----- Array item binding doesn't work :( ------
        public bool tmpCandidate0 => this._candidates[0];
        public bool tmpCandidate1 => this._candidates[1];
        public bool tmpCandidate2 => this._candidates[2];
        public bool tmpCandidate3 => this._candidates[3];
        public bool tmpCandidate4 => this._candidates[4];
        public bool tmpCandidate5 => this._candidates[5];
        public bool tmpCandidate6 => this._candidates[6];
        public bool tmpCandidate7 => this._candidates[7];
        public bool tmpCandidate8 => this._candidates[8];
        public bool tmpCandidate9 => this._candidates[9];

        // -----                :/ :(                -----

        public readonly bool Readonly = false;

        
        
        
        public ShapedRectangle_View View { get; private set; }

        public event EventHandler ViewTappedEvent;



        public Sudoku_Cell(int row, int column, int value = 0)
        {
            this._candidates = new bool[Sudoku_Board.BOARD_DIMENSION];
            this.Row = row;
            this.Column = column;
            this.Value = value;
            this.Readonly = value != 0;
        }

        public void SetCandidate(int index, bool value) {
            this._candidates[index] = value;
            OnPropertyChanged($"tmpCandidate{index}");
        }

        public void SetAllCandidates(bool value)
        {
            for (int i = 0; i < Sudoku_Board.BOARD_DIMENSION; i++) this.SetCandidate(i, value);
        }

        public void BindView(ShapedRectangle_View view, Label[] candidateLabels)
        {
            this.View = view;
            this.View.OnTapEvent = new Func<ShapedRectangle_View, Task>(sender =>
            {
                this.ViewTappedEvent.Invoke(this, EventArgs.Empty);
                return Task.CompletedTask;
            });

            Binding viewBinding = new Binding() {
                Source = this,
                Path = nameof(this.TextValue),
                Mode = BindingMode.OneWay
            };

            view.SetBinding(ShapedRectangle_View.TextContentProperty, viewBinding);

            for(int i = 0; i < Sudoku_Board.BOARD_DIMENSION; i++)
            {

                Binding candidateBinding = new Binding() { 
                    Source = this,
                    Path = $"tmpCandidate{i}",
                    Mode = BindingMode.OneWay
                };
                
                candidateLabels[i].SetBinding(Label.IsVisibleProperty, candidateBinding);
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
