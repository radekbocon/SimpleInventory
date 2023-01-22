using SimpleInventory.Core.Extentions;
using SimpleInventory.Wpf.Controls;
using SimpleInventory.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Wpf.Services
{
    public class NotificationService : ObservableBase, INotificationService
    {
        private readonly MainWindow _mainWindow;
        private ObservableCollection<NotificationViewModel> _notifications = new ObservableCollection<NotificationViewModel>();

        public ObservableCollection<NotificationViewModel> Notifications
        {
            get => _notifications;
            set { SetProperty(ref _notifications, value); }
        }

        public NotificationService(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _mainWindow.NotificationLayer.ItemsSource = Notifications;
        }

        public void Show(string title, string message, NotificationType notificationType, double durationInSeconds)
        {
            ShowAsync(title, message, notificationType, durationInSeconds).Await();
        }

        public void Dismiss(NotificationViewModel notification)
        {
            if (Notifications.Contains(notification))
            {
                Notifications.Remove(notification);
            }
        }

        private async Task ShowAsync(string title, string message, NotificationType notificationType, double durationInSeconds)
        {
            var notification = new NotificationViewModel(title, message, this, notificationType);
            Notifications.Add(notification);
            await Task.Delay(TimeSpan.FromSeconds(durationInSeconds));
            if (Notifications.Contains(notification))
            {
                Notifications.Remove(notification);
            }
        }
    }
}
