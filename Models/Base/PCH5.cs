using Newtonsoft.Json;

namespace Integrador;

public class PCH5
{
    [JsonProperty("WTCode")]
    public string? TaxCode { get; set; }

    [JsonProperty(nameof(Rate))]
    public string? Rate { get; set; }

    [JsonProperty(nameof(TaxbleAmnt))]
    public string? TaxbleAmnt { get; set; }

    [JsonProperty(nameof(WTAmnt))]
    public string? WTAmnt { get; set; }
}
