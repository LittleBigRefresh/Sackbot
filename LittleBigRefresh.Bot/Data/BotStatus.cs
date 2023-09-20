using Discord;

namespace LittleBigRefresh.Bot.Data;

public struct BotStatus
{
    public BotStatus(ActivityType type, string status)
    {
        Type = type;
        Status = status;
    }

    public ActivityType Type { get; set; }
    public string Status { get; set; }
}