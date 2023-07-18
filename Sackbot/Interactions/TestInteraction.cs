using Discord.WebSocket;
using Sackbot.Core;

namespace Sackbot.Interactions;

public class TestInteraction : ICommandInteraction
{
    public string Name => "test";
    public string Description => "yo man";

    public async Task PerformInteraction(SackbotClient client, SocketSlashCommand interaction)
    {
        await interaction.RespondAsync("incredible.");
    }
}