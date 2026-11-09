using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Discloud.DTOs;

/// <summary>
/// The kind of an entry in the application file explorer.
/// </summary>
/// <remarks>
/// This is a string-backed value: compare it with the static properties, or create one from any string.
/// </remarks>
[JsonConverter(typeof(StringValueJsonConverter<ExplorerItemType>))]
[DebuggerDisplay("{Value,nq}")]
public readonly struct ExplorerItemType : IEquatable<ExplorerItemType>, IStringValue<ExplorerItemType>
{
    private readonly string? _value;

    /// <summary>
    /// Initializes a new <see cref="ExplorerItemType"/> from its raw API value.
    /// </summary>
    /// <param name="value">The raw value sent to and received from the API.</param>
    /// <exception cref="ArgumentException"><paramref name="value"/> is null, empty or whitespace.</exception>
    public ExplorerItemType(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        _value = value;
    }

    /// <summary>A regular file. Raw value: <c>file</c>.</summary>
    public static ExplorerItemType File { get; } = new("file");

    /// <summary>A directory. Raw value: <c>directory</c>.</summary>
    public static ExplorerItemType Directory { get; } = new("directory");

    /// <summary>Gets the raw value sent to and received from the API.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Converts a raw API value to a <see cref="ExplorerItemType"/>.</summary>
    /// <param name="value">The raw value.</param>
    public static implicit operator ExplorerItemType(string value) => new(value);

    /// <summary>Determines whether two <see cref="ExplorerItemType"/> values are equal.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator ==(ExplorerItemType left, ExplorerItemType right) => left.Equals(right);

    /// <summary>Determines whether two <see cref="ExplorerItemType"/> values differ.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator !=(ExplorerItemType left, ExplorerItemType right) => !left.Equals(right);

    /// <summary>Creates a <see cref="ExplorerItemType"/> from its raw API value.</summary>
    /// <param name="value">The raw value.</param>
    /// <returns>The corresponding <see cref="ExplorerItemType"/>.</returns>
    public static ExplorerItemType FromString(string value) => new(value);

    /// <inheritdoc/>
    public bool Equals(ExplorerItemType other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ExplorerItemType other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the raw API value.</summary>
    /// <returns>The raw value.</returns>
    public override string ToString() => Value;

    static ExplorerItemType IStringValue<ExplorerItemType>.Create(string value) => new(value);
}
