using Newtonsoft.Json;

namespace Integrador.Models.Base;

public class AllocBinC
{
    [JsonProperty("@attributes")]
    public Attributes? Attributes { get; set; }
}
