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
    public partial class Alert_View : ContentView
    {
        // ----- backgroundColor PROPERTY -----

        public static readonly new BindableProperty BackgroundColorProperty = BindableProperty.Create(
            nameof(BackgroundColor),
            typeof(Color),
            typeof(Alert_View),
            defaultValue: Color.White,
            defaultBindingMode: BindingMode.TwoWay
            );
        public new Color BackgroundColor
        {
            get { return (Color) base.GetValue(BackgroundColorProperty); }
            set { base.SetValue(BackgroundColorProperty, value); }
        }

        // ----- borderColor PROPERTY -----

        public static readonly  BindableProperty BorderColorProperty = BindableProperty.Create(
            nameof(BorderColor),
            typeof(Color),
            typeof(Alert_View),
            defaultValue: Color.Black,
            defaultBindingMode: BindingMode.TwoWay
            );
        public Color BorderColor
        {
            get { return (Color)base.GetValue(BorderColorProperty); }
            set { base.SetValue(BorderColorProperty, value); }
        }

        // ----- textContent PROPERTY -----

        public static readonly BindableProperty TextContentProperty = BindableProperty.Create(
            nameof(TextContent),
            typeof(string),
            typeof(Alert_View),
            defaultValue: "",
            defaultBindingMode: BindingMode.TwoWay
            );

        public string TextContent
        {
            get { return (string)base.GetValue(TextContentProperty); }
            set { base.SetValue(TextContentProperty, value); }
        }


        public Alert_View()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public async void Summon(string text, Color backgroundColor)
        {
            this.TextContent = text;
            this.BackgroundColor = backgroundColor;

            this.InputTransparent = false;
            alertBackground.Scale = 1;

            await outerBorder.ScaleTo(1.2, 200);
            await outerBorder.ScaleTo(1, 200);
        }


        private async void Event_Tapped(object sender, EventArgs e)
        {
            await outerBorder.ScaleTo(1.2, 200);
            await outerBorder.ScaleTo(0, 200);
            alertBackground.Scale = 0;
            this.InputTransparent = true;


        }

        
    }
}