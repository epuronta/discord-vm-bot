using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Discord;
using Discord.WebSocket;

namespace DiscordBot
{
    public class ServerStatusCommand : ISlashCommand
    {
        public ServerStatusCommand(AzureConfig config, ArmClient azClient)
        {
            _config = config;
            _azClient = azClient;
        }
        private readonly AzureConfig _config;
        private readonly ArmClient _azClient;

        private const string NAME = "server-status";
        public string GetCommandName() => NAME;

        public SlashCommandBuilder GetBuilder()
        {
            var cb = new SlashCommandBuilder();
            cb.WithName(NAME);
            cb.WithDescription("Gets server status");

            return cb;
        }

        public async Task ExecuteAsync(SocketSlashCommand command)
        {
            Console.WriteLine("Getting server status..");
            await command.FollowupAsync("Getting server status..");
            try
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(120));

                var vmId = new ResourceIdentifier(_config.VirtualMachineId);
                var vm = _azClient.GetVirtualMachineResource(vmId);
            
                var status = new AzVmStatus((await vm.InstanceViewAsync()).Value.Statuses);
                await command.FollowupAsync($"Provisioning state: {status.ProvisioningState}\nPower state: {status.PowerState}");
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine("Getting VM status timed out");
                Console.WriteLine(e.ToString());
                await command.FollowupAsync("Getting VM status timed out");
            }
        }
    }
}
