using Discord;
using Discord.WebSocket;
using Sackbot.Core;
using Sackbot.Modules;

namespace Sackbot.Interactions;

public class HelpInteraction : CommandInteraction
{
    public override string Name => "help";
    public override string Description => "Shows a list of commands that can be used";
    
    public override async Task PerformInteraction(SackbotClient client, SocketSlashCommand interaction)
    {
        IReadOnlyList<CommandInteraction> interactions = client.GetModule<InteractionModule>().Interactions;

        EmbedBuilder builder = new()
        {
            Title = $"{interactions.Count} commands",
            Color = new Color(0x65, 0x64, 0xdd)
        };

        foreach (CommandInteraction command in interactions)
        {
            builder.AddField('/' + command.Name, command.Description);
        }

        await interaction.RespondAsync(embed: builder.Build(), ephemeral: true);
    }
}