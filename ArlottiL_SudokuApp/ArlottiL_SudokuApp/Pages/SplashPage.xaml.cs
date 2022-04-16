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
            
        }

        private void MenuSizeHelper_SizeChanged(object sender, EventArgs e)
        {
            BoxView helper = menuSizeHelper as BoxView;

            double size = Math.Min(Menu_View.MAX_SIZE, Math.Min(helper.Width, helper.Height));

            gridMenu.WidthRequest = size;
            gridMenu.HeightRequest = size;
            
        }
    }
}