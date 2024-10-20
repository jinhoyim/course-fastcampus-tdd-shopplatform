using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Orders.Api;
using Orders.Infrastructure;

namespace Orders.UnitTests;

public class OrdersServer : TestServer
{
    private static readonly object s_dbMigrationLock = new();
    public const string ConnectionString = "Host=localhost;port=15432;Database=OrderingDB_Testing;Username=testuser;Password=mysecret-password#";
    
    public OrdersServer(
        IServiceProvider services,
        IOptions<TestServerOptions> optionsAccessor)
        : base(services, optionsAccessor)
    {
        
    }

    public static OrdersServer Create()
    {
        // TestServer.Server 프로퍼티는 IServer 를 반환한다.
        // Factory에 IServer에 싱글턴으로 OrdersServer를 등록해뒀다.
        OrdersServer server = (OrdersServer)new Factory().Server;
        // 여러 테스트가 동시에 실행될 때 테스트 서버의 디비 마이그레이션이 동시에 실행되지 않도록 한다.
        lock (s_dbMigrationLock)
        {
            using IServiceScope scope = server.Services.CreateScope();
            OrdersDbContext dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
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
                services.AddSingleton<IServer, OrdersServer>();
            });

            builder.ConfigureAppConfiguration(config =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"ConnectionStrings:OrdersDbConnection", ConnectionString}
                }!);
            });
            return base.CreateHost(builder);
        }
    }
}