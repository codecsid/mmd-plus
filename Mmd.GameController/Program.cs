using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using DataAccess.Data;
using DataAccess.Data.Abstract;
using DataAccess.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mmd.GameController
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    ConfigureServices(services);
                })
                .UseConsoleLifetime()
                .Build();

                host.Run();

        }

        private static bool ValidateServerCertificate(
                object sender,
                X509Certificate certificate,
                X509Chain chain,
                SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                serviceCollection.AddOptions();
                serviceCollection.Configure<AppSettings>(configuration.GetSection("AppSettings"));

                //add configured instance of logging
                serviceCollection.AddLogging(cfg => cfg.AddConsole()).Configure<LoggerFilterOptions>(cfg => cfg.MinLevel = LogLevel.Warning);

                //add services
                serviceCollection.AddScoped<HttpClient>(factory =>
                {
                    HttpClientHandler clientHandler = new HttpClientHandler();
                    clientHandler.ServerCertificateCustomValidationCallback = ValidateServerCertificate;

                    return new HttpClient(clientHandler);
                });

                serviceCollection.AddTransient<IGameInstance, GameInstance>();

                serviceCollection.AddScoped<IGameControllerService, GameControllerService>();

 
                string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION") ?? configuration.GetConnectionString("CodeCompDatabase");

                serviceCollection.AddDbContext<DataContext>(options =>
                {
                    options.UseNpgsql(connectionString);
                });

                serviceCollection.AddHostedService<App>();

            }

            catch (Exception ex)
            {
                Console.WriteLine("Error while configuring services");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
