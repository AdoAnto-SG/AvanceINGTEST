using System.Text.RegularExpressions;
using FindexMapper.Core.Base;
using FindexMapper.Core.Enum;
using Newtonsoft.Json;

namespace Integrador.Models.Base;

public class InvoiceReceiver
{
    [JsonProperty(nameof(CardCode))]
    public string? CardCode { get; set; }

    [JsonProperty(nameof(CardName))]
    public string? CardName { get; set; }

    [JsonProperty("U_DocumentoIdetificacion_FE")]
    public string? UDocumentoIdetificacionFE { get; set; }

    [JsonProperty("U_ActividadEconomica_FE")]
    public string? UActividadEconomicaFE { get; set; }

    [JsonProperty("U_Pais_FE")]
    public string? UPaisFE { get; set; }

    [JsonProperty("U_Departamento_FE")]
    public string? UDepartamentoFE { get; set; }

    [JsonProperty("U_Municipio_FE")]
    public string? UMunicipioFE { get; set; }

    [JsonProperty("E_Mail")]
    public string? EMail { get; set; }

    [JsonProperty(nameof(Phone1))]
    public string? Phone1 { get; set; }

    [JsonProperty(nameof(NIT))]
    public string? NIT { get; set; }

    [JsonProperty(nameof(NRC))]
    public string? NRC { get; set; } 

    [JsonProperty(nameof(DUI))]
    public string? DUI { get; set; }

    public Address? CreateReceiverAddress()
    {
        if(string.IsNullOrWhiteSpace(this.UDepartamentoFE) || string.IsNullOrWhiteSpace(this.UMunicipioFE)) return null;

        return new Address()
        {
            Complement = $"{this.UDepartamentoFE} {this.UMunicipioFE} {this.UPaisFE}",
            Department = this.UDepartamentoFE,
            Municipality = this.UMunicipioFE
        };
    }

    public virtual (IdentificationDocumentType?, string?) HandleDocumentNumber()
    {
        Regex nitRegex = new("^([0-9]{14}|[0-9]{9})$");
        if(!string.IsNullOrWhiteSpace(NIT) && nitRegex.IsMatch(NIT)) return (IdentificationDocumentType.NIT, NIT);
        
        Regex duiRegex = new("^[0-9]{9}|[0-9]{8}-[0-9]{1}$");
        if(!string.IsNullOrWhiteSpace(DUI) && duiRegex.IsMatch(DUI)) return (IdentificationDocumentType.DUI, DUI);

        if(!string.IsNullOrWhiteSpace(NIT) || !string.IsNullOrWhiteSpace(DUI)) return (IdentificationDocumentType.Other, NIT ?? DUI);

        return (null, null);
    }
    
}
