using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Orders.Domain;
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
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v2", new OpenApiInfo { Title = "Orders.Api", Version = "v2" });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "Orders.Api v2");
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        
        

        var orderApi = app.MapGroup("api/orders");
        orderApi.MapGet("/", GetOrdersAsync).WithName("GetOrders");
        orderApi.MapPost("/", CreateOrderAsync).WithName("CreateOrder");
        // orderApi.MapPut("{orderId:long}", GetOrderAsync).WithName("GetOrder");
        
        orderApi
            .WithName("Orders API")
            .WithOpenApi();
        
        app.Run();
    }

    // private static async Task<Results<Ok<Order>, NotFound>> GetOrderAsync(HttpContext context)
    // {
    //     try
    //     {
    //         return TypedResults.Ok()
    //     }
    //     catch
    //     {
    //         return Results.NotFound();
    //     }
    // }

    private static async Task<Results<Ok, BadRequest<string>>> CreateOrderAsync(HttpContext context)
    {
        if ("".Equals(String.Empty))
            return TypedResults.BadRequest("bad request");

        return TypedResults.Ok();
    }

    private static async Task<Ok<IEnumerable<string>>> GetOrdersAsync()
    {
        return TypedResults.Ok(new List<string>() { "test" }.AsEnumerable());
    }
}