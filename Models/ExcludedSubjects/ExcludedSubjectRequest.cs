using Integrador.Models;
using Integrador.Models.Base;
using Newtonsoft.Json;

namespace Integrador;

public class ExcludedSubjectRequest : BaseRequest
{
    [JsonProperty(nameof(AdmInfo))]
    public new AdmInfo? AdmInfo { get; set; }

    [JsonProperty("OPCH")]
    public new Oinv? Oinv { get; set; }

    [JsonProperty("PCH1")]
    public new object? Inv1 { get; set; }

    [JsonProperty("PCH4")]
    public new List<Inv4>? Inv4 { get; set; }

    [JsonProperty("PCH5")]
    public List<PCH5>? Taxes { get; set; }

    [JsonProperty("PCH6")]
    public new List<Inv6>? Inv6 { get; set; }

    [JsonProperty("PCH12")]
    public new List<Inv12>? Inv12 { get; set; }

    [JsonProperty("PCH21")]
    public new List<Inv21>? Inv21 { get; set; }

    [JsonProperty("SocioDeNegocio")]
    public new InvoiceReceiver? InvoiceReceiver { get; set; }

    [JsonProperty("SociosFactDividida")]
    public new List<InvoiceReceiverDivided>? InvoiceReceiverDivideds { get; set; }
}
