﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DescentApp {
    public partial class MainPage : ContentPage {
        public MainPage() {
            InitializeComponent();
        }

        async void OnHeroButtonClicked(object sender, EventArgs e) {
            await Navigation.PushAsync(new HeroPage());
        }

        async void OnFightButtonClicked(object sender, EventArgs e) {
            await Navigation.PushAsync(new FightPage());
        }
    }
}
