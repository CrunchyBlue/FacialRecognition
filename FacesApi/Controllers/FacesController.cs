using Amazon;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Dapr;
using Dapr.Client;
using FacesApi.Commands;
using FacesApi.Config;
using FacesApi.Events;
using FacesApi.Models;
using Microsoft.AspNetCore.Mvc;
using Image = SixLabors.ImageSharp.Image;

namespace FacesApi.Controllers;

/// <summary>
/// The faces controller.
/// </summary>
[ApiController]
public class FacesController : ControllerBase
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<FacesController> _logger;
    
    /// <summary>
    /// The dapr client.
    /// </summary>
    private readonly DaprClient _daprClient;
    
    /// <summary>
    /// The config.
    /// </summary>
    private readonly AwsCredentialsConfiguration _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="FacesController"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger.
    /// </param>
    /// <param name="daprClient">
    /// The dapr client.
    /// </param>
    /// <param name="config">
    /// The config.
    /// </param>
    public FacesController(ILogger<FacesController> logger,
        DaprClient daprClient, AwsCredentialsConfiguration config)
    {
        _logger = logger;
        _daprClient = daprClient;
        _config = config;
    }

    /// <summary>
    /// The process order.
    /// </summary>
    /// <param name="command">
    /// The command.
    /// </param>
    /// <returns>
    /// The <see cref="Task{IActionResult}"/>
    /// </returns>
    [Route("processorder")]
    [HttpPost]
    [Topic("eventbus", "OrderRegisteredEvent")]
    public async Task<IActionResult> ProcessOrder(ProcessOrderCommand command)
    {
        _logger.LogInformation("ProcessOrder method entered...");

        if (ModelState.IsValid)
        {
            _logger.LogInformation($"Command params: {command.OrderId}");
            var img = await Image.LoadAsync(new MemoryStream(command.ImageData));
            await img.SaveAsync("dummy.jpg");
            var orderState =
                await _daprClient.GetStateEntryAsync<List<ProcessOrderCommand>>(
                    "redisstore", "orderList");
            var orderList = new List<ProcessOrderCommand>();

            if (orderState.Value == null)
            {
                _logger.LogInformation("OrderState Case 1");
                orderList.Add(command);
                await _daprClient.SaveStateAsync("redisstore", "orderList",
                    orderList);
            }
            else
            {
                _logger.LogInformation("OrderState Case 2");
                orderList = orderState.Value;
                orderList.Add(command);
                await _daprClient.SaveStateAsync("redisstore", "orderList",
                    orderList);
            }
        }

        return Ok();
    }

    /// <summary>
    /// The cron.
    /// </summary>
    /// <returns>
    /// The <see cref="Task{IActionResult}"/>
    /// </returns>
    [HttpPost("cron")]
    public async Task<IActionResult> Cron()
    {
        _logger.LogInformation("Cron method entered...");

        var orderState =
            await _daprClient.GetStateEntryAsync<List<ProcessOrderCommand>>(
                "redisstore", "orderList");

        if (orderState?.Value?.Count > 0)
        {
            _logger.LogInformation(
                $"Count value of the orders in the store {orderState.Value.Count}");

            var orderList = orderState.Value;
            var firstOrder = orderList[0];

            if (firstOrder != null)
            {
                _logger.LogInformation(
                    $"First order's OrderId: {firstOrder.OrderId}");

                var imageBytes = firstOrder.ImageData.ToArray();
                var imageStream = new MemoryStream(imageBytes);
                var img = await Image.LoadAsync(imageStream);
                await img.SaveAsync("dummy1.jpg");
                var facialAgePredictions =
                    await UploadPhotoAndDetectFaces(img, imageStream,
                        new MemoryStream(imageBytes));
                var orderProcessedEvent = new OrderProcessedEvent
                {
                    OrderId = firstOrder.OrderId,
                    UserEmail = firstOrder.UserEmail,
                    ImageData = firstOrder.ImageData,
                    FacialAgePredictions = facialAgePredictions
                };

                await _daprClient.PublishEventAsync("eventbus",
                    "OrderProcessedEvent", orderProcessedEvent);
                orderList.Remove(firstOrder);
                await _daprClient.SaveStateAsync("redisstore", "orderList",
                    orderList);
                _logger.LogInformation(
                    $"Order count after processing: {orderList.Count}");
                return Ok();
            }
        }

        return NoContent();
    }

    /// <summary>
    /// The upload photo and detect faces.
    /// </summary>
    /// <param name="img">
    /// The img.
    /// </param>
    /// <param name="imageBytes">
    /// The image bytes.
    /// </param>
    /// <param name="imageStream">
    /// The image stream.
    /// </param>
    /// <returns>
    /// The <see cref="T:Task{List{FacialAgePrediction}}"/>
    /// </returns>
    private async Task<List<FacialAgePrediction>> UploadPhotoAndDetectFaces(Image img, MemoryStream imageBytes,
        MemoryStream imageStream)
    {
        var accessKey = _config.AccessKey;
        var secretAccessKey = _config.SecretAccessKey;
        
        var rekognitionClient = Authenticate(accessKey, secretAccessKey);

        var image = new Amazon.Rekognition.Model.Image()
        {
            Bytes = imageBytes
        };

        var detectFacesRequest = new DetectFacesRequest()
        {
            Image = image,
            Attributes = new List<string>() { "ALL" }
        };
        
        var faceList = new List<FacialAgePrediction>();

        try
        {
            var detectFacesResponse = await rekognitionClient.DetectFacesAsync(detectFacesRequest);
            var hasAll = detectFacesRequest.Attributes.Contains("ALL");
            
            var j = 0;
            
            foreach (var face in detectFacesResponse.FaceDetails)
            {
                Console.WriteLine($"BoundingBox: top={face.BoundingBox.Left} left={face.BoundingBox.Top} width={face.BoundingBox.Width} height={face.BoundingBox.Height}");
                Console.WriteLine($"Confidence: {face.Confidence}");
                Console.WriteLine($"Landmarks: {face.Landmarks.Count}");
                Console.WriteLine($"Pose: pitch={face.Pose.Pitch} roll={face.Pose.Roll} yaw={face.Pose.Yaw}");
                Console.WriteLine($"Brightness: {face.Quality.Brightness}\tSharpness: {face.Quality.Sharpness}");

                if (hasAll)
                {
                    Console.WriteLine($"Estimated age is between {face.AgeRange.Low} and {face.AgeRange.High} years old.");
                }
                

                var h = (int)(face.BoundingBox.Height * img.Height);
                var w = (int)(face.BoundingBox.Width * img.Width);
                var x = (int)(face.BoundingBox.Left * img.Width);
                var y = (int)(face.BoundingBox.Top * img.Height);
            
                await img.Clone(context =>
                        context.Crop(new Rectangle(x, y, w, h)))
                    .SaveAsync("face" + j + ".jpg");
            
                var memoryStream = new MemoryStream();
                await img.Clone(context =>
                        context.Crop(new Rectangle(x, y, w, h)))
                    .SaveAsJpegAsync(memoryStream);
            
                faceList.Add(new FacialAgePrediction()
                {
                    Face = memoryStream.ToArray(),
                    AgePrediction = $"Estimated age is between {face.AgeRange.Low} and {face.AgeRange.High} years old."
                });
                ++j;
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"Error: {e.Message}");
            throw;
        }

        return faceList;
    }

    /// <summary>
    /// The authenticate.
    /// </summary>
    /// <param name="accessKey">
    /// The access key.
    /// </param>
    /// <param name="secretAccessKey">
    /// The secret access key.
    /// </param>
    /// <returns>
    /// The <see cref="AmazonRekognitionClient"/>
    /// </returns>
    private AmazonRekognitionClient Authenticate(string accessKey, string secretAccessKey)
    {
        return new AmazonRekognitionClient(accessKey, secretAccessKey, RegionEndpoint.USEast1);
        
    }
}