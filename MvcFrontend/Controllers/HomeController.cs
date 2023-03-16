using System.Diagnostics;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using MvcFrontend.Events;
using MvcFrontend.Models;

namespace MvcFrontend.Controllers;

/// <summary>
/// The home controller.
/// </summary>
public class HomeController : Controller
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// The dapr client.
    /// </summary>
    private readonly DaprClient _daprClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeController"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger.
    /// </param>
    /// <param name="daprClient">
    /// The dapr client.
    /// </param>
    public HomeController(ILogger<HomeController> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }

    /// <summary>
    /// The upload data.
    /// </summary>
    /// <returns>
    /// The <see cref="IActionResult"/>
    /// </returns>
    [HttpGet]
    public IActionResult UploadData()
    {
        return View();
    }

    /// <summary>
    /// The upload data.
    /// </summary>
    /// <param name="model">
    /// The model.
    /// </param>
    /// <returns>
    /// The <see cref="T:Task{IActionResult}"/>
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> UploadData(UploadDataCommand model)
    {
        MemoryStream ms = new();
        await using (var uploadedFile = model.File.OpenReadStream())
        {
            await uploadedFile.CopyToAsync(ms);
        }

        var imageData = ms.ToArray();
        model.PhotoUrl = model.File.FileName;
        model.OrderId = Guid.NewGuid();
        var eventData = new OrderReceivedEvent(model.OrderId, model.PhotoUrl, model.UserEmail, imageData);

        try
        {
            await _daprClient.PublishEventAsync("eventbus", "OrderReceivedEvent", eventData);
            _logger.LogInformation("Publishing event: OrderReceivedEvent {orderId}", model.OrderId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error publishing event: OrderReceivedEvent: OrderId: {orderId}", model.OrderId);
            throw;
        }

        ViewData["OrderId"] = model.OrderId;
        return View("Thanks");
    }

    /// <summary>
    /// The index.
    /// </summary>
    /// <returns>
    /// The <see cref="IActionResult"/>
    /// </returns>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// The privacy.
    /// </summary>
    /// <returns>
    /// The <see cref="IActionResult"/>
    /// </returns>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// The error.
    /// </summary>
    /// <returns>
    /// The <see cref="IActionResult"/>
    /// </returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}