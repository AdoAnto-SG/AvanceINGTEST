using System.Text.RegularExpressions;
using FindexMapper.Core.Base;
using FindexMapper.Core.Enum;
using Newtonsoft.Json;

namespace Integrador.Models.Base;

public class InvoiceReceiverDivided : InvoiceReceiver
{
    [JsonProperty(nameof(VatIdUnCmp))]
    public string? VatIdUnCmp { get; set; }

    [JsonProperty(nameof(Porcentaje))]
    public string? Porcentaje { get; set; }

    [JsonProperty(nameof(LineNum))]
    public string? LineNum { get; set; }

    public List<Appendix>? GetAppendices()
    {
        var correlative = string.IsNullOrWhiteSpace(LineNum) ? 1 : Convert.ToInt32(LineNum);
        var correlativeAppendix = new Appendix()
        {
            Field = "correlativo",
            Tag = "correlativo",
            Value = correlative.ToString("000")
        };
        var appedices = new List<Appendix>
        {
            correlativeAppendix
        };
        return appedices;
    }

    public override (IdentificationDocumentType?, string?) HandleDocumentNumber()
    {
        Regex nitRegex = new("^([0-9]{14}|[0-9]{9})$");
        if(!string.IsNullOrWhiteSpace(VatIdUnCmp) && UDocumentoIdetificacionFE == "36" && nitRegex.IsMatch(VatIdUnCmp)) return (IdentificationDocumentType.NIT, VatIdUnCmp);

        Regex duiRegex = new("^[0-9]{8}-[0-9]{1}$");
        if(!string.IsNullOrWhiteSpace(VatIdUnCmp) && duiRegex.IsMatch(VatIdUnCmp)) return (IdentificationDocumentType.DUI, VatIdUnCmp);

        if(!string.IsNullOrWhiteSpace(VatIdUnCmp)) return (IdentificationDocumentType.Other, VatIdUnCmp);

        return (null, null);
    }
}
