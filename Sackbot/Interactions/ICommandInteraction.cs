using Discord.WebSocket;
using JetBrains.Annotations;
using Sackbot.Core;

namespace Sackbot.Interactions;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface ICommandInteraction
{
    public string Name { get; }
    public string Description { get; }
    
    public Task PerformInteraction(SackbotClient client, SocketSlashCommand interaction);
}