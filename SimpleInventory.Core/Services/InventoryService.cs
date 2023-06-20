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
        private readonly IMongoCollection<InventoryEntryModel> _inventoryCollection;
        private readonly IMongoCollection<ItemModel> _itemsCollection;

        public InventoryService(MongoDbConnection db)
        {
            _db = db;
            _inventoryCollection = _db.ConnectToMongo<InventoryEntryModel>("Inventory");
            _itemsCollection = _db.ConnectToMongo<ItemModel>("Items");
        }

        public async Task DeleteOne(InventoryEntryModel item)
        {
            await _inventoryCollection.DeleteOneAsync(x => x.Id == item.Id);
        }

        public async Task<InventoryEntryModel> GetByIdAsync(string id)
        {
            var item = await _inventoryCollection.FindAsync(x => x.Id == id);
            return item.SingleOrDefault();
        }

        public async Task<ItemModel> GetItemByIdAsync(string id)
        {
            var item = await _itemsCollection.FindAsync(x => x.Id == id);
            return item.SingleOrDefault();
        }

        public InventoryEntryModel GetById(string id)
        {
            var item = _inventoryCollection.Find(x => x.Id == id);
            return item.SingleOrDefault();
        }

        public InventoryEntryModel GetById(string id, IClientSessionHandle session)
        {
            var item = _inventoryCollection.Find(session, x => x.Id == id);
            return item.SingleOrDefault();
        }

        public InventoryEntryModel GetByItemId(string id)
        {
            var item = _inventoryCollection.Find(x => x.Item.Id == id);
            return item.SingleOrDefault();
        }

        public InventoryEntryModel GetByItemId(string id, IClientSessionHandle session)
        {
            var item = _inventoryCollection.Find(session, x => x.Item.Id == id);
            return item.SingleOrDefault();
        }

        public async Task<List<InventoryEntryModel>> Search(string key)
        {
            var filter = Builders<InventoryEntryModel>.Filter.Regex("Name", new BsonRegularExpression("/.*" + key + ".*/i"));
            var items = await _inventoryCollection.Find(filter).ToListAsync();
            return items.ToList();
        }

        public async Task<List<InventoryEntryModel>> GetAllEntriesAsync()
        {
            var items = await _inventoryCollection.FindAsync(_ => true);
            return items.ToList();
        }

        public async Task<List<ItemModel>> GetAllItemsAsync()
        {
            var items = await _itemsCollection.FindAsync(_ => true);
            return items.ToList();
        }

        public List<ItemModel> GetAllItems()
        {
            var items = _itemsCollection.Find(_ => true);
            return items.ToList();
        }

        public async Task UpsertMany(List<InventoryEntryModel> items)
        {
            foreach (var item in items)
            {
                item.Id ??= ObjectId.GenerateNewId().ToString();
                await _inventoryCollection.ReplaceOneAsync(x => x.Id == item.Id, item, new ReplaceOptions { IsUpsert = true });
            }
        }

        public void UpsertOne(InventoryEntryModel item)
        {
            item.Id ??= ObjectId.GenerateNewId().ToString();
            _inventoryCollection.ReplaceOne(x => x.Id == item.Id, item, new ReplaceOptions { IsUpsert = true });
        }

        public void UpsertOne(InventoryEntryModel item, IClientSessionHandle session)
        {
            item.Id ??= ObjectId.GenerateNewId().ToString();
            _inventoryCollection.ReplaceOne(session, x => x.Id == item.Id, item, new ReplaceOptions { IsUpsert = true });
        }

        public async Task UpsertOneAsync(InventoryEntryModel item)
        {
            item.Id ??= ObjectId.GenerateNewId().ToString();
            await _inventoryCollection.ReplaceOneAsync(x => x.Id == item.Id, item, new ReplaceOptions { IsUpsert = true });
        }

        public async Task UpsertOneItemAsync(ItemModel item)
        {
            item.Id ??= ObjectId.GenerateNewId().ToString();
            await _itemsCollection.ReplaceOneAsync(x => x.Id == item.Id, item, new ReplaceOptions { IsUpsert = true });
        }

        public async Task ReceiveItem(InventoryEntryModel receive)
        {
            var entry = _inventoryCollection.Find(x => x.Item == receive.Item && x.Location == receive.Location).SingleOrDefault();

            if (entry == null)
            {
                entry = receive;
                entry.Id = ObjectId.GenerateNewId().ToString();
            }
            else
            {
                entry.Quantity += receive.Quantity;
            }
            await UpsertOneAsync(entry);
        }

        public async Task MoveItemAsync(InventoryEntryModel model, string newLocation, uint quantityToMove)
        {
            var resultEntries = new List<InventoryEntryModel>();
            var entryAtOldLocation = _inventoryCollection.Find(x => x.Item == model.Item && x.Location == model.Location).SingleOrDefault();

            var entryAtNewLocation = _inventoryCollection.Find(x => x.Item == model.Item && x.Location == newLocation).SingleOrDefault();

            if (entryAtNewLocation is null)
            {
                resultEntries.Add(new InventoryEntryModel { Id = entryAtOldLocation.Id, Item = entryAtOldLocation.Item, Location = entryAtOldLocation.Location, Quantity = entryAtOldLocation.Quantity - quantityToMove });
                resultEntries.Add(new InventoryEntryModel { Id = ObjectId.GenerateNewId().ToString(), Item = entryAtOldLocation.Item, Location = newLocation, Quantity = quantityToMove });
            }
            else
            {
                resultEntries.Add(new InventoryEntryModel { Id = entryAtOldLocation.Id, Item = entryAtOldLocation.Item, Location = entryAtOldLocation.Location, Quantity = entryAtOldLocation.Quantity - quantityToMove });
                entryAtNewLocation.Quantity += quantityToMove;
                resultEntries.Add(entryAtNewLocation);
            }
            await UpsertMany(resultEntries);
            await CleanupEmptyRecordsAsync();
        }

        private async Task CleanupEmptyRecordsAsync()
        {
            var entries = await GetAllEntriesAsync();
            foreach (var entry in entries)
            {
                if (entry.Quantity == 0 && entry.LockedQuantity == 0)
                {
                    await DeleteOne(entry);
                }
            }
        }
    } 
}

