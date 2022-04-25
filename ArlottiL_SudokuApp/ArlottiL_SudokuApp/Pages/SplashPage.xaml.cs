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
    public partial class SplashPage : ContentPage
    {
        public SplashPage()
        {
            InitializeComponent();

            menuSizeHelper.SizeChanged += MenuSizeHelper_SizeChanged;
            gridMenu.RequestResponseEvent += GridMenu_RequestResponseEvent;
            
        }

        private async void GridMenu_RequestResponseEvent(object sender, Sudoku_DTO e)
        {
            if(e == null)
            {
                await lblError.ScaleTo(1, 400);
                return;
            }

            await Navigation.PushAsync(new GamePage(e));


        }

        private void MenuSizeHelper_SizeChanged(object sender, EventArgs e)
        {
            BoxView helper = menuSizeHelper as BoxView;

            double size = Math.Min(Menu_View.MAX_SIZE, Math.Min(helper.Width, helper.Height));

            gridMenu.WidthRequest = size;
            gridMenu.HeightRequest = size;
            
        }

        private async void lblError_Tapped(object sender, EventArgs e)
        {
            Frame f = sender as Frame;
            await f.ScaleTo(1.2, 300);
            await f.ScaleTo(0, 200);
        }
    }
}