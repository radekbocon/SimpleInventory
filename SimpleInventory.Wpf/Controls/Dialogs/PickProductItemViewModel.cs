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
    public class PickProductItemViewModel : PickItemViewModel<ItemModel>
    {
        private readonly IInventoryService _inventoryService;
        public override string Title => "Pick Item";
        public override string DisplayProperty => nameof(SelectedItem.Name);

        public PickProductItemViewModel(IInventoryService inventoryService, INavigationService navigationService, Action<ItemModel> callback) : base(navigationService, callback)
        {
            _inventoryService = inventoryService;
        }

        protected override async Task GetItems()
        {
            var list = await _inventoryService.GetAllItemsAsync();
            Items = new ObservableCollection<ItemModel>(list);
        }
    }
}
