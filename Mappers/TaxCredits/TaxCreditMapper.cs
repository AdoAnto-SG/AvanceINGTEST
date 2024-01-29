using Microsoft.Extensions.Options;
using FindexMapper.Core.Base;
using FindexMapper.Core.Entities;
using FindexMapper.Core.Utils;
using FindexMapper.Service.Services.ControlNumbers;
using Integrador.Models;
using Integrador.Models.Base;
using MySqlX.XDevAPI.Relational;
using FindexMapper.Service.Interfaces;
using Integrador.Models.sections;
using FindexMapper.Core.Enum;
using Integrador.Utils;

namespace Integrador.Mappers.TaxCredits;

public sealed class TaxCreditMapper : ITaxCreditMapper
{
    private readonly IControlNumberService _controlNumberService;
    private readonly ISourceProvider _sourceProvider;
    private readonly SenderInfo _senderInfo;

    public TaxCreditMapper(IControlNumberService controlNumberService, IOptions<SenderInfo> options, ISourceProvider sourceProvider)
    {
        _controlNumberService = controlNumberService;
        _senderInfo = options.Value;
        _sourceProvider = sourceProvider;
    }

    public TaxCreditReceipt Map(BaseRequest? input, FindexMapper.Service.Enum.Environment environment, List<Inv1> inv1s)
    {
		try
		{
			if (input is null) return new TaxCreditReceipt();
			var taxCredit = new TaxCreditReceipt()
			{
                Identification = CreateIdentification(input.Oinv, environment),
                Sender = CreateSender(),
                Receiver = CreateReceiver(input.InvoiceReceiver),
                DocumentBody = CreateDocumentBody(inv1s),
                Summary = CreateSummary(input.Oinv, inv1s)
            };
			return taxCredit;

        }
		catch (Exception)
		{
			return new TaxCreditReceipt();
		}
    }

    private FindexMapper.Core.Base.TaxCreditReceipt.Summary CreateSummary(Oinv? oinv,  List<Inv1> inv1s)
    {
        try
        {
            if (oinv is null) return new FindexMapper.Core.Base.TaxCreditReceipt.Summary();

            bool isTaxed = oinv.U_TipoVenta_FE == "Gravada";
            bool isNotSubject = oinv.U_TipoVenta_FE == "No Sujeta";
            var total = Math.Round(inv1s.Sum(x => Convert.ToDecimal(x.LineTotal)), 2);
            var totalAmount = Math.Round(Convert.ToDecimal(oinv.DocTotal), 2);
            var vatSum = Math.Round(Convert.ToDecimal(oinv.VatSum), 2);
            return new FindexMapper.Core.Base.TaxCreditReceipt.Summary()
            {
                DiscountPercentage = Math.Round(Convert.ToDecimal(oinv.DiscPrcnt), 2),
                TotalDiscount = Math.Round(Convert.ToDecimal(oinv.DiscSum), 2),
                Subtotal = total,
                TotalTaxed = isTaxed && !isNotSubject ? total : 0,
                TotalExempt = !isTaxed && !isNotSubject ? totalAmount : 0,
                TotalNotSubject = isNotSubject && !isTaxed ? totalAmount : 0,
                Status = oinv.U_Formapago_FE?.ToEnum(FindexMapper.Core.Enum.OperationStatus.Cash) ?? FindexMapper.Core.Enum.OperationStatus.Cash,
                Payment = CreatePayment(oinv),
                SubTotalSales = total,
                TotalDescription = totalAmount.ToString("F").ToInvoiceFormat(true) ?? string.Empty,
                TotalAmount = totalAmount,
                TotalToPay = totalAmount,
                Tribute = CreateTributes(isTaxed, vatSum),
            };
        }
        catch (Exception)
        {
            return new FindexMapper.Core.Base.TaxCreditReceipt.Summary();
        }
    }

    private static ICollection<Tribute>? CreateTributes(bool isTaxed, decimal vatSum)
    {
        if(!isTaxed) return null;
        return new HashSet<Tribute>()
        {
            new()
            {
                Codigo = "20",
                Description = "Impuesto al Valor Agregado 13%",
                Value = vatSum
            }
        };
    }

    private static ICollection<Payment>? CreatePayment(Oinv row)
    {
        try
        {
            if (row.U_Formapago_FE != "2") return null;
            return new List<Payment>()
            {
                new()
                {
                    Amount = Math.Round(Convert.ToDecimal(row.DocTotal), 2),
                    Code = row.U_MetodoPago_FE ?? string.Empty,
                    Period = Convert.ToDecimal(row.U_TipoPlazo_FE),
                    Timeframe = row.U_CantidadTiempo_FE
                }
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    private ICollection<FindexMapper.Core.Base.TaxCreditReceipt.DocumentBody> CreateDocumentBody(List<Inv1>? inv1s)
    {
        try
        {
            if(inv1s is null) return Enumerable.Empty<FindexMapper.Core.Base.TaxCreditReceipt.DocumentBody>().ToList();
            int index = 1;
            var result = inv1s.Select(x =>
            {
                bool isTaxed = (string?)x?.U_TipoVenta_FE == "Gravada";
                var isString = x?.U_Concepto1 is string;
                return new FindexMapper.Core.Base.TaxCreditReceipt.DocumentBody()
                {
                    Number = index++,
                    Description = $"{x?.Dscription} {(isString ? (string?)x?.U_Concepto1 : string.Empty)}",
                    Quantity = Convert.ToDecimal(x?.Quantity) == 0 ? 1 : Convert.ToDecimal(x?.Quantity),
                    Unitprice = Convert.ToDecimal(x?.Price),
                    UnitOfMeasurement = 99, //otro,
                    Type = x?.U_TipoItem_FE?.ToEnum(FindexMapper.Core.Enum.DocumentBodyType.Product) ?? FindexMapper.Core.Enum.DocumentBodyType.Product,
                    TaxableSale = isTaxed ? Math.Round(Convert.ToDecimal(x?.LineTotal), 2) : 0,
                    Tributes = new HashSet<string>(){"20"}
                };
            }).ToList();
            return result;
        }
        catch (Exception)
        {
            return Enumerable.Empty<FindexMapper.Core.Base.TaxCreditReceipt.DocumentBody>().ToList();
        }
    }

    private FindexMapper.Core.Base.TaxCreditReceipt.Receiver CreateReceiver(InvoiceReceiver? oinv)
    {
        try
        {
            if (oinv is null) return new FindexMapper.Core.Base.TaxCreditReceipt.Receiver();
            var economicActivity = _sourceProvider.Catalog(new
            {
                CatalogId = 19,
                Key = oinv.UActividadEconomicaFE
            });
            return new FindexMapper.Core.Base.TaxCreditReceipt.Receiver()
            {
                Nit = oinv.NIT ?? string.Empty,
                Nrc = StringUtils.RemoveGuionFromString(oinv.NRC)?.Replace("\\", "").Replace("/", "") ?? string.Empty,
                Name = oinv.CardName ?? string.Empty,
                Address = new Address()
                {
                    Complement = oinv.UPaisFE ?? string.Empty,
                    Department = oinv.UDepartamentoFE ?? string.Empty,
                    Municipality = oinv.UMunicipioFE ?? string.Empty
                },
                EconomicActivity = economicActivity.Name ?? string.Empty,
                EconomicActivityCode = oinv.UActividadEconomicaFE ?? string.Empty,
                Email = oinv.EMail ?? string.Empty,
                Phone = oinv.Phone1
            };
        }
        catch (Exception)
        {
            return new FindexMapper.Core.Base.TaxCreditReceipt.Receiver();
        }
    }

    private static Address CreateReceiverAddress(Oinv? oinv, List<Inv12>? inv12)
    {
        try
        {
            if (oinv is null || inv12 is null) return new Address();
            var complement = inv12?.FirstOrDefault()?.Address2S;
            return new Address()
            {
                Complement = complement ?? string.Empty,
                Department = string.Empty,
                Municipality = string.Empty
            };

        }
        catch (Exception)
        {

            throw;
        }
    }

    private static (string?, FindexMapper.Core.Enum.IdentificationDocumentType?) HandleDocumentNumber(string? documentType, string? documentNumber)
    {
        try
        {
            if (string.IsNullOrEmpty(documentType) || string.IsNullOrEmpty(documentNumber)) return (null, null);
            if (documentType == "NA") return (null, null);
            var docType = documentType.ToEnum(FindexMapper.Core.Enum.IdentificationDocumentType.DUI);
            return (documentNumber, docType);
        }
        catch (Exception)
        {
            return (null, null);
        }
    }

    private FindexMapper.Core.Base.TaxCreditReceipt.Sender CreateSender()
    {
        try
        {
            return new FindexMapper.Core.Base.TaxCreditReceipt.Sender()
            {
                Address = new Address()
                {
                    Complement = _senderInfo.AddressComplement,
                    Department = _senderInfo.Department,
                    Municipality = _senderInfo.Municipality
                },
                EconomicActivity = _senderInfo.EconomicActivity,
                EconomicActivityCode = _senderInfo.EconomicActivityCode,
                Email = _senderInfo.Email,
                Establishment = FindexMapper.Core.Enum.EstablishmentType.Branch,
                Name = _senderInfo.Name,
                Nit = _senderInfo.NIT,
                Nrc = _senderInfo.NRC,
                Phone = _senderInfo.Phone,
                TradeName = _senderInfo.TradeName,
                EstablishmentCode = _senderInfo.EstablishmentCode,
                PointOfSaleCode = _senderInfo.PointOfSaleCode,
                MHEstablishmentCode = _senderInfo.MHEstablishmentCode,
                MHPointOfSaleCode = _senderInfo.MHPointOfSaleCode
            };
        }
        catch (Exception)
        {
            return new FindexMapper.Core.Base.TaxCreditReceipt.Sender();
        }
    }

    private Identification CreateIdentification(Oinv? oinv, FindexMapper.Service.Enum.Environment environment)
    {
        try
        {
            if (oinv is null) return new Identification();
            var issueDate = oinv.GetIssueDate();
            return new Identification()
            {
                Type = FindexMapper.Core.Enum.DocumentType.TaxCreditReceipt,
                IssueDate = issueDate,
                IssueTime = issueDate.ToString("HH:mm:ss"),
                Model = FindexMapper.Core.Enum.ModelType.Normal,
                Operation = FindexMapper.Core.Enum.OperationType.Normal,
                Version = FindexMapper.Core.Constants.CreditoFiscalJsonSchemaVersion,
                Environment = (FindexMapper.Core.Enum.Environment)environment,
                Identifier = StringUtils.GenerateSeededGuid(
                    nit: _senderInfo.NIT,
                    documentType: DocumentType.TaxCreditReceipt,
                    oinv.DocNum ?? string.Empty
                )
            };
        }
        catch (Exception)
        {
            return new Identification()
            {
                Type = FindexMapper.Core.Enum.DocumentType.TaxCreditReceipt,
                IssueDate = DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).DateTime,
                IssueTime = DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).DateTime.ToString("HH:mm:ss"),
                Model = FindexMapper.Core.Enum.ModelType.Normal,
                Operation = FindexMapper.Core.Enum.OperationType.Normal,
                Version = FindexMapper.Core.Constants.CreditoFiscalJsonSchemaVersion,
                Environment = (FindexMapper.Core.Enum.Environment)environment,
                Identifier = Guid.NewGuid().ToString("D").ToUpperInvariant(),
                ControlNumber = ""
            };
        }
    }

    private static DateTime GetIssueDate(string? docDate, string? docTime)
    {
        try
        {
            if (string.IsNullOrEmpty(docDate) || string.IsNullOrEmpty(docTime))
            {
                return DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).DateTime;
            }
            string dateFormat = "yyyyMMdd";
            if (!DateTime.TryParseExact(
                docDate,
                dateFormat,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var issueDate))
            {
                issueDate = DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).DateTime;
            }

            string timeFormat = "HHmm";

            if (!DateTime.TryParseExact(
                docTime,
                timeFormat,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var issueTime))
            {
                issueTime = DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).DateTime;
            }

            issueDate.AddHours(issueTime.Hour).AddMinutes(issueTime.Minute);

            return issueDate;
        }
        catch (Exception)
        {
            return DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).DateTime;
        }
    }
}
