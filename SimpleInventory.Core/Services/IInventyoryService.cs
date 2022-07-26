using SimpleInventory.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Core.Services
{
    public interface IInventoryService
    {
        public Task<List<ItemModel>> GetInventyoryItems();
        public Task<ItemModel> GetInventyoryItemById(string id);
        public Task<List<ItemModel>> GetInventyoryItemByName(string name);
        public Task UpsertInventoryItems(List<ItemModel> item);
        public Task UpsertInventoryItem(ItemModel item);
        public Task DeleteInventoryItem(ItemModel item);
    }
}
