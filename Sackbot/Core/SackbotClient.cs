using Discord;
using Discord.WebSocket;
using Microsoft.VisualBasic.CompilerServices;
using NotEnoughLogs;

namespace Sackbot.Core;

public class SackbotClient : IDisposable
{
    public readonly DiscordSocketClient Discord;
    private readonly SackbotConfiguration _config;
    private readonly LoggerContainer<SackbotContext> _logger;

    private readonly List<IModule> _modules = new();

    public SackbotClient(LoggerContainer<SackbotContext> logger, SackbotConfiguration config)
    {
        DiscordSocketConfig clientConfig = new()
        {
            GatewayIntents = GatewayIntents.None,
            AlwaysDownloadUsers = false,
            AlwaysResolveStickers = false,
            AlwaysDownloadDefaultStickers = false,
            MaxWaitBetweenGuildAvailablesBeforeReady = 0,
            #if DEBUG
            LogLevel = LogSeverity.Debug
            #endif
        };

        this._logger = logger;
        this._config = config;
        this.Discord = new DiscordSocketClient(clientConfig);

        this.Discord.Log += message =>
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

    public void Initialize()
    {
        foreach (IModule module in this._modules)
        {
            this._logger.LogInfo(SackbotContext.Startup, "Initializing module " + module.GetType().Name);
            module.Initialize(this);
        }
    }

    public async Task StartAsync()
    {
        await this.Discord.LoginAsync(TokenType.Bot, this._config.Token);
        await this.Discord.StartAsync();
    }

    public void AddModule<TModule>() where TModule : IModule, new()
    {
        this.AddModule(new TModule());
    }

    private void AddModule(IModule module) => this._modules.Add(module);

    public void Dispose()
    {
        this.Discord.Dispose();
        GC.SuppressFinalize(this);
    }
}