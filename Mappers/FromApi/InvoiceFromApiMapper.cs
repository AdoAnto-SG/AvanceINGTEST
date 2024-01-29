using Dapper;
using FindexMapper.Core.Base;
using FindexMapper.Core.Base.Invoice;
using FindexMapper.Core.Entities;
using FindexMapper.Core.Enum;
using FindexMapper.Core.Utils;
using FindexMapper.Service.Data.Interfaces;
using FindexMapper.Service.Services.ControlNumbers;
using Integrador.Models;
using Integrador.Models.sections;
using Microsoft.Extensions.Options;
using Enum = FindexMapper.Service.Enum;

namespace Integrador.Mappers.FromApi;

public class InvoiceFromApiMapper : IInvoiceFromApiMapper
{
    private readonly ISqLiteDatabaseProvider? _databaseProvider;
    private readonly IControlNumberService? _controlNumberService;
    private readonly SenderInfo _senderInfo;

    public InvoiceFromApiMapper(ISqLiteDatabaseProvider? databaseProvider, IControlNumberService? controlNumberService, IOptions<SenderInfo> options)
    {
        _databaseProvider = databaseProvider;
        _controlNumberService = controlNumberService;
        _senderInfo = options.Value;
    }

    public Invoice MapToInvoice(Request request, Enum.Environment environment)
    {
        try
        {
            var invoice = new Invoice()
            {
                Sender = CreateSender(request.Issuer),
                Receiver = CreateReceiver(request.Receiver),
                DocumentBody = CreateDocumentBody(request),
                Summary = CreateSummary(request),
                Identification = CreateIdentification(environment)
            };
            return invoice;
        }
        catch (Exception)
        {
            return new Invoice();
        }
    }

    private static Identification CreateIdentification(Enum.Environment environment)
    {
        return new Identification()
        {
            Identifier = Guid.NewGuid().ToString("D").ToUpperInvariant(),
            Environment = (FindexMapper.Core.Enum.Environment)environment,
            Type = DocumentType.Invoice,
            Version = FindexMapper.Core.Constants.ConsumidorFinalJsonSchemaVersion,
            Operation = OperationType.Normal,
            Model = ModelType.Normal,
            IssueDate = DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).DateTime,
            IssueTime = DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).ToString("hh:mm:ss")
        };
    }

    private static FindexMapper.Core.Base.Invoice.Summary CreateSummary(Request request)
    {
        if (request is null) return new FindexMapper.Core.Base.Invoice.Summary();
        return new()
        {
            Subtotal = request.Total,
            TotalTaxed = request.Total,
            TotalAmount = request.Total,
            TotalVat = request.TotalIva,
            TotalToPay = request.Total,
            SubTotalSales = request.Total,
            Status = request?.WayToPay?.ToEnum(OperationStatus.Cash) ?? OperationStatus.Cash,
            TotalDescription = request?.Total.ToString().ToInvoiceFormat(true) ?? string.Empty,
            Payment = new List<Payment>()
            {
                new()
                {
                    Amount = request?.Total ?? 0,
                    Code = request?.WayToPay?.PadLeft(2,'0') ?? string.Empty
                }
            }
        };
    }

    private static List<FindexMapper.Core.Base.Invoice.DocumentBody> CreateDocumentBody(Request request)
    {
        var description = request.Description ?? string.Empty;
        return new List<FindexMapper.Core.Base.Invoice.DocumentBody>()
            {
                new()
                {
                    Quantity = request.Quantity == 0 ? 1 : request.Quantity,
                    Description = description.ToUTF8(),
                    UnitOfMeasurement = 99,
                    Number = 1,
                    Type = DocumentBodyType.Service,
                    Unitprice = request.Totalunitario,
                    TaxableSale = request.Total,
                    Vat = request.TotalIva
                }
            };
    }

    private FindexMapper.Core.Base.Invoice.Receiver? CreateReceiver(Models.sections.Receiver? receiver)
    {
        try
        {
            if (receiver is null) return new FindexMapper.Core.Base.Invoice.Receiver();
            var docmentNumberHasValue = !string.IsNullOrWhiteSpace(receiver.Document);
            var invoiceReceiver = new FindexMapper.Core.Base.Invoice.Receiver()
            {
                DocumentType = docmentNumberHasValue ? receiver.DocumentType?.ToString().ToEnum(IdentificationDocumentType.DUI) : null,
                DocumentNumber = StringUtils.HandleDocumentNumber(receiver.DocumentType?.ToString().ToEnum(IdentificationDocumentType.DUI), receiver.Document),
                Name = !string.IsNullOrWhiteSpace(receiver.Fullname) ? receiver.Fullname : null,
                EconomicActivityCode = receiver.EconomicActivity,
                EconomicActivity = GetEconomicActivityDescription(receiver.EconomicActivity),
                Address = CreateAddress(receiver),
                Phone = receiver.Phone,
                Email = receiver.Email
            };
            return invoiceReceiver;
        }
        catch (Exception)
        {
            return new FindexMapper.Core.Base.Invoice.Receiver();
        }
    }

    private static FindexMapper.Core.Base.Address? CreateAddress(Models.sections.Receiver? receiver)
    {
        if (receiver is null ||
            string.IsNullOrEmpty(receiver.Department) ||
            string.IsNullOrEmpty(receiver.Municipality) ||
            string.IsNullOrEmpty(receiver.Address)) return null;
        return new FindexMapper.Core.Base.Address()
        {
            Department = receiver.Department?.PadLeft(2, '0') ?? string.Empty,
            Municipality = receiver.Municipality ?? string.Empty,
            Complement = receiver.Address ?? string.Empty
        };
    }

    private string? GetEconomicActivityDescription(string? economicActivityCode)
    {
        try
        {
            var connection = _databaseProvider?.ObtainConnection();
            var sql = $@"SELECT key FROM catalog_items where catalog_id = 19 and key = @EconomicActivityCode";
            connection?.Open();
            var description = connection.QueryFirstOrDefault<string>(sql, new { EconomicActivityCode = economicActivityCode });
            return description;
        }
        catch (Exception)
        {

            throw;
        }
    }

    private FindexMapper.Core.Base.Invoice.Sender CreateSender(Issuer? issuer)
    {
        try
        {
            if (issuer is null) return new FindexMapper.Core.Base.Invoice.Sender();
            var sender = new FindexMapper.Core.Base.Invoice.Sender()
            {
                Nit = StringUtils.RemoveGuionFromString(issuer.Nit) ?? string.Empty,
                Nrc = StringUtils.RemoveGuionFromString(issuer.Nrc) ?? string.Empty,
                Name = issuer.Name ?? string.Empty,
                EconomicActivity = issuer.EconomicActivity ?? string.Empty,
                EconomicActivityCode = "41001",
                Phone = issuer.Phone ?? string.Empty,
                Email = issuer.Email ?? string.Empty,
                Address = new FindexMapper.Core.Base.Address()
                {
                    Department = GetDepartmentCodeByName(issuer.Department),
                    Municipality = GetMunicipalityCodeByName(issuer.Municipality),
                    Complement = issuer.Address ?? string.Empty
                },
                Establishment = FindexMapper.Core.Enum.EstablishmentType.Store,
                TradeName = issuer.Name,
                EstablishmentCode = _senderInfo.EstablishmentCode,
                PointOfSaleCode = _senderInfo.PointOfSaleCode,
                MHEstablishmentCode = _senderInfo.MHEstablishmentCode,
                MHPointOfSaleCode = _senderInfo.MHPointOfSaleCode
            };
            return sender;
        }
        catch (Exception)
        {
            return new FindexMapper.Core.Base.Invoice.Sender();
        }
    }

    private string GetDepartmentCodeByName(string? name)
    {
        try
        {
            var connection = _databaseProvider?.ObtainConnection();
            var sql = $"SELECT key FROM catalog_items where catalog_id = 12 and name like @Name";
            connection?.Open();
            var code = connection.QueryFirstOrDefault<int>(sql, new { Name = $"%{name}%" });
            return code.ToString("00") ?? "05";
        }
        catch (Exception)
        {
            return "05";
        }
    }

    private string GetMunicipalityCodeByName(string? name)
    {
        try
        {
            var connection = _databaseProvider?.ObtainConnection();
            var sql = $@"SELECT key FROM catalog_items where catalog_id = 13 and name like @Name";
            connection?.Open();
            var code = connection.QueryFirstOrDefault<int>(sql, new { Name = $"%{name}%" });
            if (code == 0)
            {
                return "01";
            }
            return code.ToString("00") ?? "01";
        }
        catch (Exception)
        {
            return "01";
        }
    }
}
