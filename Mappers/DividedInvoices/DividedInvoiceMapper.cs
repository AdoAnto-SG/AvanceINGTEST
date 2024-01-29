using FindexMapper.Core.Base;
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
using SQLitePCL;
using System;
using System.Globalization;

namespace Integrador.Mappers.DividedInvoices;

public class DividedInvoiceMapper : IDividedInvoiceMapper
{
    private readonly IControlNumberService _controlNumberService;
    private readonly ISourceProvider _sourceProvider;
    private readonly SenderInfo _senderInfo;

    public DividedInvoiceMapper(IControlNumberService controlNumberService, IOptions<SenderInfo> options, ISourceProvider sourceProvider)
    {
        _controlNumberService = controlNumberService;
        _senderInfo = options.Value;
        _sourceProvider = sourceProvider;
    }

    public List<Invoice> Map(BaseRequest? input, FindexMapper.Service.Enum.Environment environment, List<Inv1> inv1s)
    {
		try
		{
            var result = new List<Invoice>();
			foreach (var item in input?.InvoiceReceiverDivideds ?? new List<InvoiceReceiverDivided>())
			{
                var invoice = new Invoice()
                {
                    Identification = CreateIdentification(input?.Oinv, environment, item),
                    Sender = CreateSender(),
                    Receiver = CreateReceiver(item),
                    DocumentBody = CreateDocumentBody(inv1s, item),
                    Summary = CreateSummary(input?.Oinv, item, inv1s),
                    Appendix = item.GetAppendices()
                };
                result.Add(invoice);
            }
            return result;
		}
		catch (Exception)
		{
            return Enumerable.Empty<Invoice>().ToList();
		}
    }

    private FindexMapper.Core.Base.Invoice.Summary CreateSummary(Oinv? row, InvoiceReceiverDivided item, List<Inv1> inv1s)
    {
        try
        {
            if (row is null || item is null) return new FindexMapper.Core.Base.Invoice.Summary();
            var total = inv1s.Sum(x => Convert.ToDecimal(x.LineTotal));
            decimal percentage = Math.Round(Convert.ToDecimal(item.Porcentaje), 2);
            decimal totalDiscount = Math.Round(Convert.ToDecimal(row.DiscSum) * (percentage / 100), 2);
            decimal subTotal = Math.Round(total * (percentage / 100), 2);
            decimal totalTaxed = subTotal;
            decimal subTotalSales = totalTaxed;
            decimal totalAmount = totalTaxed;
            decimal totalVat = Math.Round(Convert.ToDecimal(row.VatSum) * (percentage / 100), 2);

            bool isTaxed = row.U_TipoVenta_FE == "Gravada";
            bool isNotSubject = row.U_TipoVenta_FE == "No Sujeta";

            return new FindexMapper.Core.Base.Invoice.Summary()
            {
                DiscountPercentage = Convert.ToDecimal(row.DiscPrcnt),
                TotalDiscount = totalDiscount,
                Subtotal = subTotal,
                TotalTaxed = isTaxed && !isNotSubject ? totalTaxed : 0,
                TotalExempt = !isTaxed && !isNotSubject ? totalAmount : 0,
                TotalNotSubject = isNotSubject && !isTaxed ? totalAmount : 0,
                Status = row.U_Formapago_FE?.ToEnum(FindexMapper.Core.Enum.OperationStatus.Cash) ?? FindexMapper.Core.Enum.OperationStatus.Cash,
                Payment = CreatePayment(row, totalAmount),
                SubTotalSales = subTotalSales,
                TotalDescription = totalAmount.ToString("F").ToInvoiceFormat(true) ?? string.Empty,
                TotalAmount = totalAmount,
                TotalToPay = totalAmount,
                TotalVat = totalVat
            };
        }
        catch (Exception)
        {
            return new FindexMapper.Core.Base.Invoice.Summary();
        }
    }

    private static ICollection<Payment>? CreatePayment(Oinv row, decimal amount)
    {
        try
        {
            if (row.U_Formapago_FE != "2") return null;
            return new List<Payment>()
            {
                new()
                {
                    Amount = amount,
                    Code = row.U_MetodoPago_FE ?? string.Empty,
                    Period = 1,
                    Timeframe = "02"
                }
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    private ICollection<FindexMapper.Core.Base.Invoice.DocumentBody> CreateDocumentBody(List<Inv1>? inv1s, InvoiceReceiverDivided? item)
    {
        try
        {
            if(inv1s is null || item is null) return Enumerable.Empty<FindexMapper.Core.Base.Invoice.DocumentBody>().ToList();
            int index = 1;
            var result = inv1s.Select(x =>
            {
                decimal percentage = Math.Round(Convert.ToDecimal(item.Porcentaje), 2);
                decimal unitPrice = Math.Round(Convert.ToDecimal(x.Price), 2);
                bool isPercentageComplete = percentage == 100;
                decimal newUnitPrice = unitPrice * (percentage / 100);
                bool isTaxed = (string?)x?.U_TipoVenta_FE == "Gravada";
                bool isNotSubject = (string?)x?.U_TipoVenta_FE == "No Sujeta";

                decimal vatSum = Convert.ToDecimal(x?.VatSum);
                decimal newVatSum = Math.Round(vatSum * (percentage / 100), 2);

                decimal lineTotal = Math.Round(Convert.ToDecimal(x?.LineTotal), 2);
                decimal newLineTotal = Math.Round(lineTotal * (percentage / 100), 2);
                var isString = x?.U_Concepto1 is string;
                return new FindexMapper.Core.Base.Invoice.DocumentBody()
                {
                    Number = index++,
                    Description = $"{x?.Dscription} {(isString ? (string?)x?.U_Concepto1 : string.Empty)} porcentaje a pagar: {percentage}" ?? $"porcentaje a pagar: {percentage}",
                    Quantity = Convert.ToDecimal(x?.Quantity) == 0 ? 1 : Math.Round(Convert.ToDecimal(x?.Quantity), 2),
                    Unitprice = Math.Round(newUnitPrice, 2),
                    Vat = Math.Round(newVatSum, 2),
                    UnitOfMeasurement = 99, //otro,
                    Type = x?.U_TipoItem_FE?.ToEnum(FindexMapper.Core.Enum.DocumentBodyType.Product) ?? FindexMapper.Core.Enum.DocumentBodyType.Product,
                    TaxableSale = isTaxed && !isNotSubject ? newLineTotal : 0,
                    TaxExemptSale = !isTaxed && !isNotSubject ? newLineTotal : 0,
                    NotSubjectSale = isNotSubject && !isTaxed ? newLineTotal : 0
                };
            }).ToList();
            return result;
        }
        catch (Exception)
        {
            return Enumerable.Empty<FindexMapper.Core.Base.Invoice.DocumentBody>().ToList();
        }    
    }

    private FindexMapper.Core.Base.Invoice.Receiver? CreateReceiver(InvoiceReceiverDivided item)
    {
        try
        {
            if (item is null) return null;
            var documentNumberHasValue = !string.IsNullOrWhiteSpace(item.VatIdUnCmp);
            var economicActivity = _sourceProvider.Catalog(new
            {
                CatalogId = 19,
                Key = item.UActividadEconomicaFE
            });

            var documentNumber = StringUtils.HandleDocumentNumber(item.UDocumentoIdetificacionFE?.ToEnum(FindexMapper.Core.Enum.IdentificationDocumentType.DUI), item.VatIdUnCmp);
            return new FindexMapper.Core.Base.Invoice.Receiver()
            {
                Name = item.CardName,
                DocumentType = documentNumberHasValue ? item.UDocumentoIdetificacionFE?.ToEnum(FindexMapper.Core.Enum.IdentificationDocumentType.DUI) : null,
                DocumentNumber = string.IsNullOrWhiteSpace(documentNumber) ? null : documentNumber,
                Address = CreateReceiverAddress(item),
                EconomicActivity = economicActivity?.Name,
                EconomicActivityCode = economicActivity?.Key,
                Email = item.EMail,
                Phone = item.Phone1
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static Address? CreateReceiverAddress(InvoiceReceiverDivided item)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(item.UDepartamentoFE) || string.IsNullOrWhiteSpace(item.UMunicipioFE) || string.IsNullOrWhiteSpace(item.UPaisFE))
                return null;
            return new Address()
            {
                Complement = item.UPaisFE,
                Department = item.UDepartamentoFE,
                Municipality = item.UMunicipioFE
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    private FindexMapper.Core.Base.Invoice.Sender CreateSender()
    {
        try
        {
            return new FindexMapper.Core.Base.Invoice.Sender()
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
            return new FindexMapper.Core.Base.Invoice.Sender();
        }
    }

    private Identification CreateIdentification(Oinv? row, FindexMapper.Service.Enum.Environment environment, InvoiceReceiverDivided invoice)
    {
        try
        {
            if (row is null) return new Identification();
            var issueDate = GetIssueDate(row.DocDate, row.DocTime);
            return new Identification()
            {
                Type = FindexMapper.Core.Enum.DocumentType.Invoice,
                IssueDate = issueDate,
                IssueTime = issueDate.ToString("HH:mm:ss"),
                Model = FindexMapper.Core.Enum.ModelType.Normal,
                Operation = FindexMapper.Core.Enum.OperationType.Normal,
                Version = FindexMapper.Core.Constants.ConsumidorFinalJsonSchemaVersion,
                Environment = (FindexMapper.Core.Enum.Environment)environment,
                Identifier = CreateIdentifier(row, invoice)
            };
        }
        catch (Exception)
        {
            return new Identification()
            {
                Type = FindexMapper.Core.Enum.DocumentType.Invoice,
                IssueDate = DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).DateTime,
                IssueTime = DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).DateTime.ToString("HH:mm:ss"),
                Model = FindexMapper.Core.Enum.ModelType.Normal,
                Operation = FindexMapper.Core.Enum.OperationType.Normal,
                Version = FindexMapper.Core.Constants.ConsumidorFinalJsonSchemaVersion,
                Environment = (FindexMapper.Core.Enum.Environment)environment,
                Identifier = CreateIdentifier(row, invoice),
                ControlNumber = ""
            };
        }
    }

    private string CreateIdentifier(Oinv? row, InvoiceReceiverDivided invoice)
    {
        var invoiceId = $"{invoice.CardCode}{row?.DocNum}";
        return StringUtils.GenerateSeededGuid(
            nit: _senderInfo.NIT,
            documentType: DocumentType.Invoice, 
            invoiceId: invoiceId ?? string.Empty);
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

            var time = new TimeSpan(issueTime.Hour, issueTime.Minute, 0);
            issueDate = issueDate.Add(time);
            return issueDate;
        }
        catch (Exception)
        {
            return DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).DateTime;
        }
    }
}
