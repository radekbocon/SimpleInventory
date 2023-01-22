using SimpleInventory.Wpf.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SimpleInventory.Wpf.Converters
{
    public class NotificationTypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not NotificationType)
            {
                return new DrawingImage();
            }
            switch ((NotificationType)value)
            {
                case NotificationType.Info: return Application.Current.Resources["InfoIcon"] as DrawingImage;
                case NotificationType.Error: return Application.Current.Resources["ErrorIcon"] as DrawingImage;
                case NotificationType.Warning: return Application.Current.Resources["WarningIcon"] as DrawingImage;
                default: return Application.Current.Resources["InfoIcon"] as DrawingImage;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
