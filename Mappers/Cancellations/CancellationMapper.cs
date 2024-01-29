using System.Globalization;
using FindexMapper.Core;
using FindexMapper.Core.Base.Cancellation;
using FindexMapper.Core.Utils;
using Integrador.Models;
using Integrador.Models.Base;
using Microsoft.Extensions.Options;

namespace Integrador.Mappers.Cancellations;

public class CancellationMapper : ICancellationMapper
{
    private readonly SenderInfo _senderInfo;
    public CancellationMapper(IOptions<SenderInfo> options)
    {
        _senderInfo = options.Value;
    }
    public Cancellation Map(BaseRequest? input, FindexMapper.Service.Enum.Environment environment, List<Inv1> inv1s)
    {
        try
        {
            if(input is null) return new Cancellation();
            var reason = CreateReason(input?.InvoiceReceiver, input?.Oinv);
            var document = CreateDocument(input?.InvoiceReceiver, input?.Oinv);

            if(reason.TipoAnulacion == FindexMapper.Core.Enum.InvalidationType.CancelOperationCompleted)
                document.IdentifierR = null;

            return new Cancellation()
            {
                Identification = CreateIdentification(input?.Oinv, environment),
                Sender = CreateSender(),
                Document = document,
                Reason = reason
            };
        }
        catch (System.Exception)
        {
            return new Cancellation();
        }
    }

    private static Reason CreateReason(InvoiceReceiver? invoiceReceiver, Oinv? oinv)
    {
        try
        {
            if(invoiceReceiver == null || oinv == null) return new Reason();
            return new Reason()
            {
                TipoAnulacion = oinv.U_RazonAnulacion_FE?.ToEnum(FindexMapper.Core.Enum.InvalidationType.InformationError) ?? FindexMapper.Core.Enum.InvalidationType.InformationError,
                InvalidationReason = oinv.Error,
                ResponsibleName = oinv.CardName ?? string.Empty,
                DocumentType = FindexMapper.Core.Enum.IdentificationDocumentType.NIT,
                DocumentNumber = oinv.U_nit_prov ?? string.Empty,
                RequesterName = invoiceReceiver.CardName ?? string.Empty,
                RequesterDocumentNumber = invoiceReceiver.NIT ?? string.Empty,
                TipDocSolicita = invoiceReceiver?.UDocumentoIdetificacionFE?.ToEnum(FindexMapper.Core.Enum.IdentificationDocumentType.NIT) ?? FindexMapper.Core.Enum.IdentificationDocumentType.NIT
            };
        }
        catch (System.Exception)
        {
            return new Reason();
        }
    }

    private static Document CreateDocument(InvoiceReceiver? invoiceReceiver, Oinv? oinv)
    {
        try
        {
            if(invoiceReceiver == null || oinv == null) return new Document();
            string format = "yyyy-MM-dd HH:mm:ss";
            var issueDate = DateTime.ParseExact(oinv.ProccessDate ?? string.Empty, format, CultureInfo.InvariantCulture);
            return new Document()
            {
                TipoDocumento = invoiceReceiver.UDocumentoIdetificacionFE?.ToEnum(FindexMapper.Core.Enum.IdentificationDocumentType.DUI) ?? FindexMapper.Core.Enum.IdentificationDocumentType.DUI,
                NumDocumento = invoiceReceiver.NIT ?? string.Empty,
                Name = invoiceReceiver.CardName ?? string.Empty,
                Email = invoiceReceiver.EMail ?? string.Empty,
                Phone = invoiceReceiver.Phone1,
                Identifier = oinv?.UCodigoGeneracionFE ?? string.Empty,
                IdentifierR = Guid.NewGuid().ToString("D").ToUpperInvariant(),
                ControlNumber = oinv?.ControlNumber ?? string.Empty,
                IssueDate = issueDate,
                MontoIva = Convert.ToDecimal(oinv?.VatSum),
                Type = oinv?.GetDocumentType() ?? FindexMapper.Core.Enum.DocumentType.Invoice,
                StampOfReceived = oinv?.ReceptionSign ?? string.Empty
            };
        }
        catch (System.Exception)
        {
            return new Document();
        }
    }

    private Sender CreateSender()
    {
        try
        {
            return new Sender()
            {
                Email = _senderInfo.Email,
                TipoEstablecimiento = FindexMapper.Core.Enum.EstablishmentType.Branch,
                Name = _senderInfo.Name,
                Nit = _senderInfo.NIT,
                Phone = StringUtils.RemoveGuionFromString(_senderInfo.Phone),
                TradeName = _senderInfo.TradeName,
                EstablishmentCode = _senderInfo.EstablishmentCode,
                PointOfSaleCode = _senderInfo.PointOfSaleCode,
                MHEstablishmentCode = _senderInfo.MHEstablishmentCode,
                MHPointOfSaleCode = _senderInfo.MHPointOfSaleCode
            };
        }
        catch (System.Exception)
        {
            return new Sender()
            {
                Email = _senderInfo.Email,
                TipoEstablecimiento = FindexMapper.Core.Enum.EstablishmentType.Branch,
                Name = _senderInfo.Name,
                Nit = _senderInfo.NIT,
                Phone = _senderInfo.Phone,
                TradeName = _senderInfo.TradeName ?? string.Empty,
                EstablishmentCode = _senderInfo.EstablishmentCode,
                PointOfSaleCode = _senderInfo.PointOfSaleCode,
                MHEstablishmentCode = _senderInfo.MHEstablishmentCode,
                MHPointOfSaleCode = _senderInfo.MHPointOfSaleCode
            };
        }
    }

    private Identification CreateIdentification(Oinv? oinv, FindexMapper.Service.Enum.Environment environment)
    {
        try
        {
            if(oinv is null) return new Identification();
            var issueDate = GetIssueDate(oinv.DocDate, oinv.DocTime);
            return new Identification()
            {
                Version = FindexMapper.Core.Constants.AnulacionJsonSchemaVersion,
                Environment = (FindexMapper.Core.Enum.Environment)environment,
                CancellationDate = issueDate,
                CancellationTime = issueDate.ToString("HH:mm:ss"),
                Identifier = Guid.NewGuid().ToString("D").ToUpperInvariant()
            };
        }
        catch (System.Exception)
        {
            return new Identification()
            {
                Version = FindexMapper.Core.Constants.AnulacionJsonSchemaVersion,
                Environment = (FindexMapper.Core.Enum.Environment)environment,
                CancellationDate = DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).DateTime,
                CancellationTime = DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).DateTime.ToString("HH:mm:ss"),
                Identifier = Guid.NewGuid().ToString("D").ToUpperInvariant()
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
