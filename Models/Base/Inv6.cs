using Integrador.Converters;
using Newtonsoft.Json;

namespace Integrador.Models.Base;

public class Inv6
{
    [JsonProperty(nameof(DocEntry))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long DocEntry { get; set; }

    [JsonProperty("InstlmntID")]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long InstlmntId { get; set; }

    [JsonProperty(nameof(ObjType))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long ObjType { get; set; }

    [JsonProperty(nameof(LogInstanc))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long LogInstanc { get; set; }

    [JsonProperty(nameof(DueDate))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long DueDate { get; set; }

    [JsonProperty(nameof(Status))]
    public string? Status { get; set; }

    [JsonProperty(nameof(DunnLevel))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long DunnLevel { get; set; }

    [JsonProperty(nameof(InsTotal))]
    public string? InsTotal { get; set; }

    [JsonProperty("InsTotalFC")]
    public string? InsTotalFc { get; set; }

    [JsonProperty(nameof(InsTotalSy))]
    public string? InsTotalSy { get; set; }

    [JsonProperty(nameof(PaidToDate))]
    public string? PaidToDate { get; set; }

    [JsonProperty("PaidFC")]
    public string? PaidFc { get; set; }

    [JsonProperty(nameof(PaidSys))]
    public string? PaidSys { get; set; }

    [JsonProperty(nameof(VatSum))]
    public string? VatSum { get; set; }

    [JsonProperty("VatSumFC")]
    public string? VatSumFc { get; set; }

    [JsonProperty(nameof(VatSumSy))]
    public string? VatSumSy { get; set; }

    [JsonProperty(nameof(VatPaid))]
    public string? VatPaid { get; set; }

    [JsonProperty("VatPaidFC")]
    public string? VatPaidFc { get; set; }

    [JsonProperty(nameof(VatPaidSys))]
    public string? VatPaidSys { get; set; }

    [JsonProperty(nameof(TotalExpns))]
    public string? TotalExpns { get; set; }

    [JsonProperty("TotalExpFC")]
    public string? TotalExpFc { get; set; }

    [JsonProperty("TotalExpSC")]
    public string? TotalExpSc { get; set; }

    [JsonProperty(nameof(ExpAppl))]
    public string? ExpAppl { get; set; }

    [JsonProperty("ExpApplFC")]
    public string? ExpApplFc { get; set; }

    [JsonProperty("ExpApplSC")]
    public string? ExpApplSc { get; set; }

    [JsonProperty("WTSum")]
    public string? WtSum { get; set; }

    [JsonProperty("WTSumFC")]
    public string? WtSumFc { get; set; }

    [JsonProperty("WTSumSC")]
    public string? WtSumSc { get; set; }

    [JsonProperty("WTApplied")]
    public string? WtApplied { get; set; }

    [JsonProperty("WTAppliedF")]
    public string? WtAppliedF { get; set; }

    [JsonProperty("WTAppliedS")]
    public string? WtAppliedS { get; set; }

    [JsonProperty(nameof(TotalBlck))]
    public string? TotalBlck { get; set; }

    [JsonProperty(nameof(TotalBlckF))]
    public string? TotalBlckF { get; set; }

    [JsonProperty(nameof(TotalBlckS))]
    public string? TotalBlckS { get; set; }

    [JsonProperty("VATBlck")]
    public string? VatBlck { get; set; }

    [JsonProperty("VATBlckFC")]
    public string? VatBlckFc { get; set; }

    [JsonProperty("VATBlckSC")]
    public string? VatBlckSc { get; set; }

    [JsonProperty(nameof(ExpnsBlck))]
    public string? ExpnsBlck { get; set; }

    [JsonProperty(nameof(ExpnsBlckF))]
    public string? ExpnsBlckF { get; set; }

    [JsonProperty(nameof(ExpnsBlckS))]
    public string? ExpnsBlckS { get; set; }

    [JsonProperty("WTBlocked")]
    public string? WtBlocked { get; set; }

    [JsonProperty("WTBlockedF")]
    public string? WtBlockedF { get; set; }

    [JsonProperty("WTBlockedS")]
    public string? WtBlockedS { get; set; }

    [JsonProperty(nameof(InstPrcnt))]
    public string? InstPrcnt { get; set; }

    [JsonProperty(nameof(DunWizBlck))]
    public string? DunWizBlck { get; set; }

    [JsonProperty(nameof(DunDate))]
    public AllocBinC? DunDate { get; set; }

    [JsonProperty(nameof(Paid))]
    public string? Paid { get; set; }

    [JsonProperty(nameof(PaidFrgn))]
    public string? PaidFrgn { get; set; }

    [JsonProperty(nameof(PaidSc))]
    public string? PaidSc { get; set; }

    [JsonProperty("reserved")]
    public string? Reserved { get; set; }

    [JsonProperty(nameof(TaxOnExp))]
    public string? TaxOnExp { get; set; }

    [JsonProperty(nameof(TaxOnExpFc))]
    public string? TaxOnExpFc { get; set; }

    [JsonProperty(nameof(TaxOnExpSc))]
    public string? TaxOnExpSc { get; set; }

    [JsonProperty(nameof(TaxOnExpAp))]
    public string? TaxOnExpAp { get; set; }

    [JsonProperty(nameof(TaxOnExApF))]
    public string? TaxOnExApF { get; set; }

    [JsonProperty(nameof(TaxOnExApS))]
    public string? TaxOnExApS { get; set; }

    [JsonProperty(nameof(TaxOnExBlo))]
    public string? TaxOnExBlo { get; set; }

    [JsonProperty(nameof(TaxOnExBlF))]
    public string? TaxOnExBlF { get; set; }

    [JsonProperty(nameof(TaxOnExBlS))]
    public string? TaxOnExBlS { get; set; }

    [JsonProperty(nameof(LvlUpdDate))]
    public AllocBinC? LvlUpdDate { get; set; }

    [JsonProperty(nameof(Ordered))]
    public string? Ordered { get; set; }

    [JsonProperty(nameof(PaidDpm))]
    public string? PaidDpm { get; set; }

    [JsonProperty(nameof(PaidDpmFc))]
    public string? PaidDpmFc { get; set; }

    [JsonProperty(nameof(PaidDpmSc))]
    public string? PaidDpmSc { get; set; }

    [JsonProperty("EncryptIV")]
    public AllocBinC? EncryptIv { get; set; }
}
