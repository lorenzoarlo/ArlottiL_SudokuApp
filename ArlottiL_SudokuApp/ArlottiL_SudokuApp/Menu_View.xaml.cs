using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ArlottiL_SudokuApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Menu_View : ContentView
    {
        public const int MAX_SIZE = 600;

        public const int MENU_DIMENSION = 3;

        public Menu_View()
        {
            InitializeComponent();
            this.InitializeInterface();
        }

        private async void InitializeInterface()
        {
            double cellSize = 1.0 / MENU_DIMENSION;
            string[,] characters =  {
                {"1","8","3"},
                {"6","▶","⚙"},
                {"7","2","5"} 
            };

            SudokuCell_View middle = null;
            for (int row = 0; row < MENU_DIMENSION; row++)
            {
                for (int column = 0; column < MENU_DIMENSION; column++)
                {
                    SudokuCell_View cell = new SudokuCell_View()
                    {
                        Style = (Style) this.Resources["cellFrame-style"],
                        TextContent = characters[row, column]
                    };

                    if (row == 1 && column == 1) middle = cell;


                    double x = Utilities.GetProportionalCoordinate((double)column * cellSize, cellSize);
                    double y = Utilities.GetProportionalCoordinate((double)row * cellSize, cellSize);

                    boardContainer.Children
                        .Add(cell,
                        new Rect(x, y, cellSize, cellSize),
                        AbsoluteLayoutFlags.All);
                    boardContainer.LowerChild(cell);
                }
            }

            middle.Style = (Style)this.Resources["specialCell-style"];
            
            int pulseAnimationID = Utilities.NewPulseAnimation();
            Utilities.RunPulseAnimation(pulseAnimationID, middle.ContentLabel, 1, 1.4, 2000);

        }
    }
}