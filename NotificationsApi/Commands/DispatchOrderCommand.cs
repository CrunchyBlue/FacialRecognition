using NotificationsApi.Models;

namespace NotificationsApi.Commands;

/// <summary>
/// The dispatch order command.
/// </summary>
public class DispatchOrderCommand
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