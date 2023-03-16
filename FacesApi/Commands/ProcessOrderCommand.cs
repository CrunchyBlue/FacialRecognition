namespace FacesApi.Commands;

/// <summary>
/// The process order command.
/// </summary>
public class ProcessOrderCommand
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
    /// Gets or sets the age prediction.
    /// </summary>
    public string AgePrediction { get; set; }
}