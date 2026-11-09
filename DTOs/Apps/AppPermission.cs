using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Discloud.DTOs;

/// <summary>
/// A permission that can be granted to a moderator of an application.
/// </summary>
/// <remarks>
/// This is a string-backed value: compare it with the static properties, or create one from any string.
/// Unknown values sent by the API are preserved, so newer permissions can still be read and sent back.
/// </remarks>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "Mirrors the Discloud API concept; unrelated to code access security.")]
[JsonConverter(typeof(StringValueJsonConverter<AppPermission>))]
[DebuggerDisplay("{Value,nq}")]
public readonly struct AppPermission : IEquatable<AppPermission>, IStringValue<AppPermission>
{
    private readonly string? _value;

    /// <summary>
    /// Initializes a new <see cref="AppPermission"/> from its raw API value.
    /// </summary>
    /// <param name="value">The raw value sent to and received from the API.</param>
    /// <exception cref="ArgumentException"><paramref name="value"/> is null, empty or whitespace.</exception>
    public AppPermission(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        _value = value;
    }

    /// <summary>Download backups of the application. Raw value: <c>backup_app</c>.</summary>
    public static AppPermission BackupApp { get; } = new("backup_app");

    /// <summary>Upload new versions of the application code. Raw value: <c>commit_app</c>.</summary>
    public static AppPermission CommitApp { get; } = new("commit_app");

    /// <summary>Change the RAM allocated to the application. Raw value: <c>edit_ram</c>.</summary>
    public static AppPermission EditRam { get; } = new("edit_ram");

    /// <summary>Read the application logs. Raw value: <c>logs_app</c>.</summary>
    public static AppPermission LogsApp { get; } = new("logs_app");

    /// <summary>Restart the application. Raw value: <c>restart_app</c>.</summary>
    public static AppPermission RestartApp { get; } = new("restart_app");

    /// <summary>Start the application. Raw value: <c>start_app</c>.</summary>
    public static AppPermission StartApp { get; } = new("start_app");

    /// <summary>Read the application status. Raw value: <c>status_app</c>.</summary>
    public static AppPermission StatusApp { get; } = new("status_app");

    /// <summary>Stop the application. Raw value: <c>stop_app</c>.</summary>
    public static AppPermission StopApp { get; } = new("stop_app");

    /// <summary>Gets the raw value sent to and received from the API.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Converts a raw API value to a <see cref="AppPermission"/>.</summary>
    /// <param name="value">The raw value.</param>
    public static implicit operator AppPermission(string value) => new(value);

    /// <summary>Determines whether two <see cref="AppPermission"/> values are equal.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator ==(AppPermission left, AppPermission right) => left.Equals(right);

    /// <summary>Determines whether two <see cref="AppPermission"/> values differ.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator !=(AppPermission left, AppPermission right) => !left.Equals(right);

    /// <summary>Creates a <see cref="AppPermission"/> from its raw API value.</summary>
    /// <param name="value">The raw value.</param>
    /// <returns>The corresponding <see cref="AppPermission"/>.</returns>
    public static AppPermission FromString(string value) => new(value);

    /// <inheritdoc/>
    public bool Equals(AppPermission other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is AppPermission other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the raw API value.</summary>
    /// <returns>The raw value.</returns>
    public override string ToString() => Value;

    static AppPermission IStringValue<AppPermission>.Create(string value) => new(value);
}
