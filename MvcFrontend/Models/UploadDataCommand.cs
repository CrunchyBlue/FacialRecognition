namespace MvcFrontend.Models;

/// <summary>
/// The upload data command.
/// </summary>
public class UploadDataCommand
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
    /// Gets or sets the photo url.
    /// </summary>
    public string PhotoUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the file.
    /// </summary>
    public IFormFile File { get; set; }
}