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
    public class PickProductItemViewModel : PickItemViewModel<InventoryEntryViewModel>
    {
        private readonly IInventoryService _inventoryService;
        private readonly IMapper _mapper; 
        public override string Title => "Pick Item";
        public override string DisplayProperty => nameof(SelectedItem.DisplayProperty);

        public PickProductItemViewModel(IInventoryService inventoryService, INavigationService navigationService, IMapper mapper, Action<InventoryEntryViewModel> callback) : base(navigationService, mapper, callback)
        {
            _inventoryService = inventoryService;
            _mapper = mapper;
        }

        protected override async Task GetItems()
        {
            var list = await _inventoryService.GetAllEntriesAsync();
            var vmList = _mapper.Map<List<InventoryEntryViewModel>>(list);
            Items = new ObservableCollection<InventoryEntryViewModel>(vmList);
        }
    }
}
