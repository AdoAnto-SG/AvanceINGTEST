using Integrador.Converters;
using Newtonsoft.Json;

namespace Integrador.Models.Base;

public class Inv12
{
    [JsonProperty(nameof(DocEntry))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long DocEntry { get; set; }

    [JsonProperty(nameof(TaxId0))]
    public string? TaxId0 { get; set; }

    [JsonProperty(nameof(TaxId1))]
    public string? TaxId1 { get; set; }

    [JsonProperty(nameof(TaxId2))]
    public string? TaxId2 { get; set; }

    [JsonProperty(nameof(TaxId3))]
    public string? TaxId3 { get; set; }

    [JsonProperty(nameof(TaxId4))]
    public string? TaxId4 { get; set; }

    [JsonProperty(nameof(TaxId5))]
    public string? TaxId5 { get; set; }

    [JsonProperty(nameof(TaxId6))]
    public string? TaxId6 { get; set; }

    [JsonProperty(nameof(TaxId7))]
    public string? TaxId7 { get; set; }

    [JsonProperty(nameof(TaxId8))]
    public string? TaxId8 { get; set; }

    [JsonProperty(nameof(TaxId9))]
    public string? TaxId9 { get; set; }

    [JsonProperty(nameof(State))]
    public string? State { get; set; }

    [JsonProperty(nameof(County))]
    public string? County { get; set; }

    [JsonProperty(nameof(Incoterms))]
    public string? Incoterms { get; set; }

    [JsonProperty(nameof(Vehicle))]
    public string? Vehicle { get; set; }

    [JsonProperty(nameof(VidState))]
    public string? VidState { get; set; }

    [JsonProperty(nameof(NfRef))]
    public string? NfRef { get; set; }

    [JsonProperty(nameof(Carrier))]
    public string? Carrier { get; set; }

    [JsonProperty(nameof(QoP))]
    public string? QoP { get; set; }

    [JsonProperty(nameof(PackDesc))]
    public string? PackDesc { get; set; }

    [JsonProperty(nameof(Brand))]
    public string? Brand { get; set; }

    [JsonProperty("NoSU")]
    public string? NoSu { get; set; }

    [JsonProperty(nameof(NetWeight))]
    public string? NetWeight { get; set; }

    [JsonProperty(nameof(GrsWeight))]
    public string? GrsWeight { get; set; }

    [JsonProperty(nameof(LogInstanc))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long LogInstanc { get; set; }

    [JsonProperty(nameof(ObjectType))]
    [JsonConverter(typeof(PurpleParseStringConverter))]
    public long ObjectType { get; set; }

    [JsonProperty(nameof(TaxId10))]
    public string? TaxId10 { get; set; }

    [JsonProperty(nameof(TransCat))]
    public string? TransCat { get; set; }

    [JsonProperty(nameof(FormNo))]
    public string? FormNo { get; set; }

    [JsonProperty(nameof(TaxId11))]
    public string? TaxId11 { get; set; }

    [JsonProperty(nameof(StreetS))]
    public string? StreetS { get; set; }

    [JsonProperty(nameof(BlockS))]
    public string? BlockS { get; set; }

    [JsonProperty(nameof(BuildingS))]
    public string? BuildingS { get; set; }

    [JsonProperty(nameof(CityS))]
    public string? CityS { get; set; }

    [JsonProperty(nameof(ZipCodeS))]
    public string? ZipCodeS { get; set; }

    [JsonProperty(nameof(CountyS))]
    public string? CountyS { get; set; }

    [JsonProperty(nameof(StateS))]
    public string? StateS { get; set; }

    [JsonProperty(nameof(CountryS))]
    public string? CountryS { get; set; }

    [JsonProperty(nameof(AddrTypeS))]
    public string? AddrTypeS { get; set; }

    [JsonProperty(nameof(StreetNoS))]
    public string? StreetNoS { get; set; }

    [JsonProperty(nameof(StreetB))]
    public string? StreetB { get; set; }

    [JsonProperty(nameof(BlockB))]
    public string? BlockB { get; set; }

    [JsonProperty(nameof(BuildingB))]
    public string? BuildingB { get; set; }

    [JsonProperty(nameof(CityB))]
    public string? CityB { get; set; }

    [JsonProperty(nameof(ZipCodeB))]
    public string? ZipCodeB { get; set; }

    [JsonProperty(nameof(CountyB))]
    public string? CountyB { get; set; }

    [JsonProperty(nameof(StateB))]
    public string? StateB { get; set; }

    [JsonProperty(nameof(CountryB))]
    public string? CountryB { get; set; }

    [JsonProperty(nameof(AddrTypeB))]
    public string? AddrTypeB { get; set; }

    [JsonProperty(nameof(StreetNoB))]
    public string? StreetNoB { get; set; }

    [JsonProperty("ImpORExp")]
    public string? ImpOrExp { get; set; }

    [JsonProperty(nameof(Vat))]
    public string? Vat { get; set; }

    [JsonProperty(nameof(AltCrdNamB))]
    public string? AltCrdNamB { get; set; }

    [JsonProperty(nameof(AltTaxIdB))]
    public string? AltTaxIdB { get; set; }

    [JsonProperty(nameof(Address2S))]
    public string? Address2S { get; set; }

    [JsonProperty(nameof(Address3S))]
    public string? Address3S { get; set; }

    [JsonProperty(nameof(Address2B))]
    public string? Address2B { get; set; }

    [JsonProperty(nameof(Address3B))]
    public string? Address3B { get; set; }

    [JsonProperty(nameof(MainUsage))]
    public string? MainUsage { get; set; }

    [JsonProperty(nameof(GlbLocNumS))]
    public string? GlbLocNumS { get; set; }

    [JsonProperty(nameof(GlbLocNumB))]
    public string? GlbLocNumB { get; set; }

    [JsonProperty("CollectDT")]
    public string? CollectDt { get; set; }

    [JsonProperty("TransprtDT")]
    public string? TransprtDt { get; set; }

    [JsonProperty("TransprtRS")]
    public string? TransprtRs { get; set; }

    [JsonProperty(nameof(TaxId12))]
    public string? TaxId12 { get; set; }

    [JsonProperty(nameof(TaxId13))]
    public string? TaxId13 { get; set; }

    [JsonProperty(nameof(ImpExpNo))]
    public string? ImpExpNo { get; set; }

    [JsonProperty(nameof(ImpExpDate))]
    public string? ImpExpDate { get; set; }

    [JsonProperty("BpGSTType")]
    public string? BpGstType { get; set; }

    [JsonProperty("BpGSTN")]
    public string? BpGstn { get; set; }

    [JsonProperty(nameof(BpStateCod))]
    public string? BpStateCod { get; set; }

    [JsonProperty("BPStatGSTN")]
    public string? BpStatGstn { get; set; }

    [JsonProperty("LocGSTType")]
    public string? LocGstType { get; set; }

    [JsonProperty("LocGSTN")]
    public string? LocGstn { get; set; }

    [JsonProperty(nameof(LocStatCod))]
    public string? LocStatCod { get; set; }

    [JsonProperty("LocStaGSTN")]
    public string? LocStaGstn { get; set; }

    [JsonProperty(nameof(BpCountry))]
    public string? BpCountry { get; set; }

    [JsonProperty(nameof(OrigImpNo))]
    public string? OrigImpNo { get; set; }

    [JsonProperty(nameof(OrigImpDat))]
    public string? OrigImpDat { get; set; }

    [JsonProperty(nameof(ExportType))]
    public string? ExportType { get; set; }

    [JsonProperty(nameof(PortCode))]
    public string? PortCode { get; set; }

    [JsonProperty(nameof(BoEValue))]
    public string? BoEValue { get; set; }

    [JsonProperty("IsIGSTAct")]
    public string? IsIgstAct { get; set; }

    [JsonProperty(nameof(ClaimRefun))]
    public string? ClaimRefun { get; set; }

    [JsonProperty(nameof(TaxRateDif))]
    public string? TaxRateDif { get; set; }

    [JsonProperty("BPGdsIssP")]
    public string? BpGdsIssP { get; set; }

    [JsonProperty("CNPJGIP")]
    public string? Cnpjgip { get; set; }

    [JsonProperty("CPFGIP")]
    public string? Cpfgip { get; set; }

    [JsonProperty("StreetGIP")]
    public string? StreetGip { get; set; }

    [JsonProperty("StrtNoGIP")]
    public string? StrtNoGip { get; set; }

    [JsonProperty("BldngGIP")]
    public string? BldngGip { get; set; }

    [JsonProperty("ZipGIP")]
    public string? ZipGip { get; set; }

    [JsonProperty("BlockGIP")]
    public string? BlockGip { get; set; }

    [JsonProperty("CityGIP")]
    public string? CityGip { get; set; }

    [JsonProperty("CountyGIP")]
    public string? CountyGip { get; set; }

    [JsonProperty("StateGIP")]
    public string? StateGip { get; set; }

    [JsonProperty("CountryGIP")]
    public string? CountryGip { get; set; }

    [JsonProperty("PhoneGIP")]
    public string? PhoneGip { get; set; }

    [JsonProperty("EMailGIP")]
    public string? EMailGip { get; set; }

    [JsonProperty("DptDateGIP")]
    public string? DptDateGip { get; set; }

    [JsonProperty("BPDelivryP")]
    public string? BpDelivryP { get; set; }

    [JsonProperty("CNPJDlvryP")]
    public string? CnpjDlvryP { get; set; }

    [JsonProperty("CPFDlvryP")]
    public string? CpfDlvryP { get; set; }

    [JsonProperty(nameof(StrtDlvryP))]
    public string? StrtDlvryP { get; set; }

    [JsonProperty(nameof(StrNoDlvrP))]
    public string? StrNoDlvrP { get; set; }

    [JsonProperty(nameof(BldDlvryP))]
    public string? BldDlvryP { get; set; }

    [JsonProperty(nameof(ZipDlvryP))]
    public string? ZipDlvryP { get; set; }

    [JsonProperty(nameof(BlckDlvryP))]
    public string? BlckDlvryP { get; set; }

    [JsonProperty(nameof(CityDlvryP))]
    public string? CityDlvryP { get; set; }

    [JsonProperty(nameof(CntyDlvryP))]
    public string? CntyDlvryP { get; set; }

    [JsonProperty(nameof(StatDlvryP))]
    public string? StatDlvryP { get; set; }

    [JsonProperty(nameof(CtryDlvryP))]
    public string? CtryDlvryP { get; set; }

    [JsonProperty(nameof(FoneDlvryP))]
    public string? FoneDlvryP { get; set; }

    [JsonProperty(nameof(MailDlvryP))]
    public string? MailDlvryP { get; set; }

    [JsonProperty(nameof(DpDtDlvryP))]
    public string? DpDtDlvryP { get; set; }

    [JsonProperty("AuthedCNPJ")]
    public string? AuthedCnpj { get; set; }

    [JsonProperty(nameof(TaxId14))]
    public string? TaxId14 { get; set; }
}
