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
        private readonly MongoDbConnection _db = new MongoDbConnection();

        public async Task<List<OrderSummaryModel>> GetAll()
        {
            var collection = _db.ConnectToMongo<OrderSummaryModel>("Orders");
            var orders = await collection.Find(_ => true).ToListAsync();
            return orders.ToList();
        }

        public async Task<OrderModel> GetByNumber(string id)
        {
            var collection = _db.ConnectToMongo<OrderModel>("Orders");
            var order = await collection.FindAsync(x => x.Id == id);
            return order.Single();
        }

        public async Task UpsertOne(OrderModel order)
        {
            order.Id ??= ObjectId.GenerateNewId().ToString();
            var collection = _db.ConnectToMongo<OrderModel>("Orders");
            await collection.ReplaceOneAsync(x => x.Id == order.Id, order, new ReplaceOptions { IsUpsert = true });
        }

        public async Task UpsertMany(List<OrderModel> orders)
        {
            foreach (var order in orders)
            {
                order.Id ??= ObjectId.GenerateNewId().ToString();
                var collection = _db.ConnectToMongo<OrderModel>("Orders");
                await collection.ReplaceOneAsync(x => x.Id == order.Id, order, new ReplaceOptions { IsUpsert = true });
            }
        }
    }
}
