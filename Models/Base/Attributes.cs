using Integrador.Converters;
using Newtonsoft.Json;

namespace Integrador.Models.Base;

public class Attributes
{
    [JsonProperty("nil")]
    public bool Nil { get; set; }
}
