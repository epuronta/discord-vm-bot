using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiscordBot
{
    public interface ISlashCommand
    {
        public string GetCommandName();
        public SlashCommandBuilder GetBuilder();
        public Task ExecuteAsync(SocketSlashCommand command);
    }
}
