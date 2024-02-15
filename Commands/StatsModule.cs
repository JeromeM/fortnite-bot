using Discord;
using Discord.Commands;
using FortniteBot.Helpers;
using FortniteBot.Data.Stats;
using Newtonsoft.Json;

namespace FortniteBot.Commands
{
    public class StatsModule : ModuleBase<SocketCommandContext>
    {
        private Embed? builtEmbed;

        [Command("stats")]
        [Summary("Get stats for a Fortnite account")]
        public async Task StatsCommand([Summary("Fortnite account name")] string accountName)
        {
            string apiKey = ConfigurationHelper.GetByName("Discord:API:Key");
            string URL = ConfigurationHelper.GetByName("Discord:API:Stats:URL");

            using HttpClient client = new();
            try
            {
                client.DefaultRequestHeaders.Add("Authorization", apiKey);
                HttpResponseMessage response = await client.GetAsync($"{URL}?name={accountName}");

                // Vérifiez si la requête a réussi
                if (response.IsSuccessStatusCode)
                {
                    // Lecture de la réponse JSON
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<StatsData>(jsonResponse);
                    var keyboardScore = 0;
                    var gamepadScore = 0;
                    var touchScore = 0;

                    if (responseData != null)
                    {
                        if (responseData.Data.Stats.KeyboardMouse != null)
                        {
                            keyboardScore = responseData.Data.Stats.KeyboardMouse.Overall.Score;
                        }
                        if (responseData.Data.Stats.Gamepad != null)
                        {
                            gamepadScore = responseData.Data.Stats.Gamepad.Overall.Score;
                        }
                        if (responseData.Data.Stats.Touch != null)
                        {
                            touchScore = responseData.Data.Stats.Touch.Overall.Score;
                        }

                        var preferedGamestyle = "Keyboard / Mouse";
                        var max = keyboardScore;
                        if (gamepadScore > max)
                        {
                            preferedGamestyle = "Gamepad";
                            max = gamepadScore;
                        }
                        if (touchScore > max)
                        {
                            preferedGamestyle = "Tablet / Phone";
                            max = touchScore;
                        }

                        var embed = new EmbedBuilder
                        {
                            Title = $"__Stats for account {accountName}__",
                            Description = $"- Level : **{responseData.Data.BattlePass.Level}**\n" +
                                $"- Games played : **{responseData.Data.Stats.All.Overall.Matches}**\n" +
                                $"- Prefered game style is **{preferedGamestyle}**\n" +
                                $"------------------------------------\n",
                            Color = new Color(0, 255, 255),
                        };

                        // Victories
                        embed.AddField(
                            "Victories",
                            $":crown: {responseData.Data.Stats.All.Overall.Wins}",
                            inline: true
                        );

                        // Top 3
                        embed.AddField(
                            "Top 3",
                            $":third_place: {responseData.Data.Stats.All.Overall.Top3}",
                            inline: true
                        );

                        // Top 5
                        embed.AddField(
                            "Top 5",
                            $":five: {responseData.Data.Stats.All.Overall.Top5}",
                            inline: true
                        );

                        // Kills
                        embed.AddField(
                            "Kills",
                            $":crossed_swords: {responseData.Data.Stats.All.Overall.Kills}",
                            inline: true
                        );

                        // Deaths
                        embed.AddField(
                            "Deaths",
                            $":skull_crossbones: {responseData.Data.Stats.All.Overall.Deaths}",
                            inline: true
                        );

                        // Ratio
                        embed.AddField(
                            "Ratio K/D",
                            $":nerd: {responseData.Data.Stats.All.Overall.KD}",
                            inline: true
                        );

                        builtEmbed = embed.Build();

                    }

                    await ReplyAsync(embed: builtEmbed);

                }
                else if ((int)response.StatusCode == 400)
                {
                    await ReplyAsync("Invalid or missing parameter(s)");
                }
                else if ((int)response.StatusCode == 403)
                {
                    await ReplyAsync($"Account **{accountName}** stats are private");
                }
                else if ((int)response.StatusCode == 404)
                {
                    await ReplyAsync($"Account **{accountName}** does not exist or has no stats");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Une erreur s'est produite : {ex.Message}");
            }
        }
    }
}