using MongoDB.Driver;
using SimpleInventory.Core.Models;

namespace SimpleInventory.Core.Services
{
    public interface IOrderService
    {
        Task<List<OrderSummaryModel>> GetAll();
        Task<OrderModel> GetByNumberAsync(string id);
        OrderModel GetByNumber(string id);
        Task UpsertMany(List<OrderModel> orders);
        Task UpsertOneAsync(OrderModel order);
        void UpsertOne(OrderModel order);
    }
}