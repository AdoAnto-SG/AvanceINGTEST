using Newtonsoft.Json;

namespace Integrador.Models.Base;

public class BaseRequest
{
    [JsonProperty(nameof(AdmInfo))]
    public AdmInfo? AdmInfo { get; set; }

    [JsonProperty("OINV")]
    public Oinv? Oinv { get; set; }

    [JsonProperty("INV1")]
    public object? Inv1 { get; set; }

    [JsonProperty("INV4")]
    public List<Inv4>? Inv4 { get; set; }

    [JsonProperty("INV6")]
    public List<Inv6>? Inv6 { get; set; }

    [JsonProperty("INV12")]
    public List<Inv12>? Inv12 { get; set; }

    [JsonProperty("INV21")]
    public List<Inv21>? Inv21 { get; set; }

    [JsonProperty("SocioDeNegocio")]
    public InvoiceReceiver? InvoiceReceiver { get; set; }

    [JsonProperty("SociosFactDividida")]
    public List<InvoiceReceiverDivided>? InvoiceReceiverDivideds { get; set; }
}

