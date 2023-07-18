using Discord.WebSocket;
using JetBrains.Annotations;
using Sackbot.Core;
using Sackbot.Interactions.Arguments;

namespace Sackbot.Interactions;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public abstract class CommandInteraction : IInteraction
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    
    public virtual List<CommandArgument> Arguments => new(0); 
    
    public abstract Task PerformInteraction(SackbotClient client, SocketSlashCommand interaction);
}