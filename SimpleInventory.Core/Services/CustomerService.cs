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
    public class CustomerService : ICustomerService
    {
        private readonly MongoDbConnection _db = new MongoDbConnection();

        public async Task DeleteOne(CustomerModel customer)
        {
            var collection = _db.ConnectToMongo<CustomerModel>("Customers");
            await collection.DeleteOneAsync(x => x.Id == customer.Id);
        }

        public async Task<CustomerModel> GetById(string id)
        {
            var collection = _db.ConnectToMongo<CustomerModel>("Customers");
            var customer = await collection.FindAsync(x => x.Id == id);
            return customer.Single();
        }

        public async Task<List<CustomerModel>> GetAll()
        {
            var collection = _db.ConnectToMongo<CustomerModel>("Customers");
            var customers = await collection.Find(_ => true).ToListAsync();
            return customers.ToList();
        }

        public async Task UpsertOne(CustomerModel customer)
        {
            customer.Id ??= ObjectId.GenerateNewId().ToString();
            var collection = _db.ConnectToMongo<CustomerModel>("Customers");
            await collection.ReplaceOneAsync(x => x.Id == customer.Id, customer, new ReplaceOptions { IsUpsert = true });
        }

        public async Task UpsertMany(List<CustomerModel> customers)
        {
            foreach (var customer in customers)
            {
                customer.Id ??= ObjectId.GenerateNewId().ToString();
                var collection = _db.ConnectToMongo<CustomerModel>("Customers");
                await collection.ReplaceOneAsync(x => x.Id == customer.Id, customer, new ReplaceOptions { IsUpsert = true }); 
            }
        }
    }
}
