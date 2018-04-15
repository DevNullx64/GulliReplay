using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.Res;
using System.Threading.Tasks;

namespace GulliReplay.Droid
{
    [Activity(Label = "GulliReplay", Icon = "@drawable/icon", Theme = "@style/splashscreen", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        // Field, properties, and method for Video Picker
        public static MainActivity Current { private set; get; }
        public static readonly int PickImageId = 1000;

        protected override void OnCreate(Bundle bundle)
        {
            Current = this;

            App.DisplayScreenWidth = (uint)Resources.DisplayMetrics.WidthPixels;
            App.DisplayScreenHeight = (uint)Resources.DisplayMetrics.HeightPixels;
            App.DisplayScaleFactor = (double)Resources.DisplayMetrics.Density;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.Window.RequestFeature(WindowFeatures.ActionBar);
            base.SetTheme(Resource.Style.MyTheme);

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            LoadApplication(new App());
        }

        protected void setUiOptions()
        {
            //====================================
            int uiOptions = (int)Window.DecorView.SystemUiVisibility;

            //uiOptions |= (int)SystemUiFlags.LowProfile;
            //uiOptions |= (int)SystemUiFlags.Fullscreen;
            uiOptions |= (int)SystemUiFlags.HideNavigation;
            uiOptions |= (int)SystemUiFlags.ImmersiveSticky;

            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;
            //====================================

        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            setUiOptions();
        }

        public TaskCompletionSource<string> PickImageTaskCompletionSource { set; get; }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == PickImageId)
            {
                if ((resultCode == Result.Ok) && (data != null))
                {
                    // Set the filename as the completion of the Task
                    PickImageTaskCompletionSource.SetResult(data.DataString);
                }
                else
                {
                    PickImageTaskCompletionSource.SetResult(null);
                }
            }
        }
    }
}
