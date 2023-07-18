using Discord.WebSocket;

namespace Sackbot.Interactions.Arguments;

public class StringCommandArgument : CommandArgument
{
    public string GetString(SocketSlashCommand interaction) => (string)this.GetData(interaction);
}