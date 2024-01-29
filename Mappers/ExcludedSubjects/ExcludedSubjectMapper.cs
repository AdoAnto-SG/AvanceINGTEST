using FindexMapper.Core.Base;
using FindexMapper.Core.Entities;
using FindexMapper.Core.Enum;
using FindexMapper.Core.Utils;
using FindexMapper.Service.Interfaces;
using Integrador.Models;
using Integrador.Models.Base;
using Integrador.Utils;
using Microsoft.Extensions.Options;

namespace Integrador;

public class ExcludedSubjectMapper : IExcludedSubjectMapper
{
    private readonly ISourceProvider _sourceProvider;
    private readonly SenderInfo _senderInfo;

    public ExcludedSubjectMapper(ISourceProvider sourceProvider, IOptions<SenderInfo> options)
    {
        _sourceProvider = sourceProvider;
        _senderInfo = options.Value;
    }

    public ExcludedSubjectInvoice Map(ExcludedSubjectRequest? input, FindexMapper.Service.Enum.Environment environment, List<Inv1> inv1s)
    {
        try
        {
            if(input is null) return new ExcludedSubjectInvoice();
            var excludedSubjectInvoice = new ExcludedSubjectInvoice()
            {
                Identification = CreateIdentification(input.Oinv, environment),
                Sender = CreateSender(),
                Receiver = CreateReceiver(input.InvoiceReceiver),
                DocumentBody = CreateDocumentBody(inv1s),
                Summary = CreateSummary(input.Oinv, input.Taxes)
            };
            return excludedSubjectInvoice;
        }
        catch (System.Exception)
        {
            return new ExcludedSubjectInvoice();
        }
    }

    private FindexMapper.Core.Base.ExcludedSubjectInvoice.Summary CreateSummary(Oinv? oinv, List<PCH5>? taxes)
    {
        try
        {
            if (oinv is null) return new FindexMapper.Core.Base.ExcludedSubjectInvoice.Summary();
            var iva = taxes?.FirstOrDefault(x => x.TaxCode == "13RT");
            var isr = taxes?.FirstOrDefault(x => x.TaxCode == "10RF");
            var totalToPay = oinv.GetTotalToPay(vat: Math.Round(Convert.ToDecimal(iva?.WTAmnt), 2), isr: Math.Round(Convert.ToDecimal(isr?.WTAmnt), 2));
            return new FindexMapper.Core.Base.ExcludedSubjectInvoice.Summary()
            {
                DiscountPercentage = Math.Round(Convert.ToDecimal(oinv.DiscPrcnt), 2),
                TotalDiscount = Math.Round(Convert.ToDecimal(oinv.DiscSum), 2),
                Subtotal = Math.Round(Convert.ToDecimal(oinv.DocTotal), 2),
                Status = oinv.U_Formapago_FE?.ToEnum(FindexMapper.Core.Enum.OperationStatus.Cash) ?? FindexMapper.Core.Enum.OperationStatus.Cash,
                TotalDescription = totalToPay.ToString().ToInvoiceFormat(true) ?? string.Empty,
                TotalToPay = totalToPay,
                TotalPurchase = Math.Round(Convert.ToDecimal(oinv.DocTotal), 2),
                WithheldIncome = Math.Round(Convert.ToDecimal(isr?.WTAmnt), 2),
                WithheldVat = Math.Round(Convert.ToDecimal(iva?.WTAmnt), 2),
                Payment = CreatePayment(oinv, totalToPay)
            };
        }
        catch (Exception)
        {
            return new FindexMapper.Core.Base.ExcludedSubjectInvoice.Summary();
        }
    }

    private static ICollection<Payment>? CreatePayment(Oinv row, decimal totalToPay)
    {
        try
		{
            var isCredit = row.U_Formapago_FE == "2";
			return new List<Payment>()
			{
				new Payment()
				{
					Amount = totalToPay,
					Code = row.U_MetodoPago_FE ?? string.Empty,
                    Period = isCredit ? Convert.ToDecimal(row.U_TipoPlazo_FE) : null,
                    Timeframe = isCredit ? row.U_CantidadTiempo_FE : null
                }
			};
		}
		catch (Exception)
		{
			return null;
		}
    }

    private ICollection<FindexMapper.Core.Base.ExcludedSubjectInvoice.DocumentBody> CreateDocumentBody(List<Inv1> inv1s)
    {
        try
        {
            if(inv1s is null) return Enumerable.Empty<FindexMapper.Core.Base.ExcludedSubjectInvoice.DocumentBody>().ToList();
            int index = 1;
            var result = inv1s.Select(x =>
            {
                var isString = x?.U_Concepto1 is string;
                return new FindexMapper.Core.Base.ExcludedSubjectInvoice.DocumentBody()
                {
                    Number = index++,
                    Description = $"{x?.Dscription} {(isString ? (string?)x?.U_Concepto1 : string.Empty)}",
                    Quantity = Convert.ToDecimal(x?.Quantity) == 0 ? 1 : Math.Round(Convert.ToDecimal(x?.Quantity), 2),
                    Unitprice = Math.Round(Convert.ToDecimal(x?.Price), 2),
                    UnitOfMeasurement = 99, //otro,
                    Type = x?.U_TipoItem_FE?.ToEnum(DocumentBodyType.Product) ?? DocumentBodyType.Product,
                    Purchase = Math.Round(Convert.ToDecimal(x?.LineTotal), 2)
                };
            }).ToList();
            return result;
        }
        catch (Exception)
        {
            return Enumerable.Empty<FindexMapper.Core.Base.ExcludedSubjectInvoice.DocumentBody>().ToList();
        }
    }

    private FindexMapper.Core.Base.ExcludedSubjectInvoice.Receiver CreateReceiver(InvoiceReceiver? invoiceReceiver)
    {
        try
        {
            if (invoiceReceiver is null) return new FindexMapper.Core.Base.ExcludedSubjectInvoice.Receiver();

            var economicActivity = _sourceProvider.Catalog(new
            {
                CatalogId = 19,
                Key = invoiceReceiver.UActividadEconomicaFE
            });

            var documentNumberResult = invoiceReceiver.HandleDocumentNumber();
            var economicActivityCodeHasValue = !string.IsNullOrWhiteSpace(invoiceReceiver.UActividadEconomicaFE);
            return new FindexMapper.Core.Base.ExcludedSubjectInvoice.Receiver()
            {
                DocumentType = documentNumberResult.Item1,
                DocumentNumber = documentNumberResult.Item2,
                Name = invoiceReceiver.CardName ?? string.Empty,
                EconomicActivity = economicActivityCodeHasValue ? economicActivity.Name : null,
                EconomicActivityCode = economicActivityCodeHasValue ? invoiceReceiver.UActividadEconomicaFE : null,
                Email = invoiceReceiver.EMail,
                Phone = invoiceReceiver.Phone1,
                Address = invoiceReceiver.CreateReceiverAddress() ?? new Address()  
            };
        }
        catch (System.Exception)
        {
            return new FindexMapper.Core.Base.ExcludedSubjectInvoice.Receiver();
        }
    }

    private static (IdentificationDocumentType?, string?) HandleDocumentNumber(string? nit, string? dui)
    {
        if (!string.IsNullOrWhiteSpace(nit)) return (IdentificationDocumentType.NIT, nit);

        if (!string.IsNullOrWhiteSpace(dui)) return (IdentificationDocumentType.DUI, dui);

        return (null, null);
    }

    private FindexMapper.Core.Base.ExcludedSubjectInvoice.Sender CreateSender()
    {
        try
        {
            return new FindexMapper.Core.Base.ExcludedSubjectInvoice.Sender()
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
                Name = _senderInfo.Name,
                Nit = _senderInfo.NIT,
                Nrc = _senderInfo.NRC,
                Phone = _senderInfo.Phone,
                EstablishmentCode = _senderInfo.EstablishmentCode,
                PointOfSaleCode = _senderInfo.PointOfSaleCode,
                MHEstablishmentCode = _senderInfo.MHEstablishmentCode,
                MHPointOfSaleCode = _senderInfo.MHPointOfSaleCode
            };
        }
        catch (Exception)
        {
            return new FindexMapper.Core.Base.ExcludedSubjectInvoice.Sender();
        }
    }

    private Identification CreateIdentification(Oinv? row, FindexMapper.Service.Enum.Environment environment)
    {
        try
        {
            if (row is null) return new Identification();
            var issueDate = row.GetIssueDate();
            return new Identification()
            {
                Type = FindexMapper.Core.Enum.DocumentType.ExcludedSubjectInvoice,
                IssueDate = issueDate,
                IssueTime = issueDate.ToString("HH:mm:ss"),
                Model = FindexMapper.Core.Enum.ModelType.Normal,
                Operation = FindexMapper.Core.Enum.OperationType.Normal,
                Version = FindexMapper.Core.Constants.FacturaSujetoExcluidoJsonSchemaVersion,
                Environment = (FindexMapper.Core.Enum.Environment)environment,
                Identifier = StringUtils.GenerateSeededGuid(
                    nit: _senderInfo.NIT,
                    documentType: DocumentType.ExcludedSubjectInvoice,
                    row.DocNum ?? string.Empty
                )
            };
        }
        catch (Exception)
        {
            return new Identification()
            {
                Type = FindexMapper.Core.Enum.DocumentType.ExcludedSubjectInvoice,
                IssueDate = DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).DateTime,
                IssueTime = DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).DateTime.ToString("HH:mm:ss"),
                Model = FindexMapper.Core.Enum.ModelType.Normal,
                Operation = FindexMapper.Core.Enum.OperationType.Normal,
                Version = FindexMapper.Core.Constants.FacturaSujetoExcluidoJsonSchemaVersion,
                Environment = (FindexMapper.Core.Enum.Environment)environment,
                Identifier = Guid.NewGuid().ToString("D").ToUpperInvariant(),
                ControlNumber = ""
            };
        }
    }
}
