using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Discloud.DTOs;

/// <summary>
/// The role of a member inside a team.
/// </summary>
/// <remarks>
/// This is a string-backed value: compare it with the static properties, or create one from any string.
/// <see cref="Admin"/>, <see cref="Developer"/> and <see cref="Viewer"/> can be assigned. <see cref="Owner"/> is only reported for the team owner and cannot be assigned.
/// </remarks>
[JsonConverter(typeof(StringValueJsonConverter<TeamRole>))]
[DebuggerDisplay("{Value,nq}")]
public readonly struct TeamRole : IEquatable<TeamRole>, IStringValue<TeamRole>
{
    private readonly string? _value;

    /// <summary>
    /// Initializes a new <see cref="TeamRole"/> from its raw API value.
    /// </summary>
    /// <param name="value">The raw value sent to and received from the API.</param>
    /// <exception cref="ArgumentException"><paramref name="value"/> is null, empty or whitespace.</exception>
    public TeamRole(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        _value = value;
    }

    /// <summary>Full management access to the team resources. Raw value: <c>admin</c>.</summary>
    public static TeamRole Admin { get; } = new("admin");

    /// <summary>Development access to the team resources. Raw value: <c>developer</c>.</summary>
    public static TeamRole Developer { get; } = new("developer");

    /// <summary>Read-only access to the team resources. Raw value: <c>viewer</c>.</summary>
    public static TeamRole Viewer { get; } = new("viewer");

    /// <summary>The owner of the team. Reported by the API; it cannot be assigned. Raw value: <c>owner</c>.</summary>
    public static TeamRole Owner { get; } = new("owner");

    /// <summary>Gets the raw value sent to and received from the API.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Converts a raw API value to a <see cref="TeamRole"/>.</summary>
    /// <param name="value">The raw value.</param>
    public static implicit operator TeamRole(string value) => new(value);

    /// <summary>Determines whether two <see cref="TeamRole"/> values are equal.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator ==(TeamRole left, TeamRole right) => left.Equals(right);

    /// <summary>Determines whether two <see cref="TeamRole"/> values differ.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator !=(TeamRole left, TeamRole right) => !left.Equals(right);

    /// <summary>Creates a <see cref="TeamRole"/> from its raw API value.</summary>
    /// <param name="value">The raw value.</param>
    /// <returns>The corresponding <see cref="TeamRole"/>.</returns>
    public static TeamRole FromString(string value) => new(value);

    /// <inheritdoc/>
    public bool Equals(TeamRole other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is TeamRole other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the raw API value.</summary>
    /// <returns>The raw value.</returns>
    public override string ToString() => Value;

    static TeamRole IStringValue<TeamRole>.Create(string value) => new(value);
}
