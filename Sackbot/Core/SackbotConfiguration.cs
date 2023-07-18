using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sackbot.Core;

[JsonObject(MemberSerialization.OptOut, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public struct SackbotConfiguration
{
    public static readonly SackbotConfiguration ExampleConfiguration = new()
    {
        Token = "PUT DISCORD TOKEN HERE",
    };
    
    public string Token { get; set; }
}