using FindexMapper.Core.Base;
using FindexMapper.Core.Entities;
using FindexMapper.Core.Enum;
using FindexMapper.Core.Utils;
using FindexMapper.Service.Interfaces;
using FindexMapper.Service.Services.ControlNumbers;
using Integrador.Models;
using Integrador.Models.Base;
using Integrador.Models.sections;
using Integrador.Utils;
using Microsoft.Extensions.Options;

namespace Integrador.Mappers.Invoices;

public class InvoiceMapper : IInvoiceMapper
{
	private readonly IControlNumberService _controlNumberService;
    private readonly ISourceProvider _sourceProvider;
    private readonly SenderInfo _senderInfo;

    public InvoiceMapper(IControlNumberService controlNumberService, IOptions<SenderInfo> options, ISourceProvider sourceProvider)
    {
        _controlNumberService = controlNumberService;
        _senderInfo = options.Value;
        _sourceProvider = sourceProvider;
    }

    public Invoice Map(BaseRequest? input, FindexMapper.Service.Enum.Environment environment, List<Inv1> inv1s)
	{
		try
		{
			if (input is null) return new Invoice();
			var invoice = new Invoice()
			{
				Identification = CreateIdentification(input.Oinv, environment),
				Sender = CreateSender(),
				Receiver = CreateReceiver(input.InvoiceReceiver),
				DocumentBody = CreateDocumentBody(inv1s),
				Summary = CreateSummary(input.Oinv)
			};
			return invoice;
		}
		catch (Exception)
		{
			return new Invoice();
		}
	}

	private static FindexMapper.Core.Base.Invoice.Summary CreateSummary(Oinv? row)
	{
		try
		{
			if (row is null) return new FindexMapper.Core.Base.Invoice.Summary();
            bool isTaxed = row.U_TipoVenta_FE == "Gravada";
            bool isNotSubject = row.U_TipoVenta_FE == "No Sujeta";

			var totalAmount = Math.Round(Convert.ToDecimal(row.DocTotal), 2);
            return new FindexMapper.Core.Base.Invoice.Summary()
			{
				DiscountPercentage = Math.Round(Convert.ToDecimal(row.DiscPrcnt), 2),
				TotalDiscount= Math.Round(Convert.ToDecimal(row.DiscSum), 2),
				Subtotal = Math.Round(Convert.ToDecimal(row.DocTotal), 2),
                TotalTaxed = isTaxed && !isNotSubject ? totalAmount : 0,
                TotalExempt = !isTaxed && !isNotSubject ? totalAmount : 0,
                TotalNotSubject = isNotSubject && !isTaxed ? totalAmount : 0,
                Status = row.U_Formapago_FE?.ToEnum(FindexMapper.Core.Enum.OperationStatus.Cash) ?? FindexMapper.Core.Enum.OperationStatus.Cash,
				Payment = CreatePayment(row),
				SubTotalSales = totalAmount,
				TotalDescription = totalAmount.ToString("F").ToInvoiceFormat(true) ?? string.Empty,
				TotalAmount = totalAmount,
				TotalToPay = totalAmount,
				TotalVat = Math.Round(Convert.ToDecimal(row.VatSum), 2)
			};
		}
		catch (Exception)
		{
			return new FindexMapper.Core.Base.Invoice.Summary();
		}
	}

	private static ICollection<Payment>? CreatePayment(Oinv row)
	{
		try
		{
			if (row.U_Formapago_FE != "2") return null;
			return new List<Payment>()
			{
				new Payment()
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

	private static ICollection<FindexMapper.Core.Base.Invoice.DocumentBody> CreateDocumentBody(List<Inv1> inv1s)
	{
		try
		{
			if(inv1s is null) return Enumerable.Empty<FindexMapper.Core.Base.Invoice.DocumentBody>().ToList();
			int index = 1;
			var result = inv1s.Select(x =>
			{
				bool isTaxed = (string?)x?.U_TipoVenta_FE == "Gravada";
				var isString = x?.U_Concepto1 is string;
				return new FindexMapper.Core.Base.Invoice.DocumentBody()
				{
					Number = index++,
					Description = $"{x?.Dscription} {(isString ? (string?)x?.U_Concepto1 : string.Empty)}",
                    Quantity = Convert.ToDecimal(x?.Quantity) == 0 ? 1 : Math.Round(Convert.ToDecimal(x?.Quantity), 2),
                    Unitprice = Math.Round(Convert.ToDecimal(x?.Price), 2),
					Vat = Math.Round(Convert.ToDecimal(x?.VatSum), 2),
					UnitOfMeasurement = 99, //otro,
					Type = x?.U_TipoItem_FE?.ToEnum(FindexMapper.Core.Enum.DocumentBodyType.Product) ?? FindexMapper.Core.Enum.DocumentBodyType.Product,
					TaxableSale = isTaxed ? Math.Round(Convert.ToDecimal(x?.LineTotal), 2) : 0
				};
			}).ToList();
			return result;

		}
		catch (Exception)
		{
			return  Enumerable.Empty<FindexMapper.Core.Base.Invoice.DocumentBody>().ToList();
		}
	}

	private FindexMapper.Core.Base.Invoice.Receiver? CreateReceiver(InvoiceReceiver? row)
    {
		try
		{
			if(row is null) return null;
			var number = string.IsNullOrEmpty(row.DUI) ? row.NIT : row.DUI ?? null;
			var documentNumber = HandleDocumentNumber(row.UDocumentoIdetificacionFE, number);

            var economicActivity = _sourceProvider.Catalog(new
            {
                CatalogId = 19,
                Key = row.UActividadEconomicaFE
            });

            return new FindexMapper.Core.Base.Invoice.Receiver()
			{
				DocumentNumber = documentNumber.Item1,
				DocumentType = documentNumber.Item2,
				Name = !string.IsNullOrWhiteSpace(row.CardName) ? row.CardName : null,
				Address = row.CreateReceiverAddress(),
				EconomicActivity = economicActivity.Name,
				EconomicActivityCode = row.UActividadEconomicaFE,
                Email = row.EMail,
				Nrc = StringUtils.RemoveGuionFromString(row.NRC)?.Replace("\\", "").Replace("/", "") ?? string.Empty,
				Phone = row.Phone1
			};
		}
		catch (Exception)
		{
			return null;
		}
	}

	private static (string?, FindexMapper.Core.Enum.IdentificationDocumentType?) HandleDocumentNumber(string? documentType, string? documentNumber)
	{
		try
		{
			if(string.IsNullOrEmpty(documentType) || string.IsNullOrEmpty(documentNumber)) return (null, null);
			if (documentType == "NA") return (null, null);
			var docType = documentType.ToEnum(FindexMapper.Core.Enum.IdentificationDocumentType.DUI);
			documentNumber = StringUtils.HandleDocumentNumber(docType, documentNumber);
			return (documentNumber, docType);
		}
		catch (Exception)
		{
			return (null, null);
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

	private Identification CreateIdentification(Oinv? row, FindexMapper.Service.Enum.Environment environment)
	{
		try
		{
			if (row is null) return new Identification();
			var issueDate = row.GetIssueDate();
			return new Identification()
			{
				Type = FindexMapper.Core.Enum.DocumentType.Invoice,
				IssueDate = issueDate,
				IssueTime = issueDate.ToString("HH:mm:ss"),
				Model = FindexMapper.Core.Enum.ModelType.Normal,
				Operation = FindexMapper.Core.Enum.OperationType.Normal,
				Version = FindexMapper.Core.Constants.ConsumidorFinalJsonSchemaVersion,
				Environment = (FindexMapper.Core.Enum.Environment)environment,
				Identifier = StringUtils.GenerateSeededGuid(
					nit: _senderInfo.NIT,
					documentType: DocumentType.Invoice,
					row.DocNum ?? string.Empty
                )
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
				Identifier = Guid.NewGuid().ToString("D").ToUpperInvariant(),
				ControlNumber = ""
			};
		}
	}

	private static DateTime GetIssueDate(string? docDate, string? docTime)
	{
		try
		{
			if(string.IsNullOrEmpty(docDate) || string.IsNullOrEmpty(docTime))
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
