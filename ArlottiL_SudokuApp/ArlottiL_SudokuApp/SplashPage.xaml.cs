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
        public const double MENUSIZE_FACTOR = 0.8;
        public SplashPage()
        {
            InitializeComponent();

            RelativeLayout.SetXConstraint(appMenu,
                Constraint.RelativeToParent(parent => parent.Width * 0.5 - (appMenu.Width / 2)));

            RelativeLayout.SetYConstraint(appMenu,
                Constraint.RelativeToParent(parent => parent.Height * 0.5 - (appMenu.Height / 2)));
            
            
            screenContainer.SizeChanged += OnScreenContainerSizeChanged;
        }


        private void OnScreenContainerSizeChanged(object sender, EventArgs e)
        {
            RelativeLayout container = sender as RelativeLayout;

            double minDimension = Math.Min(Menu_View.MAX_SIZE, Math.Min(container.Width, container.Height));

            double newSize = minDimension * MENUSIZE_FACTOR;

            RelativeLayout.SetWidthConstraint(appMenu, Constraint.Constant(newSize));
            RelativeLayout.SetHeightConstraint(appMenu, Constraint.Constant(newSize));
        }
    }
}