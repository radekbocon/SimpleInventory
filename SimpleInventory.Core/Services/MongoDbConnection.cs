using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Core.Services
{
    public class MongoDbConnection
    {
        private const string DATABASE_NAME = "SimpleInventoryDB";

        private readonly IMongoClient _client;

        public MongoDbConnection(string connectionString)
        {
            _client = new MongoClient(connectionString);
        }

        public IMongoCollection<T> ConnectToMongo<T>(in string collection)
        {
            var db = _client.GetDatabase(DATABASE_NAME);
            return db.GetCollection<T>(collection);
        }

        public IMongoClient GetClient()
        {
            return _client;  
        }
    }
}
