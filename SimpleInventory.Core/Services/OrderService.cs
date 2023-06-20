using MongoDB.Bson;
using MongoDB.Driver;
using SimpleInventory.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Core.Services
{
    public class OrderService : IOrderService
    {
        private readonly MongoDbConnection _db;
        private readonly IMongoClient _client;
        private readonly IInventoryService _inventoryService;

        public OrderService(IInventoryService inventoryService, MongoDbConnection db)
        {
            _inventoryService = inventoryService;
            _db = db;
            _client = _db.GetClient();
        }

        public async Task<List<OrderSummaryModel>> GetAll()
        {
            var collection = _db.ConnectToMongo<OrderSummaryModel>("Orders");
            var orders = await collection.Find(_ => true).ToListAsync();
            return orders.ToList();
        }

        public async Task<OrderModel> GetByNumberAsync(string id)
        {
            var collection = _db.ConnectToMongo<OrderModel>("Orders");
            var order = await collection.FindAsync(x => x.Id == id);
            return order.SingleOrDefault();
        }

        public OrderModel GetById(string id)
        {
            var collection = _db.ConnectToMongo<OrderModel>("Orders");
            var order = collection.Find(x => x.Id == id);
            return order.SingleOrDefault();
        }

        public void UpsertOne(OrderModel order)
        {
            order.Id ??= ObjectId.GenerateNewId().ToString();
            order.StartDate = order.StartDate == DateTime.MinValue ? DateTime.Now : order.StartDate;
            order.LastUpdateDate = DateTime.Now;
            ProcessInventoryTransaction(order);
            var collection = _db.ConnectToMongo<OrderModel>("Orders");
            collection.ReplaceOne(x => x.Id == order.Id, order, new ReplaceOptions { IsUpsert = true});
        }

        public async Task UpsertOneAsync(OrderModel order)
        {
            order.Id ??= ObjectId.GenerateNewId().ToString();
            order.StartDate = order.StartDate == DateTime.MinValue ? DateTime.Now : order.StartDate;
            order.LastUpdateDate = DateTime.Now;
            ProcessInventoryTransaction(order);
            var collection = _db.ConnectToMongo<OrderModel>("Orders");
            await collection.ReplaceOneAsync(x => x.Id == order.Id, order, new ReplaceOptions { IsUpsert = true } );
        }

        public async Task UpsertMany(List<OrderModel> orders)
        {
            foreach (var order in orders)
            {
                await UpsertOneAsync(order);
            }
        }

        private void ProcessInventoryTransaction(OrderModel orderNew)
        {
            if (orderNew.Lines == null || orderNew.Status == OrderStatus.Draft)
            {
                return;
            }

            try
            {
                using (var session = _client.StartSession())
                {
                    session.StartTransaction();

                    OrderModel orderOriginal = GetById(orderNew.Id);
                    if (orderOriginal != null && orderOriginal.Lines != null && orderOriginal.Status != OrderStatus.Draft)
                    {
                        foreach (var line in orderOriginal.Lines)
                        {
                            var inventoryEntry = _inventoryService.GetByItemId(line.Item.Id, session);
                            inventoryEntry.Quantity += line.Quantity;
                            inventoryEntry.LockedQuantity -= line.Quantity;
                            _inventoryService.UpsertOne(inventoryEntry, session);
                        }
                    }

                    foreach (var line in orderNew.Lines)
                    {
                        var inventoryEntry = _inventoryService.GetByItemId(line.Item.Id, session);
                        inventoryEntry.Quantity -= line.Quantity;
                        inventoryEntry.LockedQuantity += line.Quantity;
                        _inventoryService.UpsertOne(inventoryEntry, session);
                    }

                    var collection = _db.ConnectToMongo<OrderModel>("Orders");
                    collection.ReplaceOne(session, x => x.Id == orderNew.Id, orderNew, new ReplaceOptions { IsUpsert = true });

                    session.CommitTransaction();
                }
            }
            catch (StackOverflowException)
            {
                throw;
            }
        }
    }
}
