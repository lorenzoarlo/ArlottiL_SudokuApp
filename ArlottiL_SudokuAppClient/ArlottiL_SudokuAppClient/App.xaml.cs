using Xamarin.Forms;

[assembly: ExportFont("PublicPixel.ttf")]
[assembly: ExportFont("NotoSans.ttf")]
namespace ArlottiL_SudokuAppClient
{
    public partial class App : Application
    {
        private bool _cronometroRunning = false;

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
            if (Game_Page.Cronometro.IsRunning)
            {
                Game_Page.Cronometro.Stop();
                this._cronometroRunning=true;
            }
        }

        protected override void OnResume()
        {
            if (this._cronometroRunning)
            {
                Game_Page.Cronometro.Start();
                this._cronometroRunning=false;
            }
        }
    }
}
