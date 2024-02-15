﻿using Discord;
using Discord.Commands;
using FortniteBot.Stats;
using Newtonsoft.Json;

namespace FortniteBot.News
{
    public class NewsModule : ModuleBase<SocketCommandContext>
    {
        private Embed? builtEmbedBattleRoyale;
        private Embed? builtEmbedSaveTheWorld;

        [Command("news")]
        [Summary("Get news for Fortnite")]
        public async Task NewsCommand()
        {
            string apiKey = ConfigurationHelper.GetByName("Discord:API:Key");
            string URL = ConfigurationHelper.GetByName("Discord:API:News:URL");

            using HttpClient client = new();
            try
            {
                client.DefaultRequestHeaders.Add("Authorization", apiKey);
                HttpResponseMessage response = await client.GetAsync($"{URL}");

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
                                Title = $"__Latest Battle Royale News__",
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
                                Title = $"__Latest Save The World News__",
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
                    await ReplyAsync("Invalid or missing parameter(s)");
                }
                else if ((int)response.StatusCode == 404)
                {
                    await ReplyAsync("News are empty or do not exist");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Une erreur s'est produite : {ex.Message}");
            }
        }
    }
}
