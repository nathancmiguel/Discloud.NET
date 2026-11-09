using System.Text.Json.Serialization;

namespace Discloud.DTOs;

/// <summary>
/// The resource usage of a running application (<c>GET /app/{appID}/status</c>).
/// </summary>
public sealed record AppStatus
{
    /// <summary>The unique identifier of the application.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>The container state as sent by the API (for example <c>Online</c>).</summary>
    [JsonPropertyName("container")]
    public string? Container { get; init; }

    /// <summary>The current CPU usage, formatted by the API (for example <c>0.00%</c>).</summary>
    [JsonPropertyName("cpu")]
    public string? Cpu { get; init; }

    /// <summary>The current memory usage and limit, formatted by the API (for example <c>39.9MB/100MB</c>).</summary>
    [JsonPropertyName("memory")]
    public string? Memory { get; init; }

    /// <summary>The storage used by the application, formatted by the API (for example <c>1.999GB</c>).</summary>
    [JsonPropertyName("ssd")]
    public string? Ssd { get; init; }

    /// <summary>The network traffic of the application.</summary>
    [JsonPropertyName("netIO")]
    public NetworkIo? NetIO { get; init; }

    /// <summary>The time since the last restart, formatted by the API (for example <c>30 minutes</c>).</summary>
    [JsonPropertyName("last_restart")]
    public string? LastRestart { get; init; }

    /// <summary>When the application container was started.</summary>
    [JsonPropertyName("startedAt")]
    public DateTimeOffset? StartedAt { get; init; }
}

/// <summary>
/// Network traffic counters, formatted by the API (for example <c>61KB</c>).
/// </summary>
public sealed record NetworkIo
{
    /// <summary>The downloaded traffic.</summary>
    [JsonPropertyName("down")]
    public string? Down { get; init; }

    /// <summary>The uploaded traffic.</summary>
    [JsonPropertyName("up")]
    public string? Up { get; init; }
}

/// <summary>
/// The logs of an application (<c>GET /app/{appID}/logs</c>).
/// </summary>
public sealed record AppLogs
{
    /// <summary>The unique identifier of the application.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>The terminal output.</summary>
    [JsonPropertyName("terminal")]
    public Terminal? Terminal { get; init; }
}

/// <summary>
/// The terminal output of an application or database container.
/// </summary>
public sealed record Terminal
{
    /// <summary>The complete recent output.</summary>
    [JsonPropertyName("big")]
    public string? Big { get; init; }

    /// <summary>A condensed version of the recent output.</summary>
    [JsonPropertyName("small")]
    public string? Small { get; init; }

    /// <summary>A URL where the output can be viewed online, when available.</summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }
}

/// <summary>
/// A generated application backup (<c>GET /app/{appID}/backup</c>).
/// </summary>
public sealed record AppBackup
{
    /// <summary>The unique identifier of the application.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>The URL from which the backup can be downloaded.</summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>The backup status. Only sent when backups for all applications are requested.</summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }
}

/// <summary>
/// The process state of an application after a start request.
/// </summary>
public sealed record AppRunState
{
    /// <summary>Whether the application is online.</summary>
    [JsonPropertyName("online")]
    public bool? Online { get; init; }

    /// <summary>Whether the application was terminated for exceeding its RAM.</summary>
    [JsonPropertyName("ramKilled")]
    public bool? RamKilled { get; init; }

    /// <summary>The exit code of the application process.</summary>
    [JsonPropertyName("exitCode")]
    public int? ExitCode { get; init; }
}

/// <summary>
/// The result of starting one application or all of them (<c>PUT /app/{appID}/start</c>).
/// </summary>
public sealed record AppStartResponse : DiscloudResponse
{
    /// <summary>The state of the application. Sent when a single application is started.</summary>
    [JsonPropertyName("appStatus")]
    public AppRunState? AppStatus { get; init; }

    /// <summary>The per-application outcome. Sent when all applications are started.</summary>
    [JsonPropertyName("apps")]
    public BulkStartResult? Apps { get; init; }
}

/// <summary>
/// The per-application outcome of starting all applications.
/// </summary>
public sealed record BulkStartResult
{
    /// <summary>The applications that were started.</summary>
    [JsonPropertyName("started")]
    public IReadOnlyList<string>? Started { get; init; }

    /// <summary>The applications that were already online.</summary>
    [JsonPropertyName("alreadyOnline")]
    public IReadOnlyList<string>? AlreadyOnline { get; init; }

    /// <summary>The applications that were already being processed.</summary>
    [JsonPropertyName("alreadyInProcess")]
    public IReadOnlyList<string>? AlreadyInProcess { get; init; }
}

/// <summary>
/// The result of restarting one application or all of them (<c>PUT /app/{appID}/restart</c>).
/// </summary>
public sealed record AppRestartResponse : DiscloudResponse
{
    /// <summary>The per-application outcome. Sent when all applications are restarted.</summary>
    [JsonPropertyName("apps")]
    public BulkRestartResult? Apps { get; init; }
}

/// <summary>
/// The per-application outcome of restarting all applications.
/// </summary>
public sealed record BulkRestartResult
{
    /// <summary>The applications that were restarted.</summary>
    [JsonPropertyName("restarted")]
    public IReadOnlyList<string>? Restarted { get; init; }

    /// <summary>The applications that were offline.</summary>
    [JsonPropertyName("alreadyOffline")]
    public IReadOnlyList<string>? AlreadyOffline { get; init; }

    /// <summary>The applications that were already being processed.</summary>
    [JsonPropertyName("alreadyInProcess")]
    public IReadOnlyList<string>? AlreadyInProcess { get; init; }
}

/// <summary>
/// The result of stopping one application or all of them (<c>PUT /app/{appID}/stop</c>).
/// </summary>
public sealed record AppStopResponse : DiscloudResponse
{
    /// <summary>The per-application outcome. Sent when all applications are stopped.</summary>
    [JsonPropertyName("apps")]
    public BulkStopResult? Apps { get; init; }
}

/// <summary>
/// The per-application outcome of stopping all applications.
/// </summary>
public sealed record BulkStopResult
{
    /// <summary>The applications that were stopped. The API spells this key <c>stoped</c>.</summary>
    [JsonPropertyName("stoped")]
    public IReadOnlyList<string>? Stopped { get; init; }

    /// <summary>The applications that were already offline.</summary>
    [JsonPropertyName("alreadyOffline")]
    public IReadOnlyList<string>? AlreadyOffline { get; init; }

    /// <summary>The applications that were already being processed.</summary>
    [JsonPropertyName("alreadyInProcess")]
    public IReadOnlyList<string>? AlreadyInProcess { get; init; }
}

/// <summary>
/// The result of deleting one application or all of them (<c>DELETE /app/{appID}/delete</c>).
/// </summary>
public sealed record AppDeleteResponse : DiscloudResponse
{
    /// <summary>The per-application outcome. Sent when all applications are deleted.</summary>
    [JsonPropertyName("apps")]
    public BulkDeleteResult? Apps { get; init; }
}

/// <summary>
/// The per-application outcome of deleting all applications.
/// </summary>
public sealed record BulkDeleteResult
{
    /// <summary>The applications that were removed. The API spells this key <c>removealled</c>.</summary>
    [JsonPropertyName("removealled")]
    public IReadOnlyList<string>? Removed { get; init; }

    /// <summary>The applications that were offline.</summary>
    [JsonPropertyName("alreadyOffline")]
    public IReadOnlyList<string>? AlreadyOffline { get; init; }

    /// <summary>The applications that were already being processed.</summary>
    [JsonPropertyName("alreadyInProcess")]
    public IReadOnlyList<string>? AlreadyInProcess { get; init; }
}

/// <summary>
/// The output of a command sent to an application terminal (<c>PUT /app/{appID}/console</c>).
/// </summary>
public sealed record AppConsoleResult
{
    /// <summary>The unique identifier of the application.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>The command output.</summary>
    [JsonPropertyName("shell")]
    public ConsoleShellOutput? Shell { get; init; }
}

/// <summary>
/// The output of a terminal command.
/// </summary>
public sealed record ConsoleShellOutput
{
    /// <summary>Whether the application is online.</summary>
    [JsonPropertyName("online")]
    public bool? Online { get; init; }

    /// <summary>The standard output of the command.</summary>
    [JsonPropertyName("stdout")]
    public string? Stdout { get; init; }

    /// <summary>The standard error of the command.</summary>
    [JsonPropertyName("stderr")]
    public string? Stderr { get; init; }
}

/// <summary>
/// The result of executing a command inside an application container (<c>PUT /app/{appID}/exec</c>).
/// </summary>
public sealed record ExecResponse : DiscloudResponse
{
    /// <summary>The command output.</summary>
    [JsonPropertyName("exec")]
    public ExecOutput? Exec { get; init; }
}

/// <summary>
/// The output of a command executed inside an application container.
/// </summary>
public sealed record ExecOutput
{
    /// <summary>The standard output of the command.</summary>
    [JsonPropertyName("stdout")]
    public string? Stdout { get; init; }

    /// <summary>The standard error of the command.</summary>
    [JsonPropertyName("stderr")]
    public string? Stderr { get; init; }

    /// <summary>The exit code of the command.</summary>
    [JsonPropertyName("exitCode")]
    public int? ExitCode { get; init; }
}

internal sealed record AppStatusResponse : DiscloudResponse
{
    [JsonPropertyName("apps")]
    [JsonConverter(typeof(SingleOrArrayJsonConverter<AppStatus>))]
    public IReadOnlyList<AppStatus>? Apps { get; init; }
}

internal sealed record AppLogsResponse : DiscloudResponse
{
    [JsonPropertyName("apps")]
    [JsonConverter(typeof(SingleOrArrayJsonConverter<AppLogs>))]
    public IReadOnlyList<AppLogs>? Apps { get; init; }
}

internal sealed record AppBackupResponse : DiscloudResponse
{
    [JsonPropertyName("backups")]
    [JsonConverter(typeof(SingleOrArrayJsonConverter<AppBackup>))]
    public IReadOnlyList<AppBackup>? Backups { get; init; }
}

internal sealed record AppConsoleResponse : DiscloudResponse
{
    [JsonPropertyName("apps")]
    public AppConsoleResult? Apps { get; init; }
}

internal sealed record AppConsoleRequest([property: JsonPropertyName("command")] string Command);

internal sealed record ExecRequest([property: JsonPropertyName("cmd")] string Cmd);
