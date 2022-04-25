using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ArlottiL_SudokuApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GamePage : ContentPage
    {
        Stopwatch _cronometro = new Stopwatch();

        public GamePage(Sudoku_DTO sudokuDTO)
        {
            BindingContext = this;
            InitializeComponent();
            
            
            sudokuBoard.Initialize(sudokuDTO);
            lblDifficulty.Text = sudokuDTO.Difficulty.ToString();

            for (int i = 0; i < SudokuBoard_View.BOARD_DIMENSION; i++)
            {
                Label lblNumberButton = new Label
                {
                    Style = (Style)this.Resources["lblNumberButton-style"],
                    Text = $"{i + 1}",
                };
                TapGestureRecognizer tapped = new TapGestureRecognizer();
                tapped.Tapped += LblNumberButton_Tapped;
                lblNumberButton.GestureRecognizers.Add(tapped);
                gridContainer.Children.Add(lblNumberButton);
                Grid.SetColumn(lblNumberButton, i);
            }

            boardSizeHelper.SizeChanged += BoardSizeHelper_SizeChanged;


            this._cronometro.Start();
            Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            {
                int minuti = Convert.ToInt32(Math.Floor(this._cronometro.Elapsed.TotalMinutes));
                int secondi = this._cronometro.Elapsed.Seconds;
                lblTime.Text = $"{minuti.ToString().PadLeft(2,'0')}:{secondi.ToString().PadLeft(2, '0')}";
                return true;
            });
        }

        private void BoardSizeHelper_SizeChanged(object sender, EventArgs e)
        {
            BoxView helper = boardSizeHelper as BoxView;

            double size = Math.Min(SudokuBoard_View.MAX_SIZE, Math.Min(helper.Width, helper.Height));

            sudokuBoard.WidthRequest = size;
            sudokuBoard.HeightRequest = size;

        }

        private async void LblNumberButton_Tapped(object sender, EventArgs e)
        {
            Label lblNumberButton = sender as Label;
            await lblNumberButton.ScaleTo(1.25, 200);
            await lblNumberButton.ScaleTo(1, 100);


        }
    }
}