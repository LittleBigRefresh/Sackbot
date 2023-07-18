using Newtonsoft.Json;
using NotEnoughLogs;
using NotEnoughLogs.Loggers;
using Sackbot.Core;
using Sackbot.Modules;

const string configFilename = "sackbot.json";
string configPath = Path.Combine(Environment.CurrentDirectory, configFilename);

using LoggerContainer<SackbotContext> logger = new();
logger.RegisterLogger(new ConsoleLogger());

SackbotConfiguration? configuration = null;
if (!File.Exists(configPath))
{
    string exampleJson = JsonConvert.SerializeObject(SackbotConfiguration.ExampleConfiguration, Formatting.Indented);
    File.WriteAllText(configPath, exampleJson);
    
    logger.LogInfo(SackbotContext.Startup, "Generated a blank config at " + configPath);
    Environment.Exit(1);
    return;
}

string configText = File.ReadAllText(configPath);
configuration = JsonConvert.DeserializeObject<SackbotConfiguration>(configText);

if (configuration == null)
{
    logger.LogCritical(SackbotContext.Startup, "Failed to read configuration due to an unknown error. Cannot continue.");
    Environment.Exit(1);
    return;
}

using SackbotClient client = new(logger, configuration.Value);
client.AddModule<InteractionModule>();
client.Initialize();

await client.StartAsync();
await Task.Delay(-1);