using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using NotificationsApi.Commands;
using NotificationsApi.Events;
using NotificationsApi.Utils;

namespace NotificationsApi.Controllers;

/// <summary>
/// The notifications controller.
/// </summary>
[ApiController]
public class NotificationsController : ControllerBase
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<NotificationsController> _logger;
    
    /// <summary>
    /// The dapr client.
    /// </summary>
    private readonly DaprClient _daprClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationsController"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger.
    /// </param>
    /// <param name="daprClient">
    /// The dapr client.
    /// </param>
    public NotificationsController(ILogger<NotificationsController> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }

    /// <summary>
    /// The send email.
    /// </summary>
    /// <param name="command">
    /// The command.
    /// </param>
    /// <returns>
    /// The <see cref="Task{IActionResult}"/>
    /// </returns>
    [Route("sendemail")]
    [HttpPost]
    [Topic("eventbus", "OrderProcessedEvent")]
    public async Task<IActionResult> SendEmail(DispatchOrderCommand command)
    {
        _logger.LogInformation("SendEmail method entered");
        _logger.LogInformation($"Order received for dispatch {command.OrderId}");

        var metaData = new Dictionary<string, string>()
        {
            ["emailFrom"] = "orders@facedoctor.com",
            ["emailTo"] = command.UserEmail,
            ["subject"] = $"FaceDoctor Order {command.OrderId}"
        };

        var rootFolder =
            AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin", StringComparison.Ordinal));
        var facialAgePredictions = command.FacialAgePredictions;

        if (facialAgePredictions.Any())
        {
            var j = 0;

            foreach (var facialAgePrediction in facialAgePredictions)
            {
                var img = await Image.LoadAsync(new MemoryStream(facialAgePrediction.Face));
                await img.SaveAsync(rootFolder + "/Images/face" + j + ".jpg");
                ++j;
            }
        }  
        else
        {
            _logger.LogInformation("No faces detected");
        }

        var body = EmailUtils.CreateEmailBody(command);

        await _daprClient.InvokeBindingAsync("sendmail", "create", body, metaData);

        var eventMessage = new OrderDispatchedEvent()
        {
            OrderId = command.OrderId,
            DispatchDateTime = DateTime.UtcNow
        };

        await _daprClient.PublishEventAsync<OrderDispatchedEvent>("eventbus",
            "OrderDispatchedEvent", eventMessage);
        _logger.LogInformation($"Order dispatched for OrderId {eventMessage.OrderId} at {eventMessage.DispatchDateTime}");

        return Ok();
    }
}