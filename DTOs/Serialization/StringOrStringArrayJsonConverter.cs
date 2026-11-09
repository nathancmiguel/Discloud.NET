using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discloud.DTOs;

/// <summary>
/// Reads string lists the API sometimes sends as a bare string (for example <c>apts</c>, which is an empty
/// string when nothing is installed). An empty string becomes an empty list; a non-empty string is split on commas.
/// </summary>
internal sealed class StringOrStringArrayJsonConverter : JsonConverter<IReadOnlyList<string>>
{
    public override IReadOnlyList<string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                var text = reader.GetString();
                return string.IsNullOrWhiteSpace(text)
                    ? []
                    : text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            case JsonTokenType.StartArray:
                var items = new List<string>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    if (reader.TokenType != JsonTokenType.String)
                    {
                        throw new JsonException($"Expected a JSON string inside the array but found {reader.TokenType}.");
                    }

                    items.Add(reader.GetString()!);
                }

                return items;
            default:
                throw new JsonException($"Expected a JSON string or array but found {reader.TokenType}.");
        }
    }

    public override void Write(Utf8JsonWriter writer, IReadOnlyList<string> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var item in value)
        {
            writer.WriteStringValue(item);
        }

        writer.WriteEndArray();
    }
}
