using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Orders.Api.Events;
using Orders.Application.Commands;
using Orders.Application.Events;
using Orders.Application.Messaging;
using Orders.Domain.Exception;
using Orders.Domain.Model;

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
        orderRouteBuilder.MapPost("{orderId:Guid}/place-order", PlaceOrder).WithName("PlaceOrder");
        orderRouteBuilder.MapPost("{orderId:Guid}/start-order", StartOrder).WithName("StartOrder");
        orderRouteBuilder.MapPost("handle/bank-transfer-payment-completed", HandleBankTransferPaymentCompleted)
            .WithName("HandleBankTransferPaymentCompleted");
        orderRouteBuilder.MapPost("accept/payment-approved", AcceptPaymentApproved).WithName("AcceptPaymentApproved");
        orderRouteBuilder.MapPost("handle/item-shipped", HandleItemShipped).WithName("HandleItemShipped");

        orderRouteBuilder
            .WithName("Orders API")
            .AllowAnonymous()
            .WithOpenApi();

        return orderRouteBuilder;
    }

    private static async Task<Ok<IEnumerable<Order>>> GetOrders(
        [FromQuery(Name = "user-id")] Guid? userId,
        [FromQuery(Name = "shop-id")] Guid? shopId,
        OrderService orderService
    )
    {
        var orders = await orderService.GetOrders(userId, shopId);
        return TypedResults.Ok(orders);
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
        Guid orderId,
        [FromBody] PlaceOrder command,
        [FromServices] SellersService sellers,
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
            if (await sellers.ShopExists(command.ShopId))
            {
                var order = await orderService.PlaceOrder(
                    orderId,
                    userId: command.UserId,
                    shopId: command.ShopId,
                    itemId: command.ItemId,
                    price: command.Price);
                return TypedResults.Created($"orders/{order.Id}", order);
            }
            else
            {
                return TypedResults.BadRequest("Shop not found");
            }
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
            var order = await orderService.StartOrder(orderId, command.PaymentTransactionId);
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

    private static async Task<Results<Ok<Order>, NotFound<string>, BadRequest<string>>>
        HandleBankTransferPaymentCompleted(
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

    private static async Task<Accepted> AcceptPaymentApproved(
        [FromBody] ExternalPaymentApproved listenedEvent,
        [FromServices] IBus<PaymentApproved> bus)
    {
        PaymentApproved message = new PaymentApproved(
            listenedEvent.tid,
            listenedEvent.approved_at);

        await bus.Send(message);
        return TypedResults.Accepted("");
    }

    private static async Task<Results<Ok<Order>, NotFound<string>, BadRequest<string>>> HandleItemShipped(
        [FromBody] ItemShipped listenedEvent,
        OrderService orderService)
    {
        try
        {
            var order = await orderService.ItemShipped(listenedEvent.OrderId, listenedEvent.EventTimeUtc);
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