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
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();
            
            //configuration
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .Build();
            
            //api setup
            var apiBase = Configuration.GetSection("Connection:WebApi").Value;
            var user = new User();
            Configuration.GetSection(nameof(User)).Bind(user);
            
            //action
            using (var client = new HttpClient())
            {
                var result = await client.PostAsJsonAsync($"{apiBase}/Auth/Register", user);
                if (result.IsSuccessStatusCode)
                {
                    //todo: logging
                }
            }
        }
    }
}