using Discord;
using Discord.Commands;
using FortniteBot.Helpers;
using FortniteBot.Data.Stats;
using Newtonsoft.Json;

using static FortniteBot.FortniteResourceManager;
using System.Security.Principal;

namespace FortniteBot.Commands
{
    public class StatsModule : ModuleBase<SocketCommandContext>
    {
        private Embed? builtEmbed;

        private readonly string apiKey = ConfigurationHelper.GetByName("Discord:API:Key");
        private readonly string URL = ConfigurationHelper.GetByName("Discord:API:Stats:URL");

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

                        var preferedGamestyle = GV("kmouse");
                        var max = keyboardScore;
                        if (gamepadScore > max)
                        {
                            preferedGamestyle = GV("gamepad");
                            max = gamepadScore;
                        }
                        if (touchScore > max)
                        {
                            preferedGamestyle = GV("tphone");
                            max = touchScore;
                        }

                        var embed = new EmbedBuilder
                        {
                            Title = $"__{GV("statsfor")} {AccountName}__",
                            Description = $"- {GV("level")} : **{responseData.Data.BattlePass.Level}**\n" +
                                $"- {GV("gplayed")} : **{responseData.Data.Stats.All.Overall.Matches}**\n" +
                                $"- {GV("preferedgs")} **{preferedGamestyle}**\n" +
                                $"------------------------------------\n",
                            ImageUrl = responseData.Data.Image,
                            Color = new Color(0, 255, 255),
                        };

                        // Victories
                        embed.AddField(
                            $"{GV("victories")}",
                            $":crown: {responseData.Data.Stats.All.Overall.Wins}",
                            inline: true
                        );

                        // Top 3
                        embed.AddField(
                            $"{GV("top3")}",
                            $":third_place: {responseData.Data.Stats.All.Overall.Top3}",
                            inline: true
                        );

                        // Top 5
                        embed.AddField(
                            $"{GV("top5")}",
                            $":five: {responseData.Data.Stats.All.Overall.Top5}",
                            inline: true
                        );

                        // Kills
                        embed.AddField(
                            $"{GV("kills")}",
                            $":crossed_swords: {responseData.Data.Stats.All.Overall.Kills}",
                            inline: true
                        );

                        // Deaths
                        embed.AddField(
                            $"{GV("deaths")}",
                            $":skull_crossbones: {responseData.Data.Stats.All.Overall.Deaths}",
                            inline: true
                        );

                        // Ratio
                        embed.AddField(
                            $"{GV("ratio")}",
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
                    await ReplyAsync($"{GV("account")} **{AccountName}** {GV("pstats")}");
                }
                else if ((int)response.StatusCode == 404)
                {
                    await ReplyAsync($"{GV("account")} **{AccountName}** {GV("noexist")}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{GV("error")} : {ex.Message}");
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
            return $"**{GV("usage")}** : !stats **{GV("paccount")}** _season_|_lifetime_ _image_\n" +
                $"__{GV("mandatory")}__ :\n" +
                $"\t**{GV("paccount")}** : {GV("accname")}\n\n" +
                $"{GV("optionnal")} :\n" +
                $"\t**season** | **lifetime** ({GV("default")} _lifetime_): {GV("showstats")}\n" +
                $"\t**image** : {GV("addimage")}";
        }
    }
}