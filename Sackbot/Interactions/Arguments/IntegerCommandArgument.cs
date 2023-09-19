using Discord.WebSocket;

namespace Sackbot.Interactions.Arguments;

public class IntegerCommandArgument : CommandArgument
{
    public int GetInt32(SocketSlashCommand interaction) => (int)this.GetData(interaction);
}