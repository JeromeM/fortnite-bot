using Discord;
using Discord.Commands;
using FortniteBot.Helpers;
using FortniteBot.Data.Stats;
using FortniteBot.Commands.Interface;
using Newtonsoft.Json;

namespace FortniteBot.Commands
{
    public class StatsModule : ModuleBase<SocketCommandContext>, ICommandsInterface
    {
        private Embed? builtEmbed;

        public string ApiKey { get; } = ConfigurationHelper.GetByName("Discord:API:Key");
        public string URL { get; } = ConfigurationHelper.GetByName("Discord:API:Stats:URL");

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

            var rm = new FortniteResourceManager(Context.Guild.Id.ToString());

            using HttpClient client = new();
            try
            {
                client.DefaultRequestHeaders.Add("Authorization", ApiKey);
                HttpResponseMessage response = await client.GetAsync(GenerateUrlParams(URL, rm.Language));

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

                        var preferedGamestyle = rm.GV("kmouse");
                        var max = keyboardScore;
                        if (gamepadScore > max)
                        {
                            preferedGamestyle = rm.GV("gamepad");
                            max = gamepadScore;
                        }
                        if (touchScore > max)
                        {
                            preferedGamestyle = rm.GV("tphone");
                            max = touchScore;
                        }

                        var embed = new EmbedBuilder
                        {
                            Title = $"__{rm.GV("statsfor")} {AccountName}__",
                            Description = $"- {rm.GV("level")} : **{responseData.Data.BattlePass.Level}**\n" +
                                $"- {rm.GV("gplayed")} : **{responseData.Data.Stats.All.Overall.Matches}**\n" +
                                $"- {rm.GV("preferedgs")} **{preferedGamestyle}**\n" +
                                $"------------------------------------\n",
                            ImageUrl = responseData.Data.Image,
                            Color = new Color(0, 255, 255),
                        };

                        // Victories
                        embed.AddField(
                            $"{rm.GV("victories")}",
                            $":crown: {responseData.Data.Stats.All.Overall.Wins}",
                            inline: true
                        );

                        // Top 3
                        embed.AddField(
                            $"{rm.GV("top3")}",
                            $":third_place: {responseData.Data.Stats.All.Overall.Top3}",
                            inline: true
                        );

                        // Top 5
                        embed.AddField(
                            $"{rm.GV("top5")}",
                            $":five: {responseData.Data.Stats.All.Overall.Top5}",
                            inline: true
                        );

                        // Kills
                        embed.AddField(
                            $"{rm.GV("kills")}",
                            $":crossed_swords: {responseData.Data.Stats.All.Overall.Kills}",
                            inline: true
                        );

                        // Deaths
                        embed.AddField(
                            $"{rm.GV("deaths")}",
                            $":skull_crossbones: {responseData.Data.Stats.All.Overall.Deaths}",
                            inline: true
                        );

                        // Ratio
                        embed.AddField(
                            $"{rm.GV("ratio")}",
                            $":nerd: {responseData.Data.Stats.All.Overall.KD}",
                            inline: true
                        );

                        builtEmbed = embed.Build();

                    }

                    await ReplyAsync(embed: builtEmbed);

                }
                else if ((int)response.StatusCode == 400)
                {
                    await ReplyAsync(Usage(rm));
                }
                else if ((int)response.StatusCode == 403)
                {
                    await ReplyAsync($"{rm.GV("account")} **{AccountName}** {rm.GV("pstats")}");
                }
                else if ((int)response.StatusCode == 404)
                {
                    await ReplyAsync($"{rm.GV("account")} **{AccountName}** {rm.GV("noexist")}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{rm.GV("error")} : {ex.Message}");
            }
        }

        private string GenerateUrlParams(string URL, string language)
        {
            string[] parameters = Params.Split(" ");
            List<string> urlParams = [$"name={AccountName}", $"language={language.ToLower()}"];

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

        public string Usage(FortniteResourceManager rm)
        {
            return $"**{rm.GV("usage")}** : !stats **{rm.GV("paccount")}** _season_|_lifetime_ _image_\n\n" +
                $"__{rm.GV("mandatory")}__ :\n" +
                $"\t**{rm.GV("paccount")}** : {rm.GV("accname")}\n\n" +
                $"__{rm.GV("optionnal")}__ :\n" +
                $"\t**season** | **lifetime** ({rm.GV("default")} _lifetime_): {rm.GV("showstats")}\n" +
                $"\t**image** : {rm.GV("addimage")}";
        }
    }
}