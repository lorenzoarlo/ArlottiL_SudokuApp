using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Shapes;
using Xamarin.Forms.Xaml;

namespace ArlottiL_SudokuApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SudokuBoard_View : ContentView
    {
        public const int MAX_SIZE = 900;
        
        public const int BOARD_DIMENSION = 9;
        
        public const int REGION_DIMENSION = 3;

        public static readonly Sudoku_Cell[,] Board = new Sudoku_Cell[BOARD_DIMENSION, BOARD_DIMENSION];
        public static string Mission { get; private set; }
        public static string Solution { get; private set; }

        public static Sudoku_Cell CellFocused { get; set; } = null;
        
        public static Style Cell_Style;

        public static Style FocusedCell_Style;
        
        public static Style RegionFrame_Style;

        public static Style HighlightedCell_Style;

        public static Style DarkHighlightedCell_Style;

        public SudokuBoard_View()
        {
            this.InitializeComponent();

            Cell_Style = (Style)this.Resources["cell-style"];
            FocusedCell_Style = (Style)this.Resources["focusedCell-style"];
            RegionFrame_Style = (Style)this.Resources["regionFrame-style"];
            HighlightedCell_Style = (Style)this.Resources["highlightedCell-style"];
            DarkHighlightedCell_Style = (Style)this.Resources["darkHighlightedCell-style"];
        }
        public static int GetRegionIndex(int row, int column) => (row / REGION_DIMENSION) * REGION_DIMENSION + (column / REGION_DIMENSION);

        public void Initialize(Sudoku_DTO originalDTO)
        {
            Mission = originalDTO.Mission;
            Solution = originalDTO.Solution;

            // ----- SUDOKU CELLS -----
            const double CELL_SIZE = 1.0 / BOARD_DIMENSION;
            const double CANDIDATE_SIZE = 1.0 / (REGION_DIMENSION * BOARD_DIMENSION);
            for (int rowCell = 0; rowCell < BOARD_DIMENSION; rowCell++)
            {
                double notPropYCell = (double) rowCell * CELL_SIZE;
                double yCell = Utilities.GetProportionalCoordinate(notPropYCell, CELL_SIZE);

                for (int columnCell = 0; columnCell < BOARD_DIMENSION; columnCell++)
                {
                    Board[rowCell, columnCell] = new Sudoku_Cell(rowCell, columnCell, int.Parse(Mission[rowCell * BOARD_DIMENSION + columnCell].ToString()));
                    Board[rowCell, columnCell].View.Style = Cell_Style;

                    Board[rowCell, columnCell].ViewClickedEvent += Cell_Tapped;

                    double notPropXCell = (double)columnCell * CELL_SIZE;
                    double xCell = Utilities.GetProportionalCoordinate(notPropXCell, CELL_SIZE);

                    boardContainer.Children
                        .Add(Board[rowCell, columnCell].View,
                        new Rect(xCell, yCell, CELL_SIZE, CELL_SIZE),
                        AbsoluteLayoutFlags.All);

                    boardContainer.LowerChild(Board[rowCell, columnCell].View);

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
                                Style = (Style)this.Resources["lblCandidate-style"]
                            };

                            boardContainer.Children
                                .Add(candidatesLabels[candidateIndex],
                                new Rect(xCandidate, yCandidate, CANDIDATE_SIZE, CANDIDATE_SIZE),
                                AbsoluteLayoutFlags.All);
                        }
                    }
                    Board[rowCell, columnCell].CandidateLabels = candidatesLabels;
                }
            }

            // ----- REGION RECTANGLES -----

            double REGION_SIZE = 1.0 / REGION_DIMENSION;
            for (int row = 0; row < REGION_DIMENSION; row++)
            {
                for (int column = 0; column < REGION_DIMENSION; column++)
                {
                    Xamarin.Forms.Shapes.Rectangle regionFrame = new Xamarin.Forms.Shapes.Rectangle()
                    {
                        Style = RegionFrame_Style
                    };

                    double x = Utilities.GetProportionalCoordinate((double)column * REGION_SIZE, REGION_SIZE);
                    double y = Utilities.GetProportionalCoordinate((double)row * REGION_SIZE, REGION_SIZE);

                    boardContainer.Children
                        .Add(regionFrame,
                        new Rect(x, y, REGION_SIZE, REGION_SIZE),
                        AbsoluteLayoutFlags.All);
                }
            }
            
        }

        public Sudoku_Cell[] GetRow(int rowIndex)
        {
            Sudoku_Cell[] row = new Sudoku_Cell[BOARD_DIMENSION];
            for (int column = 0; column < BOARD_DIMENSION; column++)
            {
                row[column] = Board[rowIndex, column];
            }
            return row;
        }

        public Sudoku_Cell[] GetColumn(int columnIndex)
        {
            Sudoku_Cell[] column = new Sudoku_Cell[BOARD_DIMENSION];
            for (int row = 0; row < BOARD_DIMENSION; row++)
            {
                column[row] = Board[row, columnIndex];
            }
            return column;
        }

        public Sudoku_Cell[] GetRegion(int regionIndex)
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


        private void Cell_Tapped(object sender, EventArgs e)
        {
            Sudoku_Cell senderCell = (sender as Sudoku_Cell);

            if(CellFocused != null) CellFocused.View.Style = Cell_Style;

            CellFocused = senderCell;
            HighlightNeighbours(senderCell.Row, senderCell.Column, senderCell.Value);
            CellFocused.View.Style = FocusedCell_Style;

        }

        private void HighlightNeighbours(int cellRow, int cellColumn, int value)
        {
            for(int row = 0; row < BOARD_DIMENSION; row++)
            {
                for(int column = 0; column < BOARD_DIMENSION; column++)
                {
                    if(row == cellRow || column == cellColumn || GetRegionIndex(row, column) == GetRegionIndex(cellRow, cellColumn))
                    {
                        Board[row, column].View.Style = HighlightedCell_Style;
                        continue;
                    }
                    
                    if(Board[row, column].Value == value && value != 0)
                    {
                        Board[row, column].View.Style = DarkHighlightedCell_Style;
                        continue;
                    }

                    Board[row, column].View.Style = Cell_Style;

                }
            }
        }

        public override string ToString()
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