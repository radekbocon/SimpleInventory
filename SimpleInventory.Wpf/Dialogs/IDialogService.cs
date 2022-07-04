using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Wpf.Dialogs
{
    public interface IDialogService
    {
        void ShowDialog(string name, Action<bool> callback);
    }
}
