namespace NotificationsApi.Models;

/// <summary>
/// The facial age prediction.
/// </summary>
public class FacialAgePrediction
{
    /// <summary>
    /// Gets or sets the face.
    /// </summary>
    public byte[] Face { get; set; }
    
    /// <summary>
    /// Gets or sets the age prediction.
    /// </summary>
    public string AgePrediction { get; set; }
}