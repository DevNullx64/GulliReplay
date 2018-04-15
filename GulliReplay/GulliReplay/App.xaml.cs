﻿using Xamarin.Forms;

namespace GulliReplay
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new ProgramPage());
        }

        public static int DisplayScreenWidth { get; internal set; }
        public static int DisplayScreenHeight { get; internal set; }
        public static double DisplayScaleFactor { get; internal set; }
    }
}