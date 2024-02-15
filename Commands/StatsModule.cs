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

        private string AccountName;
        private string Params;

        [Command("stats")]
        [Summary("Get stats for a Fortnite account")]
        public async Task StatsCommand(
            [Summary("Sets the account name")] string accountName="",
            [Remainder][Summary("Other params")] string prms=""
            )
        {
            AccountName = accountName;
            Params = prms;

            string apiKey = ConfigurationHelper.GetByName("Discord:API:Key");
            string URL = ConfigurationHelper.GetByName("Discord:API:Stats:URL");

            using HttpClient client = new();
            try
            {
                client.DefaultRequestHeaders.Add("Authorization", apiKey);
                HttpResponseMessage response = await client.GetAsync(GenerateUrlParams(URL));

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
                            Title = $"__Stats for account {AccountName}__",
                            Description = $"- Level : **{responseData.Data.BattlePass.Level}**\n" +
                                $"- Games played : **{responseData.Data.Stats.All.Overall.Matches}**\n" +
                                $"- Prefered game style is **{preferedGamestyle}**\n" +
                                $"------------------------------------\n",
                            ImageUrl = responseData.Data.Image,
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
                    await ReplyAsync(Usage());
                }
                else if ((int)response.StatusCode == 403)
                {
                    await ReplyAsync($"Account **{AccountName}** stats are private");
                }
                else if ((int)response.StatusCode == 404)
                {
                    await ReplyAsync($"Account **{AccountName}** does not exist or has no stats");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Une erreur s'est produite : {ex.Message}");
            }
        }

        private string GenerateUrlParams(string URL)
        {
            string[] parameters = Params.Split(" ");
            List<string> urlParams = [$"name={AccountName}"];
        

            if (parameters.Length > 0)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i] == "season" || parameters[i] == "lifetime")
                    {
                        urlParams.Add($"timeWindow={parameters[i]}");
                    }
                    if (parameters[i] == "image")
                    {
                        urlParams.Add($"image=all");
                    }
                }
            }
            return $"{URL}?{string.Join("&", urlParams)}";
        }

        private static string Usage()
        {
            return "**Usage** : !stats **playerAccount** _season_|_lifetime_ _image_\n" +
                "__Mandatory__ :\n" +
                "\t**playerAccount** : Name of the account\n\n" +
                "__Optionnal__ :\n" +
                "\t**season** | **lifetime** (default _lifetime_): Show stats from current Season, or for the all lifetime\n" +
                "\t**image** : Add an API generated image with all the stats";
        }
    }
}