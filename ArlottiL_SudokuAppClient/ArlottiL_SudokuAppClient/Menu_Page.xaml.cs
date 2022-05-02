using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace ArlottiL_SudokuAppClient
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Menu_Page : ContentPage
    {
        public struct DifficultyImagePair
        {
            public ImageSource ImageSource { get; set; }
            public Sudoku_Board.Difficulty Difficulty { get; set; }
        }

        public List<DifficultyImagePair> Difficulties { get; set; } = new List<DifficultyImagePair>()
        {
            new DifficultyImagePair() { ImageSource = ImageSource.FromResource("ArlottiL_SudokuAppClient.Resources.casual.gif"), Difficulty = Sudoku_Board.Difficulty.Casual },
            new DifficultyImagePair() { ImageSource = ImageSource.FromResource("ArlottiL_SudokuAppClient.Resources.easy.gif"), Difficulty = Sudoku_Board.Difficulty.Easy },
            new DifficultyImagePair() { ImageSource = ImageSource.FromResource("ArlottiL_SudokuAppClient.Resources.medium.gif"), Difficulty = Sudoku_Board.Difficulty.Medium },
            new DifficultyImagePair() { ImageSource = ImageSource.FromResource("ArlottiL_SudokuAppClient.Resources.hard.gif"), Difficulty = Sudoku_Board.Difficulty.Hard },
            new DifficultyImagePair() { ImageSource = ImageSource.FromResource("ArlottiL_SudokuAppClient.Resources.expert.gif"), Difficulty = Sudoku_Board.Difficulty.Expert },
            new DifficultyImagePair() { ImageSource = ImageSource.FromResource("ArlottiL_SudokuAppClient.Resources.evil.gif"), Difficulty = Sudoku_Board.Difficulty.Evil }
        };


        public Menu_Page() 
        {
            InitializeComponent();
            BindingContext = this;

            btnPlay_image.ActivatePulse();
            btnPlay_image.OnTapEvent = new Func<Image, Task> (async (sender) =>
            {
                double maxSize = Math.Max(Application.Current.MainPage.Width, Application.Current.MainPage.Height);
                double scaleFactor = (maxSize / sender.Width) * 5;

                await sender.ScaleTo(scaleFactor, 800);

                await StartANewGame();
                sender.Scale = 1;
            });
            RelativeLayout.SetBoundsConstraint(btnPlay_image, RelativeLayout.GetBoundsConstraint(btnPlay_cell));

            btnSettings_image.ActivatePulse();
            btnSettings_image.OnTapEvent = new Func<Image, Task>(async sender =>
            {
                await sender.RotateTo(180, 500);
                await sender.ScaleTo(1.2, 200);
                sender.Rotation = 0;
                sender.Scale = 1;

                await Navigation.PushAsync(new Settings_Page());

            });
            RelativeLayout.SetBoundsConstraint(btnSettings_image, RelativeLayout.GetBoundsConstraint(btnSettings_cell));

            RelativeLayout.SetBoundsConstraint(frame_selectDifficulty_image, RelativeLayout.GetBoundsConstraint(selectDifficulty_cell));

        }

        private void BoxMenu_sizeHelper_SizeChanged(object sender, EventArgs e)
        {
            BoxView sizeHelper = sender as BoxView;

            double boxMenu_maxSize = (double) App.Current.Resources["MenuMaxSize"];
            //double boxMenu_maxSize = 800.0;
            double size = Math.Min(boxMenu_maxSize, Math.Min(sizeHelper.Width, sizeHelper.Height));

            boxMenu_container.WidthRequest = size;
            boxMenu_container.HeightRequest = size;
        }

        private async Task StartANewGame()
        {
            CarouselView carousel = selectDifficulty as CarouselView;
            Sudoku_Board.Difficulty difficultySelected = ((DifficultyImagePair)carousel.CurrentItem).Difficulty;

            HttpClient httpClient = new HttpClient();
            string response = "";
            bool success = true;
            Sudoku_DTO requestedSudoku = null;
            try
            {
                response = await httpClient.GetStringAsync($"{Sudoku_Board.REQUEST_SUDOKU_BASE_URL}{difficultySelected}");
            }
            catch (Exception)
            {
                success = false;
            }

            if (success)
            {
                requestedSudoku = (Sudoku_DTO)JsonConvert.DeserializeObject(response, typeof(Sudoku_DTO));
                await Navigation.PushAsync(new Game_Page(requestedSudoku));
            }
            else
            {
                menuPage_alert.Summon("Errore di connessione!", Color.Crimson);
            }
        }





    }
}