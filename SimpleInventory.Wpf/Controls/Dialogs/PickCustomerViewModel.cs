using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Wpf.Controls.Dialogs
{
    internal class PickCustomerViewModel : PickItemViewModel<CustomerModel>
    {
        private readonly ICustomerService _customerService;

        public override string Title => "Pick Customer";
        public override string DisplayProperty => nameof(SelectedItem.CompanyName);

        public PickCustomerViewModel(ICustomerService customerService, INavigationService navigationService, Action<CustomerModel> callback) : base(navigationService, callback)
        {
            _customerService = customerService;
        }

        protected override async Task GetItems()
        {
            var list = await _customerService.GetAll();
            Items = new ObservableCollection<CustomerModel>(list);
        }
    }
}
