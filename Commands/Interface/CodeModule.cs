using Discord;
using Discord.Commands;
using FortniteBot.Helpers;
using FortniteBot.Data.Code;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore.Storage.Json;


namespace FortniteBot.Commands
{
    [Group("Search")]
    public class CodeModule : ModuleBase<SocketCommandContext>
    {
        private Embed BuildEmbed2;
        private Embed BuildEmbed1;

        public string ApiKey { get; } = ConfigurationHelper.GetByName("Discord:API:Key");
        public string URL { get; } = ConfigurationHelper.GetByName("Discord:API:Code:URL");

        private string Code;
        private string Params;
        

        [Command("code")]
        [Summary("Show Code EpicGames")]
        public async Task CodeCommand(
            [Summary("Sets the Code name")] string code = "",
            [Remainder][Summary("Other params")] string prms = "")
        {
            Code = code;
            using HttpClient client = new();
            try
            {
                client.DefaultRequestHeaders.Add("Authorization", ApiKey);
                HttpResponseMessage response = await client.GetAsync($"{URL}");
                if (response.IsSuccessStatusCode)
                {
                    string jsonresponse = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<CodeData>(jsonresponse);
                    if (responseData != null)
                    {
                        var embed = new EmbedBuilder
                        {
                            Title = "__There is the CodeCreator__"
                        };
                        BuildEmbed1 = embed.Build();
                    }
                    await ReplyAsync(embed: BuildEmbed1);


                    if (responseData != null)
                    {
                        var embed = new EmbedBuilder
                        {
                            Title = $"{responseData.Data.Code}" +
                            $"\n---------------------------------"
                        };

                        embed.AddField("AccountName", $"**{responseData.Data.Account.Name}**",inline: true);
                        embed.AddField("Status", $"**{responseData.Data.Statues}**",inline: true);
                        BuildEmbed2 = embed.Build();
                    };
                    await ReplyAsync(embed: BuildEmbed2);
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Une erreur s'est produite : {ex.Message}");
            }
        }
    }
}
