using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Discloud;

/// <summary>
/// Configuration for <see cref="DiscloudClient"/>.
/// </summary>
/// <remarks>
/// <para>
/// When registered through <c>AddDiscloudClient</c>, the options are bound from the <c>Discloud</c> configuration
/// section, so the token can come from <c>appsettings.json</c>, user secrets or the <c>Discloud__Token</c>
/// environment variable.
/// </para>
/// <para>
/// The token is never written by <see cref="ToString"/>, never serialized and hidden from the debugger.
/// </para>
/// </remarks>
/// <example>
/// <code language="json">
/// {
///   "Discloud": {
///     "Token": "set-me-with-user-secrets-or-an-environment-variable"
///   }
/// }
/// </code>
/// </example>
public sealed class DiscloudClientOptions
{
    /// <summary>
    /// The name of the configuration section the options are bound from by default: <c>Discloud</c>.
    /// </summary>
    public const string SectionName = "Discloud";

    /// <summary>
    /// Gets the address of the public Discloud API: <c>https://api.discloud.app/v2/</c>.
    /// </summary>
    public static Uri DefaultBaseAddress { get; } = new("https://api.discloud.app/v2/");

    /// <summary>
    /// Gets the default request timeout: 100 seconds.
    /// </summary>
    public static TimeSpan DefaultTimeout { get; } = TimeSpan.FromSeconds(100);

    /// <summary>
    /// Gets or sets the Discloud API token, sent in the <c>api-token</c> header of every request. Required.
    /// </summary>
    /// <remarks>
    /// Obtain a token from your Discloud account (see the Discloud documentation). Never commit it to source control:
    /// use user secrets during development and environment variables or a secret manager in production.
    /// </remarks>
    [JsonIgnore]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public string? Token { get; set; }

    /// <summary>
    /// Gets or sets the base address of the API. Defaults to <see cref="DefaultBaseAddress"/>.
    /// Must be an absolute HTTP or HTTPS address.
    /// </summary>
    public Uri BaseAddress { get; set; } = DefaultBaseAddress;

    /// <summary>
    /// Gets or sets the timeout applied to each request. Defaults to <see cref="DefaultTimeout"/>.
    /// Use <see cref="System.Threading.Timeout.InfiniteTimeSpan"/> to disable it.
    /// </summary>
    /// <remarks>
    /// Uploading or committing large applications can take longer than the default.
    /// When the client is registered through dependency injection this value is applied to the
    /// <see cref="HttpClient"/> created by <see cref="IHttpClientFactory"/>.
    /// </remarks>
    public TimeSpan Timeout { get; set; } = DefaultTimeout;

    /// <summary>
    /// Returns a description of the options that never includes the token.
    /// </summary>
    /// <returns>A string such as <c>DiscloudClientOptions { BaseAddress = …, Timeout = …, Token = *** }</c>.</returns>
    public override string ToString()
        => $"DiscloudClientOptions {{ BaseAddress = {BaseAddress}, Timeout = {Timeout}, Token = {(string.IsNullOrWhiteSpace(Token) ? "<not set>" : "***")} }}";
}
