using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Reflection;

namespace ArlottiL_SudokuApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Menu_View : ContentView
    {
        public const int MAX_SIZE = 600;

        static bool btnSettings_clicked = false;

        static bool btnPlay_clicked = false;

        public struct Selectable_Difficulty
        {
            public Sudoku_DTO.Sudoku_Difficulty Difficulty;
            
            public ImageSource ImageSource;
        }

        //ObservableCollection<Selectable_Difficulty> DifficultiesSources = new ObservableCollection<Selectable_Difficulty>()
        //{
        //    new Selectable_Difficulty() { Difficulty = Sudoku_DTO.Sudoku_Difficulty.Casual, ImageSource = ImageSource.FromResource("ArlottiL_SudokuApp.Resources.casual.gif")},
        //    new Selectable_Difficulty() { Difficulty = Sudoku_DTO.Sudoku_Difficulty.Easy, ImageSource = ImageSource.FromResource("ArlottiL_SudokuApp.Resources.easy.gif")},
        //    new Selectable_Difficulty() { Difficulty = Sudoku_DTO.Sudoku_Difficulty.Medium, ImageSource = ImageSource.FromResource("ArlottiL_SudokuApp.Resources.medium.gif")},
        //    new Selectable_Difficulty() { Difficulty = Sudoku_DTO.Sudoku_Difficulty.Hard, ImageSource = ImageSource.FromResource("ArlottiL_SudokuApp.Resources.hard.gif")},
        //    new Selectable_Difficulty() { Difficulty = Sudoku_DTO.Sudoku_Difficulty.Expert, ImageSource = ImageSource.FromResource("ArlottiL_SudokuApp.Resources.expert.gif")},
        //    new Selectable_Difficulty() { Difficulty = Sudoku_DTO.Sudoku_Difficulty.Evil, ImageSource = ImageSource.FromResource("ArlottiL_SudokuApp.Resources.evil.gif")}
        //};

        public ObservableCollection<ImageSource> DifficultiesSources { get; set; } = new ObservableCollection<ImageSource>()
        {
            ImageSource.FromResource("ArlottiL_SudokuApp.Resources.casual.gif", typeof(ImageResourceExtension).GetTypeInfo().Assembly),
            ImageSource.FromResource("ArlottiL_SudokuApp.Resources.easy.gif", typeof(ImageResourceExtension).GetTypeInfo().Assembly),
            ImageSource.FromResource("ArlottiL_SudokuApp.Resources.medium.gif", typeof(ImageResourceExtension).GetTypeInfo().Assembly),
            ImageSource.FromResource("ArlottiL_SudokuApp.Resources.hard.gif", typeof(ImageResourceExtension).GetTypeInfo().Assembly),
            ImageSource.FromResource("ArlottiL_SudokuApp.Resources.expert.gif", typeof(ImageResourceExtension).GetTypeInfo().Assembly),
            ImageSource.FromResource("ArlottiL_SudokuApp.Resources.evil.gif", typeof(ImageResourceExtension).GetTypeInfo().Assembly)
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

            int animationID = Utilities.NewPulseAnimation();
            TapGestureRecognizer btnPlay_tapGestureRecognizer = new TapGestureRecognizer();
            btnPlay_tapGestureRecognizer.Tapped += async (sender, e) => {
                if (btnPlay_clicked) return;
                
                Utilities.DeletePulseAnimation(animationID);
                Image icon = sender as Image;
                
                btnPlay_clicked = true;
                const double DEFAULT_ICON_SCALE = 0.5;
                double maxSize = Math.Max(Application.Current.MainPage.Width, Application.Current.MainPage.Height);
                double scaleFactor = ((maxSize / icon.Width) / DEFAULT_ICON_SCALE) * 1.75;

                await icon.ScaleTo(scaleFactor, 1000);
                await icon.ScaleTo(DEFAULT_ICON_SCALE, 0);

                btnPlay_clicked = false;
            };
            btnPlay_image.GestureRecognizers.Add(btnPlay_tapGestureRecognizer);
            Utilities.RunPulseAnimation(animationID, btnPlay_image, 0.5, 0.6, 1500);


            // ----- btnSettings -----
            RelativeLayout.SetBoundsConstraint(btnSettings_image, RelativeLayout.GetBoundsConstraint(btnSettings_cell));
            TapGestureRecognizer btnSettings_tapGestureRecognizer = new TapGestureRecognizer();
            btnSettings_tapGestureRecognizer.Tapped += async (sender, e) => {
                if (btnSettings_clicked) return;
                Image icon = sender as Image;

                btnSettings_clicked = true;

                await icon.RotateTo(360, 500);
                await icon.ScaleTo(0.65, 200);

                await Navigation.PushAsync(new SettingsPage());
                
                await icon.RotateTo(0, 0);
                await icon.ScaleTo(0.5, 0);

                btnSettings_clicked = false;
            };
            btnSettings_image.GestureRecognizers.Add(btnSettings_tapGestureRecognizer);

            // ----- selDifficulty -----

            CarouselView sel = selDifficulty as CarouselView;
            RelativeLayout.SetBoundsConstraint(sel, RelativeLayout.GetBoundsConstraint(btnDifficulty_cell));
        }


        //private async void InitializeInterface()
        //{
        //    double cellSize = 1.0 / MENU_DIMENSION;
        //    string[,] characters =  {
        //        {"1","8","3"},
        //        {"6","▶","⚙"},
        //        {"7","2","5"} 
        //    };

        //    SudokuCell_View middle = null;
        //    for (int row = 0; row < MENU_DIMENSION; row++)
        //    {
        //        for (int column = 0; column < MENU_DIMENSION; column++)
        //        {
        //            SudokuCell_View cell = new SudokuCell_View()
        //            {
        //                Style = (Style) this.Resources["cellFrame-style"],
        //                TextContent = characters[row, column]
        //            };

        //            if (row == 1 && column == 1) middle = cell;


        //            double x = Utilities.GetProportionalCoordinate((double)column * cellSize, cellSize);
        //            double y = Utilities.GetProportionalCoordinate((double)row * cellSize, cellSize);

        //            boardContainer.Children
        //                .Add(cell,
        //                new Rect(x, y, cellSize, cellSize),
        //                AbsoluteLayoutFlags.All);
        //            boardContainer.LowerChild(cell);
        //        }
        //    }

        //    middle.Style = (Style)this.Resources["specialCell-style"];

        //    int pulseAnimationID = Utilities.NewPulseAnimation();
        //    Utilities.RunPulseAnimation(pulseAnimationID, middle.ContentLabel, 1, 1.4, 2000);

        //}
    }
}