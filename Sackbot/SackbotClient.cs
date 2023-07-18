using Discord;
using Discord.WebSocket;
using NotEnoughLogs;

namespace Sackbot;

public class SackbotClient : IDisposable
{
    private readonly DiscordSocketClient _client;
    private readonly SackbotConfiguration _config;
    private readonly LoggerContainer<SackbotContext> _logger;

    public SackbotClient(LoggerContainer<SackbotContext> logger, SackbotConfiguration config)
    {
        DiscordSocketConfig clientConfig = new()
        {
            AlwaysDownloadUsers = false,
            AlwaysResolveStickers = false,
            AlwaysDownloadDefaultStickers = false,
            #if DEBUG
            LogLevel = LogSeverity.Debug
            #endif
        };

        this._logger = logger;
        this._config = config;
        this._client = new DiscordSocketClient(clientConfig);

        this._client.Log += message =>
        {
            SackbotContext context = SackbotContext.Discord;
            string msg = $"{message.Message}{message.Exception}";

            if (Enum.TryParse(message.Source, out SackbotContext ctx))
                context = ctx;
            else msg = $"{message.Source}: {msg}";

            switch (message.Severity)
            {
                case LogSeverity.Critical:
                    this._logger.LogCritical(context, msg);
                    break;
                case LogSeverity.Error:
                    this._logger.LogError(context, msg);
                    break;
                case LogSeverity.Warning:
                    this._logger.LogWarning(context, msg);
                    break;
                case LogSeverity.Info:
                    this._logger.LogInfo(context, msg);
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    this._logger.LogDebug(context, msg);
                    break;
                default:
                    throw new NotImplementedException(message.Severity.ToString());
            }
            
            return Task.CompletedTask;
        };
    }

    public Task Initialize() => this._client.LoginAsync(TokenType.Bot, this._config.Token);
    public Task StartAsync() => this._client.StartAsync();

    public void Dispose()
    {
        this._client.Dispose();
        GC.SuppressFinalize(this);
    }
}