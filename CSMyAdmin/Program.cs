using CSMyAdmin.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CSMyAdmin;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var dbType = hostContext.Configuration.GetValue<string>("DbKey") ?? "sqlite";
                
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    switch (dbType.ToLower())
                    {
                        case "postgres":
                        case "postgresql":
                            options.UseNpgsql(GetConnectionString(hostContext, "PostgreSQL"));
                            break;
                        case "mysql":
                        case "mariadb":
                            options.UseM(GetConnectionString(hostContext, "MySQL")); 
                            break;
                        default:
                            options.UseSqlite(GetConnectionString(hostContext, "Sqlite"));
                            break;
                    }
                });

                services.AddSingleton<App>();
            })
            .Build();
        
        var app = host.Services.GetRequiredService<App>();
        await app.Run();
    }

    private static string GetConnectionString(HostBuilderContext hostContext, string dbName)
    {
        var connectionString = hostContext.Configuration.GetConnectionString(dbName);
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new Exception($"Connection string for {dbName} is missing");
        }
        
        return connectionString;
    }
}