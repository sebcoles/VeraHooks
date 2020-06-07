using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;
using VeracodeService;
using VeracodeService.Repositories;
using VeracodeWebhooks.Configuration;
using WebhookLogic;
using WebhookLogic.Configuration;

namespace WebhookRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
#if DEBUG
                .AddJsonFile($"appsettings.Development.json", false)
#else
                .AddJsonFile("appsettings.json", false)
#endif
                .Build();

            var name = Configuration.GetSection("Context").GetValue(typeof(string), "Name");
            Console.WriteLine($"Starting agent {name}...");

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDbContext<ApplicationDbContext>
                (options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            serviceCollection.Configure<VeracodeConfiguration>(options => Configuration.GetSection("Veracode").Bind(options));
            serviceCollection.Configure<TimerConfiguration>(options => Configuration.GetSection("Timer").Bind(options));
            serviceCollection.Configure<ContextConfiguration>(options => Configuration.GetSection("Context").Bind(options));
            serviceCollection.Configure<AgentConfiguration>(options => Configuration.GetSection("Agent").Bind(options));
            serviceCollection.AddScoped<IVeracodeWrapper, VeracodeWrapper>();
            serviceCollection.AddScoped<IVeracodeRepository, VeracodeRepository>();
            serviceCollection.AddScoped<IGenericRepository<MitigationWebhook>, WebhookRepository>();
            serviceCollection.AddScoped<IGenericRepository<WebhookLog>, GenericRepository<WebhookLog>>();
            serviceCollection.AddScoped<IHttpPostService, HttpPostService>();
            serviceCollection.AddScoped<IWebhookHandler, WebhookHandler>();
            serviceCollection.AddScoped<ITimerService, TimerService>();
            serviceCollection.AddScoped<IStatusService, StatusService>();           

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var service = serviceProvider.GetService<ITimerService>();     
            Task.Factory.StartNew(() => service.GenerateCallHomeTimer());
            service.GenerateTimer();
        }
    }
}
