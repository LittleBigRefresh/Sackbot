using Sackbot.Core;
using Sackbot.Interactions;
using Sackbot.Modules;

using SackbotClient client = SackbotClient.CreateSackbot();

InteractionModule interactions = new();
interactions.AddInteraction<TestInteraction>();
client.AddModule(interactions);

client.Initialize();

await client.StartAsync();
await Task.Delay(-1);