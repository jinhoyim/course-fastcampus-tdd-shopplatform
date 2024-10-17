using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Collections.Specialized;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orders.Domain;
using Orders.Domain.Exception;
using Orders.Infrastructure;

namespace Orders.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        builder.Services.AddDbContext<OrdersDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("OrdersDbConnection"));
        });

        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader()
                // new QueryStringApiVersionReader("api-version"),
                // new HeaderApiVersionReader("X-Api-Version"),
                // new MediaTypeApiVersionReader("X-Version")
            );
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        builder.Services.AddScoped<OrderService>();
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Orders.Api", Version = "Version 1" });
            c.SwaggerDoc("v2", new OpenApiInfo { Title = "Orders.Api", Version = "Version 2" });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orders.Api v1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "Orders.Api v2");
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

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
        
        app.Run();
    }

    private static async Task<Results<Ok<Order>, BadRequest<string>>> ItemShippedAsync(
        Guid orderId,
        OrderService orderService)
    {
        try
        {
            var order = await orderService.ItemShipped(orderId);
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
        Guid orderId,
        OrderService orderService)
    {
        try
        {
            var order = await orderService.PaymentCompleted(orderId);
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

    private static async Task<Results<Ok<Order>, BadRequest<string>>> StartOrderAsync(
        Guid orderId,
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
        CreateOrderRequest request,
        OrderService orderService)
    {
        if (request.UserId == Guid.Empty ||
            request.ShopId == Guid.Empty ||
            request.ItemId == Guid.Empty)
        {
            return TypedResults.BadRequest("bad request");
        }
        
        var order = await orderService.PlaceOrder(request.UserId, request.ShopId, request.ItemId);
        return TypedResults.Created($"orders/{order.Id}", order);
    }

    private static async Task<Ok<IEnumerable<Order>>> GetOrdersAsync(
        OrderService orderService
    )
    {
        var orders = await orderService.GetOrders();
        return TypedResults.Ok(orders.AsEnumerable());
    }
}

public record CreateOrderRequest(
    Guid UserId,
    Guid ShopId,
    Guid ItemId
    );