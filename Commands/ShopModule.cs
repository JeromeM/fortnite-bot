using Discord;
using Discord.Commands;
using FortniteBot.Shop;
using Newtonsoft.Json;


namespace FortniteBot
{
   public class ShopModule : ModuleBase<SocketCommandContext>
    {
        private Embed builtEmbed;

        [Command("shop")]
        [Summary("See the shop of fortnite today")]
        public async Task ShopCommand()
        {
            string apiKey = ConfigurationHelper.GetByName("Discord:API:Key");
            string URL = ConfigurationHelper.GetByName("Discord:API:Shop:URL");

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
                        var embed = new EmbedBuilder
                        {
                            Title = $"__{responseData.Data.Featured.Entries[0].Bundle.Name}__",
                            ImageUrl = responseData.Data.Featured.Entries[0].Bundle.Image,
                            Description = responseData.Data.Featured.Entries[0].Bundle.Info +
                                $"\n----------------------------------------------\n",
                            Color = new Color(0, 255, 255),

                         
                           
                        };

                        embed.AddField("Price", $"{responseData.Data.Featured.Entries[0].RegularPrice} :moneybag:", inline: true);
                        embed.AddField("Special Price", $"{responseData.Data.Featured.Entries[0].FinalPrice} :moneybag:", inline: true);

                        //var isGiftable = responseData.Data.Featured.Entries[0].Giftable;
                        var isGiftable = false;
                        var giftableText = isGiftable.ToString();
                        if (isGiftable)
                        {
                            giftableText = isGiftable.ToString() + " :gift:";
                        }
                        embed.AddField("Giftable", giftableText, inline: true);
                        builtEmbed = embed.Build();

                    }
                    await ReplyAsync(embed: builtEmbed);


                }




            }
            catch (Exception ex)
            {

                Console.WriteLine($"Une erreur s'est produite : {ex.Message}");

            }














        }
    }
}
