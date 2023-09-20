using System.Reflection;
using LittleBigRefresh.Bot.Modules;
using Sackbot.Core;
using Sackbot.Interactions;
using Sackbot.Modules;

using SackbotClient client = SackbotClient.CreateSackbot();

InteractionModule interactions = new();
interactions.AddInteraction<HelpInteraction>();
interactions.AddInteractionsFromAssembly(Assembly.GetExecutingAssembly());
client.AddModule(interactions);
client.AddModule<ErrorCodeModule>();
client.AddModule<StatusModule>();

client.Initialize();

await client.StartAsync();
await Task.Delay(-1);