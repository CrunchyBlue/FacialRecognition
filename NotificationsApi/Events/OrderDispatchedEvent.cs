namespace NotificationsApi.Events;

/// <summary>
/// The order dispatched event.
/// </summary>
public class OrderDispatchedEvent
{
    /// <summary>
    /// Gets or sets the order id.
    /// </summary>
    public Guid OrderId { get; set; }
    
    /// <summary>
    /// The dispatch date time.
    /// </summary>
    public DateTime DispatchDateTime { get; set; }
}