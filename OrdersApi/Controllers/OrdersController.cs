using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using OrdersApi.Commands;
using OrdersApi.Enums;
using OrdersApi.Events;
using OrdersApi.Models;
using OrdersApi.Repositories;

namespace OrdersApi.Controllers;

/// <summary>
/// The order controller.
/// </summary>
[ApiController]
public class OrdersController : ControllerBase
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<OrdersController> _logger;
    
    /// <summary>
    /// The orders repository.
    /// </summary>
    private readonly IOrdersRepository _ordersRepository;
    
    /// <summary>
    /// The dapr client.
    /// </summary>
    private readonly DaprClient _daprClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrdersController"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger.
    /// </param>
    /// <param name="ordersRepository">
    /// The orders repository.
    /// </param>
    /// <param name="daprClient">
    /// The dapr client.
    /// </param>
    public OrdersController(ILogger<OrdersController> logger,
        IOrdersRepository ordersRepository, DaprClient daprClient)
    {
        _logger = logger;
        _ordersRepository = ordersRepository;
        _daprClient = daprClient;
    }

    /// <summary>
    /// The order received.
    /// </summary>
    /// <param name="command">
    /// The command.
    /// </param>
    /// <returns>
    /// The <see cref="Task{IActionResult}"/>
    /// </returns>
    [Route("OrderReceived")]
    [HttpPost]
    [Topic("eventbus", "OrderReceivedEvent")]
    public async Task<IActionResult> OrderReceived(OrderReceivedCommand command)
    {
        if (command is
            {
                PhotoUrl: { }, ImageData: { }, UserEmail: { }
            })
        {
            _logger.LogInformation(
                $"Cloud event {command.OrderId} {command.UserEmail} received");
            var img =
                await Image.LoadAsync(new MemoryStream(command.ImageData));
            await img.SaveAsync("dummy.jpg");

            var order = new Order
            {
                OrderId = command.OrderId,
                ImageData = command.ImageData,
                PhotoUrl = command.PhotoUrl,
                UserEmail = command.UserEmail,
                Status = Status.Registered,
                OrderDetails = new List<OrderDetail>()
            };

            var orderExists =
                await _ordersRepository.GetOrderAsync(order.OrderId);

            if (orderExists == null)
            {
                await _ordersRepository.RegisterOrder(order);

                var orderRegisteredEvent = new OrderRegisteredEvent
                {
                    OrderId = order.OrderId,
                    UserEmail = order.UserEmail,
                    ImageData = order.ImageData
                };

                await _daprClient.PublishEventAsync("eventbus",
                    "OrderRegisteredEvent", orderRegisteredEvent);
                _logger.LogInformation(
                    $"For {order.OrderId}, OrderRegisteredEvent published");
            }

            return Ok();
        }

        return BadRequest();
    }

    /// <summary>
    /// The order processed.
    /// </summary>
    /// <param name="command">
    /// The command.
    /// </param>
    /// <returns>
    /// The <see cref="Task{IActionResult}"/>
    /// </returns>
    [Route("orderprocessed")]
    [HttpPost]
    [Topic("eventbus", "OrderProcessedEvent")]
    public async Task<IActionResult> OrderProcessed(
        OrderStatusChangedToProcessedCommand command)
    {
        _logger.LogInformation("OrderProcessed method entered");

        if (ModelState.IsValid)
        {
            var order = await _ordersRepository.GetOrderAsync(command.OrderId);

            if (order != null)
            {
                order.Status = Status.Processed;

                var j = 1;

                foreach (var facialAgePrediction in command.FacialAgePredictions)
                {
                    var img = await Image.LoadAsync(new MemoryStream(facialAgePrediction.Face));
                    await img.SaveAsync("face" + j + ".jpg");

                    ++j;

                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        FaceData = facialAgePrediction.Face,
                        AgePrediction = facialAgePrediction.AgePrediction
                    };

                    order.OrderDetails.Add(orderDetail);
                }

                await _ordersRepository.UpdateOrder(order);
            }
        }

        return NoContent();
    }

    /// <summary>
    /// The order dispatched.
    /// </summary>
    /// <param name="command">
    /// The command.
    /// </param>
    /// <returns>
    /// The <see cref="Task{IActionResult}"/>
    /// </returns>
    [Route("orderdispatched")]
    [HttpPost]
    [Topic("eventbus", "OrderDispatchedEvent")]
    public async Task<IActionResult> OrderDispatched(OrderStatusChangedToDispatchedCommand command)
    {
        _logger.LogInformation("OrderDispatched method entered");

        if (ModelState.IsValid)
        {
            _logger.LogInformation(
                $"Order dispatched message received for OrderId: {command.OrderId}");

            var order = await _ordersRepository.GetOrderAsync(command.OrderId);

            if (order != null)
            {
                order.Status = Status.Dispatched;
                await _ordersRepository.UpdateOrder(order);
            }

            return NoContent();
        }

        return BadRequest();
    }
}