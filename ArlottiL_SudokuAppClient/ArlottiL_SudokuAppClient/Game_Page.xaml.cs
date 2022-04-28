using System;
using System.Diagnostics;
using System.Linq;
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

        public Game_Page(Sudoku_DTO baseDto)
        {
            InitializeComponent();
            BindingContext = this;

            // -> NumberButtons
            for(int column = 0; column < Sudoku_Board.BOARD_DIMENSION; column++)
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

            // -> btnNote
            btnNote.OnTapEvent = new Func<Image, Task>(async sender =>
            {
                Sudoku_Board.NoteMode = !Sudoku_Board.NoteMode;
                btnNote.IconSource = (Sudoku_Board.NoteMode) ? ImageSource.FromResource("ArlottiL_SudokuAppClient.Resources.btnNoteActive_icon.png") : ImageSource.FromResource("ArlottiL_SudokuAppClient.Resources.btnNoteInactive_icon.png");

                foreach (Label label in numberButtons) 
                    label.TextColor = (Color) ((Sudoku_Board.NoteMode) ? this.Resources["candidateNumberButton_color"] : App.Current.Resources["DefaultCellTextColor"]);
                
                await sender.ScaleTo(1.2, 200);
                await sender.ScaleTo(1, 200);
            });

            // -> btnAnnulla
            btnAnnulla.OnTapEvent = new Func<Image, Task>(async sender =>
            {
                if(!Sudoku_Board.Actions.Any())
                {
                    Utilities.ShakeAnimation(sender);
                    return;
                }

                Sudoku_Board.UndoLastAction();
                if(!Sudoku_Board.Actions.Any()) btnAnnulla.IconSource = ImageSource.FromResource("ArlottiL_SudokuAppClient.Resources.btnActionBackInactive_icon.png");

                await sender.ScaleTo(1.2, 200);
                await sender.ScaleTo(1, 200);
            });

            // -> btnCancella
            btnCancella.OnTapEvent = new Func<Image, Task>(async sender =>
            {
                if (Sudoku_Board.FocusedCell == null || Sudoku_Board.FocusedCell.Readonly) 
                {
                    Utilities.ShakeAnimation(sender);
                    return;
                }

                if(Sudoku_Board.FocusedCell.Value != 0) Sudoku_Board.ApplyAction(new Sudoku_ValueAction(Sudoku_Board.FocusedCell.Row, Sudoku_Board.FocusedCell.Column, 0));

                await sender.ScaleTo(1.2, 200);
                await sender.ScaleTo(1, 200);
            });

            Sudoku_Board.Start(boardLayout, baseDto);

            // -> Cronometro
            Cronometro.Start();
            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                int minuti = Convert.ToInt32(Math.Floor(Cronometro.Elapsed.TotalMinutes));
                int secondi = Cronometro.Elapsed.Seconds;
                lblTime.Text = $"{minuti.ToString().PadLeft(2, '0')}:{secondi.ToString().PadLeft(2, '0')}";
                return true;
            });



            Sudoku_Board.SudokuCompletedEvent += Sudoku_Board_SudokuCompletedEvent;


        }

        private void Sudoku_Board_SudokuCompletedEvent(object sender, bool e)
        {
            string message = e ? "Sudoku completato con successo!" : "Sudoku errato!";
            Color backgroundColor = e ? Color.Green : Color.Crimson;
            gamePage_alert.Summon(message, backgroundColor);
        }


        private async void NumberButton_Tapped(object sender, EventArgs e)
        {
            Label label = sender as Label;
            if (Sudoku_Board.FocusedCell == null || Sudoku_Board.FocusedCell.Readonly)
            {
                Utilities.ShakeAnimation(label);
                return;
            }

            int value = Convert.ToInt32(label.Text);

            InsertInBoard(value);


            await label.ScaleTo(1.2, 200);
            await label.ScaleTo(1, 200);
        }

        private void InsertInBoard(int value)
        {
            if (Sudoku_Board.NoteMode) Sudoku_Board.ApplyAction(new Sudoku_CandidateAction(Sudoku_Board.FocusedCell.Row, Sudoku_Board.FocusedCell.Column, value - 1, !Sudoku_Board.FocusedCell.Candidates[value - 1]));
            else
            {
                Sudoku_ValueAction action = new Sudoku_ValueAction(Sudoku_Board.FocusedCell.Row, Sudoku_Board.FocusedCell.Column, value);
                if (action.Value != Sudoku_Board.FocusedCell.Value) Sudoku_Board.ApplyAction(action);
            }

            if (Sudoku_Board.Actions.Count == 1) btnAnnulla.IconSource = ImageSource.FromResource("ArlottiL_SudokuAppClient.Resources.btnActionBackActive_icon.png");
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