using Asp.Versioning;
using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Orders.Api.Events;
using Orders.Application.Events;
using Orders.Application.Messaging;
using Orders.Domain;
using Orders.Infrastructure;
using Orders.Infrastructure.Messaging;

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
        builder.Services.AddSingleton(CreateStorageQueueBus);
        builder.Services.AddSingleton<IBus<PaymentApproved>>(GetStorageQueueBus);
        builder.Services.AddSingleton<IAsyncObservable<PaymentApproved>>(GetStorageQueueBus);
        builder.Services.AddHttpClient<SellersService>();
            
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

        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
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

        app.MapBuild();

        PaymentApprovedEventHandler.Listen(app.Services);
        
        app.Run();
    }

    private static StorageQueueBus CreateStorageQueueBus(IServiceProvider provider)
    {
        IConfiguration config = provider.GetRequiredService<IConfiguration>();
        QueueClient client = new(
            config["Storage:ConnectionString"],
            config["Storage:Queues:PaymentApproved"]);
        return new StorageQueueBus(client);
    }
    
    private static StorageQueueBus GetStorageQueueBus(IServiceProvider provider)
        => provider.GetRequiredService<StorageQueueBus>();
}