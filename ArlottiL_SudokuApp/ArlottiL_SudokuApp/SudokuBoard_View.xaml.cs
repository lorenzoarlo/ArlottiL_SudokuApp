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
        public const int BOARD_DIMENSION = 9;

        public const int REGION_DIMENSION = 3;

        public const int MAX_SIZE = 800;

        SudokuCell_View[,] cellsTable = new SudokuCell_View[BOARD_DIMENSION, BOARD_DIMENSION];

        public SudokuBoard_View()
        {
            this.InitializeComponent();
            this.InitializeInterface();
        }

        private void InitializeInterface()
        {
            double cellSize = 1.0 / BOARD_DIMENSION;
            for (int row = 0; row < BOARD_DIMENSION; row++)
            {
                for(int column = 0; column < BOARD_DIMENSION; column++)
                {
                    cellsTable[row, column] = new SudokuCell_View()
                    {
                        Style = (Style) this.Resources["cellFrame-style"]
                    };

                    double x = Utilities.GetProportionalCoordinate((double) column * cellSize, cellSize);
                    double y = Utilities.GetProportionalCoordinate((double) row * cellSize, cellSize);

                    boardContainer.Children
                        .Add(cellsTable[row, column], 
                        new Rect(x, y, cellSize, cellSize),
                        AbsoluteLayoutFlags.All);
                    boardContainer.LowerChild(cellsTable[row, column]);
                }
            }


            double regionSize = 1.0 / REGION_DIMENSION;
            for (int row = 0; row < REGION_DIMENSION; row++)
            {
                for (int column = 0; column < REGION_DIMENSION; column++)
                {
                    Xamarin.Forms.Shapes.Rectangle regionFrame = new Xamarin.Forms.Shapes.Rectangle()
                    {
                        Style = (Style)this.Resources["regionFrame-style"]
                    };

                    double x = Utilities.GetProportionalCoordinate((double) column * regionSize, regionSize);
                    double y = Utilities.GetProportionalCoordinate((double) row * regionSize, regionSize);

                    boardContainer.Children
                        .Add(regionFrame,
                        new Rect(x, y, regionSize, regionSize),
                        AbsoluteLayoutFlags.All);
                }
            }




        }

        


    }
}