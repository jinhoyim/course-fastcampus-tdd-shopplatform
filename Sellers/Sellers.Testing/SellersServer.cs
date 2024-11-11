using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Sellers;

public sealed class SellersServer : TestServer
{
    private static readonly object s_dbMigrationLock = new();
    
    public SellersServer(
        IServiceProvider services,
        IOptions<TestServerOptions> optionsAccessor)
        : base(services, optionsAccessor)
    {
    }

    public static SellersServer Create(string connectionString = SellersDatabase.DefaultConnectionString)
    {
        SellersServer server = (SellersServer)new Factory(connectionString).Server;
        lock (s_dbMigrationLock)
        {
            using IServiceScope scope = server.Services.CreateScope();
            SellersDbContext dbContext = scope.ServiceProvider.GetRequiredService<SellersDbContext>();
            dbContext.Database.Migrate();
        }
        return server;
    }

    private sealed class Factory : WebApplicationFactory<Program>
    {
        private readonly string connectionString;

        public Factory(string connectionString) => this.connectionString = connectionString;
        
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IServer, SellersServer>();
            });

            builder.ConfigureAppConfiguration(config =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>()
                {
                    { "ConnectionStrings:SellersDbConnection", connectionString }
                }!);
            });
            return base.CreateHost(builder);
        }
    }
}