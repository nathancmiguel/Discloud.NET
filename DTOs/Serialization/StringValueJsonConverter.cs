using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discloud.DTOs;

/// <summary>
/// Serializes string-backed value types as plain JSON strings. Unknown values are preserved rather than rejected,
/// so new values introduced by the API do not break deserialization.
/// </summary>
internal sealed class StringValueJsonConverter<T> : JsonConverter<T>
    where T : struct, IStringValue<T>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected a JSON string for {typeof(T).Name} but found {reader.TokenType}.");
        }

        var value = reader.GetString();
        return string.IsNullOrEmpty(value) ? default : T.Create(value);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Value);
}
