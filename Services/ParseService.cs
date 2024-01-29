using System.Globalization;
using FindexMapper.Core.Entities;
using FindexMapper.Service.Interfaces;
using Google.Protobuf;
using Integrador.Converters;
using Integrador.Mappers.Cancellations;
using Integrador.Mappers.DividedInvoices;
using Integrador.Mappers.Export;
using Integrador.Mappers.FromApi;
using Integrador.Mappers.Invoices;
using Integrador.Mappers.TaxCredits;
using Integrador.Models.Base;
using Integrador.Models.Invoices;
using Integrador.Models.sections;
using Integrador.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Integrador.Services;

public class ParseService : IParseService
{
	private readonly IInvoiceMapper _invoiceMapper;
	private readonly ITaxCreditMapper _taxCreditMapper;
	private readonly IDividedInvoiceMapper _dividedInvoiceMapper;
	private readonly IInvoiceFromApiMapper _invoiceFromApiMapper;
	private readonly IExportMapper _exportMapper;
	private readonly IExcludedSubjectMapper _excludedSubjectMapper;
	private readonly ICancellationMapper _cancellationMapper;

    public ParseService(IInvoiceMapper invoiceMapper, ITaxCreditMapper taxCreditMapper, IDividedInvoiceMapper dividedInvoiceMapper, IInvoiceFromApiMapper invoiceFromApiMapper, IExportMapper exportMapper, IExcludedSubjectMapper excludedSubjectMapper, ICancellationMapper cancellationMapper)
    {
        _invoiceMapper = invoiceMapper;
        _taxCreditMapper = taxCreditMapper;
        _dividedInvoiceMapper = dividedInvoiceMapper;
        _invoiceFromApiMapper = invoiceFromApiMapper;
        _exportMapper = exportMapper;
        _excludedSubjectMapper = excludedSubjectMapper;
        _cancellationMapper = cancellationMapper;
    }

    public object? Parse(JToken resource, FindexMapper.Service.Enum.Environment environment)
	{
		if(resource is JArray)
		{
			var result = resource.ToList();
			var request = result.Select(x => x.ToString()).ToList();
            var requests = Utilities.GetRequests(request ?? new List<string>());
            var invoiceRequest = requests[0];
            if (invoiceRequest is null) return new Invoice();
            return _invoiceFromApiMapper.MapToInvoice(invoiceRequest, environment);
        }
        var documentBody = resource["BOM"]?["BO"]?["INV1"]?["row"] ?? resource["BOM"]?["BO"]?["PCH1"]?["row"] ;

		List<Inv1> inv1s = new();
		if (documentBody is JArray)
		{
			inv1s = documentBody?.ToObject<List<Inv1>>() ?? new List<Inv1>();
		}
		if(documentBody is JObject)
		{
			var result = documentBody?.ToObject<Inv1?>();
			if (result is not null) inv1s.Add(result);
		}

		
		var documentType = inv1s.FirstOrDefault()?.TaxCode;
        var baseSource = (resource["BOM"]?["BO"]) ?? throw new JsonSerializationException("\"Content\" property is missing.");

		var oinv = baseSource["OINV"]?["row"];
        var isIns = oinv?["isIns"]?.Value<string?>();
		var uIsExport = oinv?["U_EsExportacion"]?.Value<string?>();
		var uExcludedSubjectFE = baseSource["OPCH"]?["row"]?["U_SujetoExcluido_FE"]?.Value<string?>();

		var isDividedInvoice = !string.IsNullOrWhiteSpace(isIns) && isIns == "Y";
		if(isDividedInvoice) return  _dividedInvoiceMapper.Map(Deserialize(resource.ToString()), environment, inv1s);

		var isExport = !isDividedInvoice && uIsExport == "Y";
		if(isExport) return _exportMapper.Map(Deserialize(resource.ToString()), environment, inv1s);

		var isExcludedSubject = !string.IsNullOrWhiteSpace(uExcludedSubjectFE) && uExcludedSubjectFE == "1";
		if(isExcludedSubject) return _excludedSubjectMapper.Map(DeserializeToExcludedSubject(resource.ToString()), environment, inv1s);

		var isTaxCredit = documentType == "IVA_FISC";
		if (isTaxCredit) return _taxCreditMapper.Map(Deserialize(resource.ToString()), environment, inv1s);

		var isInvoice = !isDividedInvoice && !isExport && !isTaxCredit;
		if (isInvoice) return _invoiceMapper.Map(Deserialize(resource.ToString()), environment, inv1s);
		
		
		
		
        throw new InvalidOperationException("El tipo de DTE no es soportado");

    }
	public object? Invalidate(JToken resource, FindexMapper.Service.Enum.Environment environment)
	{
		var docDate = resource["BOM"]?["BO"]?["OINV"]?["row"]?["DocDate"]?.Value<string>() ?? resource["BOM"]?["BO"]?["OPCH"]?["row"]?["DocDate"]?.Value<string>();
		var dateFormat = "yyyyMMdd";
		var date = DateTime.ParseExact(docDate ?? "20231205", dateFormat, CultureInfo.InvariantCulture);
		var dateToCompare = new DateTime(2024, 01, 03);
		if(date < dateToCompare) return new {};
		var documentBody = resource["BOM"]?["BO"]?["INV1"]?["row"] ?? resource["BOM"]?["BO"]?["PCH1"]?["row"] ;
		List<Inv1> inv1s = new();
		if (documentBody is JArray)
		{
			inv1s = documentBody?.ToObject<List<Inv1>>() ?? new List<Inv1>();
		}
		if(documentBody is JObject)
		{
			var result = documentBody?.ToObject<Inv1?>();
			if (result is not null) inv1s.Add(result);
		}
		var canceledProperty = resource["BOM"]?["BO"]?["OINV"]?["row"]?["CANCELED"] ?? resource["BOM"]?["BO"]?["OPCH"]?["row"]?["CANCELED"];
		var canceled = canceledProperty?.Value<string>();
		if(canceled == "C") return _cancellationMapper.Map(Deserialize(resource.ToString()), environment, inv1s);
		return new {};
	}
	private static BaseRequest Deserialize(string json)
	{
        return JsonConvert.DeserializeObject<BaseRequest>(json ?? string.Empty, new RequestConverter()) ?? new BaseRequest();
    }

	private static ExcludedSubjectRequest DeserializeToExcludedSubject(string json)
	{
		return JsonConvert.DeserializeObject<ExcludedSubjectRequest>(json ?? string.Empty, new RequestConverter()) ?? new ExcludedSubjectRequest();	 
	}
}
