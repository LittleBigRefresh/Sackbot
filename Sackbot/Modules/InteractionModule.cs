using System.Reflection;
using Discord;
using Discord.WebSocket;
using Sackbot.Core;
using Sackbot.Interactions;
using Sackbot.Interactions.Arguments;

namespace Sackbot.Modules;

public class InteractionModule : IModule
{
    private readonly List<CommandInteraction> _commandInteractions = new();
    private SackbotClient _client = null!;

    public IReadOnlyList<CommandInteraction> Interactions => _commandInteractions.AsReadOnly();

    public void AddInteraction<TInteraction>() where TInteraction : CommandInteraction, new()
    {
        this._commandInteractions.Add(new TInteraction());
    }

    public void AddInteractionsFromAssembly(Assembly assembly)
    {
        IEnumerable<Type> commandTypes = assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(CommandInteraction)) && t != typeof(CommandInteraction));
        
        foreach (Type commandType in commandTypes)
        {
            CommandInteraction? command = (CommandInteraction?)Activator.CreateInstance(commandType);
            if (command == null) throw new InvalidOperationException();
            _commandInteractions.Add(command);
        }
    }
    
    public void Initialize(SackbotClient client)
    {
        client.Discord.InteractionCreated += HandleInteraction;

        client.Discord.Connected += async () =>
        {
            List<ApplicationCommandProperties> properties = new(_commandInteractions.Count);
            foreach (CommandInteraction command in _commandInteractions)
            {
                SlashCommandBuilder commandBuilder = new()
                {
                    Name = command.Name,
                    Description = command.Description,
                    IsNsfw = false,
                    IsDMEnabled = true,
                    IsDefaultPermission = true
                };
                
                foreach (CommandArgument argument in command.Arguments)
                {
                    SlashCommandOptionBuilder argumentBuilder = new()
                    {
                        Name = argument.Name,
                        Description = argument.Description,
                        IsRequired = argument.Required,
                    };

                    switch (argument)
                    {
                        case StringCommandArgument:
                        {
                            argumentBuilder.Type = ApplicationCommandOptionType.String;
                            break;
                        }
                    }

                    commandBuilder.AddOption(argumentBuilder);
                }
                
                properties.Add(commandBuilder.Build());
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
        foreach (CommandInteraction command in _commandInteractions)
        {
            if(command.Name != interaction.CommandName) continue;
            await command.PerformInteraction(this._client, interaction);
        }
    }
}