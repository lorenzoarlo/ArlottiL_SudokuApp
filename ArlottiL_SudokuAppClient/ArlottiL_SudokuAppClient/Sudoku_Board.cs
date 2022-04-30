using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ArlottiL_SudokuAppClient
{
    public class Sudoku_Board
    {
        public enum Difficulty
        {
            Casual = -1,
            Easy = 0,
            Medium = 1,
            Hard = 2,
            Expert = 3,
            Evil = 4,
        }

        public const string REQUEST_SUDOKU_BASE_URL = "http://localhost:5156/api/Sudoku/";

        public const int BOARD_DIMENSION = 9;

        public const int REGION_DIMENSION = 3;

        public Sudoku_Cell[,] Board;

        public string Mission;

        public string Solution;

        public bool NoteMode;

        public Stack<Sudoku_Action> Actions;

        public event EventHandler<Sudoku_Action> OnActionEvent;

        public Sudoku_Board(Sudoku_DTO dto)
        {
            this.Board = new Sudoku_Cell[BOARD_DIMENSION, BOARD_DIMENSION];
            this.Mission = dto.Mission;
            this.Solution = dto.Solution;
            
            for(int row = 0; row < BOARD_DIMENSION; row++)
            {
                for(int column = 0; column < BOARD_DIMENSION; column++)
                {
                    int value = Convert.ToInt32(this.Mission[row * BOARD_DIMENSION + column].ToString());
                    this.Board[row, column] = new Sudoku_Cell(row, column, value);
                }
            }

            this.NoteMode = false;
            this.Actions = new Stack<Sudoku_Action>();
        }

        public static int GetRegionIndex(int row, int column) => (row / REGION_DIMENSION) * REGION_DIMENSION + (column / REGION_DIMENSION);
        
        public IEnumerable<Sudoku_Cell> GetRow(int rowIndex)
        {
            Sudoku_Cell[] row = new Sudoku_Cell[BOARD_DIMENSION];
            for (int column = 0; column < BOARD_DIMENSION; column++)
            {
                row[column] = this.Board[rowIndex, column];
            }
            return row;
        }

        public IEnumerable<Sudoku_Cell> GetColumn(int columnIndex)
        {
            

            Sudoku_Cell[] column = new Sudoku_Cell[BOARD_DIMENSION];
            for (int row = 0; row < BOARD_DIMENSION; row++)
            {
                column[row] = this.Board[row, columnIndex];
            }
            return column;
        }

        public IEnumerable<Sudoku_Cell> GetRegion(int regionIndex)
        {
            

            Sudoku_Cell[] region = new Sudoku_Cell[BOARD_DIMENSION];
            int initialRow = (regionIndex / REGION_DIMENSION) * REGION_DIMENSION;
            int initialColumn = (regionIndex % REGION_DIMENSION) * REGION_DIMENSION;

            for (int i = 0; i < REGION_DIMENSION; i++)
            {
                for (int j = 0; j < REGION_DIMENSION; j++)
                {
                    region[i * REGION_DIMENSION + j] = Board[initialRow + i, initialColumn + j];
                }
            }
            return region;
        }

        public IEnumerable<Sudoku_Cell> GetCellNeighbours(int row, int column)
        {
            

            List<Sudoku_Cell> neighbours = new List<Sudoku_Cell>();

            IEnumerable<Sudoku_Cell> myRow = this.GetRow(row);

            IEnumerable<Sudoku_Cell> myColumn = this.GetColumn(column);

            IEnumerable<Sudoku_Cell> myRegion = this.GetRegion(GetRegionIndex(row, column));


            for (int i = 0; i < BOARD_DIMENSION; i++)
            {
                if (i != column) neighbours.Add(myRow.ElementAt(i));

                if (i != row) neighbours.Add(myColumn.ElementAt(i));

                if (myRegion.ElementAt(i).Row != row && myRegion.ElementAt(i).Column != column) neighbours.Add(myRegion.ElementAt(i));
            }

            return neighbours;
        }
        
        public void ApplyActions(List<Sudoku_Action> actions)
        {
            foreach (Sudoku_Action action in actions)
            {
                if (action is Sudoku_ValueAction) this.ApplyAction(action as Sudoku_ValueAction);
                else this.ApplyAction(action as Sudoku_CandidateAction);
            }
        }

        public void ApplyAction(Sudoku_ValueAction action)
        {
            for (int i = 0; i < BOARD_DIMENSION; i++)
            {
                if (Board[action.Row, action.Column].Candidates[i]) 
                {
                    action.CandidatesModifed.Push(new Sudoku_CandidateAction(action.Row, action.Column, action.Value - 1, false));
                }
            }
            action.PreviousValue = this.Board[action.Row, action.Column].Value;
            this.Board[action.Row, action.Column].Value = action.Value;

            if(action.Value != 0)
            {
                foreach (Sudoku_Cell cell in this.GetCellNeighbours(action.Row, action.Column))
                {
                    if (cell.Value == 0 && cell.Candidates[action.Value - 1])
                    {
                        cell.SetCandidate(action.Value - 1, false);
                        action.CandidatesModifed.Push(new Sudoku_CandidateAction(cell.Row, cell.Column, action.Value - 1, false));
                    }
                }
            }
            Actions.Push(action);

            this.OnActionEvent.Invoke(this, action);
        }
        public void ApplyAction(Sudoku_CandidateAction action)
        {
            if (this.Board[action.Row, action.Column].Value != 0) return;

            this.Board[action.Row, action.Column].SetCandidate(action.CandidateIndex, action.Value);

            this.Actions.Push(action);
        }

        public void DeApplyActions(List<Sudoku_Action> actions)
        {
            foreach (Sudoku_Action action in actions)
            {
                if (action is Sudoku_ValueAction) this.DeApplyAction(action as Sudoku_ValueAction);
                else this.DeApplyAction(action as Sudoku_CandidateAction);
            }
        }

        public void DeApplyAction(Sudoku_ValueAction action)
        {
            while (action.CandidatesModifed.Any()) this.DeApplyAction(action.CandidatesModifed.Pop());

            this.Board[action.Row, action.Column].Value = action.PreviousValue;

            this.OnActionEvent.Invoke(this, action);
        }

        public void DeApplyAction(Sudoku_CandidateAction action)
        {
            this.Board[action.Row, action.Column].SetCandidate(action.CandidateIndex, !action.Value);
        }

        public void UndoLastAction()
        {
            if (!this.Actions.Any()) return;
            Sudoku_Action lastAction = this.Actions.Pop();
            if (lastAction is Sudoku_ValueAction) this.DeApplyAction(lastAction as Sudoku_ValueAction);
            else this.DeApplyAction(lastAction as Sudoku_CandidateAction);
        }

        public bool IsComplete()
        {
            for (int row = 0; row < BOARD_DIMENSION; row++)
            {
                for (int column = 0; column < BOARD_DIMENSION; column++)
                {
                    if (this.Board[row, column].Value == 0) return false;
                }
            }
            return true;
        }

        public bool IsCorrect()
        {
            string me = this.ToString();
            for (int i = 0; i < BOARD_DIMENSION * BOARD_DIMENSION; i++)
            {
                if (me[i] != Solution[i] && me[i] != '0') return false;
            }
            return true;
        }

        public string CandidatesString()
        {
            string toReturn = "";
            for (int row = 0; row < BOARD_DIMENSION; row++)
            {
                for (int column = 0; column < BOARD_DIMENSION; column++)
                {
                    toReturn += this.Board[row, column].CandidatesString;
                }
            }
            return toReturn;
        }


        public override string ToString()
        {
            string toReturn = "";
            for (int row = 0; row < BOARD_DIMENSION; row++)
            {
                for (int column = 0; column < BOARD_DIMENSION; column++)
                {
                    toReturn += this.Board[row, column].Value;
                }
            }
            return toReturn;
        }




    }

}
