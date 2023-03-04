using SimpleInventory.Wpf.Properties;
using SimpleInventory.Wpf.ViewModels.PageViewModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleInventory.Wpf.Services
{
    public class SettingsService : ISettingsService
    {
        public void SetTheme(string themeUri)
        {
            ResourceDictionary theme = App.Current.Resources.MergedDictionaries.FirstOrDefault(x => x.Source.OriginalString.EndsWith("Theme.xaml"));
            if (theme != null)
            {
                App.Current.Resources.MergedDictionaries.Remove(theme);
            }

            App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(themeUri, UriKind.Relative),
            });

            Settings.Default.Theme = themeUri;
        }

        public string GetTheme()
        {
            return Settings.Default.Theme;
        }
    }
}
