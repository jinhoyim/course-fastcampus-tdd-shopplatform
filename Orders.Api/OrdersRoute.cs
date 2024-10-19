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

        orderRouteBuilder.MapGet("/", GetOrders).WithName("GetOrders");
        orderRouteBuilder.MapGet("{orderId:Guid}", FindOrder).WithName("FindOrder");
        orderRouteBuilder.MapPost("/", PlaceOrder).WithName("PlaceOrder");
        orderRouteBuilder.MapPost("{orderId:Guid}/start-order", StartOrder).WithName("StartOrder");
        orderRouteBuilder.MapPost("handle/bank-transfer-payment-completed", HandleBankTransferPaymentCompleted)
            .WithName("HandleBankTransferPaymentCompleted");
        orderRouteBuilder.MapPost("handle/item-shipped", HandleItemShipped).WithName("HandleItemShipped");
        
        orderRouteBuilder
            .WithName("Orders API")
            .AllowAnonymous()
            .WithOpenApi();

        return orderRouteBuilder;
    }
    
    private static async Task<Ok<IEnumerable<Order>>> GetOrders(
        OrderService orderService
    )
    {
        var orders = await orderService.GetOrders();
        return TypedResults.Ok(orders.AsEnumerable());
    }
    
    private static async Task<Results<Ok<Order>, NotFound>> FindOrder(
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
    
    private static async Task<Results<Created<Order>, BadRequest<string>>> PlaceOrder(
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
        catch (InvalidOrderException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
    
    private static async Task<Results<Ok<Order>, NotFound<string>, BadRequest<string>>> StartOrder(
        Guid orderId,
        [FromBody] StartOrder command,
        OrderService orderService)
    {
        try
        {
            var order = await orderService.StartOrder(orderId);
            return TypedResults.Ok(order);
        }
        catch (OrderNotFoundException ex)
        {
            return TypedResults.NotFound(ex.Message);
        }
        catch (OrderProcessException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
    
    private static async Task<Results<Ok<Order>, NotFound<string>, BadRequest<string>>> HandleBankTransferPaymentCompleted(
        [FromBody] BankTransferPaymentCompleted listenedEvent,
        OrderService orderService)
    {
        try
        {
            var order = await orderService.PaymentCompleted(listenedEvent.OrderId, listenedEvent.EventTimeUtc);
            return TypedResults.Ok(order);
        }
        catch (OrderNotFoundException ex)
        {
            return TypedResults.NotFound(ex.Message);
        }
        catch (OrderProcessException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
    
    private static async Task<Results<Ok<Order>, NotFound<string>, BadRequest<string>>> HandleItemShipped(
        [FromBody] ItemShipped listenedEvent,
        OrderService orderService)
    {
        try
        {
            var order = await orderService.ItemShipped(listenedEvent.OrderID, listenedEvent.EventTimeUtc);
            return TypedResults.Ok(order);
        }
        catch (OrderNotFoundException ex)
        {
            return TypedResults.NotFound(ex.Message);
        }
        catch (OrderProcessException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}
