using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using FortniteBot.Helpers;

namespace FortniteBot
{
    public class FortniteBot
    {
        private static DiscordSocketClient _discordClient;
        private static CommandService _commandService;

        public static async Task Main(string[] args)
        {

            // Récupération du Token dans la configuration
            var token = ConfigurationHelper.GetByName("Discord:Bot:Token");


            // Client Discord.NET
            _discordClient = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.MessageContent | GatewayIntents.GuildMessages,
            });
            _discordClient.Log += Logger.Log;
            _discordClient.MessageReceived += HandleCommandAsync;

            // Service qui va gérer les commandes
            _commandService = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info,
                CaseSensitiveCommands = false
            });
            _commandService.Log += Logger.Log;

            // Arrêt lorsqu'on fait un Ctrl + C (déconnexion propre de Discord)
            Console.CancelKeyPress += async (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;

                await _discordClient.LogoutAsync();
                await _discordClient.StopAsync();

                await Task.Delay(1000);
                Environment.Exit(0);
            };

            // Connexion à Discord
            await _discordClient.LoginAsync(TokenType.Bot, token);
            await _discordClient.StartAsync();

            // Gestion des commandes
            //await _commandService.AddModuleAsync<StatsModule>(null);
            await _commandService.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);

            // On laisse tourner
            await Task.Delay(Timeout.Infinite);
        }

        private static async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            if (messageParam is not SocketUserMessage message) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix('!', ref argPos) ||
                message.HasMentionPrefix(_discordClient.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_discordClient, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await _commandService.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);
        }

    }
}