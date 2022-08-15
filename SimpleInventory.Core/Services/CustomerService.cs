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

        public async Task DeleteCustomer(CustomerModel customer)
        {
            var collection = _db.ConnectToMongo<CustomerModel>("Customers");
            await collection.DeleteOneAsync(x => x.Id == customer.Id);
        }

        public async Task<CustomerModel> GetCustomer(string id)
        {
            var collection = _db.ConnectToMongo<CustomerModel>("Customers");
            var customer = await collection.FindAsync(x => x.Id == id);
            return customer.Single();
        }

        public async Task<List<CustomerModel>> GetCustomers()
        {
            var collection = _db.ConnectToMongo<CustomerModel>("Customers");
            var customers = await collection.FindAsync(_ => true);
            return customers.ToList();
        }

        public async Task UpsertCustomer(CustomerModel customer)
        {
            customer.Id ??= ObjectId.GenerateNewId().ToString();
            var collection = _db.ConnectToMongo<CustomerModel>("Customers");
            await collection.ReplaceOneAsync(x => x.Id == customer.Id, customer, new ReplaceOptions { IsUpsert = true });
        }
    }
}
