using FacesApi.Models;

namespace FacesApi.Events;

/// <summary>
/// The order processed event.
/// </summary>
public class OrderProcessedEvent
{
    /// <summary>
    /// Gets or sets the order id.
    /// </summary>
    public Guid OrderId { get; set; }
    
    /// <summary>
    /// Gets or sets the user email.
    /// </summary>
    public string UserEmail { get; set; }
    
    /// <summary>
    /// Gets or sets the image data.
    /// </summary>
    public byte[] ImageData { get; set; }
    
    /// <summary>
    /// Gets or sets the facial age predictions.
    /// </summary>
    public List<FacialAgePrediction> FacialAgePredictions { get; set; }
}