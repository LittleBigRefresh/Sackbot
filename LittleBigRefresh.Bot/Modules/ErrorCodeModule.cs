using System.Net.Http.Json;
using LittleBigRefresh.Bot.Wiki;
using Sackbot.Core;

namespace LittleBigRefresh.Bot.Modules;

public class ErrorCodeModule : IModule
{
    public List<ErrorCode> ErrorCodes { get; } = new();
    
    public void Initialize(SackbotClient client)
    {
        using HttpClient httpClient = new();
        WikiPage? page = httpClient.GetFromJsonAsync<WikiPage>("https://www.psdevwiki.com/ps3/rest.php/v1/page/Error_Codes").Result;

        if (page == null) return;

        string section = string.Empty;
        foreach (string line in page.Source.Split("\n"))
        {
            if (line.StartsWith("=="))
            {
                section = line[3..^3];
                continue;
            }

            if (line.StartsWith("| "))
            {
                string[] error = line.Split("||");
                ErrorCode code = new()
                {
                    Section = section,
                };

                if (error.Length >= 3)
                {
                    code.Name = error[0][2..].Trim(' ');
                    code.Code = error[1].Trim(' ');
                    code.Description = error[2].Trim(' ');
                }
                else if (error.Length >= 2)
                {
                    code.Name = error[0][2..].Trim(' ');
                    code.Code = error[1].Trim(' ');
                    code.Description = "No description was provided for this error.";
                }
                else
                {
                    Console.WriteLine(line);
                }
                
                this.ErrorCodes.Add(code);
            }
        }
    }
}