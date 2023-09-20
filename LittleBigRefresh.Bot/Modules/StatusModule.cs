using Discord;
using LittleBigRefresh.Bot.Data;
using Sackbot.Core;

namespace LittleBigRefresh.Bot.Modules;

public class StatusModule : IModule
{
    private readonly List<BotStatus> _statuses = new()
    {
        new BotStatus(ActivityType.Playing, "LittleBigPlanet 1"),
        new BotStatus(ActivityType.Playing, "LittleBigPlanet 2"),
        new BotStatus(ActivityType.Playing, "LittleBigPlanet 3"),
        new BotStatus(ActivityType.Playing, "LittleBigPlanet PS Vita"),
        new BotStatus(ActivityType.Playing, "LittleBigPlanet PSP"),
        new BotStatus(ActivityType.Playing, "LittleBigPlanet Karting"),
        new BotStatus(ActivityType.Playing, "Sackboy: A Big Adventure"),
        new BotStatus(ActivityType.Playing, "Vib Ribbon"),
        new BotStatus(ActivityType.Playing, "Peeing Simulator 2"),
        new BotStatus(ActivityType.Playing, "The Bunker"),
        new BotStatus(ActivityType.Playing, "Work N' Buy - City RPG"),
        new BotStatus(ActivityType.Playing, "Hide 'n' Seek: In the Office"),
        new BotStatus(ActivityType.Playing, "ENOUGH WATCH STREAM! GIRLS HERE!"),
        new BotStatus(ActivityType.Playing, "Long-Jump CHALLENGE"),
        new BotStatus(ActivityType.Playing, "HARD? 4 [JPN]"),
        new BotStatus(ActivityType.Playing, "the jerma level"),
        new BotStatus(ActivityType.Playing, "Clockworx 2"),
        
        new BotStatus(ActivityType.Watching, "You"),
        
        new BotStatus(ActivityType.CustomStatus, "Refreshing Refresher"),
        new BotStatus(ActivityType.CustomStatus, "How about legendary-guacamole?"),
    };

    private const int StatusUpdateIntervalSeconds = 60;
    
    public void Initialize(SackbotClient client)
    {
        Task.Factory.StartNew(async () =>
        {
            while (true)
            {
                this.SetRandomStatus(client);
                await Task.Delay(StatusUpdateIntervalSeconds * 1000);
            }
        });
    }

    private void SetRandomStatus(SackbotClient client)
    {
        int index = Random.Shared.Next(0, this._statuses.Count);
        BotStatus status = this._statuses[index];

        if (status.Type == ActivityType.CustomStatus)
        {
            client.Discord.SetCustomStatusAsync(status.Status);
            return;
        }

        client.Discord.SetGameAsync(status.Status, type: status.Type);
    }
}