using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ArlottiL_SudokuAppClient
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShapedRectangle_View : ContentView
    {
        private const double FONT_SIZE_FACTOR = 0.5;

        // ----- backgroundColor PROPERTY -----

        public static readonly new BindableProperty BackgroundColorProperty = BindableProperty.Create(
            nameof(BackgroundColor),
            typeof(Color),
            typeof(ShapedRectangle_View),
            defaultValue: Color.White,
            defaultBindingMode: BindingMode.TwoWay
            );
        public new Color BackgroundColor
        {
            get { return (Color)base.GetValue(BackgroundColorProperty); }
            set { base.SetValue(BackgroundColorProperty, value); }
        }

        // ----- borderColor PROPERTY -----

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
            nameof(BorderColor),
            typeof(Brush),
            typeof(ShapedRectangle_View),
            defaultValue: Brush.Black,
            defaultBindingMode: BindingMode.TwoWay
            );

        public Brush BorderColor
        {
            get { return (Brush)base.GetValue(BorderColorProperty); }
            set { base.SetValue(BorderColorProperty, value); }
        }

        // ----- fontColor PROPERTY -----

        public static readonly BindableProperty FontColorProperty = BindableProperty.Create(
            nameof(FontColor),
            typeof(Color),
            typeof(ShapedRectangle_View),
            defaultValue: Color.Black,
            defaultBindingMode: BindingMode.TwoWay
            );
        public Color FontColor
        {
            get { return (Color)base.GetValue(FontColorProperty); }
            set { base.SetValue(FontColorProperty, value); }
        }

        // ----- borderThickness PROPERTY -----

        public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(
            nameof(BorderThickness),
            typeof(double),
            typeof(ShapedRectangle_View),
            defaultValue: 1.0,
            defaultBindingMode: BindingMode.TwoWay
            );

        public double BorderThickness
        {
            get { return (double)base.GetValue(BorderThicknessProperty); }
            set { base.SetValue(BorderThicknessProperty, value); }
        }

        // ----- textContent PROPERTY -----

        public static readonly BindableProperty TextContentProperty = BindableProperty.Create(
            nameof(TextContent),
            typeof(string),
            typeof(ShapedRectangle_View),
            defaultValue: "",
            defaultBindingMode: BindingMode.TwoWay
            );

        public string TextContent
        {
            get { return (string)base.GetValue(TextContentProperty); }
            set { base.SetValue(TextContentProperty, value); }
        }

        // ----- fontSize PROPERTY -----

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            nameof(FontSize),
            typeof(double),
            typeof(ShapedRectangle_View),
            defaultValue: 11.0,
            defaultBindingMode: BindingMode.TwoWay
            );

        public double FontSize
        {
            get { return (double)base.GetValue(FontSizeProperty); }
            set { base.SetValue(FontSizeProperty, value); }
        }

        // ----- PROPERTIES -----

        public Func<ShapedRectangle_View, Task> OnTapEvent { get; set; } = new Func<ShapedRectangle_View, Task>(sender => Task.CompletedTask);

        public ShapedRectangle_View()
        {
            InitializeComponent();
            this.BindingContext = this;

            this.viewFrame.SizeChanged += ViewFrame_SizeChanged;
        }

        private void ViewFrame_SizeChanged(object sender, EventArgs e)
        {
            Xamarin.Forms.Shapes.Rectangle viewFrame = sender as Xamarin.Forms.Shapes.Rectangle;
            this.FontSize = viewFrame.Width * FONT_SIZE_FACTOR;
        }

        private async void Event_Tapped(object sender, EventArgs e) => await this.OnTapEvent.Invoke(this);
    }
}