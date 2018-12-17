using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DescentApp.Services;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DescentApp {
    public partial class App : Application {
        public App() {
            InitializeComponent();
            // uncommented because Xmarin does not put the texts in 
            // the correct folder :(

            DependencyService.Register<FileDataStore>();
            //DependencyService.Register<StaticDataStore>();
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart() {
            // Handle when your app starts
        }

        protected override void OnSleep() {
            // Handle when your app sleeps
        }

        protected override void OnResume() {
            // Handle when your app resumes
        }
    }
}
