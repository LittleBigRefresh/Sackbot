using Discord.WebSocket;
using Sackbot.Core;
using Sackbot.Interactions;
using Sackbot.Interactions.Arguments;

namespace LittleBigRefresh.Bot.Interactions;

public class ErrorCodeInteraction : CommandInteraction
{
    public override string Name => "error-code";
    public override string Description => "Gets information about an error code";

    private readonly StringCommandArgument CodeArgument = new()
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
        await interaction.RespondAsync(CodeArgument.GetString(interaction));
    }
}