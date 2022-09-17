using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SimpleInventory.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Core.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly MongoDbConnection _db = new MongoDbConnection();

        public async Task DeleteOne(ItemModel item)
        {
            var collection = _db.ConnectToMongo<ItemModel>("Items");
            await collection.DeleteOneAsync(x => x.Id == item.Id);
        }

        public async Task<ItemModel> GetById(string id)
        {
            var collection = _db.ConnectToMongo<ItemModel>("Items");
            var item = await collection.FindAsync(x => x.Id == id);
            return item.Single();
        }

        public async Task<List<ItemModel>> SearchByName(string name)
        {
            var collection = _db.ConnectToMongo<ItemModel>("Items");
            var items = await collection.FindAsync(x => x.Name == name);
            return items.ToList();
        }

        public async Task<List<ItemModel>> GetAll()
        {
            var collection = _db.ConnectToMongo<ItemModel>("Items");
            var items = await collection.FindAsync(_ => true);
            return items.ToList();
        }

        public async Task UpsertMany(List<ItemModel> items)
        {
            foreach (var item in items)
            {
                item.Id ??= ObjectId.GenerateNewId().ToString();
                var collection = _db.ConnectToMongo<ItemModel>("Items");
                await collection.ReplaceOneAsync(x => x.Id == item.Id, item, new ReplaceOptions { IsUpsert = true });
            }
        }

        public async Task UpsertOne(ItemModel item)
        {
            item.Id ??= ObjectId.GenerateNewId().ToString();
            var collection = _db.ConnectToMongo<ItemModel>("Items");
            await collection.ReplaceOneAsync(x => x.Id == item.Id, item, new ReplaceOptions { IsUpsert = true });
        }
    }
}
