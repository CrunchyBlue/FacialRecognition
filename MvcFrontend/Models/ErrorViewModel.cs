namespace MvcFrontend.Models;

/// <summary>
/// The error view model.
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// Gets or sets the request id.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// The show request id.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}