using Discord.WebSocket;

namespace Sackbot.Interactions.Arguments;

public abstract class CommandArgument : IInteraction
{
    public required string Name { get; init; }
    public required string Description { get; init; }

    public bool Required { get; set; } = false;

    public object GetData(SocketSlashCommand interaction)
    {
        foreach (SocketSlashCommandDataOption option in interaction.Data.Options)
        {
            if (option.Name != this.Name) continue;

            if (option.Value == null)
            {
                if (this.Required) throw new InvalidOperationException("Expected argument's data that was not available");
                return null!;
            }

            return option.Value;
        }

        throw new InvalidOperationException("Expected argument that was not available");
    }
}