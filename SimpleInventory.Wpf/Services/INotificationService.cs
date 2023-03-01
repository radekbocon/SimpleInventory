using SimpleInventory.Wpf.Controls;
using System;
using System.Threading.Tasks;

namespace SimpleInventory.Wpf.Services
{
    public interface INotificationService
    {
        private const double DEFAULT_DURATION_IN_SECONDS = 3;

        void Dismiss(NotificationViewModel notification);
        void Show(string title, string message, NotificationType notificationType, double durationInSeconds = DEFAULT_DURATION_IN_SECONDS);
    }
}