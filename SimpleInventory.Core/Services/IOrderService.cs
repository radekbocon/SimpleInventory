using SimpleInventory.Core.Models;

namespace SimpleInventory.Core.Services
{
    public interface IOrderService
    {
        Task<List<OrderSummaryModel>> GetAll();
        Task<OrderModel> GetByNumber(string id);
        Task UpsertMany(List<OrderModel> orders);
        Task UpsertOne(OrderModel order);
    }
}