﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ArlottiL_SudokuApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SudokuCell_View : ContentView
    {
        private const double FONT_SIZE_FACTOR = 0.5;

        // ----- BINDING PROPERTIES -----

        public static readonly new BindableProperty BackgroundColorProperty = BindableProperty.Create(
            nameof(BackgroundColor),
            typeof(Color),
            typeof(SudokuBoard_View),
            defaultValue: Color.White,
            defaultBindingMode: BindingMode.TwoWay
            );

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
            nameof(BorderColor),
            typeof(Brush),
            typeof(SudokuBoard_View),
            defaultValue: Brush.Black,
            defaultBindingMode: BindingMode.TwoWay
            );

        public static readonly BindableProperty FontColorProperty = BindableProperty.Create(
            nameof(FontColor),
            typeof(Color),
            typeof(SudokuBoard_View),
            defaultValue: Color.Black,
            defaultBindingMode: BindingMode.TwoWay
            );

        public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(
            nameof(BorderThickness),
            typeof(double),
            typeof(SudokuBoard_View),
            defaultValue: 1.0,
            defaultBindingMode: BindingMode.TwoWay
            );

        public static readonly BindableProperty TextContentProperty = BindableProperty.Create(
            nameof(TextContent),
            typeof(string),
            typeof(SudokuBoard_View),
            defaultValue: "",
            defaultBindingMode: BindingMode.TwoWay
            );

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            nameof(FontSize),
            typeof(double),
            typeof(SudokuBoard_View),
            defaultValue: 11.0,
            defaultBindingMode: BindingMode.TwoWay
            );

        // ----- PROPERTIES -----

        public new Color BackgroundColor
        {
            get { return (Color)base.GetValue(BackgroundColorProperty); }
            set { base.SetValue(BackgroundColorProperty, value); }
        }

        public Brush BorderColor
        {
            get { return (Brush) base.GetValue(BorderColorProperty); }
            set { base.SetValue(BorderColorProperty, value); }
        }

        public Color FontColor
        {
            get { return (Color)base.GetValue(FontColorProperty); }
            set { base.SetValue(FontColorProperty, value); }
        }

        public double BorderThickness
        {
            get { return (double) base.GetValue(BorderThicknessProperty); }
            set { base.SetValue(BorderThicknessProperty, value); }
        }
        public string TextContent
        {
            get { return (string)base.GetValue(TextContentProperty); }
            set { base.SetValue(TextContentProperty, value); }
        }

        public double FontSize
        {
            get { return (double)base.GetValue(FontSizeProperty); }
            set { base.SetValue(FontSizeProperty, value); }
        }
        public Label ContentLabel { get { return this.lblContent; }  }

        public SudokuCell_View()
        {
            InitializeComponent();
            this.BindingContext = this;
            this.frame.SizeChanged += OnCellFrameSizeChanged;
        }

        private void OnCellFrameSizeChanged(object sender, EventArgs e)
        {
            Xamarin.Forms.Shapes.Rectangle frame = sender as Xamarin.Forms.Shapes.Rectangle;
            this.FontSize = frame.Width * FONT_SIZE_FACTOR;
        }

    }
}