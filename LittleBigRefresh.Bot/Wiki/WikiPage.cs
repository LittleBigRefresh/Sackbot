using Newtonsoft.Json;

namespace LittleBigRefresh.Bot.Wiki;

#nullable disable

[JsonObject]
public class WikiPage
{
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("key")] public string Key { get; set; }
    [JsonProperty("title")] public string Title { get; set; }
    [JsonProperty("content_model")] public string ContentModel { get; set; }
    [JsonProperty("source")] public string Source { get; set; }
}