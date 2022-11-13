using MongoDB.Driver;
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
        public Task<List<InventoryEntryModel>> GetAllEntriesAsync();
        public Task<List<ItemModel>> GetAllItemsAsync();
        public List<ItemModel> GetAllItems();
        public Task<InventoryEntryModel> GetByIdAsync(string id);
        public InventoryEntryModel GetById(string id);
        public InventoryEntryModel GetById(string id, IClientSessionHandle session);
        public Task<List<InventoryEntryModel>> Search(string key);
        public Task UpsertMany(List<InventoryEntryModel> item);
        public Task UpsertOneAsync(InventoryEntryModel item);
        public void UpsertOne(InventoryEntryModel item);
        public void UpsertOne(InventoryEntryModel item, IClientSessionHandle session);
        public Task DeleteOne(InventoryEntryModel item);
        public Task<ItemModel> GetItemByIdAsync(string id);
        public Task UpsertOneItemAsync(ItemModel item);
        public Task ReceiveItem(InventoryEntryModel receive);
        InventoryEntryModel GetByItemId(string id);
        InventoryEntryModel GetByItemId(string id, IClientSessionHandle session);
    }
}
