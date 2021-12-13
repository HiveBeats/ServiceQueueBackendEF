using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace UserFiller
{
    public class Program
    {
        public static IConfiguration Configuration { get; set; }
        
        public static async Task Main(string[] args)
        {
            //todo: setup our DI
            using (var serviceProvider = new ServiceCollection()
                .AddLogging(config => config.ClearProviders().AddConsole().SetMinimumLevel(LogLevel.Trace))
                .BuildServiceProvider())
            {
                //configure console logging
                serviceProvider.GetService<ILoggerFactory>();
                
                var logger = serviceProvider.GetService<ILoggerFactory>()
                    .CreateLogger<Program>();
                
                //configuration
                Configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true, true)
                    .AddEnvironmentVariables(prefix: "FILLR")
                    .Build();
            
                //api setup
                var apiBase = Configuration.GetSection("Connection:WebApi").Value;
                var user = new User();
                Configuration.GetSection(nameof(User)).Bind(user);
                
                logger.LogInformation($"FILLR starts with: {apiBase}");
            
                //action
                using (var client = new HttpClient())
                {
                    logger.LogInformation("FILLR requesting...");
                    
                    var result = await client.PostAsJsonAsync($"{apiBase}/Auth/Register", user);
                    if (result.IsSuccessStatusCode)
                    {
                        logger.LogInformation("FILLR request unsuccessfull");
                    }
                    else logger.LogInformation("FILLR request successfull");
                }
                
                logger.LogInformation("FILLR closing.");
            }
        }
    }
}