using SimpleInventory.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Wpf.Services
{
    public interface INavigationService
    {
        void DialogResult(bool resultPositive);
        void ModalResult(bool doAction);
        void OpenPage(ViewModelBase viewModel);
        void GoBack();
        void ShowDialog(ViewModelBase viewModel, Action<bool> callback, double dialogWidth = 0);
        void ShowModal(ViewModelBase viewModel, Action<bool> callback, double modalWidth = 0);
    }
}
