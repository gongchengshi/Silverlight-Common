///////////////////////////////////////////////////////////////////////////////
//  COPYRIGHT (c) 2011 Schweitzer Engineering Laboratories, Pullman, WA
//////////////////////////////////////////////////////////////////////////////

using System.Windows;

namespace InfragisticsTest
{
    public class App : DeathToXAML.SilverlightApplication
    {
        public App()
        {
            Startup += Application_Startup;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var mainView = new MainPage();
            mainView.DataContext = new TestViewModel();
            RootVisual = mainView;
        }
    }
}
