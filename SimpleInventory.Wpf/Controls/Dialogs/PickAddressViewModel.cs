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
    public class PickAddressViewModel : PickItemViewModel<AddressModel>
    {
        private readonly ICustomerService _customerService;
        private string _id;

        public override string Title => "Pick Address";
        public override string DisplayProperty => nameof(SelectedItem.Line1);

        public PickAddressViewModel(string id, ICustomerService customerService, INavigationService navigationService, Action<AddressModel> callback) : base(navigationService, callback)
        {
            _customerService = customerService;
            _id = id;
        }

        protected override async Task GetItems()
        {
            if (_id == null) return;
            
            var customer = await _customerService.GetById(_id);
            Items = new ObservableCollection<AddressModel>(customer.Addresses);
        }
    }
}
