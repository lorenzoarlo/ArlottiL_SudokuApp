using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ArlottiL_SudokuAppClient
{
    public abstract class Sudoku_Board
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

        const int REGION_DIMENSION = 3;

        public static Sudoku_Cell[,] Board = new Sudoku_Cell[BOARD_DIMENSION, BOARD_DIMENSION];

        public static string Mission;

        public static string Solution;

        public static Sudoku_Cell FocusedCell = null;

        public static bool NoteMode = false;

        public static Stack<Sudoku_Action> Actions = new Stack<Sudoku_Action>();

        public static event EventHandler<bool> SudokuCompletedEvent;

        public static bool IsCorrect
        {
            get
            {
                string me = ToString();
                for(int i = 0; i < BOARD_DIMENSION * BOARD_DIMENSION; i++)
                {
                    if (me[i] != Solution[i] && me[i] != '0') return false;
                }
                return true;
            }
        }

        public static bool IsComplete
        {
            get
            {
                for(int row = 0; row < BOARD_DIMENSION; row++)
                {
                    for(int column = 0; column < BOARD_DIMENSION; column++)
                    {
                        if (Board[row, column].Value == 0) return false; 
                    }
                }
                return true;
            }
        }

        public static void Start(AbsoluteLayout boardLayout, Sudoku_DTO sudokuDTO)
        {
            Mission = sudokuDTO.Mission;
            Solution = sudokuDTO.Solution;

            // -> Delete all children
            boardLayout.Children.Clear();

            // -> Draw cells
            const double CELL_SIZE = 1.0 / BOARD_DIMENSION;
            const double CANDIDATE_SIZE = 1.0 / (REGION_DIMENSION * BOARD_DIMENSION);
            for (int rowCell = 0; rowCell < BOARD_DIMENSION; rowCell++)
            {
                double notPropYCell = (double)rowCell * CELL_SIZE;
                double yCell = Utilities.GetProportionalCoordinate(notPropYCell, CELL_SIZE);

                for (int columnCell = 0; columnCell < BOARD_DIMENSION; columnCell++)
                {
                    ShapedRectangle_View cellView = new ShapedRectangle_View()
                    {
                        Style = (Style)App.Current.Resources["defaultCell_style"]
                    };

                    double notPropXCell = (double)columnCell * CELL_SIZE;
                    double xCell = Utilities.GetProportionalCoordinate(notPropXCell, CELL_SIZE);

                    boardLayout.Children
                        .Add(cellView,
                        new Rect(xCell, yCell, CELL_SIZE, CELL_SIZE),
                        AbsoluteLayoutFlags.All);

                    Label[] candidatesLabels = new Label[BOARD_DIMENSION];
                    for (int rowCandidate = 0; rowCandidate < REGION_DIMENSION; rowCandidate++)
                    {
                        double notPropYCandidate = notPropYCell + ((double)rowCandidate * CANDIDATE_SIZE);
                        double yCandidate = Utilities.GetProportionalCoordinate(notPropYCandidate, CANDIDATE_SIZE);

                        for (int columnCandidate = 0; columnCandidate < REGION_DIMENSION; columnCandidate++)
                        {
                            double notPropXCandidate = notPropXCell + ((double)columnCandidate * CANDIDATE_SIZE);
                            double xCandidate = Utilities.GetProportionalCoordinate(notPropXCandidate, CANDIDATE_SIZE);

                            int candidateIndex = rowCandidate * REGION_DIMENSION + columnCandidate;
                            candidatesLabels[candidateIndex] = new Label()
                            {
                                Text = $"{candidateIndex + 1}",
                                Style = (Style)App.Current.Resources["candidateLabel_style"]
                            };

                            boardLayout.Children
                                .Add(candidatesLabels[candidateIndex],
                                new Rect(xCandidate, yCandidate, CANDIDATE_SIZE, CANDIDATE_SIZE),
                                AbsoluteLayoutFlags.All);
                        }
                    }
                    Board[rowCell, columnCell] = new Sudoku_Cell(cellView, candidatesLabels, rowCell, columnCell, int.Parse($"{Mission[rowCell * BOARD_DIMENSION + columnCell]}"));
                    Board[rowCell, columnCell].CellViewTappedEvent += OnCell_Tapped; ;
                }
            }

            // -> Draw region frames
            double REGION_SIZE = 1.0 / REGION_DIMENSION;
            for (int row = 0; row < REGION_DIMENSION; row++)
            {
                for (int column = 0; column < REGION_DIMENSION; column++)
                {
                    Xamarin.Forms.Shapes.Rectangle regionFrame = new Xamarin.Forms.Shapes.Rectangle()
                    {
                        Style = (Style) App.Current.Resources["regionFrame_style"]
                    };

                    double x = Utilities.GetProportionalCoordinate((double)column * REGION_SIZE, REGION_SIZE);
                    double y = Utilities.GetProportionalCoordinate((double)row * REGION_SIZE, REGION_SIZE);

                    boardLayout.Children
                        .Add(regionFrame,
                        new Rect(x, y, REGION_SIZE, REGION_SIZE),
                        AbsoluteLayoutFlags.All);
                }
            }


            // -> Draw board frame

            Xamarin.Forms.Shapes.Rectangle boardFrame = new Xamarin.Forms.Shapes.Rectangle()
            {
                Style = (Style)App.Current.Resources["boardFrame_style"]
            };

            boardLayout.Children
                .Add(boardFrame,
                new Rect(0, 0, 1, 1),
                AbsoluteLayoutFlags.All);
        }

        private static void OnCell_Tapped(object sender, EventArgs e)
        {
            FocusedCell = sender as Sudoku_Cell;   
            HighlightBoard();
        }
        public static int GetRegionIndex(int row, int column) => (row / REGION_DIMENSION) * REGION_DIMENSION + (column / REGION_DIMENSION);
        
        public static Sudoku_Cell[] GetRow(int rowIndex)
        {
            Sudoku_Cell[] row = new Sudoku_Cell[BOARD_DIMENSION];
            for (int column = 0; column < BOARD_DIMENSION; column++)
            {
                row[column] = Board[rowIndex, column];
            }
            return row;
        }

        public static Sudoku_Cell[] GetColumn(int columnIndex)
        {
            Sudoku_Cell[] column = new Sudoku_Cell[BOARD_DIMENSION];
            for (int row = 0; row < BOARD_DIMENSION; row++)
            {
                column[row] = Board[row, columnIndex];
            }
            return column;
        }

        public static Sudoku_Cell[] GetRegion(int regionIndex)
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

        public static List<Sudoku_Cell> GetNeighbours(int row, int column)
        {
            List<Sudoku_Cell> neighbours = new List<Sudoku_Cell>();

            Sudoku_Cell[] myRow = GetRow(row);

            Sudoku_Cell[] myColumn = GetColumn(column);

            Sudoku_Cell[] myRegion = GetRegion(GetRegionIndex(row, column));


            for (int i = 0; i < BOARD_DIMENSION; i++)
            {
                if (i != column) neighbours.Add(myRow[i]);

                if (i != row) neighbours.Add(myColumn[i]);

                if (myRegion[i].Row != row && myRegion[i].Column != column) neighbours.Add(myRegion[i]);
            }

            return neighbours;
        }
        public static void ApplyActions(List<Sudoku_Action> actions)
        {
            foreach (Sudoku_Action action in actions)
            {
                if (action is Sudoku_ValueAction) ApplyAction(action as Sudoku_ValueAction);
                else ApplyAction(action as Sudoku_CandidateAction);
            }
        }

        public static void ApplyAction(Sudoku_ValueAction action)
        {
            for(int i = 0; i < BOARD_DIMENSION; i++)
            {
                if (Board[action.Row, action.Column].Candidates[i]) 
                {
                    action.CandidatesModifed.Push(new Sudoku_CandidateAction(action.Row, action.Column, action.Value - 1, false));
                }
            }
            action.PreviousValue = Board[action.Row, action.Column].Value;
            Board[action.Row, action.Column].Value = action.Value;

            if(action.Value != 0)
            {
                foreach (Sudoku_Cell cell in GetNeighbours(action.Row, action.Column))
                {
                    if (cell.Value == 0 && cell.Candidates[action.Value - 1])
                    {
                        cell.SetCandidate(action.Value - 1, false);
                        action.CandidatesModifed.Push(new Sudoku_CandidateAction(cell.Row, cell.Column, action.Value - 1, false));
                    }
                }
            }

            Actions.Push(action);
            HighlightBoard();
        }
        public static void ApplyAction(Sudoku_CandidateAction action)
        {
            if (Board[action.Row, action.Column].Value != 0) return;

            Board[action.Row, action.Column].SetCandidate(action.CandidateIndex, action.Value);

            Actions.Push(action);

            if(IsComplete)
            {
                SudokuCompletedEvent.Invoke(null, IsCorrect);
            }

        }

        public static void DeApplyActions(List<Sudoku_Action> actions)
        {
            foreach (Sudoku_Action action in actions)
            {
                if (action is Sudoku_ValueAction) DeApplyAction(action as Sudoku_ValueAction);
                else DeApplyAction(action as Sudoku_CandidateAction);
            }
        }

        public static void DeApplyAction(Sudoku_ValueAction action)
        {
            while (action.CandidatesModifed.Any()) DeApplyAction(action.CandidatesModifed.Pop());

            Board[action.Row, action.Column].Value = action.PreviousValue;

            HighlightBoard();


        }

        public static void DeApplyAction(Sudoku_CandidateAction action)
        {
            Board[action.Row, action.Column].SetCandidate(action.CandidateIndex, !action.Value);
        }

        public static void UndoLastAction()
        {
            if (!Actions.Any()) return;
            Sudoku_Action lastAction = Actions.Pop();
            if (lastAction is Sudoku_ValueAction) DeApplyAction(lastAction as Sudoku_ValueAction);
            else DeApplyAction(lastAction as Sudoku_CandidateAction);
        }

        private static void HighlightBoard()
        {
            if (FocusedCell == null) return;

            for (int row = 0; row < BOARD_DIMENSION; row++)
            {
                for (int column = 0; column < BOARD_DIMENSION; column++)
                {
                    if (row == FocusedCell.Row || column == FocusedCell.Column || GetRegionIndex(row, column) == FocusedCell.Region)
                    {
                        Board[row, column].CellView.Style = (Style)App.Current.Resources["highlightedCell_style"];
                        continue;
                    }

                    if (FocusedCell.Value != 0 && Board[row, column].Value == FocusedCell.Value)
                    {
                        Board[row, column].CellView.Style = (Style)App.Current.Resources["darkHighlightedCell_style"];
                        continue;
                    }

                    Board[row, column].CellView.Style = (Style)App.Current.Resources["defaultCell_style"];
                }
            }
            FocusedCell.CellView.Style = (Style)App.Current.Resources["focusedCell_style"];
        }
        public new static string ToString()
        {
            string toReturn = "";
            for (int row = 0; row < BOARD_DIMENSION; row++)
            {
                for (int column = 0; column < BOARD_DIMENSION; column++)
                {
                    toReturn += Board[row, column].Value;
                }
            }
            return toReturn;
        }
    }

}
