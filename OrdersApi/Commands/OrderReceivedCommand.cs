namespace OrdersApi.Commands;

/// <summary>
/// The order received command.
/// </summary>
public class OrderReceivedCommand
{
    /// <summary>
    /// Gets or sets the order id.
    /// </summary>
    public Guid OrderId { get; set; }
    
    /// <summary>
    /// Gets or sets the photo url.
    /// </summary>
    public string PhotoUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the image data.
    /// </summary>
    public byte[] ImageData { get; set; }
    
    /// <summary>
    /// Gets or sets the user email.
    /// </summary>
    public string UserEmail { get; set; }
}