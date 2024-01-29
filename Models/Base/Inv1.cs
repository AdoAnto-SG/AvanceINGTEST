using Newtonsoft.Json;

namespace Integrador.Models.Base;

public class Inv1
{
    [JsonProperty(nameof(Dscription))]
    public string? Dscription { get; set; }

    [JsonProperty(nameof(Quantity))]
    public string? Quantity { get; set; }

    [JsonProperty(nameof(Price))]
    public string? Price { get; set; }

    [JsonProperty(nameof(DiscPrcnt))]
    public string? DiscPrcnt { get; set; }

    [JsonProperty(nameof(LineTotal))]
    public string? LineTotal { get; set; }

    [JsonProperty(nameof(VatSum))]
    public string? VatSum { get; set; }

    [JsonProperty(nameof(TaxCode))]
    public string? TaxCode { get; set; }

    [JsonProperty(nameof(U_TipoItem_FE))]
    public string? U_TipoItem_FE { get; set; }

    

    [JsonProperty(nameof(U_TipoVenta_FE))]
    public object? U_TipoVenta_FE { get; set; }

    [JsonProperty(nameof(U_Concepto1))]
    public object? U_Concepto1 { get; set; }
}
