using OrdersApi.Models;

namespace OrdersApi.Commands;

/// <summary>
/// The order status changed to processed command.
/// </summary>
public class OrderStatusChangedToProcessedCommand
{
    /// <summary>
    /// Gets or sets the order id.
    /// </summary>
    public Guid OrderId { get; set; }
    
    /// <summary>
    /// Gets or sets the facial age predictions.
    /// </summary>
    public List<FacialAgePrediction> FacialAgePredictions { get; set; }
}