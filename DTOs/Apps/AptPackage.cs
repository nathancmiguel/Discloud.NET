using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Discloud.DTOs;

/// <summary>
/// An APT package bundle that can be installed on an application.
/// </summary>
/// <remarks>
/// This is a string-backed value: compare it with the static properties, or create one from any string.
/// These are the values accepted by the API at the time of writing; any other value is sent as-is.
/// </remarks>
[JsonConverter(typeof(StringValueJsonConverter<AptPackage>))]
[DebuggerDisplay("{Value,nq}")]
public readonly struct AptPackage : IEquatable<AptPackage>, IStringValue<AptPackage>
{
    private readonly string? _value;

    /// <summary>
    /// Initializes a new <see cref="AptPackage"/> from its raw API value.
    /// </summary>
    /// <param name="value">The raw value sent to and received from the API.</param>
    /// <exception cref="ArgumentException"><paramref name="value"/> is null, empty or whitespace.</exception>
    public AptPackage(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        _value = value;
    }

    /// <summary>Native dependencies for <c>canvas</c>. Raw value: <c>canvas</c>.</summary>
    public static AptPackage Canvas { get; } = new("canvas");

    /// <summary>FFmpeg. Raw value: <c>ffmpeg</c>.</summary>
    public static AptPackage Ffmpeg { get; } = new("ffmpeg");

    /// <summary>A Java runtime. Raw value: <c>java</c>.</summary>
    public static AptPackage Java { get; } = new("java");

    /// <summary>OpenGL libraries. Raw value: <c>libgl</c>.</summary>
    public static AptPackage Libgl { get; } = new("libgl");

    /// <summary>OpenSSL. Raw value: <c>openssl</c>.</summary>
    public static AptPackage OpenSsl { get; } = new("openssl");

    /// <summary>Dependencies required by Puppeteer / headless Chromium. Raw value: <c>puppeteer</c>.</summary>
    public static AptPackage Puppeteer { get; } = new("puppeteer");

    /// <summary>Common command-line tools. Raw value: <c>tools</c>.</summary>
    public static AptPackage Tools { get; } = new("tools");

    /// <summary>Gets the raw value sent to and received from the API.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Converts a raw API value to a <see cref="AptPackage"/>.</summary>
    /// <param name="value">The raw value.</param>
    public static implicit operator AptPackage(string value) => new(value);

    /// <summary>Determines whether two <see cref="AptPackage"/> values are equal.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator ==(AptPackage left, AptPackage right) => left.Equals(right);

    /// <summary>Determines whether two <see cref="AptPackage"/> values differ.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator !=(AptPackage left, AptPackage right) => !left.Equals(right);

    /// <summary>Creates a <see cref="AptPackage"/> from its raw API value.</summary>
    /// <param name="value">The raw value.</param>
    /// <returns>The corresponding <see cref="AptPackage"/>.</returns>
    public static AptPackage FromString(string value) => new(value);

    /// <inheritdoc/>
    public bool Equals(AptPackage other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is AptPackage other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the raw API value.</summary>
    /// <returns>The raw value.</returns>
    public override string ToString() => Value;

    static AptPackage IStringValue<AptPackage>.Create(string value) => new(value);
}
