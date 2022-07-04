using SimpleInventory.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SimpleInventory.Wpf.Converters
{
    internal class DraftItemsToBoolConverter : IValueConverter
    {
        public object Convert(object values, Type targetType, object parameter, CultureInfo culture)
        {
            List<ItemModel> items;
            try
            {
                items = new List<ItemModel>((IEnumerable<ItemModel>)values);
            }
            catch (Exception)
            {
                return false;
            }
            if (items.Any(x => x.IsDraft))
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
