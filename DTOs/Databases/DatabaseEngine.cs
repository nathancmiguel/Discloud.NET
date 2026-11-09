using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Discloud.DTOs;

/// <summary>
/// A managed database engine.
/// </summary>
/// <remarks>
/// This is a string-backed value: compare it with the static properties, or create one from any string.
/// Call <c>IDatabasesClient.ListPresetsAsync</c> to discover the engines and versions currently offered.
/// </remarks>
[JsonConverter(typeof(StringValueJsonConverter<DatabaseEngine>))]
[DebuggerDisplay("{Value,nq}")]
public readonly struct DatabaseEngine : IEquatable<DatabaseEngine>, IStringValue<DatabaseEngine>
{
    private readonly string? _value;

    /// <summary>
    /// Initializes a new <see cref="DatabaseEngine"/> from its raw API value.
    /// </summary>
    /// <param name="value">The raw value sent to and received from the API.</param>
    /// <exception cref="ArgumentException"><paramref name="value"/> is null, empty or whitespace.</exception>
    public DatabaseEngine(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        _value = value;
    }

    /// <summary>PostgreSQL. Raw value: <c>postgres</c>.</summary>
    public static DatabaseEngine Postgres { get; } = new("postgres");

    /// <summary>MySQL. Raw value: <c>mysql</c>.</summary>
    public static DatabaseEngine MySql { get; } = new("mysql");

    /// <summary>MariaDB. Raw value: <c>mariadb</c>.</summary>
    public static DatabaseEngine MariaDb { get; } = new("mariadb");

    /// <summary>MongoDB. Raw value: <c>mongodb</c>.</summary>
    public static DatabaseEngine MongoDb { get; } = new("mongodb");

    /// <summary>Redis. Raw value: <c>redis</c>.</summary>
    public static DatabaseEngine Redis { get; } = new("redis");

    /// <summary>Valkey. Raw value: <c>valkey</c>.</summary>
    public static DatabaseEngine Valkey { get; } = new("valkey");

    /// <summary>ClickHouse. Raw value: <c>clickhouse</c>.</summary>
    public static DatabaseEngine ClickHouse { get; } = new("clickhouse");

    /// <summary>Gets the raw value sent to and received from the API.</summary>
    public string Value => _value ?? string.Empty;

    /// <summary>Converts a raw API value to a <see cref="DatabaseEngine"/>.</summary>
    /// <param name="value">The raw value.</param>
    public static implicit operator DatabaseEngine(string value) => new(value);

    /// <summary>Determines whether two <see cref="DatabaseEngine"/> values are equal.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator ==(DatabaseEngine left, DatabaseEngine right) => left.Equals(right);

    /// <summary>Determines whether two <see cref="DatabaseEngine"/> values differ.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator !=(DatabaseEngine left, DatabaseEngine right) => !left.Equals(right);

    /// <summary>Creates a <see cref="DatabaseEngine"/> from its raw API value.</summary>
    /// <param name="value">The raw value.</param>
    /// <returns>The corresponding <see cref="DatabaseEngine"/>.</returns>
    public static DatabaseEngine FromString(string value) => new(value);

    /// <inheritdoc/>
    public bool Equals(DatabaseEngine other) => string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is DatabaseEngine other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <summary>Returns the raw API value.</summary>
    /// <returns>The raw value.</returns>
    public override string ToString() => Value;

    static DatabaseEngine IStringValue<DatabaseEngine>.Create(string value) => new(value);
}
