using AutoMapper;
using SimpleInventory.Core.Models;
using SimpleInventory.Core.Services;
using SimpleInventory.Wpf.Services;
using SimpleInventory.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Wpf.Controls.Dialogs
{
    internal class PickCustomerViewModel : PickItemViewModel<CustomerViewModel>
    {
        private readonly ICustomerService _customerService;

        public override string Title => "Pick Customer";
        public override string DisplayProperty => nameof(SelectedItem.CompanyName);

        public PickCustomerViewModel(ICustomerService customerService, INavigationService navigationService, IMapper mapper, Action<CustomerViewModel> callback) : base(navigationService, mapper, callback)
        {
            _customerService = customerService;
        }

        protected override async Task GetItems()
        {
            var list = await _customerService.GetAll();
            var vmList = _mapper.Map<ObservableCollection<CustomerViewModel>>(list);
            Items = new ObservableCollection<CustomerViewModel>(vmList);
        }
    }
}
