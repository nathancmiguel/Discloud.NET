using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Discloud.DTOs;

/// <summary>
/// Reads properties the API sends as a single object for one resource and as an array when the special
/// <c>all</c> identifier is used. Both shapes are exposed as a list.
/// </summary>
internal sealed class SingleOrArrayJsonConverter<T> : JsonConverter<IReadOnlyList<T>>
{
    public override IReadOnlyList<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var itemInfo = (JsonTypeInfo<T>)options.GetTypeInfo(typeof(T));

        if (reader.TokenType == JsonTokenType.StartArray)
        {
            var items = new List<T>();
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                var item = JsonSerializer.Deserialize(ref reader, itemInfo);
                if (item is not null)
                {
                    items.Add(item);
                }
            }

            return items;
        }

        var single = JsonSerializer.Deserialize(ref reader, itemInfo);
        return single is null ? [] : [single];
    }

    public override void Write(Utf8JsonWriter writer, IReadOnlyList<T> value, JsonSerializerOptions options)
    {
        var itemInfo = (JsonTypeInfo<T>)options.GetTypeInfo(typeof(T));
        writer.WriteStartArray();
        foreach (var item in value)
        {
            JsonSerializer.Serialize(writer, item, itemInfo);
        }

        writer.WriteEndArray();
    }
}
