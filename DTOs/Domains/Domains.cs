using System.Text.Json.Serialization;

namespace Discloud.DTOs;

/// <summary>
/// The state of a subdomain. Sent by the API as an integer.
/// </summary>
public enum SubdomainStatus
{
    /// <summary>The subdomain is not linked to an application (<c>1</c>).</summary>
    Available = 1,

    /// <summary>The subdomain is in use by an application and cannot be deleted (<c>2</c>).</summary>
    InUse = 2,
}

/// <summary>
/// A <c>*.discloud.app</c> subdomain owned by the user.
/// </summary>
public sealed record Subdomain
{
    /// <summary>The subdomain name, without the <c>.discloud.app</c> suffix.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>The user ID of the owner.</summary>
    [JsonPropertyName("userID")]
    public string? UserId { get; init; }

    /// <summary>Whether the subdomain is available or in use.</summary>
    [JsonPropertyName("status")]
    public SubdomainStatus? Status { get; init; }

    /// <summary>When the subdomain was created, as Unix time in milliseconds.</summary>
    [JsonPropertyName("date")]
    public long? Date { get; init; }
}

/// <summary>
/// A custom domain linked to an application.
/// </summary>
public sealed record CustomDomain
{
    /// <summary>The domain name.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>The DNS state as sent by the API (for example <c>active</c> or <c>pending_check</c>).</summary>
    [JsonPropertyName("dns_status")]
    public string? DnsStatus { get; init; }

    /// <summary>When the domain was registered, as Unix time in milliseconds.</summary>
    [JsonPropertyName("date")]
    public long? Date { get; init; }

    /// <summary>Whether the domain uses the legacy configuration.</summary>
    [JsonPropertyName("isLegacy")]
    public bool? IsLegacy { get; init; }

    /// <summary>Whether the DNS records are valid. Sent by the verification route.</summary>
    [JsonPropertyName("valid")]
    public bool? Valid { get; init; }

    /// <summary>A description of the DNS state. Sent by the verification route.</summary>
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    /// <summary>The application the domain points to.</summary>
    [JsonPropertyName("app")]
    public CustomDomainApp? App { get; init; }

    /// <summary>The DNS records to create at your DNS provider.</summary>
    [JsonPropertyName("instructions")]
    public IReadOnlyList<DnsInstruction>? Instructions { get; init; }
}

/// <summary>
/// The application a custom domain points to.
/// </summary>
public sealed record CustomDomainApp
{
    /// <summary>The unique identifier of the application.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>The name of the application.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>Whether the application is online.</summary>
    [JsonPropertyName("online")]
    public bool? Online { get; init; }

    /// <summary>Whether the application is a bot or a site.</summary>
    [JsonPropertyName("type")]
    public AppType? Type { get; init; }
}

/// <summary>
/// A DNS record required by a custom domain.
/// </summary>
public sealed record DnsInstruction
{
    /// <summary>The record type (for example <c>A</c> or <c>CNAME</c>).</summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    /// <summary>The record name.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>The record content.</summary>
    [JsonPropertyName("content")]
    public string? Content { get; init; }
}

/// <summary>
/// The result of moving a custom domain to another application (<c>PUT /customdomain/{domainName}/edit</c>).
/// </summary>
public sealed record CustomDomainMoveResponse : DiscloudResponse
{
    /// <summary>Whether the domain was migrated from the legacy configuration.</summary>
    [JsonPropertyName("migrated")]
    public bool? Migrated { get; init; }

    /// <summary>The moved domain.</summary>
    [JsonPropertyName("domain")]
    public string? Domain { get; init; }
}

/// <summary>
/// The result of removing a custom domain (<c>DELETE /customdomain/{domainName}/remove</c>).
/// </summary>
public sealed record CustomDomainRemoveResponse : DiscloudResponse
{
    /// <summary>The removed domain.</summary>
    [JsonPropertyName("domain")]
    public string? Domain { get; init; }
}

internal sealed record SubdomainResponse : DiscloudResponse
{
    [JsonPropertyName("subdomain")]
    public Subdomain? Subdomain { get; init; }
}

internal sealed record SubdomainListResponse : DiscloudResponse
{
    [JsonPropertyName("subdomains")]
    public IReadOnlyList<Subdomain>? Subdomains { get; init; }
}

internal sealed record CustomDomainResponse : DiscloudResponse
{
    [JsonPropertyName("domain")]
    public CustomDomain? Domain { get; init; }
}

internal sealed record CustomDomainListResponse : DiscloudResponse
{
    [JsonPropertyName("domains")]
    public IReadOnlyList<CustomDomain>? Domains { get; init; }
}

internal sealed record CustomDomainCreateRequest(
    [property: JsonPropertyName("domainName")] string DomainName,
    [property: JsonPropertyName("appID")] string AppId);

internal sealed record CustomDomainMoveRequest([property: JsonPropertyName("newAppID")] string NewAppId);
