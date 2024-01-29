using FindexMapper.Core.Utils;
using Newtonsoft.Json;

namespace Integrador.Models.Base;

public class Oinv
{
    [JsonProperty(nameof(DocNum))]
    public string? DocNum { get; set; }   

    [JsonProperty(nameof(DocDate))]
    public string? DocDate { get; set; }

    [JsonProperty(nameof(CardName))]
    public string? CardName { get; set; }

    [JsonProperty(nameof(VatSum))]
    public string? VatSum { get; set; }


    [JsonProperty(nameof(DiscPrcnt))]
    public string? DiscPrcnt { get; set; }

    [JsonProperty(nameof(DiscSum))]
    public string? DiscSum { get; set; }

    [JsonProperty(nameof(DocTotal))]
    public string? DocTotal { get; set; }

    [JsonProperty(nameof(DocTime))]
    public string? DocTime { get; set; }

    [JsonProperty(nameof(U_nit_prov))]
    public string? U_nit_prov { get; set; }

    [JsonProperty(nameof(U_nrc_prov))]
    public string? U_nrc_prov { get; set; }

    [JsonProperty(nameof(U_tipo_doc))]
    public string? U_tipo_doc { get; set; }

    [JsonProperty(nameof(U_num_doc))]
    public string? U_num_doc { get; set; }

    [JsonProperty(nameof(U_Cod_Sucursal))]
    public string? U_Cod_Sucursal { get; set; }

    [JsonProperty(nameof(U_Formapago_FE))]
    public string? U_Formapago_FE { get; set; }

    [JsonProperty(nameof(U_MetodoPago_FE))]
    public string? U_MetodoPago_FE { get; set; }

    [JsonProperty(nameof(U_TipoPlazo_FE))]
    public string? U_TipoPlazo_FE { get; set; }

    [JsonProperty(nameof(U_CantidadTiempo_FE))]
    public string? U_CantidadTiempo_FE { get; set; }

    [JsonProperty(nameof(U_TipoItem_FE))]
    public string? U_TipoItem_FE { get; set; }

    [JsonProperty(nameof(U_ImpuestosAplicados_FE))]
    public string? U_ImpuestosAplicados_FE { get; set; }

    [JsonProperty(nameof(U_RazonAnulacion_FE))]
    public string? U_RazonAnulacion_FE { get; set; }

    [JsonProperty(nameof(U_TipoVenta_FE))]
    public string? U_TipoVenta_FE { get; set; }

    [JsonProperty("NnSbAmnt")]
    public string? TotalExcludedSubject { get; set; }

    [JsonProperty("U_CodigoGeneracion_FE")]
    public string? UCodigoGeneracionFE { get; set; }

    [JsonProperty("U_N_Control_FE")]
    public string? ControlNumber { get; set; }

    [JsonProperty("U_FechaProcesamiento_FE")]
    public string? ProccessDate { get; set; }

    [JsonProperty("U_SelloRecepcion_FE")]
    public string? ReceptionSign { get; set; }

    [JsonProperty("U_Error_FE")]
    public string? Error { get; set; }

    public FindexMapper.Core.Enum.DocumentType GetDocumentType()
    {
        var controlNumberSplitted = this.ControlNumber?.Split('-');
        var type = controlNumberSplitted?[1];
        return type?.ToEnum(FindexMapper.Core.Enum.DocumentType.Invoice) ?? FindexMapper.Core.Enum.DocumentType.Invoice;
    }
    public DateTime GetIssueDate()
    {
        try
        {
            if (string.IsNullOrEmpty(this.DocDate) || string.IsNullOrEmpty(this.DocTime))
            {
                return DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).DateTime;
            }
            string dateFormat = "yyyyMMdd";
            if (!DateTime.TryParseExact(
                this.DocDate,
                dateFormat,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var issueDate))
            {
                issueDate = DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).DateTime;
            }

            string timeFormat = "HHmm";

            if (!DateTime.TryParseExact(
                this.DocTime,
                timeFormat,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var issueTime))
            {
                issueTime = DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).DateTime;
            }

            var time = new TimeSpan(issueTime.Hour, issueTime.Minute, 0);
            issueDate = issueDate.Add(time);

            return issueDate;
        }
        catch (System.Exception)
        {
            return DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).DateTime;
        }
    }

    public decimal GetTotalToPay(decimal vat, decimal isr)
    {
        var docTotal = Math.Round(Convert.ToDecimal(DocTotal), 2);
        var totalToPay = docTotal - (vat + isr);
        return totalToPay;
    }
}
