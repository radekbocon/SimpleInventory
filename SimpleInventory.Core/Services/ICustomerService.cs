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
        public Task<List<CustomerModel>> GetCustomers();
        public Task<CustomerModel> GetCustomer(string id);
        public Task UpsertCustomer(CustomerModel customer); 
        public Task DeleteCustomer(CustomerModel customer);

    }
}
