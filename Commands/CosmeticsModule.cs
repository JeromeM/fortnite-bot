using Discord;
using Discord.Commands;
using FortniteBot.Data.Cosmetics;
using FortniteBot.Helpers;
using Newtonsoft.Json;



namespace Fortnite_Bot.Commands
{
    public class CosmeticsModule : ModuleBase<SocketCommandContext>
    {   //Creéation de a fonction embed et name 
        private Embed BuildEmbed;
        private string Name;
        //Se connecte a l'API de fortnite API
        private readonly string apiKey = ConfigurationHelper.GetByName("Discord:API:Key");
        private readonly string URL = ConfigurationHelper.GetByName("Discord:API:Cosmetic:URL");
        //Création de la command 
        [Command("search")]
        [Summary("Search a Skin")]
        public async Task SearchCommand(
           [Remainder][Summary("Search terms")] string name = ""
             )
        {
            Name = name;

            using HttpClient Client = new();

            try
            {
                //Donne le paramuètre de name a l'URL 
                Client.DefaultRequestHeaders.Add("Authorization", apiKey);
                HttpResponseMessage response = await Client.GetAsync($"{URL}?searchLanguage=fr&name={Uri.EscapeDataString(name)}");

                if (response.IsSuccessStatusCode)
                {

                    string jsonresponse = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<CosmeticsData>(jsonresponse);
                     
                    if (responseData != null)
                    {
                        //Création du message 
                        var embed = new EmbedBuilder
                        {
                            
                            Title = $"__{responseData.Data.Name}__",
                            ImageUrl = responseData.Data.Images.Featured,
                            Description = responseData.Data.Description +
                            $"\n----------------------------------------------\n",
                            Color = new Color (0, 255, 255),
                              Footer = new Discord.EmbedFooterBuilder
                              {
                                  Text = responseData.Data.Introduction.Text

                              }
                              
                        };
                        embed.AddField("Value",$"{responseData.Data.Rarity.DisplayValue}",inline: true);
                       //Création du message 
                        BuildEmbed = embed.Build();
                    };
                    await ReplyAsync(embed: BuildEmbed);
                }

            }
            catch (Exception ex)
            {
                // Gère les exceptions, par exemple, en les journalisant ou en affichant un message d'erreur.
                Console.WriteLine($"Une erreur s'est produite : {ex.Message}");
            }
        }
        

    }


    
}