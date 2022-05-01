using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ArlottiL_SudokuAppClient
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Game_Page : ContentPage
    {
        Label[] numberButtons = new Label[Sudoku_Board.BOARD_DIMENSION];

        public static Stopwatch Cronometro = new Stopwatch();

        public Sudoku_Board GameBoard;

        public Sudoku_Cell CellFocused = null;

        private bool _btnAnnulla_active = false;


        public Game_Page(Sudoku_DTO baseDto)
        {
            InitializeComponent();

            GameBoard = new Sudoku_Board(baseDto);

            BindingContext = this;

            this.InitializeVisualBoard();


            // -> NumberButtons
            for (int column = 0; column < Sudoku_Board.BOARD_DIMENSION; column++)
            {
                numberButtons[column] = new Label()
                {
                    Text = $"{column + 1}",
                    Style = (Style) this.Resources["numberButton_style"]
                };
                TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += NumberButton_Tapped;
                numberButtons[column].GestureRecognizers.Add(tapGestureRecognizer);

                gamePage_container.Children.Add(numberButtons[column]);
                Grid.SetColumn(numberButtons[column], column);
            }

            //// -> btnNote
            btnNote.OnTapEvent = new Func<Image, Task>(async sender =>
            {
                this.GameBoard.NoteMode = !this.GameBoard.NoteMode;
                btnNote.IconSource = (this.GameBoard.NoteMode) ? ImageSource.FromResource("ArlottiL_SudokuAppClient.Resources.btnNoteActive_icon.png") : ImageSource.FromResource("ArlottiL_SudokuAppClient.Resources.btnNoteInactive_icon.png");

                foreach (Label label in numberButtons)
                    label.TextColor = (Color)((this.GameBoard.NoteMode) ? this.Resources["candidate_color"] : this.Resources["default_color"]);

                await sender.ScaleTo(1.2, 200);
                await sender.ScaleTo(1, 200);
            });

            //// -> btnAnnulla
            btnAnnulla.OnTapEvent = new Func<Image, Task>(async sender =>
            {
                if (!this._btnAnnulla_active)
                {
                    Utilities.ShakeAnimation(sender);
                    return;
                }

                this.GameBoard.UndoLastAction();

                await sender.ScaleTo(1.2, 200);
                await sender.ScaleTo(1, 200);
            });

            //// -> btnCancella
            btnCancella.OnTapEvent = new Func<Image, Task>(async sender =>
            {
                if (this.CellFocused == null || this.CellFocused.Readonly)
                {
                    Utilities.ShakeAnimation(sender);
                    return;
                }

                if (this.CellFocused.Value != 0) this.GameBoard.ApplyAction(new Sudoku_ValueAction(this.CellFocused.Row, this.CellFocused.Column, 0));

                await sender.ScaleTo(1.2, 200);
                await sender.ScaleTo(1, 200);
            });

            // -> btnSuggerimento
            btnSuggerimento.OnTapEvent = new Func<Image, Task>(async sender =>
            {
                sender.Source = ImageSource.FromResource("ArlottiL_SudokuAppClient.Resources.btnLampActive_icon.png");
                await sender.ScaleTo(1.2, 200);
                
                if(this.GameBoard.IsCorrect())
                {
                    HttpClient httpClient = new HttpClient();
                    string response = "";
                    bool success = true;
                    Sudoku_HelperDTO helperDTO = null;
                    try
                    {
                        response = await httpClient.GetStringAsync($"{Sudoku_Board.HELPER_SUDOKU_BASE_URL}{this.GameBoard}");
                    }
                    catch (Exception)
                    {
                        success = false;
                    }
                    if (success)
                    {
                        helperDTO = (Sudoku_HelperDTO) JsonConvert.DeserializeObject(response, typeof(Sudoku_HelperDTO));
                        this.ApplyHelp(helperDTO);
                    }
                    else
                    {
                        gamePage_alert.Summon("Errore di connessione!", Color.Crimson);
                    }
                }else
                {
                    IEnumerable<Sudoku_Action> azioniDiCorrezione = this.GameBoard.GetAzioniDiCorrezione();
                    this.GameBoard.ApplyActions(azioniDiCorrezione.ToList());
                }
                
                

                sender.Source = ImageSource.FromResource("ArlottiL_SudokuAppClient.Resources.btnLampInactive_icon.png");
                await sender.ScaleTo(1, 200);
            });



            // -> Cronometro
            Cronometro.Start();
            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                int minuti = Convert.ToInt32(Math.Floor(Cronometro.Elapsed.TotalMinutes));
                int secondi = Cronometro.Elapsed.Seconds;
                lblTime.Text = $"{minuti.ToString().PadLeft(2, '0')}:{secondi.ToString().PadLeft(2, '0')}";
                return true;
            });
        }

        private void ApplyHelp(Sudoku_HelperDTO helperDTO)
        {

            List<Sudoku_Action> candidatesHelper = this.GameBoard.CompareWithCandidatesString(helperDTO.CandidateString);
            if(candidatesHelper.Any())
            {
                this.GameBoard.ApplyActions(candidatesHelper);
            }
            else
            {
                List<Sudoku_Action> actions = new List<Sudoku_Action>();
                foreach (Sudoku_ActionDTO actionDTO in helperDTO.Actions) actions.Add(actionDTO.GetAction());
                this.GameBoard.ApplyActions(actions);
            }
        }



        private void InitializeVisualBoard()
        {

            // -> Delete all children
            boardLayout.Children.Clear();

            // -> Draw cells
            const double CELL_SIZE = 1.0 / Sudoku_Board.BOARD_DIMENSION;
            const double CANDIDATE_SIZE = 1.0 / (Sudoku_Board.REGION_DIMENSION * Sudoku_Board.BOARD_DIMENSION);
            for (int rowCell = 0; rowCell < Sudoku_Board.BOARD_DIMENSION; rowCell++)
            {
                double notPropYCell = (double)rowCell * CELL_SIZE;
                double yCell = Utilities.GetProportionalCoordinate(notPropYCell, CELL_SIZE);

                for (int columnCell = 0; columnCell < Sudoku_Board.BOARD_DIMENSION; columnCell++)
                {
                    ShapedRectangle_View cellView = new ShapedRectangle_View()
                    {
                        Style = (Style) this.Resources["defaultCell_style"]
                    };

                    double notPropXCell = (double)columnCell * CELL_SIZE;
                    double xCell = Utilities.GetProportionalCoordinate(notPropXCell, CELL_SIZE);

                    boardLayout.Children
                        .Add(cellView,
                        new Rect(xCell, yCell, CELL_SIZE, CELL_SIZE),
                        AbsoluteLayoutFlags.All);

                    Label[] candidatesLabels = new Label[Sudoku_Board.BOARD_DIMENSION];
                    for (int rowCandidate = 0; rowCandidate < Sudoku_Board.REGION_DIMENSION; rowCandidate++)
                    {
                        double notPropYCandidate = notPropYCell + ((double)rowCandidate * CANDIDATE_SIZE);
                        double yCandidate = Utilities.GetProportionalCoordinate(notPropYCandidate, CANDIDATE_SIZE);

                        for (int columnCandidate = 0; columnCandidate < Sudoku_Board.REGION_DIMENSION; columnCandidate++)
                        {
                            double notPropXCandidate = notPropXCell + ((double)columnCandidate * CANDIDATE_SIZE);
                            double xCandidate = Utilities.GetProportionalCoordinate(notPropXCandidate, CANDIDATE_SIZE);

                            int candidateIndex = rowCandidate * Sudoku_Board.REGION_DIMENSION + columnCandidate;
                            candidatesLabels[candidateIndex] = new Label()
                            {
                                Text = $"{candidateIndex + 1}",
                                Style = (Style) this.Resources["candidateLabel_style"]
                            };

                            boardLayout.Children
                                .Add(candidatesLabels[candidateIndex],
                                new Rect(xCandidate, yCandidate, CANDIDATE_SIZE, CANDIDATE_SIZE),
                                AbsoluteLayoutFlags.All);
                        }
                    }
                    this.GameBoard.Board[rowCell, columnCell].BindView(cellView, candidatesLabels);
                    if (this.GameBoard.Board[rowCell, columnCell].Readonly) cellView.FontColor = (Color)this.Resources["readonly_color"];
                    this.GameBoard.Board[rowCell, columnCell].ViewTappedEvent += Cell_TappedEvent;
                }
            }

            // -> Draw region frames
            double REGION_SIZE = 1.0 / Sudoku_Board.REGION_DIMENSION;
            for (int row = 0; row < Sudoku_Board.REGION_DIMENSION; row++)
            {
                for (int column = 0; column < Sudoku_Board.REGION_DIMENSION; column++)
                {
                    Xamarin.Forms.Shapes.Rectangle regionFrame = new Xamarin.Forms.Shapes.Rectangle()
                    {
                        Style = (Style) this.Resources["regionFrame_style"]
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
                Style = (Style) this.Resources["boardFrame_style"]
            };

            boardLayout.Children
                .Add(boardFrame,
                new Rect(0, 0, 1, 1),
                AbsoluteLayoutFlags.All);


            this.GameBoard.OnActionEvent += OnAction_Event;

        }

        private void UpdateVisualBoard()
        {
            if (this.CellFocused == null) return;

            for (int row = 0; row < Sudoku_Board.BOARD_DIMENSION; row++)
            {
                for (int column = 0; column < Sudoku_Board.BOARD_DIMENSION; column++)
                {
                    if (row == this.CellFocused.Row || column == this.CellFocused.Column || Sudoku_Board.GetRegionIndex(row, column) == this.CellFocused.Region)
                    {
                        this.GameBoard.Board[row, column].View.Style = (Style)this.Resources["highlightedCell_style"];
                        continue;
                    }

                    if (CellFocused.Value != 0 && this.GameBoard.Board[row, column].Value == CellFocused.Value)
                    {
                        this.GameBoard.Board[row, column].View.Style = (Style)this.Resources["darkHighlightedCell_style"];
                        continue;
                    }

                    this.GameBoard.Board[row, column].View.Style = (Style)this.Resources["defaultCell_style"];
                }
            }
            CellFocused.View.Style = (Style)this.Resources["focusedCell_style"];
        }

        private void OnAction_Event(object sender, Sudoku_Action e)
        {
            if (this.GameBoard.Actions.Any() && !this._btnAnnulla_active)
            {
                btnAnnulla.IconSource = ImageSource.FromResource("ArlottiL_SudokuAppClient.Resources.btnActionBackActive_icon.png");
                this._btnAnnulla_active = true;
            }
            else if (!this.GameBoard.Actions.Any() && this._btnAnnulla_active)
            {
                btnAnnulla.IconSource = ImageSource.FromResource("ArlottiL_SudokuAppClient.Resources.btnActionBackInactive_icon.png");
                this._btnAnnulla_active =false;
            }
            this.UpdateVisualBoard();
        }

        private void Cell_TappedEvent(object sender, EventArgs e)
        {
            this.CellFocused = sender as Sudoku_Cell;
            this.UpdateVisualBoard();
        }

        private async void NumberButton_Tapped(object sender, EventArgs e)
        {
            Label label = sender as Label;
            if (this.CellFocused == null || this.CellFocused.Readonly)
            {
                Utilities.ShakeAnimation(label);
                return;
            }

            int value = Convert.ToInt32(label.Text);
            this.DoAction(value);


            await label.ScaleTo(1.2, 200);
            await label.ScaleTo(1, 200);
        }

        private void DoAction(int value)
        {
            if (this.GameBoard.NoteMode)
            {
                this.GameBoard.ApplyAction(new Sudoku_CandidateAction(CellFocused.Row, CellFocused.Column, value - 1, !CellFocused.Candidates[value - 1]));
            }
            else
            {
                Sudoku_ValueAction action = new Sudoku_ValueAction(CellFocused.Row, CellFocused.Column, value);
                if (action.Value != CellFocused.Value) this.GameBoard.ApplyAction(action);
            }
        }


        private void BoardLayout_sizeHelper_SizeChanged(object sender, EventArgs e)
        {
            BoxView sizeHelper = sender as BoxView;

            double boxMenu_maxSize = (double)App.Current.Resources["BoardMaxSize"];
            double size = Math.Min(boxMenu_maxSize, Math.Min(sizeHelper.Width, sizeHelper.Height));

            int intSize = Convert.ToInt32(Math.Round(size));
            int mySize = intSize - (intSize % Sudoku_Board.BOARD_DIMENSION);

            boardLayout_container.WidthRequest = mySize;
            boardLayout_container.HeightRequest = mySize;
        }
    }

}