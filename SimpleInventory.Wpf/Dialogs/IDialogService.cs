using SimpleInventory.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Wpf.Dialogs
{
    public interface IDialogService
    {
        void DialogResult(bool resultPositive);
        void ShowDialog(ViewModelBase viewModel, Action<bool> callback, double dialogWidth = 0);

    }
}
