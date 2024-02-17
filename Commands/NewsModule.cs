using Discord;
using Discord.Commands;
using FortniteBot.Data.News;
using FortniteBot.Helpers;
using FortniteBot.Commands.Interface;
using Newtonsoft.Json;

namespace FortniteBot.Commands
{
    public class NewsModule : ModuleBase<SocketCommandContext>, ICommandsInterface
    {
        private Embed? builtEmbedBattleRoyale;
        private Embed? builtEmbedSaveTheWorld;

        public string ApiKey { get; } = ConfigurationHelper.GetByName("Discord:API:Key");
        public string URL { get; } = ConfigurationHelper.GetByName("Discord:API:News:URL");

        [Command("news")]
        [Summary("Get news for Fortnite")]
        public async Task NewsCommand()
        {
            var rm = new FortniteResourceManager(Context.Guild.Id.ToString());

            using HttpClient client = new();
            try
            {
                client.DefaultRequestHeaders.Add("Authorization", ApiKey);
                HttpResponseMessage response = await client.GetAsync($"{URL}?language={rm.Language.ToLower()}");

                // Vérifiez si la requête a réussi
                if (response.IsSuccessStatusCode)
                {
                    // Lecture de la réponse JSON
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<NewsData>(jsonResponse);

                    if (responseData != null)
                    {
                        if (responseData.Data.Br != null && responseData.Data.Br.Motds != null) {
                            var motds = responseData.Data.Br.Motds;
                            var description = "";

                            foreach (Motd motd in motds) { description += $"**{motd.Title}**\n_{motd.Body}_\n\n"; }

                            var embed = new EmbedBuilder
                            {
                                Title = $"__{rm.GV("newsbr")}__",
                                ImageUrl = responseData.Data.Br.Image,
                                Description = description,
                                Color = new Color(0, 255, 255),
                            };

                            builtEmbedBattleRoyale = embed.Build();
                        }

                        if (responseData.Data.Stw != null && responseData.Data.Stw.Messages != null)
                        {
                            var message = responseData.Data.Stw.Messages[0];
                            var embed = new EmbedBuilder
                            {
                                Title = $"__{rm.GV("newsstw")}__",
                                ImageUrl = message.Image,
                                Description = $"**{message.Title}**\n_{message.Body}_\n\n",
                                Color = new Color(0, 255, 255),
                            };

                            builtEmbedSaveTheWorld = embed.Build();
                        }

                    }

                    await ReplyAsync(embeds: [builtEmbedBattleRoyale, builtEmbedSaveTheWorld]);

                }
                else if ((int)response.StatusCode == 400)
                {
                    await ReplyAsync(Usage(rm));
                }
                else if ((int)response.StatusCode == 404)
                {
                    await ReplyAsync(rm.GV("emptynews"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{rm.GV("error")} : {ex.Message}");
            }
        }

        public string Usage(FortniteResourceManager rm)
        {
            return $"**{rm.GV("usage")}** : !news _br_|_stw_\n\n" +
                $"__{rm.GV("optionnal")}__ :\n" +
                $"\t**br** | **stw** : {rm.GV("newsparam")}";
        }
    }
}
