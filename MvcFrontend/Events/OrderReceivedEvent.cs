namespace MvcFrontend.Events;

/// <summary>
/// The order received event.
/// </summary>
public class OrderReceivedEvent
{
    /// <summary>
    /// Gets or sets the order id.
    /// </summary>
    private Guid OrderId { get; set; }

    /// <summary>
    /// Gets or sets the photo url.
    /// </summary>
    private string PhotoUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the user email.
    /// </summary>
    private string UserEmail { get; set; }
    
    /// <summary>
    /// Gets or sets the image data.
    /// </summary>
    private byte[] ImageData { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderReceivedEvent"/> class.
    /// </summary>
    /// <param name="orderId">
    /// The order id.
    /// </param>
    /// <param name="photoUrl">
    /// The photo url.
    /// </param>
    /// <param name="userEmail">
    /// The user email.
    /// </param>
    /// <param name="imageData">
    /// The image data.
    /// </param>
    public OrderReceivedEvent(Guid orderId, string photoUrl, string userEmail, byte[] imageData)
    {
        OrderId = orderId;
        PhotoUrl = photoUrl;
        UserEmail = userEmail;
        ImageData = imageData;
    }
}