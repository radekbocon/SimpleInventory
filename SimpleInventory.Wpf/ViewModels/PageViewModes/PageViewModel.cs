using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Wpf.ViewModels
{
    public class PageViewModel : ViewModelBase
    {
        private bool _isActive;

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                SetProperty(ref _isActive, value);
            }
        }

        private ObservableCollection<ViewModelBase> _subPages;

        public ObservableCollection<ViewModelBase> SubPages
        {
            get 
            { 
                return _subPages; 
            }
            set 
            { 
                SetProperty(ref _subPages, value); 
            }
        }

    }
}
