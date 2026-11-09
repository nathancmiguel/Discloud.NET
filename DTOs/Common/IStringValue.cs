namespace Discloud.DTOs;

/// <summary>
/// Implemented by the string-backed value types (such as <see cref="TeamRole"/>) so a single JSON converter can
/// handle all of them.
/// </summary>
/// <typeparam name="TSelf">The implementing type.</typeparam>
internal interface IStringValue<TSelf>
    where TSelf : struct, IStringValue<TSelf>
{
    /// <summary>Gets the raw string value sent to and received from the API.</summary>
    string Value { get; }

    /// <summary>Creates an instance from a raw API value.</summary>
    static abstract TSelf Create(string value);
}
