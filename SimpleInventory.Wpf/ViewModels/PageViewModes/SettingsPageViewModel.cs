using SimpleInventory.Wpf.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using System.Resources;
using SimpleInventory.Wpf.Services;
using SimpleInventory.Wpf.Models;

namespace SimpleInventory.Wpf.ViewModels.PageViewModes
{
    public class SettingsPageViewModel : PageViewModel
    {
        public string Name { get; set; } = "Settings";

        private readonly ISettingsService _settingsService; 

		private ICommand? _changeThemeCommand;
		private ICommand? _getThemesCommand;

        private ObservableCollection<ThemeItem> _themes;

        private ThemeItem _selectedTheme;

        public ThemeItem SelectedTheme
        {
            get => _selectedTheme;
            set => SetProperty(ref _selectedTheme, value);
        }


        public ObservableCollection<ThemeItem> Themes
        {
            get => _themes;
            set => SetProperty(ref _themes, value);
        }

        public ICommand GetThemesCommand
        {
            get
            {
                if (_getThemesCommand == null)
                {
                    _getThemesCommand = new RelayCommand(
                        p => GetThemes(),
                        p => true);
                }

                return _getThemesCommand;
            }
        }

        public ICommand ChangeThemeCommand
		{
            get
            {
                if (_changeThemeCommand == null)
                {
                    _changeThemeCommand = new RelayCommand(
                        p => _settingsService.SetTheme(SelectedTheme.Uri),
                        p => SelectedTheme != null && SelectedTheme.Uri != null);
                }

                return _changeThemeCommand;
            }
        }

        public SettingsPageViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        private void GetThemes()
        {
            Themes = new ObservableCollection<ThemeItem>
            {
                new ThemeItem { Name = "Dark", Uri = "Resources/DarkTheme.xaml" },
                new ThemeItem { Name = "Light", Uri = "Resources/LightTheme.xaml" }
            };

            SelectedTheme = Themes.Where(x => x.Uri == _settingsService.GetTheme()).First();
        }
    }
}
