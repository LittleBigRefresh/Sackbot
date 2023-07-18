using System.Reflection;
using Discord;
using Discord.WebSocket;
using Sackbot.Core;
using Sackbot.Interactions;

namespace Sackbot.Modules;

public class InteractionModule : IModule
{
    private readonly List<ICommandInteraction> _commandInteractions = new();
    private SackbotClient _client = null!;
    
    public void Initialize(SackbotClient client)
    {
        client.Discord.InteractionCreated += HandleInteraction;

        IEnumerable<Type> commandTypes = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(ICommandInteraction)) && t != typeof(ICommandInteraction));

        foreach (Type commandType in commandTypes)
        {
            ICommandInteraction? command = (ICommandInteraction?)Activator.CreateInstance(commandType);
            if (command == null) throw new InvalidOperationException();
            
            Console.WriteLine("Found command " + command.Name);
            _commandInteractions.Add(command);
        }

        client.Discord.Connected += async () =>
        {
            List<ApplicationCommandProperties> properties = new(_commandInteractions.Count);
            foreach (ICommandInteraction command in _commandInteractions)
            {
                SlashCommandBuilder builder = new()
                {
                    Name = command.Name,
                    Description = command.Description,
                    IsNsfw = false,
                    IsDMEnabled = true,
                    IsDefaultPermission = true
                };
                
                properties.Add(builder.Build());
            }
            
            await client.Discord.BulkOverwriteGlobalApplicationCommandsAsync(properties.ToArray());
        };
        
        this._client = client;
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        switch (interaction)
        {
            case SocketSlashCommand commandInteraction:
                await HandleSlashCommandInteraction(commandInteraction);
                break;
            default:
                throw new NotImplementedException($"Interaction type not supported: {interaction.Type}");
        }
    }

    private async Task HandleSlashCommandInteraction(SocketSlashCommand interaction)
    {
        foreach (ICommandInteraction command in _commandInteractions)
        {
            if(command.Name != interaction.CommandName) continue;
            await command.PerformInteraction(this._client, interaction);
        }
    }
}