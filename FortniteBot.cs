using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using FortniteBot.Helpers;

using Serilog;

namespace FortniteBot
{
    public class FortniteBot
    {
        private static DiscordSocketClient _discordClient;
        private static CommandService _commandService;

        public static async Task Main()
        {

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            // Récupération du Token dans la configuration
            var token = ConfigurationHelper.GetByName("Discord:Bot:Token");

            // Client Discord.NET
            _discordClient = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.MessageContent | GatewayIntents.GuildMessages,
            });
            _discordClient.Log += Logger.LogAsync;
            _discordClient.MessageReceived += HandleCommandAsync;

            // Service qui va gérer les commandes
            _commandService = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info,
                CaseSensitiveCommands = false
            });
            _commandService.Log += Logger.LogAsync;

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
            await _commandService.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);

            // On laisse tourner
            await Task.Delay(Timeout.Infinite);
        }

        private static async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Ne pas executer la commande si c'est un message système
            if (messageParam is not SocketUserMessage message) return;

            // Permet de tracker où le prefix termine et la commande commence
            int argPos = 0;

            // Si le message n'a pas le prefix ! ou bien le message a le prefixe @,
            // ou l'auteur du message est le bot, on ne fait rien
            if (!(message.HasCharPrefix('!', ref argPos) ||
                message.HasMentionPrefix(_discordClient.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // On créé un contexte de commande Websocket basé sur le message
            var context = new SocketCommandContext(_discordClient, message);

            // On execute la commande via le contexte et la position de la commande
            await _commandService.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);
        }

    }
}