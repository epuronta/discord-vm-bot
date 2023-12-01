using Azure.ResourceManager.Compute.Models;

namespace DiscordBot
{
    public class AzVmStatus
    {
        private const string PROVISIONING_STATE_PREFIX = "ProvisioningState/";
        public const string PROVISIONING_STATE_SUCCEEDED = "succeeded";
        
        private const string POWER_STATE_PREFIX = "PowerState/";
        public const string POWER_STATE_RUNNING = "running";
        public const string POWER_STATE_DEALLOCATED = "deallocated";
        
        public AzVmStatus(IReadOnlyList<InstanceViewStatus> rawStatuses)
        {
            ProvisioningState = rawStatuses
                ?.Single(s => s?.Code?.StartsWith(PROVISIONING_STATE_PREFIX) == true)
                ?.Code
                ?.Substring(PROVISIONING_STATE_PREFIX.Length) ?? throw new ArgumentException("Invalid instance status");
            
            PowerState = rawStatuses
                ?.Single(s => s?.Code?.StartsWith(POWER_STATE_PREFIX) == true)
                ?.Code
                ?.Substring(POWER_STATE_PREFIX.Length) ?? throw new ArgumentException("Invalid instance status");
        }

        public readonly string ProvisioningState;
        public readonly string PowerState;
    }
}
