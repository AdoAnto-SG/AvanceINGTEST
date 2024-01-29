using Newtonsoft.Json;

namespace Integrador.Models.Base;

public class Bom
{
    [JsonProperty("BO")]
    public Bo? Bo { get; set; }
}
