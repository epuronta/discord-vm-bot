using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Discord;
using Discord.WebSocket;

namespace DiscordBot
{
    public class StopServerCommand : ISlashCommand
    {
        public StopServerCommand(AzureConfig config, ArmClient azClient)
        {
            _config = config;
            _azClient = azClient;
        }
        private readonly AzureConfig _config;
        private readonly ArmClient _azClient;

        private const string NAME = "stop-server";
        public string GetCommandName() => NAME;

        public SlashCommandBuilder GetBuilder()
        {
            var cb = new SlashCommandBuilder();
            cb.WithName(NAME);
            cb.WithDescription("Stops the server");

            return cb;
        }

        public async Task ExecuteAsync(SocketSlashCommand command)
        {
            Console.WriteLine("Stopping server..");
            await command.FollowupAsync("Stopping server..");
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
                if(status.PowerState != AzVmStatus.POWER_STATE_RUNNING)
                {
                    await command.FollowupAsync($"VM is not started. Power state: {status.PowerState}");
                    return;
                }

                vm.Deallocate(Azure.WaitUntil.Completed, cancellationToken: cts.Token);

                await command.FollowupAsync("VM stopped");
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine("Stopping VM timed out");
                Console.WriteLine(e.ToString());
                await command.FollowupAsync("Stopping VM timed out");
            }
        }
    }
}
