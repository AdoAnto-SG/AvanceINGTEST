using FindexMapper.Core.Base;
using FindexMapper.Core.Base.ExportInvoice;
using FindexMapper.Core.Entities;
using FindexMapper.Core.Enum;
using FindexMapper.Core.Utils;
using FindexMapper.Service.Interfaces;
using FindexMapper.Service.Services.ControlNumbers;
using Integrador.Models;
using Integrador.Models.Base;
using Integrador.Utils;
using Microsoft.Extensions.Options;
using MySqlX.XDevAPI.Relational;

namespace Integrador.Mappers.Export;

public class ExportMapper : IExportMapper
{
    private readonly IControlNumberService _controlNumberService;
    private readonly ISourceProvider _sourceProvider;
    private readonly SenderInfo _senderInfo;

    public ExportMapper(IControlNumberService controlNumberService, IOptions<SenderInfo> options, ISourceProvider sourceProvider)
    {
        _controlNumberService = controlNumberService;
        _senderInfo = options.Value;
        _sourceProvider = sourceProvider;
    }
    public ExportInvoice Map(BaseRequest? input, FindexMapper.Service.Enum.Environment environment, List<Inv1> inv1s)
    {
        try
        {
            if (input is null) return new ExportInvoice();
            var exportInvoice = new ExportInvoice()
            {
                Identification = CreateIdentification(input.Oinv, environment),
                Sender = CreateSender(),
                Receiver = CreateReceiver(input.InvoiceReceiver),
                DocumentBody = CreateDocumentBody(inv1s),
                Summary = CreateSummary(input.Oinv)
            };
            return exportInvoice;
        }
        catch (Exception)
        {
            return new ExportInvoice();
        }
    }

    private static FindexMapper.Core.Base.ExportInvoice.Summary CreateSummary(Oinv? oinv)
    {
        try
        {
            if (oinv is null) return new FindexMapper.Core.Base.ExportInvoice.Summary();
            return new FindexMapper.Core.Base.ExportInvoice.Summary()
            {
                DiscountPercentage = Math.Round(Convert.ToDecimal(oinv.DiscPrcnt), 2),
                TotalDiscount = Math.Round(Convert.ToDecimal(oinv.DiscSum), 2),
                
                TotalTaxed = Math.Round(Convert.ToDecimal(oinv.DocTotal), 2),
                Status = oinv.U_Formapago_FE?.ToEnum(FindexMapper.Core.Enum.OperationStatus.Cash) ?? FindexMapper.Core.Enum.OperationStatus.Cash,
                Payment = CreatePayment(oinv),
                TotalDescription = Math.Round(Convert.ToDecimal(oinv.DocTotal)).ToString("F").ToInvoiceFormat(true) ?? string.Empty,
                TotalAmount = Math.Round(Convert.ToDecimal(oinv.DocTotal), 2),
                TotalToPay = Math.Round(Convert.ToDecimal(oinv.DocTotal), 2),
                Discount = Math.Round(Convert.ToDecimal(oinv.DiscSum), 2)
            };
        }
        catch (Exception)
        {
            return new FindexMapper.Core.Base.ExportInvoice.Summary();
        }
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
                    Amount = Math.Round(Convert.ToDecimal(row.DocTotal) ,2),
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

    private ICollection<FindexMapper.Core.Base.ExportInvoice.DocumentBody> CreateDocumentBody(List<Inv1> inv1s)
    {
        try
        {
            if (inv1s is null) return Enumerable.Empty<FindexMapper.Core.Base.ExportInvoice.DocumentBody>().ToList();
            int index = 1;
            var result = inv1s.Select(x =>
            {
                bool isTaxed = (string?)x?.U_TipoVenta_FE == "Gravada";
                var isString = x?.U_Concepto1 is string;
                return new FindexMapper.Core.Base.ExportInvoice.DocumentBody()
                {
                    Number = index++,
                    Description = $"{x?.Dscription} {(isString ? (string?)x?.U_Concepto1 : string.Empty)}",
                    Quantity = Convert.ToDecimal(x?.Quantity) == 0 ? 1 : Math.Round(Convert.ToDecimal(x?.Quantity), 2),
                    Unitprice = Math.Round(Convert.ToDecimal(x?.Price), 2),
                    UnitOfMeasurement = 99, //otro,
                    TaxableSale = isTaxed ? Math.Round(Convert.ToDecimal(x?.LineTotal), 2) : 0,

                };
            }).ToList();
            return result;
        }
        catch (Exception)
        {
            return Enumerable.Empty<FindexMapper.Core.Base.ExportInvoice.DocumentBody>().ToList();
        }
    }

    private FindexMapper.Core.Base.ExportInvoice.Receiver CreateReceiver(InvoiceReceiver? invoiceReceiver)
    {
        try
        {
            if (invoiceReceiver is null) return new FindexMapper.Core.Base.ExportInvoice.Receiver();
            var country = _sourceProvider.Catalog(new
            {
                CatalogId = 20,
                Key = invoiceReceiver.UPaisFE
            });

            var economicActivity = _sourceProvider.Catalog(new
            {
                CatalogId = 19,
                Key = invoiceReceiver.UActividadEconomicaFE
            });
            return new FindexMapper.Core.Base.ExportInvoice.Receiver()
            {
                DocumentType = IdentificationDocumentType.Other,
                DocumentNumber = string.IsNullOrWhiteSpace(invoiceReceiver.NIT) ? invoiceReceiver.DUI : invoiceReceiver.NIT ?? string.Empty,
                Country = invoiceReceiver.UPaisFE ?? string.Empty,
                CountryName = country?.Name ?? string.Empty,
                Complement = $"{invoiceReceiver.UDepartamentoFE} {invoiceReceiver.UMunicipioFE}",
                Name = invoiceReceiver.CardName ?? string.Empty,
                EconomicActivity = economicActivity.Name,
                Email = invoiceReceiver.EMail,
                Phone = invoiceReceiver.Phone1,
                Type = PersonType.Natural
            };
        }
        catch (Exception)
        {
            return new FindexMapper.Core.Base.ExportInvoice.Receiver();
        }
    }

    private static (IdentificationDocumentType?, string?) HandleDocumentNumber(string? nit, string? dui)
    {
        if (!string.IsNullOrWhiteSpace(nit)) return (IdentificationDocumentType.NIT, nit);

        if (!string.IsNullOrWhiteSpace(dui)) return (IdentificationDocumentType.DUI, dui);

        return (null, null);

    }

    private FindexMapper.Core.Base.ExportInvoice.Sender CreateSender()
    {
        try
        {
            return new FindexMapper.Core.Base.ExportInvoice.Sender()
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
                MHPointOfSaleCode = _senderInfo.MHPointOfSaleCode,
                Type = ItemType.Goods
            };
        }
        catch (Exception)
        {
            return new FindexMapper.Core.Base.ExportInvoice.Sender();
        }
    }

    private FindexMapper.Core.Base.ExportInvoice.Identification CreateIdentification(Oinv? row, FindexMapper.Service.Enum.Environment environment)
    {
        try
        {
            if (row is null) return new FindexMapper.Core.Base.ExportInvoice.Identification();
            var issueDate = row.GetIssueDate();
            return new FindexMapper.Core.Base.ExportInvoice.Identification()
            {
                Type = FindexMapper.Core.Enum.DocumentType.ExportInvoice,
                IssueDate = issueDate,
                IssueTime = issueDate.ToString("HH:mm:ss"),
                Model = FindexMapper.Core.Enum.ModelType.Normal,
                Operation = FindexMapper.Core.Enum.OperationType.Normal,
                Version = FindexMapper.Core.Constants.FacturaExportacionElectronicaJsonSchemaVersion,
                Environment = (FindexMapper.Core.Enum.Environment)environment,
                Identifier = StringUtils.GenerateSeededGuid(
                    nit: _senderInfo.NIT,
                    documentType: DocumentType.ExportInvoice,
                    row.DocNum ?? string.Empty
                )
            };
        }
        catch (Exception)
        {
            return new FindexMapper.Core.Base.ExportInvoice.Identification()
            {
                Type = FindexMapper.Core.Enum.DocumentType.ExportInvoice,
                IssueDate = DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).DateTime,
                IssueTime = DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).DateTime.ToString("HH:mm:ss"),
                Model = FindexMapper.Core.Enum.ModelType.Normal,
                Operation = FindexMapper.Core.Enum.OperationType.Normal,
                Version = FindexMapper.Core.Constants.FacturaExportacionElectronicaJsonSchemaVersion,
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
