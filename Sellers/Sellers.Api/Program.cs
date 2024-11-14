using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sellers.CommandModel;
using Sellers.QueryModel;

namespace Sellers;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        IServiceCollection services = builder.Services;
        services.AddDbContext<SellersDbContext>(ConfigureDbContextOptions);
        services.AddSingleton<Func<SellersDbContext>>(provider =>
        {
            return GetDbContextFactory(provider);
        });
        
        services.AddSingleton<IPasswordHasher<object>, PasswordHasher<object>>();
        services.AddSingleton<PasswordVerifier>();
        services.AddSingleton<IPasswordHasher, AspNetCorePasswordHasher>();
        
        services.AddSingleton<IUserReader, BackwardCompatibleUserReader>();
        services.AddSingleton<IUserRepository, SqlUserRepository>();
        services.AddSingleton<CreateUserCommandExecutor>();
        services.AddSingleton<GrantRoleCommandExecutor>();
        services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }

    private static void ConfigureDbContextOptions(
        IServiceProvider provider,
        DbContextOptionsBuilder options)
    {
        IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
        options.UseNpgsql(configuration.GetConnectionString("SellersDbConnection"));
    }

    private static Func<SellersDbContext> GetDbContextFactory(IServiceProvider provider)
    {
        DbContextOptionsBuilder<SellersDbContext> options = new();
        ConfigureDbContextOptions(provider, options);
        return () => new SellersDbContext(options.Options);
    }
}