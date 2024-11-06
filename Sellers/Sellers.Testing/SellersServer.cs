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
    private const string ConnectionString =
        "Host=localhost;port=5432;Database=SellersDB_Testing;Username=testuser;Password=mysecret-pp#";

    private static readonly Dictionary<string,string> TestSettings = new()
    {
        { "ConnectionStrings:SellersDbConnection", ConnectionString }
    };
    
    public SellersServer(
        IServiceProvider services,
        IOptions<TestServerOptions> optionsAccessor)
        : base(services, optionsAccessor)
    {
    }

    public static SellersServer Create()
    {
        SellersServer server = (SellersServer)new Factory().Server;
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
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IServer, SellersServer>();
            });

            builder.ConfigureAppConfiguration(config =>
            {
                config.AddInMemoryCollection(TestSettings!);
            });
            return base.CreateHost(builder);
        }
    }
}