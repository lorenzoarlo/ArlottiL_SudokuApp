using Xamarin.Forms;

[assembly: ExportFont("PublicPixel.ttf")]
[assembly: ExportFont("NotoSans.ttf")]
namespace ArlottiL_SudokuAppClient
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Menu_Page());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
