using SimpleInventory.Wpf.ViewModels;
using System;
using System.Linq;

namespace SimpleInventory.Wpf.Dialogs
{
    public class DialogService : IDialogService
    {
        public void ShowDialog(ViewModelBase viewModel, Action<bool> callback, double dialogWidth = 0)
        {
            var dialog = new DialogWindow();
            var parent = App.Current.MainWindow;
            dialog.Width = dialogWidth == 0 ? (parent.Width / 1.5) : dialogWidth;

            EventHandler closeEventHandler = null;
            closeEventHandler = (s, e) =>
            {
                callback(dialog.DialogResult.Value);
                dialog.Closed -= closeEventHandler;
                parent.Opacity = 1;
            };
            dialog.Closed += closeEventHandler;

            parent.Opacity = 0.8;
            dialog.ShowInTaskbar = false;
            dialog.Content = viewModel;
            dialog.Owner = parent;
            dialog.ShowDialog();
        }

        public void DialogResult(bool dialogResult)
        {
            var window = App.Current.Windows.OfType<DialogWindow>().FirstOrDefault();
            if (window == null) return;

            if (dialogResult)
            {
                window.DialogResult = true;
            }
            else
            {
                window.DialogResult = false;
            }
            window.Close();
        }
    }
}
