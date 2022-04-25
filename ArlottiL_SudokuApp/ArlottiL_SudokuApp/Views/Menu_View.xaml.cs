using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;

namespace ArlottiL_SudokuApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Menu_View : ContentView
    {
        private const string REQUEST_SUDOKU_BASE_URL = "http://localhost:5156/api/Sudoku/";

        public event EventHandler<Sudoku_DTO> RequestResponseEvent;

        const double DEFAULT_ICON_SCALE = 0.5;

        public struct DifficultiesAndImages
        {
            public ImageSource ImageSource { get; set; }
            public Sudoku_DTO.Sudoku_Difficulty Difficulty { get; set; }
        }


        public const int MAX_SIZE = 600;


        public List<DifficultiesAndImages> SudokuDifficulties { get; set; } = new List<DifficultiesAndImages>()
        {
            new DifficultiesAndImages() { ImageSource = ImageSource.FromResource("ArlottiL_SudokuApp.Resources.casual.gif"), Difficulty = Sudoku_DTO.Sudoku_Difficulty.Casual },
            new DifficultiesAndImages() { ImageSource = ImageSource.FromResource("ArlottiL_SudokuApp.Resources.easy.gif"), Difficulty = Sudoku_DTO.Sudoku_Difficulty.Easy },
            new DifficultiesAndImages() { ImageSource = ImageSource.FromResource("ArlottiL_SudokuApp.Resources.medium.gif"), Difficulty = Sudoku_DTO.Sudoku_Difficulty.Medium },
            new DifficultiesAndImages() { ImageSource = ImageSource.FromResource("ArlottiL_SudokuApp.Resources.hard.gif"), Difficulty = Sudoku_DTO.Sudoku_Difficulty.Hard },
            new DifficultiesAndImages() { ImageSource = ImageSource.FromResource("ArlottiL_SudokuApp.Resources.expert.gif"), Difficulty = Sudoku_DTO.Sudoku_Difficulty.Expert },
            new DifficultiesAndImages() { ImageSource = ImageSource.FromResource("ArlottiL_SudokuApp.Resources.evil.gif"), Difficulty = Sudoku_DTO.Sudoku_Difficulty.Evil }
        };



        public Menu_View()
        {
            BindingContext = this;
            InitializeComponent();
            InitializeInterface();
        }

        private void InitializeInterface()
        {
            // ----- btnPlay -----
            RelativeLayout.SetBoundsConstraint(btnPlay_image, RelativeLayout.GetBoundsConstraint(btnPlay_cell));

            btnPlay_image.InitializePulse();
            btnPlay_image.ClickedAction = new Func<Image, Task>(async (sender) =>
            {
                Image icon = sender as Image;
                
                double maxSize = Math.Max(Application.Current.MainPage.Width, Application.Current.MainPage.Height);
                double scaleFactor = ((maxSize / icon.Width) / DEFAULT_ICON_SCALE) * 1.75;

                await icon.ScaleTo(scaleFactor, 1000);

                this.StartNewGame();
            });


            // ----- btnSettings -----
            RelativeLayout.SetBoundsConstraint(btnSettings_image, RelativeLayout.GetBoundsConstraint(btnSettings_cell));
            
            btnSettings_image.InitializePulse();
            btnSettings_image.ClickedAction = new Func<Image, Task>(async (sender) =>
            {
                Image icon = sender as Image;
                await icon.RotateTo(360, 500);
                await icon.ScaleTo(0.65, 200);

                await Navigation.PushAsync(new SettingsPage());

                await icon.RotateTo(0, 0);
                await icon.ScaleTo(0.5, 0);
            });

            // ----- btnDifficulties_image -----
            RelativeLayout.SetBoundsConstraint(frame_btnDifficulties_image, RelativeLayout.GetBoundsConstraint(btnDifficulty_cell));
        
        }
        
        public async void StartNewGame()
        {
            CarouselView carousel = btnDifficulties_image as CarouselView;
            Sudoku_DTO.Sudoku_Difficulty difficultySelected = ((DifficultiesAndImages) carousel.CurrentItem).Difficulty;

            HttpClient httpClient = new HttpClient();
            string response = "";
            bool success = true;
            Sudoku_DTO requestedSudoku = null;
            try
            {
                response = await httpClient.GetStringAsync($"{REQUEST_SUDOKU_BASE_URL}{difficultySelected}");
            }
            catch (Exception)
            {
                success = false;
            }
            if(success) requestedSudoku = (Sudoku_DTO) JsonConvert.DeserializeObject(response, typeof(Sudoku_DTO));

            this.RequestResponseEvent?.Invoke(btnPlay_image, requestedSudoku);


        }
        

    }
}