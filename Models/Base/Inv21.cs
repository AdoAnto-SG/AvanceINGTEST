using Integrador.Converters;
using Newtonsoft.Json;

namespace Integrador.Models.Base;

public class Inv21
{
    [JsonProperty(nameof(DocEntry))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long DocEntry { get; set; }

    [JsonProperty(nameof(ObjectType))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long ObjectType { get; set; }

    [JsonProperty(nameof(LogInstanc))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long LogInstanc { get; set; }

    [JsonProperty(nameof(RefType))]
    public string? RefType { get; set; }

    [JsonProperty(nameof(LineNum))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long LineNum { get; set; }

    [JsonProperty(nameof(RefDocEntr))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long RefDocEntr { get; set; }

    [JsonProperty(nameof(RefDocNum))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long RefDocNum { get; set; }

    [JsonProperty(nameof(ExtDocNum))]
    public string? ExtDocNum { get; set; }

    [JsonProperty(nameof(RefObjType))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long RefObjType { get; set; }

    [JsonProperty(nameof(AccessKey))]
    public string? AccessKey { get; set; }

    [JsonProperty(nameof(IssueDate))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long IssueDate { get; set; }

    [JsonProperty("IssuerCNPJ")]
    public string? IssuerCnpj { get; set; }

    [JsonProperty(nameof(IssuerCode))]
    public string? IssuerCode { get; set; }

    [JsonProperty(nameof(Model))]
    public string? Model { get; set; }

    [JsonProperty(nameof(Series))]
    public string? Series { get; set; }

    [JsonProperty(nameof(Number))]
    public string? Number { get; set; }

    [JsonProperty(nameof(RefAccKey))]
    public string? RefAccKey { get; set; }

    [JsonProperty(nameof(RefAmount))]
    public string? RefAmount { get; set; }

    [JsonProperty(nameof(SubSeries))]
    public string? SubSeries { get; set; }

    [JsonProperty(nameof(Remark))]
    public string? Remark { get; set; }

    [JsonProperty(nameof(LinkRefTyp))]
    public string? LinkRefTyp { get; set; }
}
