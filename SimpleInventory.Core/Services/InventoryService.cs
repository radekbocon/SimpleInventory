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
        private readonly MongoDbConnection _db;

        public InventoryService(MongoDbConnection db)
        {
            _db = db;
        }

        public async Task DeleteOne(ItemModel item)
        {
            var collection = _db.ConnectToMongo<ItemModel>("Items");
            await collection.DeleteOneAsync(x => x.Id == item.Id);
        }

        public async Task<ItemModel> GetByIdAsync(string id)
        {
            var collection = _db.ConnectToMongo<ItemModel>("Items");
            var item = await collection.FindAsync(x => x.Id == id);
            return item.SingleOrDefault();
        }

        public ItemModel GetById(string id)
        {
            var collection = _db.ConnectToMongo<ItemModel>("Items");
            var item = collection.Find(x => x.Id == id);
            return item.SingleOrDefault();
        }

        public ItemModel GetById(string id, IClientSessionHandle session)
        {
            var collection = _db.ConnectToMongo<ItemModel>("Items");
            var item = collection.Find(session, x => x.Id == id);
            return item.SingleOrDefault();
        }

        public async Task<List<ItemModel>> Search(string key)
        {
            var collection = _db.ConnectToMongo<ItemModel>("Items");
            var filter = Builders<ItemModel>.Filter.Regex("Name", new BsonRegularExpression("/.*" + key + ".*/i"));
            var items = await collection.Find(filter).ToListAsync();
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

        public void UpsertOne(ItemModel item)
        {
            item.Id ??= ObjectId.GenerateNewId().ToString();
            var collection = _db.ConnectToMongo<ItemModel>("Items");
            collection.ReplaceOne(x => x.Id == item.Id, item, new ReplaceOptions { IsUpsert = true });
        }

        public void UpsertOne(ItemModel item, IClientSessionHandle session)
        {
            item.Id ??= ObjectId.GenerateNewId().ToString();
            var collection = _db.ConnectToMongo<ItemModel>("Items");
            collection.ReplaceOne(session, x => x.Id == item.Id, item, new ReplaceOptions { IsUpsert = true });
        }

        public async Task UpsertOneAsync(ItemModel item)
        {
            item.Id ??= ObjectId.GenerateNewId().ToString();
            var collection = _db.ConnectToMongo<ItemModel>("Items");
            await collection.ReplaceOneAsync(x => x.Id == item.Id, item, new ReplaceOptions { IsUpsert = true });
        }
    }
}
