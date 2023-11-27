using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DiscordBot
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {

            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddHostedService<BotService>();
            builder.Services.AddSingleton(new BotConfig
            {
                Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN")
                    ?? throw new ArgumentException("DISCORD_TOKEN")
            });
            builder.Services.AddSingleton<ISlashCommand, StartServerCommand>();            


            
            builder.Services.AddSingleton<TokenCredential>(new DefaultAzureCredential());

            Console.WriteLine("Bot starting");

            var host = builder.Build();

            await host.StartAsync();
            Console.WriteLine("Bot started");
            host.WaitForShutdown();
        }

    }
}
