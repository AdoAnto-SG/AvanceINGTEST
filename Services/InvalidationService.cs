using FindexMapper.Service.Interfaces;
using Integrador.Converters;
using Integrador.Mappers.Cancellations;
using Integrador.Models.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Integrador;

public class InvalidationService : IInvalidationParseService
{
    private readonly ICancellationMapper _cancellationMapper;

    public InvalidationService(ICancellationMapper cancellationMapper)
    {
        _cancellationMapper = cancellationMapper;
    }

    public object? Parse(JToken resource, FindexMapper.Service.Enum.Environment environment)
    {
        var documentBody = resource["BOM"]?["BO"]?["INV1"]?["row"] ?? resource["BOM"]?["BO"]?["PCH1"]?["row"] ;

		List<Inv1> inv1s = new();
		if (documentBody is JArray)
		{
			inv1s = documentBody?.ToObject<List<Inv1>>() ?? new List<Inv1>();
		}
		if(documentBody is JObject)
		{
			var result = documentBody?.ToObject<Inv1?>();
			if (result is not null) inv1s.Add(result);
		}

		var canceledProperty = resource["BOM"]?["BO"]?["OINV"]?["row"]?["CANCELED"] ?? resource["BOM"]?["BO"]?["OPCH"]?["row"]?["CANCELED"];
		var canceled = canceledProperty?.Value<string>();
		if(canceled == "C") return _cancellationMapper.Map(Deserialize(resource.ToString()), environment, inv1s);
        return new {};
    }

    private static BaseRequest Deserialize(string json)
	{
        return JsonConvert.DeserializeObject<BaseRequest>(json ?? string.Empty, new RequestConverter()) ?? new BaseRequest();
    }
}
