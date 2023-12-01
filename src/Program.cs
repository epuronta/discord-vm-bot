using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Azure.ResourceManager.Resources;
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
            builder.Services.AddSingleton(new AzureConfig{
                VirtualMachineId = Environment.GetEnvironmentVariable("AZURE_VIRTUAL_MACHINE_ID")
                    ?? throw new ArgumentException("AZURE_VIRTUAL_MACHINE_ID")
            });
            builder.Services.AddTransient<ISlashCommand, StartServerCommand>();
            builder.Services.AddTransient<ISlashCommand, StopServerCommand>();
            builder.Services.AddTransient<ISlashCommand, ServerStatusCommand>();
            
            // Azure credentials from default environment variables
            builder.Services.AddSingleton<TokenCredential>(new EnvironmentCredential());
            builder.Services.AddTransient<ArmClient>();

            Console.WriteLine("Bot starting");

            var host = builder.Build();

            await host.StartAsync();
            Console.WriteLine("Bot started");
            host.WaitForShutdown();
        }

    }
}
