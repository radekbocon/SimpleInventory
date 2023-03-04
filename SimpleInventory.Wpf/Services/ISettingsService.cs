using SimpleInventory.Wpf.ViewModels.PageViewModes;

namespace SimpleInventory.Wpf.Services
{
    public interface ISettingsService
    {
        string GetTheme();
        void SetTheme(string themeUri);
    }
}