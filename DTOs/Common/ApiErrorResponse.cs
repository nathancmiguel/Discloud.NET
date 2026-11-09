using System.Text.Json;
using System.Text.Json.Serialization;

namespace Discloud.DTOs;

/// <summary>
/// The error body sent by the API. Most routes send only <c>status</c> and <c>message</c>; newer routes add a
/// machine-readable <c>code</c> (an integer or a string such as <c>SNAPSHOT:APP_BUSY</c>) and sometimes <c>data</c>.
/// </summary>
internal sealed record ApiErrorResponse : DiscloudResponse
{
    [JsonPropertyName("code")]
    [JsonConverter(typeof(StringOrNumberJsonConverter))]
    public string? Code { get; init; }

    [JsonPropertyName("data")]
    public JsonElement? Data { get; init; }
}
