using Discord;
using Discord.WebSocket;
using FortniteBot.Helpers;

namespace FortniteBot;

public class FortniteBot
{
	private static DiscordSocketClient? discordClient;

	public static async Task Main()
	{

		var token = ConfigurationHelper
			.GetByName("Discord:Bot:Token");


		discordClient = new DiscordSocketClient();
		discordClient.Log += Log;

		Console.CancelKeyPress += async (sender, eventArgs) =>
        {
            eventArgs.Cancel = true;
			
			await discordClient.LogoutAsync();
            await discordClient.StopAsync();

			await Task.Delay(1000);
            Environment.Exit(0);
        };

		await discordClient.LoginAsync(TokenType.Bot, token);
		await discordClient.StartAsync();

		await Task.Delay(-1);
	}

	private static Task Log(LogMessage msg)
	{
		Console.WriteLine(msg.ToString());
		return Task.CompletedTask;
	}
}
