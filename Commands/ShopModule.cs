using Discord;
using Discord.Commands;
using FortniteBot.Helpers;
using FortniteBot.Shop;
using Newtonsoft.Json;


namespace FortniteBot
{
   public class ShopModule : ModuleBase<SocketCommandContext>
    {
        //Création de paramètres de type Embed
        private Embed builtEmbed;
        private Embed builtEmbed1;
        //Configuration de l'API pour que la Boutique marche 
        private readonly string apiKey = ConfigurationHelper.GetByName("Discord:API:Key");
        private readonly string URL = ConfigurationHelper.GetByName("Discord:API:Shop:URL");
        //Création d'une commande "!shop"
        [Command("shop")]
        [Summary("See the shop of fortnite today ")]
        public async Task ShopCommand()
        {
            using HttpClient Client = new();
            try
            {
                Client.DefaultRequestHeaders.Add("Authorization", apiKey);
                HttpResponseMessage response = await Client.GetAsync($"{URL}");

                if (response.IsSuccessStatusCode)
                {
                    string jsonresponse = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<ShopData>(jsonresponse);

                    if (responseData != null)
                    {   //Utilisation du shuffle pour randomiser
                        var entries = responseData.Data.Featured.Entries;
                        entries.Shuffle();

                        while (entries[0].Items == null)
                        {
                            entries.Shuffle();
                        }
                        //Création du message de type embed avec le titre, l'image ect..
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
                    }; //Ajout d'une 'un embed s'attachant a l'autre embed pour faire la rariter du personnage ( galère )
                        embed.AddField("Rarity", entries[0].Items[0].Rarity.DisplayValue, inline: true);
                        //Création de l'embed
                        builtEmbed = embed.Build();
                    }//attendre la réponse et si la commande et reçue alors il envoie l'embed 
                    await ReplyAsync(embed: builtEmbed);
                }
            }
            catch (Exception)
            {

            }
            
        }

        [Command("bundle")]
        [Summary("See the shop of fortnite today")]
        public async Task ShopBundleCommand()
        {
            using HttpClient Client = new();

            try
            {
                Client.DefaultRequestHeaders.Add("Authorization", apiKey);
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

                        var embed = new EmbedBuilder
                        {
                            Title = $"__{entries[0].Bundle.Name}__",
                            ImageUrl = entries[0].Bundle.Image,
                            Description = entries[0].Bundle.Info +
                                $"\n----------------------------------------------\n",
                            Color = new Color(0, 255, 255),
                           
                        };

                        embed.AddField("Price", $"{entries[0].RegularPrice} :moneybag:", inline: true);
                        embed.AddField("Special Price", $"{entries[0].FinalPrice} :moneybag:", inline: true);

                        var isGiftable = entries[0].Giftable;
                        var giftableText = isGiftable.ToString();

                        if (isGiftable)
                        {
                            giftableText = isGiftable.ToString() + " :gift:";
                        }
                        embed.AddField("Giftable", giftableText, inline: true);
                        builtEmbed = embed.Build();

                        var embed2 = new EmbedBuilder
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
                        embed2.AddField("Value", $"__{entries[0].Items[0].Rarity.DisplayValue}__:crown:", inline: true);
                        builtEmbed1 = embed2.Build();
                    }
                    await ReplyAsync(embeds: [builtEmbed, builtEmbed1]);
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Une erreur s'est produite : {ex.Message}");

            }

        }
    }
}
