using Microsoft.Extensions.DependencyInjection;
using SimpleInventory.Wpf.Commands;
using SimpleInventory.Wpf.Services;
using SimpleInventory.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleInventory.Wpf.Controls.Dialogs
{
    public class YesNoDialogViewModel : ViewModelBase
    {
        public string? Message { get; set; }
        public string? PositiveButtonText { get; set; }
        public string? NegativeButtonText { get; set; }

        private ICommand _dialogRelustPositiveCommand;
        private ICommand _dialogRelustNegativeCommand;
        private readonly INavigationService _navigationService;

        public ICommand DialogResultPositiveCommand
        {
            get
            {
                if (_dialogRelustPositiveCommand == null)
                {
                    _dialogRelustPositiveCommand = new RelayCommand(
                        p => DialogResultPositive(),
                        p => true);
                }

                return _dialogRelustPositiveCommand;
            }
        }

        public ICommand DialogResultNegativeCommand
        {
            get
            {
                if (_dialogRelustNegativeCommand == null)
                {
                    _dialogRelustNegativeCommand = new RelayCommand(
                        p => DialogResultNegative(),
                        p => true);
                }

                return _dialogRelustNegativeCommand;
            }
        }

        public YesNoDialogViewModel(string? message, string? positiveButtonText = "Yes", string? negativeButtonText = "No")
        {
            _navigationService = App.Current.Services.GetRequiredService<INavigationService>();
            Message = message;
            PositiveButtonText = positiveButtonText;
            NegativeButtonText = negativeButtonText;
        }

        private void DialogResultPositive()
        {
            _navigationService.DialogResult(true);
        }

        private void DialogResultNegative()
        {
            _navigationService.DialogResult(false);
        }
    }
}
