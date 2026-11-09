using System.Text.Json.Serialization;

namespace Discloud.DTOs;

internal sealed record EnvVarsRequest([property: JsonPropertyName("vars")] IReadOnlyDictionary<string, string> Vars);

internal sealed record EnvVarsResponse : DiscloudResponse
{
    [JsonPropertyName("data")]
    public EnvVarsData? Data { get; init; }
}

internal sealed record EnvVarsData
{
    [JsonPropertyName("vars")]
    public IReadOnlyDictionary<string, string>? Vars { get; init; }
}

internal sealed record EnvVarsDeleteResponse : DiscloudResponse
{
    [JsonPropertyName("data")]
    public EnvVarsDeleteData? Data { get; init; }
}

internal sealed record EnvVarsDeleteData
{
    [JsonPropertyName("removed")]
    public int? Removed { get; init; }
}
