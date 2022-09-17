using SimpleInventory.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleInventory.Core.Services
{
    public interface ICustomerService
    {
        public Task<List<CustomerModel>> GetAll();
        public Task<CustomerModel> GetById(string id);
        public Task UpsertOne(CustomerModel customer); 
        public Task UpsertMany(List<CustomerModel> customers);
        public Task DeleteOne(CustomerModel customer);

    }
}
