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
    public partial class IconButton_View : ContentView
    {
        // ----- BINDING PROPERTIES -----

        public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(
            nameof(ImageSource),
            typeof(ImageSource),
            typeof(IconButton_View),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay
            );

        public static readonly BindableProperty ImageScaleProperty = BindableProperty.Create(
            nameof(ImageScale),
            typeof(double),
            typeof(IconButton_View),
            defaultValue: 1.0,
            defaultBindingMode: BindingMode.TwoWay
            );

        public static readonly BindableProperty ResultImageScaleProperty = BindableProperty.Create(
            nameof(ResultImageScale),
            typeof(double),
            typeof(IconButton_View),
            defaultValue: 1.1,
            defaultBindingMode: BindingMode.TwoWay
            );

        public static readonly BindableProperty PulseAnimationProperty = BindableProperty.Create(
            nameof(PulseAnimation),
            typeof(bool),
            typeof(IconButton_View),
            defaultValue: true,
            defaultBindingMode: BindingMode.TwoWay
            );

        // ----- PROPERTIES -----

        public ImageSource ImageSource
        {
            get { return (ImageSource)base.GetValue(ImageSourceProperty); }
            set { base.SetValue(ImageSourceProperty, value); }
        }

        public double ImageScale
        {
            get { return (double) base.GetValue(ImageScaleProperty); }
            set { base.SetValue(ImageScaleProperty, value); }
        }
        public double ResultImageScale
        {
            get { return (double)base.GetValue(ResultImageScaleProperty); }
            set { base.SetValue(ResultImageScaleProperty, value); }
        }

        public bool PulseAnimation
        {
            get { return (bool)base.GetValue(PulseAnimationProperty); }
            set { base.SetValue(PulseAnimationProperty, value); }
        }

        public Func<Image, Task> ClickedAction { get; set; } = new Func<Image, Task>(sender => Task.CompletedTask);

        private bool _imageClicked = false;

        private int _idPulseAnimation;

        public IconButton_View()
        {
            InitializeComponent();
            BindingContext = this;
        }

        
        public void InitializePulse()
        {
            if (!this.PulseAnimation) return;

            this._idPulseAnimation = Utilities.NewPulseAnimation();
            Utilities.RunPulseAnimation(this._idPulseAnimation, iconImage, this.ImageScale, this.ResultImageScale, 1500);
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (this._imageClicked) return;
            
            this._imageClicked = true;

            if (this.PulseAnimation) Utilities.StopPulseAnimation(this._idPulseAnimation);

            await this.ClickedAction.Invoke(iconImage);

            this._imageClicked = false;

            if (this.PulseAnimation) Utilities.RunPulseAnimation(this._idPulseAnimation, iconImage, this.ImageScale, this.ResultImageScale, 1500);
            
        }
    }
}