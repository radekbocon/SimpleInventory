using SimpleInventory.Wpf.Commands;
using SimpleInventory.Wpf.Services;
using SimpleInventory.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SimpleInventory.Wpf.Controls
{
    public class NotificationViewModel : ViewModelBase
    {
        private readonly INotificationService _notificationService;

        public string Title { get; set; }
        public string Message { get; set; }
        public ICommand CloseCommand { get; set; }
        public NotificationType NotificationType { get; set; }

        public NotificationViewModel(string title, string message, INotificationService notificationService, NotificationType notificationType = NotificationType.Info)
        {
            Title = title;
            Message = message;
            NotificationType = notificationType;
            _notificationService = notificationService;
            CloseCommand = new RelayCommand(p => Close(), p => true);
        }

        private void Close()
        {
            _notificationService.Dismiss(this);
        }
    }
}
