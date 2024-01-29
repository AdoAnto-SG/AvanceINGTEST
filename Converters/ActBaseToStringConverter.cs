using Integrador.Models.Base;
using Newtonsoft.Json;

namespace Integrador.Converters;

public class ActBaseToStringConverter : JsonConverter<string>
{
    public override string? ReadJson(JsonReader reader, Type objectType, string? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value is ActBaseEnt value)
        {
            if (string.IsNullOrEmpty(value?.Content)) return string.Empty;
            return value?.Content;
        }
        return reader.Value?.ToString();
    }

    public override void WriteJson(JsonWriter writer, string? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
