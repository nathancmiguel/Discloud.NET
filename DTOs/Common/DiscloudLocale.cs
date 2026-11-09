using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Discloud.DTOs;

/// <summary>
/// A language supported by the Discloud API for account messages.
/// </summary>
/// <remarks>
/// This is a string-backed value: compare it with the static properties, or create one from any string.
/// Use the predefined values. The API answers with HTTP 404 for locales it does not support.
/// </remarks>
[JsonConverter(typeof(StringValueJsonConverter<DiscloudLocale>))]
[DebuggerDisplay("{Value,nq}")]
public readonly struct DiscloudLocale : IEquatable<DiscloudLocale>, IStringValue<DiscloudLocale>
{
    private readonly string? _value;

    /// <summary>
    /// Initializes a new <see cref="DiscloudLocale"/> from its raw API value.
    /// </summary>
    /// <param name="value">The raw value sent to and received from the API.</param>
    /// <exception cref="ArgumentException"><paramref name="value"/> is null, empty or whitespace.</exception>
    public DiscloudLocale(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        _value = value;
    }

    /// <summary>Brazilian Portuguese (<c>pt-BR</c>). Raw value: <c>pt-BR</c>.</summary>
    public static DiscloudLocale PortugueseBrazil { get; } = new("pt-BR");

    /// <summary>United States English (<c>en-US</c>). Raw value: <c>en-US</c>.</summary>
    public static DiscloudLocale EnglishUnitedStates { get; } = new("en-US");

    /// <summary>Gets the raw value sent to and received from the API.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Converts a raw API value to a <see cref="DiscloudLocale"/>.</summary>
    /// <param name="value">The raw value.</param>
    public static implicit operator DiscloudLocale(string value) => new(value);

    /// <summary>Determines whether two <see cref="DiscloudLocale"/> values are equal.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator ==(DiscloudLocale left, DiscloudLocale right) => left.Equals(right);

    /// <summary>Determines whether two <see cref="DiscloudLocale"/> values differ.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator !=(DiscloudLocale left, DiscloudLocale right) => !left.Equals(right);

    /// <summary>Creates a <see cref="DiscloudLocale"/> from its raw API value.</summary>
    /// <param name="value">The raw value.</param>
    /// <returns>The corresponding <see cref="DiscloudLocale"/>.</returns>
    public static DiscloudLocale FromString(string value) => new(value);

    /// <inheritdoc/>
    public bool Equals(DiscloudLocale other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is DiscloudLocale other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the raw API value.</summary>
    /// <returns>The raw value.</returns>
    public override string ToString() => Value;

    static DiscloudLocale IStringValue<DiscloudLocale>.Create(string value) => new(value);
}
