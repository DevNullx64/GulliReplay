﻿using System;

using Xamarin.Forms;

namespace GulliReplay
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            if (Device.RuntimePlatform == Device.iOS)
                MainPage = new ProgramPage();
            else
                MainPage = new NavigationPage(new ProgramPage());
        }
    }
}