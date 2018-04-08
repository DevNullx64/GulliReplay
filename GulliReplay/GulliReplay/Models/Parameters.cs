using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace GulliReplay
{
    public enum GulliQuality
    {
        _200 = 0,
        _350 = 1,
        _750 = 2,
        _900 = 3,
        _1500 = 4
    }

    class Parameters
    {
        private static T Get<T>(T defaultValue, [CallerMemberName] string propertyName = "")
            => (Application.Current.Properties.TryGetValue(propertyName, out object result)) ? (T)result : defaultValue;
        private static void Set<T>(T value, [CallerMemberName] string propertyName = "")
            => Application.Current.Properties[propertyName] = value;

        public static GulliQuality DefaultQuality
        {
            get => (GulliQuality)Get((int)GulliQuality._900);
            set => Set((int)value);
        }

        public static void Save() => Application.Current.SavePropertiesAsync();
    }
}
