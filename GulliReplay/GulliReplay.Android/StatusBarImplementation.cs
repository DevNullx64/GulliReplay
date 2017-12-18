using Android.App;
using Android.Views;
using GulliReplay;
using GulliReplay.Droid;
using System;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(StatusBarImplementation))]
namespace GulliReplay.Droid
{
    public class StatusBarImplementation : IStatusBar
    {
        public StatusBarImplementation()
        {
        }

        WindowManagerFlags? _originalFlags = null;

        #region IStatusBar implementation

        public bool HideStatusBar()
        {
            try
            {
                //var activity = (Activity)Android.App.Application.Context;
                var activity = (Activity)Forms.Context;
                var attrs = activity.Window.Attributes;
                _originalFlags = attrs.Flags;
                attrs.Flags |= Android.Views.WindowManagerFlags.Fullscreen;
                activity.Window.Attributes = attrs;
                return true;
            } catch (Exception e)
            {
                _originalFlags = null;
                return false;
            }
        }

        public bool ShowStatusBar()
        {
            if (_originalFlags != null)
            {
                try
                {
                    //var activity = (Activity)Android.App.Application.Context;
                    var activity = (Activity)Forms.Context;
                    var attrs = activity.Window.Attributes;
                    attrs.Flags = (WindowManagerFlags)_originalFlags;
                    activity.Window.Attributes = attrs;
                    _originalFlags = null;
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
                finally
                {
                    _originalFlags = null;
                }
            }
            else return false;
        }

        #endregion
    }
}