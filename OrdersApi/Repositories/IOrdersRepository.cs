using OrdersApi.Models;

namespace OrdersApi.Repositories;

public interface IOrdersRepository
{
    public Task<Order> GetOrderAsync(Guid id);
    public Task RegisterOrder(Order order);
    public Task UpdateOrder(Order order);
}