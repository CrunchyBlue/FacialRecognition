namespace OrdersApi.Events;

/// <summary>
/// The order registered event.
/// </summary>
public class OrderRegisteredEvent
{
    /// <summary>
    /// Gets or sets the order id.
    /// </summary>
    public Guid OrderId { get; set; }
    
    /// <summary>
    /// Gets or sets the image data.
    /// </summary>
    public byte[] ImageData { get; set; }
    
    /// <summary>
    /// Gets or sets the user email.
    /// </summary>
    public string UserEmail { get; set; }
}