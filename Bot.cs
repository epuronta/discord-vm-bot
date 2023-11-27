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

    public class StartServerCommand : ISlashCommand
    {
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
            if (await TryStartVMAsync())
            {
                await command.FollowupAsync("Server started!");
            }
            else
            {
                await command.FollowupAsync("Server failed to start!");
            };
        }

        private async Task<bool> TryStartVMAsync()
        {
            try
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(15));

                await Task.Delay(TimeSpan.FromMinutes(1), cts.Token);

                return false;
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine("Starting VM timed out");
                Console.WriteLine(e.ToString());
                return false;
            }
        }
    }

    public class BotService : IHostedService
    {
        private readonly ILogger<BotService> _log;
        private readonly BotConfig _config;
        private readonly DiscordSocketClient _client;
        private readonly IReadOnlyDictionary<string, ISlashCommand> _slashCommands;

        public BotService(
            BotConfig config,
            IEnumerable<ISlashCommand> slashCommands,
            ILogger<BotService> logger
        )
        {

            _log = logger;

            config.Validate();
            _config = config;

            _slashCommands = slashCommands.ToDictionary(
                c => c.GetCommandName(),
                c => c
            );

            _client = new DiscordSocketClient();
            _client.Log += DiscordLog;
            _client.Ready += OnClientReadyAsync;
            _client.SlashCommandExecuted += OnSlashCommandExecuted;
        }

        public async Task StartAsync(CancellationToken ct)
        {
            await _client.LoginAsync(
                TokenType.Bot,
                _config.Token
            );
            await _client.StartAsync();
        }

        public async Task StopAsync(CancellationToken ct)
        {
            await _client.DisposeAsync();
        }

        private async Task OnClientReadyAsync()
        {
            await RegisterCommandsAsync();
        }

        private async Task RegisterCommandsAsync()
        {
            foreach (var cmd in _slashCommands.Values)
            {
                _log.LogDebug("Registering {CommandName}", cmd.GetCommandName());
                await _client.CreateGlobalApplicationCommandAsync(cmd.GetBuilder().Build());
            }
        }

        private async Task OnSlashCommandExecuted(SocketSlashCommand command)
        {
            await command.DeferAsync();
            if (_slashCommands.TryGetValue(command.Data.Name, out var cmd))
            {
                await cmd.ExecuteAsync(command);
            }
            else
            {
                await command.FollowupAsync($"Unknown command: {command.Data.Name}");
            }
        }

        private Task DiscordLog(LogMessage m)
        {
            _log.Log(GetLogLevel(m.Severity), m.Exception, m.Message);
            
            return Task.CompletedTask;
        }

        /// <summary>
        /// Convert Discord.Net log severity to Microsoft.Extensions.Logging log level
        /// </summary>
        private static LogLevel GetLogLevel(LogSeverity severity)
            => (LogLevel)(Math.Abs((int)severity - 5));
    }
}
