using System.Text.Json.Serialization;

namespace Discloud.DTOs;

/// <summary>
/// The authenticated Discloud account (<c>GET /user</c>).
/// </summary>
public sealed record User
{
    /// <summary>The unique identifier of the user (a Discord snowflake).</summary>
    [JsonPropertyName("userID")]
    public string? UserId { get; init; }

    /// <summary>The user's username, if known.</summary>
    [JsonPropertyName("username")]
    public string? Username { get; init; }

    /// <summary>The URL of the user's avatar, if any.</summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; init; }

    /// <summary>The total RAM available to the user, in megabytes.</summary>
    [JsonPropertyName("totalRamMb")]
    public int? TotalRamMb { get; init; }

    /// <summary>The RAM currently allocated to the user's applications, in megabytes.</summary>
    [JsonPropertyName("ramUsedMb")]
    public int? RamUsedMb { get; init; }

    /// <summary>The subdomains owned by the user.</summary>
    [JsonPropertyName("subdomains")]
    public IReadOnlyList<string>? Subdomains { get; init; }

    /// <summary>The custom domains owned by the user.</summary>
    [JsonPropertyName("customdomains")]
    public IReadOnlyList<string>? CustomDomains { get; init; }

    /// <summary>The identifiers of the applications owned by the user.</summary>
    [JsonPropertyName("apps")]
    public IReadOnlyList<string>? Apps { get; init; }

    /// <summary>The name of the user's subscription plan (for example <c>Safira</c>).</summary>
    [JsonPropertyName("plan")]
    public string? Plan { get; init; }

    /// <summary>The user's locale (for example <c>pt-BR</c>).</summary>
    [JsonPropertyName("locale")]
    public string? Locale { get; init; }

    /// <summary>The time remaining in the current plan.</summary>
    [JsonPropertyName("lastDataLeft")]
    public PlanTimeLeft? LastDataLeft { get; init; }

    /// <summary>
    /// The end date of the current plan as sent by the API: an ISO 8601 timestamp, or <c>Subscription</c> for
    /// subscription plans.
    /// </summary>
    [JsonPropertyName("planDataEnd")]
    public string? PlanDataEnd { get; init; }
}

/// <summary>
/// The time remaining in the user's current plan.
/// </summary>
public sealed record PlanTimeLeft
{
    /// <summary>Whole days remaining.</summary>
    [JsonPropertyName("days")]
    public int? Days { get; init; }

    /// <summary>Hours remaining after <see cref="Days"/>.</summary>
    [JsonPropertyName("hours")]
    public int? Hours { get; init; }

    /// <summary>Minutes remaining after <see cref="Hours"/>.</summary>
    [JsonPropertyName("minutes")]
    public int? Minutes { get; init; }

    /// <summary>Seconds remaining after <see cref="Minutes"/>.</summary>
    [JsonPropertyName("seconds")]
    public int? Seconds { get; init; }
}

/// <summary>
/// The account data used by the Discloud VS Code extension (<c>GET /vscode</c>).
/// </summary>
public sealed record VscodeUser
{
    /// <summary>The unique identifier of the user.</summary>
    [JsonPropertyName("userID")]
    public string? UserId { get; init; }

    /// <summary>The user's username, if known.</summary>
    [JsonPropertyName("username")]
    public string? Username { get; init; }

    /// <summary>The URL of the user's avatar, if any.</summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; init; }

    /// <summary>The name of the user's subscription plan.</summary>
    [JsonPropertyName("plan")]
    public string? Plan { get; init; }

    /// <summary>The user's locale.</summary>
    [JsonPropertyName("locale")]
    public string? Locale { get; init; }

    /// <summary>The total RAM available to the user, in megabytes.</summary>
    [JsonPropertyName("totalRamMb")]
    public int? TotalRamMb { get; init; }

    /// <summary>The RAM currently allocated to the user's applications, in megabytes.</summary>
    [JsonPropertyName("ramUsedMb")]
    public int? RamUsedMb { get; init; }

    /// <summary>The identifiers of the applications owned by the user.</summary>
    [JsonPropertyName("apps")]
    public IReadOnlyList<string>? Apps { get; init; }

    /// <summary>The identifiers of the applications the user moderates.</summary>
    [JsonPropertyName("appsTeam")]
    public IReadOnlyList<string>? AppsTeam { get; init; }

    /// <summary>The subdomains owned by the user.</summary>
    [JsonPropertyName("subdomains")]
    public IReadOnlyList<string>? Subdomains { get; init; }

    /// <summary>The custom domains owned by the user.</summary>
    [JsonPropertyName("customdomains")]
    public IReadOnlyList<string>? CustomDomains { get; init; }

    /// <summary>A summary of each application owned by the user.</summary>
    [JsonPropertyName("appsStatus")]
    public IReadOnlyList<VscodeAppSummary>? AppsStatus { get; init; }
}

/// <summary>
/// A summary of an application, as returned by <c>GET /vscode</c>.
/// </summary>
public sealed record VscodeAppSummary
{
    /// <summary>The unique identifier of the application.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>The name of the application.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>Whether the application is a bot or a site.</summary>
    [JsonPropertyName("type")]
    public AppType? Type { get; init; }

    /// <summary>Whether the application is online.</summary>
    [JsonPropertyName("online")]
    public bool? Online { get; init; }

    /// <summary>The RAM allocated to the application, in megabytes.</summary>
    [JsonPropertyName("ram")]
    public int? Ram { get; init; }

    /// <summary>Whether the application was terminated for exceeding its RAM.</summary>
    [JsonPropertyName("ramKilled")]
    public bool? RamKilled { get; init; }

    /// <summary>The exit code of the application process, if it stopped.</summary>
    [JsonPropertyName("exitCode")]
    public int? ExitCode { get; init; }

    /// <summary>The programming language of the application.</summary>
    [JsonPropertyName("lang")]
    public string? Language { get; init; }

    /// <summary>The main file of the application.</summary>
    [JsonPropertyName("mainFile")]
    public string? MainFile { get; init; }

    /// <summary>The URL of the application avatar.</summary>
    [JsonPropertyName("avatarURL")]
    public string? AvatarUrl { get; init; }

    /// <summary>Whether the application restarts automatically after a failure.</summary>
    [JsonPropertyName("autoRestart")]
    public bool? AutoRestart { get; init; }

    /// <summary>The APT packages installed on the application.</summary>
    [JsonPropertyName("apts")]
    public IReadOnlyList<string>? Apts { get; init; }

    /// <summary>The cluster hosting the application.</summary>
    [JsonPropertyName("clusterName")]
    public string? ClusterName { get; init; }

    /// <summary>The <c>VERSION</c> set in <c>discloud.config</c>.</summary>
    [JsonPropertyName("version")]
    public string? Version { get; init; }
}

internal sealed record UserResponse : DiscloudResponse
{
    [JsonPropertyName("user")]
    public User? User { get; init; }
}

internal sealed record VscodeUserResponse : DiscloudResponse
{
    [JsonPropertyName("user")]
    public VscodeUser? User { get; init; }
}

internal sealed record LocaleResponse : DiscloudResponse
{
    [JsonPropertyName("locale")]
    public DiscloudLocale? Locale { get; init; }
}
