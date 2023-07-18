using Discord;
using Discord.WebSocket;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using NotEnoughLogs;
using NotEnoughLogs.Loggers;

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
    
    public static SackbotClient CreateSackbot()
    {
        const string configFilename = "sackbot.json";
        string configPath = Path.Combine(Environment.CurrentDirectory, configFilename);

        LoggerContainer<SackbotContext> logger = new();
        logger.RegisterLogger(new ConsoleLogger());

        SackbotConfiguration? configuration = null;
        if (!File.Exists(configPath))
        {
            string exampleJson = JsonConvert.SerializeObject(SackbotConfiguration.ExampleConfiguration, Formatting.Indented);
            File.WriteAllText(configPath, exampleJson);
    
            logger.LogInfo(SackbotContext.Startup, "Generated a blank config at " + configPath);
            Environment.Exit(1);
        }

        string configText = File.ReadAllText(configPath);
        configuration = JsonConvert.DeserializeObject<SackbotConfiguration>(configText);

        // ReSharper disable once InvertIf
        if (configuration == null)
        {
            logger.LogCritical(SackbotContext.Startup, "Failed to read configuration due to an unknown error. Cannot continue.");
            Environment.Exit(1);
        }

        return new SackbotClient(logger, configuration.Value);
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

    public void AddModule(IModule module) => this._modules.Add(module);

    public void Dispose()
    {
        this.Discord.Dispose();
        this._logger.Dispose();
        GC.SuppressFinalize(this);
    }
}