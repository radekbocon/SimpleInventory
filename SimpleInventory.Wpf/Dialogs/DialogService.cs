using SimpleInventory.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace SimpleInventory.Wpf.Dialogs
{
    public class DialogService : IDialogService
    {
        public void ShowDialog(string name, Action<bool> callback)
        {
            var dialog = new DialogWindow();
            var parent = App.Current.MainWindow;

            EventHandler closeEventHandler = null;
            closeEventHandler = (s, e) =>
            {
                callback(dialog.DialogResult.Value);
                dialog.Closed -= closeEventHandler;
                parent.Opacity = 1;
            };
            dialog.Closed += closeEventHandler;

            parent.Opacity = 0.8;
            dialog.Owner = parent;
            dialog.ShowInTaskbar = false;
            var type = Type.GetType($"SimpleInventory.Wpf.Dialogs.{name}");
            dialog.Content = Activator.CreateInstance(type);
            dialog.ShowDialog();
        }

        
        public void ShowDialog(ViewModelBase type, Action<bool> callback)
        {
            var dialog = new DialogWindow();
            var parent = App.Current.MainWindow;
            dialog.Height = 120;
            dialog.Width = parent.Width / 1.5;

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
            dialog.Content = type;
            dialog.Owner = parent;
            dialog.ShowDialog();
        }
    }
}
