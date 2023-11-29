using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Discord;
using Discord.WebSocket;

namespace DiscordBot
{
    public class StartServerCommand : ISlashCommand
    {
        public StartServerCommand(AzureConfig config, ArmClient azClient)
        {
            _config = config;
            _azClient = azClient;
        }
        private readonly AzureConfig _config;
        private readonly ArmClient _azClient;

        private const string NAME = "start-server";
        public string GetCommandName() => NAME;

        public SlashCommandBuilder GetBuilder()
        {
            var cb = new SlashCommandBuilder();
            cb.WithName(NAME);
            cb.WithDescription("Starts the server");

            return cb;
        }

        public async Task ExecuteAsync(SocketSlashCommand command)
        {
            Console.WriteLine("Starting server..");
            await command.FollowupAsync("Starting server..");
            try
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(120));

                var vmId = new ResourceIdentifier(_config.VirtualMachineId);
                var vm = _azClient.GetVirtualMachineResource(vmId);
            
                var status = new AzVmStatus((await vm.InstanceViewAsync()).Value.Statuses);

                if(status.ProvisioningState != AzVmStatus.PROVISIONING_STATE_SUCCEEDED)
                {
                    await command.FollowupAsync($"VM is not ready. Provisioning state: {status.ProvisioningState}");
                    return;
                }
                if(status.PowerState != AzVmStatus.POWER_STATE_DEALLOCATED)
                {
                    await command.FollowupAsync($"VM is not deallocated. Power state: {status.PowerState}");
                    return;
                }

                vm.PowerOn(Azure.WaitUntil.Started, cts.Token);

                await command.FollowupAsync("VM started");
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine("Starting VM timed out");
                Console.WriteLine(e.ToString());
                await command.FollowupAsync("Starting VM timed out");
            }
        }
    }
}
