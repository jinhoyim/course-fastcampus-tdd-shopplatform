using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Orders.Api.Commands;
using Orders.Api.Events;
using Orders.Domain;
using Orders.Domain.Exception;

namespace Orders.Api;

public static class OrdersRoute
{
    public static RouteGroupBuilder? MapBuild(this IEndpointRouteBuilder app)
    {
        var orderApiVersionSet = app.NewApiVersionSet("Orders")
            .HasDeprecatedApiVersion(new ApiVersion(1))
            .HasApiVersion(new ApiVersion(2))
            .ReportApiVersions()
            .Build();
        
        var orderRouteBuilder = app.MapGroup("api/v{version:apiVersion}/orders")
            .WithApiVersionSet(orderApiVersionSet);

        orderRouteBuilder.MapGet("/", GetOrdersAsync).WithName("GetOrders");
        orderRouteBuilder.MapGet("{orderId:Guid}", GetOrderAsync).WithName("GetOrder");
        orderRouteBuilder.MapPost("/", PlaceOrderAsync).WithName("CreateOrder");
        orderRouteBuilder.MapPost("{orderId:Guid}/start-order", StartOrderAsync).WithName("StartOrder");
        orderRouteBuilder.MapPost("handle/bank-transfer-payment-completed", PaymentCompletedAsync).WithName("PaymentComplete");
        orderRouteBuilder.MapPost("handle/item-shipped", ItemShippedAsync).WithName("ItemShipped");
        
        orderRouteBuilder
            .WithName("Orders API")
            .AllowAnonymous()
            .WithOpenApi();

        return orderRouteBuilder;
    }
    
    private static async Task<Ok<IEnumerable<Order>>> GetOrdersAsync(
        OrderService orderService
    )
    {
        var orders = await orderService.GetOrders();
        return TypedResults.Ok(orders.AsEnumerable());
    }
    
    private static async Task<Results<Ok<Order>, NotFound>> GetOrderAsync(
        Guid orderId,
        OrderService orderService)
    {
        try
        {
            var order = await orderService.GetOrderById(orderId);
            return TypedResults.Ok(order);
        }
        catch (OrderNotFoundException ex)
        {
            return TypedResults.NotFound();
        }
    }
    
    private static async Task<Results<Created<Order>, BadRequest<string>>> PlaceOrderAsync(
        [FromBody] PlaceOrder command,
        OrderService orderService)
    {
        if (command.UserId == Guid.Empty ||
            command.ShopId == Guid.Empty ||
            command.ItemId == Guid.Empty)
        {
            return TypedResults.BadRequest("bad request");
        }

        try
        {
            var order = await orderService.PlaceOrder(command.UserId, command.ShopId, command.ItemId, command.Price);
            return TypedResults.Created($"orders/{order.Id}", order);
        }
        catch (OrderNotFoundException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
    
    private static async Task<Results<Ok<Order>, BadRequest<string>>> StartOrderAsync(
        Guid orderId,
        [FromBody] StartOrder command,
        OrderService orderService)
    {
        try
        {
            var order = await orderService.StartOrder(orderId);
            return TypedResults.Ok(order);
        }
        catch (Exception ex)
        {
            if (ex is OrderNotFoundException or OrderProcessException)
            {
                return TypedResults.BadRequest(ex.Message);
            }
            throw;
        }
    }
    
    private static async Task<Results<Ok<Order>, BadRequest<string>>> PaymentCompletedAsync(
        [FromBody] BankTransferPaymentCompleted listenedEvent,
        OrderService orderService)
    {
        try
        {
            var order = await orderService.PaymentCompleted(listenedEvent.OrderId, listenedEvent.EventTimeUtc);
            return TypedResults.Ok(order);
        }
        catch (Exception ex)
        {
            if (ex is OrderNotFoundException or OrderProcessException)
            {
                return TypedResults.BadRequest(ex.Message);
            }
            throw;
        }
    }
    
    private static async Task<Results<Ok<Order>, BadRequest<string>>> ItemShippedAsync(
        [FromBody] ItemShipped listenedEvent,
        OrderService orderService)
    {
        try
        {
            var order = await orderService.ItemShipped(listenedEvent.OrderID);
            return TypedResults.Ok(order);
        }
        catch (Exception ex)
        {
            if (ex is OrderNotFoundException or OrderProcessException)
            {
                return TypedResults.BadRequest(ex.Message);
            }
            throw;
        }
    }
}
