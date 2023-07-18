using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sackbot;

[JsonObject(MemberSerialization.OptOut, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public struct SackbotConfiguration
{
    public static SackbotConfiguration ExampleConfiguration = new()
    {
        Token = "PUT DISCORD TOKEN HERE",
    };
    
    public string Token { get; set; }
}