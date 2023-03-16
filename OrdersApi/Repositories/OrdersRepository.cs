using Microsoft.EntityFrameworkCore;
using OrdersApi.DbContexts;
using OrdersApi.Models;

namespace OrdersApi.Repositories;

public class
    OrdersRepository : IOrdersRepository
{
    private readonly OrdersContext
        _context;

    public OrdersRepository(
        OrdersContext context)
    {
        _context = context;
    }

    public async Task<Order>
        GetOrderAsync(Guid id)
    {
        return await _context.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.OrderId == id);
    }

    public async Task RegisterOrder(
        Order order)
    {
        _context.Add(order);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateOrder(Order order)
    {
        _context.Entry(order).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}