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
        public Task<List<ItemModel>> GetAll();
        public Task<ItemModel> GetByIdAsync(string id);
        public ItemModel GetById(string id);
        public ItemModel GetById(string id, IClientSessionHandle session);
        public Task<List<ItemModel>> Search(string key);
        public Task UpsertMany(List<ItemModel> item);
        public Task UpsertOneAsync(ItemModel item);
        public void UpsertOne(ItemModel item);
        public void UpsertOne(ItemModel item, IClientSessionHandle session);
        public Task DeleteOne(ItemModel item);
    }
}
