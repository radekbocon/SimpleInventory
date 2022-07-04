using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Core.Services
{
    public class MongoDbConnection
    {
        private const string _connectionString = "mongodb+srv://simple-inventory-client:knBh2ETaGWVDrNIR@cluster0.ltt09c9.mongodb.net/?retryWrites=true&w=majority";
        private const string _databaseName = "SimpleInventoryDB";

        public IMongoCollection<T> ConnectToMongo<T>(in string collection)
        {
            var client = new MongoClient(_connectionString);
            var db = client.GetDatabase(_databaseName);
            return db.GetCollection<T>(collection);
        }   
    }
}
