using Discord;
using Discord.WebSocket;
using LittleBigRefresh.Bot.Modules;
using LittleBigRefresh.Bot.Wiki;
using Sackbot.Core;
using Sackbot.Interactions;
using Sackbot.Interactions.Arguments;

namespace LittleBigRefresh.Bot.Interactions;

public class ErrorCodeInteraction : CommandInteraction
{
    public override string Name => "error-code";
    public override string Description => "Gets information about an error code";

    private static readonly StringCommandArgument CodeArgument = new()
    {
        Name = "code",
        Description = "The error code to look up",
        Required = true
    };

    public override List<CommandArgument> Arguments => new()
    {
        CodeArgument
    };

    public override async Task PerformInteraction(SackbotClient client, SocketSlashCommand interaction)
    {
        string code = CodeArgument.GetString(interaction);
        ErrorCodeModule errorCodeModule = client.GetModule<ErrorCodeModule>();

        ErrorCode? errorCode = errorCodeModule.ErrorCodes.FirstOrDefault(c => c.Code == code);
        if (errorCode == null)
        {
            await interaction.RespondAsync("Could not find an error by that code. Make sure you have typed the code properly, e.g. 0x80711009.");
            return;
        }

        EmbedBuilder embed = new();

        embed.WithColor(0xEE0000u);
        embed.WithTitle($"{errorCode.Section}: {errorCode.Name}");
        embed.WithDescription(errorCode.Description);
        embed.WithFooter(errorCode.Code);

        await interaction.RespondAsync("Found a matching error code.", embed: embed.Build());
    }
}