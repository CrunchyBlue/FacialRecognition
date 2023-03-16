using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OrdersApi.Enums;
using OrdersApi.Models;

namespace OrdersApi.DbContexts;

/// <summary>
/// The orders context.
/// </summary>
public class OrdersContext : DbContext
{
    /// <summary>
    /// Gets or sets the orders.
    /// </summary>
    public DbSet<Order> Orders { get; set; }
    
    /// <summary>
    /// Gets or sets the order details.
    /// </summary>
    public DbSet<OrderDetail> OrderDetails { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrdersContext"/> class.
    /// </summary>
    /// <param name="options">
    /// The options.
    /// </param>
    public OrdersContext(DbContextOptions<OrdersContext> options) : base(options)
    {
    }

    /// <summary>
    /// The on model creating.
    /// </summary>
    /// <param name="builder">
    /// The builder.
    /// </param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        var converter = new EnumToStringConverter<Status>();
        builder.Entity<Order>().Property(p => p.Status).HasConversion(converter);
    }
}