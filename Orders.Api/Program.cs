using Microsoft.OpenApi.Models;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
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

        app.MapBuild();

        app.Run();
    }
}