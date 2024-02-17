using Discord;
using Discord.Commands;
using FortniteBot.Helpers;
using FortniteBot.Shop;
using FortniteBot.Commands.Interface;
using Newtonsoft.Json;


namespace FortniteBot.Commands
{
    [Group("shop")]
    public class ShopModule : ModuleBase<SocketCommandContext>, ICommandsInterface
    {
        private Embed builtEmbedItem;
        private Embed builtEmbedBundle;
        private Embed builtEmbedBundleItem;

        public string ApiKey { get; } = ConfigurationHelper.GetByName("Discord:API:Key");
        public string URL { get; } = ConfigurationHelper.GetByName("Discord:API:Shop:URL");

        [Command("item")]
        [Summary("Show a random item from the shop of the day")]
        public async Task ItemCommand()
        {
            var rm = new FortniteResourceManager(Context.Guild.Id.ToString());

            using HttpClient Client = new();
            try
            {
                Client.DefaultRequestHeaders.Add("Authorization", ApiKey);
                HttpResponseMessage response = await Client.GetAsync($"{URL}");

                if (response.IsSuccessStatusCode)
                {
                    string jsonresponse = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<ShopData>(jsonresponse);

                    if (responseData != null)
                    {
                        var entries = responseData.Data.Featured.Entries;
                        entries.Shuffle();

                        while (entries[0].Items == null)
                        {
                            entries.Shuffle();
                        }
                        
                        var embed = new EmbedBuilder
                        {
                            Title = $"__{entries[0].Items[0].Name}__",
                            Description = entries[0].Items[0].Description,
                            ImageUrl = entries[0].Items[0].Images.Featured,
                            Color = new Color(0, 255, 255),
                            Footer = new Discord.EmbedFooterBuilder
                            {
                                Text = entries[0].Items[0].Introduction.Text
                            }
                        };
                        
                        
                        embed.AddField("Rarity", entries[0].Items[0].Rarity.DisplayValue, inline: true);

                        builtEmbedItem = embed.Build();
                    }
                    await ReplyAsync(embed: builtEmbedItem);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{rm.GV("error")} : {ex.Message}");
            }
            
        }

        [Command("bundle")]
        [Summary("See the shop of fortnite today")]
        public async Task ShopBundleCommand()
        {
            using HttpClient Client = new();

            try
            {
                Client.DefaultRequestHeaders.Add("Authorization", ApiKey);
                HttpResponseMessage response = await Client.GetAsync($"{URL}");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    var responseData = JsonConvert.DeserializeObject<ShopData>(jsonResponse);

                    if (responseData != null)
                    {
                       
                        var entries = responseData.Data.Featured.Entries;
                        entries.Shuffle();

                        while (entries[0].Bundle == null)
                        {
                            entries.Shuffle();
                        }

                        var bundleEmbed = new EmbedBuilder
                        {
                            Title = $"__{entries[0].Bundle.Name}__",
                            ImageUrl = entries[0].Bundle.Image,
                            Description = entries[0].Bundle.Info +
                                $"\n----------------------------------------------\n",
                            Color = new Color(0, 255, 255),
                           
                        };

                        bundleEmbed.AddField("Price", $"{entries[0].RegularPrice} :moneybag:", inline: true);
                        bundleEmbed.AddField("Special Price", $"{entries[0].FinalPrice} :moneybag:", inline: true);

                        var isGiftable = entries[0].Giftable;
                        var giftableText = isGiftable.ToString();

                        if (isGiftable)
                        {
                            giftableText = isGiftable.ToString() + " :gift:";
                        }
                        bundleEmbed.AddField("Giftable", giftableText, inline: true);
                        builtEmbedBundle = bundleEmbed.Build();

                        var bundleEmbedItem = new EmbedBuilder
                        {
                            Title = $"__{entries[0].Items[0].Name}__",
                            Description = entries[0].Items[0].Description +
                                $"\n----------------------------------------------\n",
                            Color = new Color(0, 255, 255),
                            ImageUrl = entries[0].Items[0].Images.Featured,

                              Footer = new Discord.EmbedFooterBuilder
                              {
                                  Text = entries[0].Items[0].Introduction.Text

                              }

                              };
                        bundleEmbedItem.AddField("Value", $"__{entries[0].Items[0].Rarity.DisplayValue}__:crown:", inline: true);
                        builtEmbedBundleItem = bundleEmbedItem.Build();
                    }
                    await ReplyAsync(embeds: [builtEmbedBundle, builtEmbedBundleItem]);
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Une erreur s'est produite : {ex.Message}");

            }

        }

        public string Usage(FortniteResourceManager rm)
        {
            return $"**{rm.GV("usage")}** : !shop **type**\n\n" +
                    $"\t**item** : Show a random item available in the shop.\n" +
                    $"\t**bundle** : Show a random bundle from the shop.\n";
        }
    }
}
