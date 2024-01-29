using Newtonsoft.Json;

namespace Integrador.Models.Base;

public class ActBaseEnt
{
    [JsonProperty("@content", NullValueHandling = NullValueHandling.Ignore)]
    public string? Content { get; set; }

    [JsonProperty("@attributes")]
    public Attributes? Attributes { get; set; }
}
