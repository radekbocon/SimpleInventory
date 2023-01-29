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

namespace SimpleInventory.Wpf.Controls
{
    public class NotificationViewModel : ViewModelBase
    {
        private readonly INotificationService _notificationService;
        private bool _isClosing;

        public string Title { get; set; }
        public string Message { get; set; }
        public ICommand CloseCommand { get; set; }
        public NotificationType NotificationType { get; set; }
        public bool IsClosing
        {
            get => _isClosing;
            set { SetProperty(ref _isClosing, value); }
        }

        public NotificationViewModel(string title, string message, NotificationType notificationType = NotificationType.Info)
        {
            Title = title;
            Message = message;
            NotificationType = notificationType;
            _notificationService = App.Current.Services.GetRequiredService<INotificationService>();
            CloseCommand = new RelayCommand(p => Close(), p => true);
        }

        public void Close()
        {
            _notificationService.Dismiss(this);
        }
    }
}
