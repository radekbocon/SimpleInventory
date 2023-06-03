using SimpleInventory.Wpf.ViewModels;

namespace SimpleInventory.Wpf.Factories
{
    public interface IViewModelFactory
    {
        T Create<T>() where T : ViewModelBase;
    }
}