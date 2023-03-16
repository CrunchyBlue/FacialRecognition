namespace FacesApi.Config;

/// <summary>
/// The aws credentials configuration.
/// </summary>
public class AwsCredentialsConfiguration
{
    /// <summary>
    /// Gets or sets the access key.
    /// </summary>
    public string AccessKey { get; set; }
    
    /// <summary>
    /// Gets or sets the secret access key.
    /// </summary>
    public string SecretAccessKey { get; set; }
}