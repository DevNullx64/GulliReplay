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
#pragma warning disable CS0618 // 'Forms.Context' est obsolète : 'Context is obsolete as of version 2.5. Please use a local context instead.'
                var activity = (Activity)Forms.Context;
#pragma warning restore CS0618 // 'Forms.Context' est obsolète : 'Context is obsolete as of version 2.5. Please use a local context instead.'
                var attrs = activity.Window.Attributes;
                _originalFlags = attrs.Flags;
                attrs.Flags |= Android.Views.WindowManagerFlags.Fullscreen;
                activity.Window.Attributes = attrs;
                return true;
#pragma warning disable CS0168 // La variable 'e' est déclarée, mais jamais utilisée
            } catch (Exception e)
#pragma warning restore CS0168 // La variable 'e' est déclarée, mais jamais utilisée
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
#pragma warning disable CS0618 // 'Forms.Context' est obsolète : 'Context is obsolete as of version 2.5. Please use a local context instead.'
                    var activity = (Activity)Forms.Context;
#pragma warning restore CS0618 // 'Forms.Context' est obsolète : 'Context is obsolete as of version 2.5. Please use a local context instead.'
                    var attrs = activity.Window.Attributes;
                    attrs.Flags = (WindowManagerFlags)_originalFlags;
                    activity.Window.Attributes = attrs;
                    _originalFlags = null;
                    return true;
                }
#pragma warning disable CS0168 // La variable 'e' est déclarée, mais jamais utilisée
                catch (Exception e)
#pragma warning restore CS0168 // La variable 'e' est déclarée, mais jamais utilisée
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