using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ArlottiL_SudokuAppClient
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IconButton_View : ContentView
    {
        // ----- iconSource PROPERTY -----

        public static readonly BindableProperty IconSourceProperty = BindableProperty.Create(
            nameof(IconSource),
            typeof(ImageSource),
            typeof(IconButton_View),
            defaultValue: null,
            defaultBindingMode: BindingMode.TwoWay
            );

        public ImageSource IconSource
        {
            get { return (ImageSource)base.GetValue(IconSourceProperty); }
            set { base.SetValue(IconSourceProperty, value); }
        }


        // ----- pulse PROPERTIES -----

        private bool _pulse = false;

        private int _pulseAnimationID = -1;

        private const double PULSE_ZOOM = 1.2;

        private const uint PULSE_DURATION = 1500;

        // ----- otherProperties -----
        public Func<Image, Task> OnTapEvent { get; set; } = new Func<Image, Task>(sender => Task.CompletedTask);

        private bool _tapped = false;

        private double _initialScale = 1;

        public IconButton_View()
        {
            InitializeComponent();
            BindingContext = this;
        }


        public void ActivatePulse()
        {
            this._pulse = true;

            if(this._pulseAnimationID == -1) this._pulseAnimationID = Utilities.NewPulseAnimation();
            Utilities.RunPulseAnimation(this._pulseAnimationID, iconImage, this.Scale, this.Scale * PULSE_ZOOM, PULSE_DURATION);
        }

        public void StopPulse()
        {
            this._pulse = false;
            if (this._pulseAnimationID == -1) return;
            Utilities.StopPulseAnimation(this._pulseAnimationID);
        }

        private async void Event_Tapped(object sender, EventArgs e)
        {
            if (_tapped) return;

            _tapped = true;

            bool hasPulse = this._pulse;

            if (hasPulse) this.StopPulse();

            await this.OnTapEvent.Invoke(iconImage);

            if (hasPulse) this.ActivatePulse();

            _tapped = false;
        }
    }
}