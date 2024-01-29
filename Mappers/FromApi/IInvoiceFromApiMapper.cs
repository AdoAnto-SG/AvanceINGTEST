using FindexMapper.Core.Entities;
using Integrador.Models.sections;
using Enum = FindexMapper.Service.Enum;

namespace Integrador.Mappers.FromApi;

public interface IInvoiceFromApiMapper
{
    Invoice MapToInvoice(Request request, Enum.Environment environment);
}
