using System.Text.Json.Serialization;

namespace Discloud.DTOs;

/// <summary>
/// The envelope shared by every Discloud API response.
/// </summary>
/// <remarks>
/// Operations that only report the outcome of an action (for example deleting a resource)
/// return this type directly. Responses that carry additional data derive from it.
/// A response whose <see cref="Status"/> is <c>error</c> is never returned to the caller:
/// the SDK raises a <c>DiscloudApiException</c> instead.
/// </remarks>
public record DiscloudResponse
{
    /// <summary>
    /// The outcome reported by the API, usually <c>ok</c>. Asynchronous operations may report <c>accepted</c>.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    /// A human-readable message describing the outcome. The language follows the account locale.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    /// <summary>
    /// A Discloud-specific status code sent by some routes (for example <c>200</c>, or <c>93</c> when an upload
    /// is missing its <c>discloud.config</c> file). <see langword="null"/> when the route does not send one.
    /// </summary>
    [JsonPropertyName("statusCode")]
    public int? StatusCode { get; init; }
}
