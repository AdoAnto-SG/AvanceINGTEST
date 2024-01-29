using Integrador.Converters;
using Newtonsoft.Json;

namespace Integrador.Models.Base;

public class AdmInfo
{
    [JsonProperty(nameof(Object))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public string? Object { get; set; }
}
