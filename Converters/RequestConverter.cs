using FindexMapper.Core.Entities;
using Integrador.Models;
using Integrador.Models.Base;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

namespace Integrador.Converters;

public class RequestConverter : JsonConverter
{
    public override bool CanConvert(Type objectType) => objectType == typeof(BaseRequest) || objectType.BaseType == typeof(BaseRequest);

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);

        if(jsonObject["BOM"]?["BO"]?["OPCH"] is not null)
        {
            var c = ParseCollection<InvoiceReceiver>(jsonObject["SocioDeNegocio"], "Row").FirstOrDefault();
            return new ExcludedSubjectRequest()
            {
                Oinv = ParseCollection<Oinv>(jsonObject["BOM"]?["BO"], "OPCH").First(),
                Inv1 = ParseCollection<Inv1>(jsonObject["BOM"]?["BO"], "PCH1"),
                Inv4 = ParseCollection<Inv4>(jsonObject["BOM"]?["BO"], "PCH4").ToList(),
                Taxes = ParseCollection<PCH5>(jsonObject["BOM"]?["BO"], "PCH5").ToList(),
                Inv6 = ParseCollection<Inv6>(jsonObject["BOM"]?["BO"], "PCH6").ToList(),
                Inv12 = ParseCollection<Inv12>(jsonObject["BOM"]?["BO"], "PCH12").ToList(),
                Inv21 = ParseCollection<Inv21>(jsonObject["BOM"]?["BO"], "PCH21").ToList(),
                InvoiceReceiver = c
            };
        }
        
        return new BaseRequest()
        {
            Oinv = ParseCollection<Oinv>(jsonObject["BOM"]?["BO"], "OINV").First(),
            Inv1 = ParseCollection<Inv1>(jsonObject["BOM"]?["BO"], "INV1"),
            Inv4 = ParseCollection<Inv4>(jsonObject["BOM"]?["BO"], "INV4").ToList(),
            Inv6 = ParseCollection<Inv6>(jsonObject["BOM"]?["BO"], "INV6").ToList(),
            Inv12 = ParseCollection<Inv12>(jsonObject["BOM"]?["BO"], "INV12").ToList(),
            Inv21 = ParseCollection<Inv21>(jsonObject["BOM"]?["BO"], "INV21").ToList(),
            InvoiceReceiver = ParseCollection<InvoiceReceiver>(jsonObject["SocioDeNegocio"], "Row").FirstOrDefault(),
            InvoiceReceiverDivideds = ParseCollection<InvoiceReceiverDivided>(jsonObject["SociosFactDividida"], "SocioDeNegocio").ToList()
        };

    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    private ICollection<T> ParseCollection<T>(JToken? jsonObject, string property)
    {
        var data = new Collection<T>();
        var source = jsonObject?[property];

        if (source == null)
        {
            return data;
        }

        if (source.Type == JTokenType.Array)
        {
            foreach (JObject item in source.Cast<JObject>())
            {
                if (item.ContainsKey("row"))
                {
                    var result = ParseCollection<T>(item, "row");
                    foreach (var e in result)
                    {
                        data.Add(e);
                    }

                    continue;
                }

                var cast = ParseObject<T>(item);
                if (cast != null)
                {
                    data.Add(cast);
                }
            }

            return data;
        }

        var jObject = source.ToObject<JObject>();
        if (jObject != null && jObject.ContainsKey("row"))
        {
            return ParseCollection<T>(jObject, "row");
        }

        var parsed = ParseObject<T>(jObject);
        if (parsed != null)
        {
            data.Add(parsed);
        }
        return data;
    }

    private T? ParseObject<T>(JObject? item)
    {
        var items = item?.Children().OfType<JProperty>()
            .Where((prop) => prop.Value?.Type == JTokenType.Object || prop.Value?.Type == JTokenType.Array);

        if (items == null)
        {
            return item!.ToObject<T>();
        }

        foreach (var token in items)
        {
            var value = token.Value?.Type == JTokenType.Object
                ? token.Value["@content"]?.Value<string?>()
                : token.FirstOrDefault()?.First?.Value<string?>();

            token.Value = value;
        }

        return item!.ToObject<T>();
    }
}
