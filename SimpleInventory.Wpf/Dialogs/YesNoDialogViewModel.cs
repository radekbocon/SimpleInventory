using SimpleInventory.Wpf.Commands;
using SimpleInventory.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleInventory.Wpf.Dialogs
{
    public class YesNoDialogViewModel : ViewModelBase
    {
        public string? Message { get; set; }
        public string? PositiveButtonText { get; set; }
        public string? NegativeButtonText { get; set; }

        private ICommand _dialogRelustPositiveCommand;
        private ICommand _dialogRelustNegativeCommand;
        private readonly IDialogService _dialogService; 

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

        public YesNoDialogViewModel(IDialogService dialogService, string? message, string? positiveButtonText = "Yes", string? negativeButtonText = "No")
        {
            Message = message;
            PositiveButtonText = positiveButtonText;
            NegativeButtonText = negativeButtonText;
            _dialogService = dialogService;
        }

        private void DialogResultPositive()
        {
            _dialogService.DialogResult(true);
        }

        private void DialogResultNegative()
        {
            _dialogService.DialogResult(false);
        }
    }
}
