using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace ArlottiL_SudokuApp
{
    public partial class MainPage : ContentPage
    {
        public const double BOARDSIZE_FACTOR = 0.9;

        public MainPage()
        {
            InitializeComponent();
            screenContainer.SizeChanged += OnScreenContainerSizeChanged;


        }

        private void OnScreenContainerSizeChanged(object sender, EventArgs e)
        {
            AbsoluteLayout container = sender as AbsoluteLayout;
            
            double minDimension = Math.Min(SudokuBoard_View.MAX_SIZE , Math.Min(container.Width, container.Height));

            double newSize = minDimension * BOARDSIZE_FACTOR;

            Rect boardBounds = AbsoluteLayout.GetLayoutBounds(sudokuBoard);
            AbsoluteLayout.SetLayoutBounds(sudokuBoard, new Xamarin.Forms.Rectangle(boardBounds.X, boardBounds.Y, newSize, newSize));

        }
    }

}
