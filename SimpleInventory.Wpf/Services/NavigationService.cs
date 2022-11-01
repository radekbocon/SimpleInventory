using SimpleInventory.Wpf.ViewModels;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Windows;

namespace SimpleInventory.Wpf.Services
{
    public class NavigationService : INavigationService
    {
        private readonly MainWindow _mainWindow;
        private Stack<ViewModelBase> _history = new Stack<ViewModelBase>();

        public NavigationService(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public void OpenPage(ViewModelBase viewModel)
        {
            if (_mainWindow == null) return;

            if (viewModel is PageViewModel)
            {
                _history.Clear();
            }

            _history.Push(viewModel);
            _mainWindow.PageContent.Content = viewModel;
        }

        public void GoBack()
        {
            _history.Pop();
            _mainWindow.PageContent.Content = _history.Peek();
        }

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

        public void ShowModal(ViewModelBase viewModel, Action<bool> callback, double modalWidth = 0)
        {
            var modal = new ModalWindow();
            var parent = Application.Current.MainWindow;
            modal.MaxWidth = modalWidth == 0 ? (parent.Width / 1.1) : modalWidth;

            EventHandler closeEventHandler = null;
            closeEventHandler = (s, e) =>
            {
                callback(modal.CallbackAction);
                modal.Closed -= closeEventHandler;
                parent.Opacity = 1;
            };
            modal.Closed += closeEventHandler;

            parent.Opacity = 0.8;
            modal.ShowInTaskbar = false;
            modal.ContentControl.Content = viewModel;
            modal.Owner = parent;
            modal.ShowDialog();
        }

        public void DialogResult(bool dialogResult)
        {
            var window = Application.Current.Windows.OfType<DialogWindow>().FirstOrDefault();
            if (window == null) return;
            window.DialogResult = dialogResult;
        }

        public void ModalResult(bool doAction)
        {
            var window = Application.Current.Windows.OfType<ModalWindow>().FirstOrDefault();
            if (window == null) return;
            window.CallbackAction = doAction;
        }
    }
}
