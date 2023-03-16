using OrdersApi.Models;

namespace OrdersApi.Commands;

/// <summary>
/// The order status changed to dispatched command.
/// </summary>
public class OrderStatusChangedToDispatchedCommand
{
    /// <summary>
    /// Gets or sets the order id.
    /// </summary>
    public Guid OrderId { get; set; }
    
    /// <summary>
    /// Gets or sets the dispatched date time.
    /// </summary>
    public DateTime DispatchedDateTime { get; set; }
}