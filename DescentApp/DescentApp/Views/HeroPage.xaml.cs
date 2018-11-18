using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DescentApp.ViewModels;

namespace DescentApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HeroPage : ContentPage {
		public HeroPage () {
            InitializeComponent();
		}
	}
}  
