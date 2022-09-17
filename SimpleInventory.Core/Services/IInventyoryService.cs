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
        public Task<ItemModel> GetById(string id);
        public Task<List<ItemModel>> SearchByName(string name);
        public Task UpsertMany(List<ItemModel> item);
        public Task UpsertOne(ItemModel item);
        public Task DeleteOne(ItemModel item);
    }
}
