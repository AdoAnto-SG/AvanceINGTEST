using Integrador.Converters;
using Integrador.Models.Base;
using Newtonsoft.Json;

namespace Integrador.Models;

public class Inv4
{
    [JsonProperty(nameof(DocEntry))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long DocEntry { get; set; }

    [JsonProperty(nameof(LineNum))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long LineNum { get; set; }

    [JsonProperty(nameof(GroupNum))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long GroupNum { get; set; }

    [JsonProperty(nameof(ExpnsCode))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long ExpnsCode { get; set; }

    [JsonProperty(nameof(RelateType))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long RelateType { get; set; }

    [JsonProperty(nameof(StcCode))]
    public string? StcCode { get; set; }

    [JsonProperty(nameof(StaCode))]
    public string? StaCode { get; set; }

    [JsonProperty("staType")]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long StaType { get; set; }

    [JsonProperty(nameof(TaxRate))]
    public string? TaxRate { get; set; }

    [JsonProperty(nameof(TaxAcct))]
    public string? TaxAcct { get; set; }

    [JsonProperty(nameof(TaxSum))]
    public string? TaxSum { get; set; }

    [JsonProperty(nameof(TaxSumFrgn))]
    public string? TaxSumFrgn { get; set; }

    [JsonProperty(nameof(TaxSumSys))]
    public string? TaxSumSys { get; set; }

    [JsonProperty(nameof(BaseSum))]
    public string? BaseSum { get; set; }

    [JsonProperty(nameof(BaseSumFrg))]
    public string? BaseSumFrg { get; set; }

    [JsonProperty(nameof(BaseSumSys))]
    public string? BaseSumSys { get; set; }

    [JsonProperty(nameof(ObjectType))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long ObjectType { get; set; }

    [JsonProperty(nameof(LogInstanc))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long LogInstanc { get; set; }

    [JsonProperty(nameof(TaxStatus))]
    public string? TaxStatus { get; set; }

    [JsonProperty(nameof(VatApplied))]
    public string? VatApplied { get; set; }

    [JsonProperty("VatAppldFC")]
    public string? VatAppldFc { get; set; }

    [JsonProperty("VatAppldSC")]
    public string? VatAppldSc { get; set; }

    [JsonProperty(nameof(LineSeq))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long LineSeq { get; set; }

    [JsonProperty(nameof(DeferrAcct))]
    public AllocBinC? DeferrAcct { get; set; }

    [JsonProperty(nameof(BaseType))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long BaseType { get; set; }

    [JsonProperty(nameof(BaseAbs))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long BaseAbs { get; set; }

    [JsonProperty(nameof(BaseSeq))]
    public string? BaseSeq { get; set; }

    [JsonProperty(nameof(DeductTax))]
    public string? DeductTax { get; set; }

    [JsonProperty(nameof(DdctTaxFrg))]
    public string? DdctTaxFrg { get; set; }

    [JsonProperty(nameof(DdctTaxSys))]
    public string? DdctTaxSys { get; set; }

    [JsonProperty(nameof(BaseAppld))]
    public string? BaseAppld { get; set; }

    [JsonProperty("BaseApldFC")]
    public string? BaseApldFc { get; set; }

    [JsonProperty("BaseApldSC")]
    public string? BaseApldSc { get; set; }

    [JsonProperty(nameof(NonDdctPrc))]
    public string? NonDdctPrc { get; set; }

    [JsonProperty(nameof(NonDdctAct))]
    public AllocBinC? NonDdctAct { get; set; }

    [JsonProperty(nameof(TaxInPrice))]
    public string? TaxInPrice { get; set; }

    [JsonProperty(nameof(Exempt))]
    public string? Exempt { get; set; }

    [JsonProperty(nameof(TaxExpAct))]
    public AllocBinC? TaxExpAct { get; set; }

    [JsonProperty(nameof(OnHoldTax))]
    public string? OnHoldTax { get; set; }

    [JsonProperty(nameof(OnHoldTaxF))]
    public string? OnHoldTaxF { get; set; }

    [JsonProperty(nameof(OnHoldTaxS))]
    public string? OnHoldTaxS { get; set; }

    [JsonProperty(nameof(InGrossRev))]
    public string? InGrossRev { get; set; }

    [JsonProperty(nameof(TaxSumOrg))]
    public string? TaxSumOrg { get; set; }

    [JsonProperty(nameof(TaxSumOrgF))]
    public string? TaxSumOrgF { get; set; }

    [JsonProperty(nameof(TaxSumOrgS))]
    public string? TaxSumOrgS { get; set; }

    [JsonProperty(nameof(OpenTax))]
    public string? OpenTax { get; set; }

    [JsonProperty("OpenTaxFC")]
    public string? OpenTaxFc { get; set; }

    [JsonProperty("OpenTaxSC")]
    public string? OpenTaxSc { get; set; }

    [JsonProperty(nameof(Unencumbrd))]
    public string? Unencumbrd { get; set; }

    [JsonProperty("TaxOnRI")]
    public string? TaxOnRi { get; set; }

    [JsonProperty(nameof(RvsChrgPrc))]
    public string? RvsChrgPrc { get; set; }

    [JsonProperty(nameof(RvsChrgTax))]
    public string? RvsChrgTax { get; set; }

    [JsonProperty("RvsChrgSC")]
    public string? RvsChrgSc { get; set; }

    [JsonProperty("RvsChrgFC")]
    public string? RvsChrgFc { get; set; }

    [JsonProperty(nameof(InFirstIns))]
    public string? InFirstIns { get; set; }

    [JsonProperty(nameof(ExtTaxRate))]
    public string? ExtTaxRate { get; set; }

    [JsonProperty(nameof(ExtTaxSum))]
    public string? ExtTaxSum { get; set; }

    [JsonProperty(nameof(TaxAmtSrc))]
    public string? TaxAmtSrc { get; set; }

    [JsonProperty(nameof(ExtTaxSumF))]
    public string? ExtTaxSumF { get; set; }

    [JsonProperty(nameof(ExtTaxSumS))]
    public string? ExtTaxSumS { get; set; }

    [JsonProperty("CESTrel")]
    public string? CesTrel { get; set; }
}
