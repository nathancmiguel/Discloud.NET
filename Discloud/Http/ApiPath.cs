using System.Text;

namespace Discloud.Http;

/// <summary>
/// Builds relative request paths with escaped segments and query strings.
/// </summary>
internal static class ApiPath
{
    /// <summary>
    /// Escapes a value used as a single path segment, so characters such as <c>/</c>, <c>?</c> or <c>#</c> cannot
    /// change the target route.
    /// </summary>
    public static string Segment(string value) => Uri.EscapeDataString(value);

    /// <summary>
    /// Appends the non-null parameters to <paramref name="path"/> as an escaped query string.
    /// </summary>
    public static string WithQuery(string path, params ReadOnlySpan<(string Name, string? Value)> parameters)
    {
        StringBuilder? builder = null;
        foreach (var (name, value) in parameters)
        {
            if (value is null)
            {
                continue;
            }

            builder ??= new StringBuilder(path);
            builder.Append(builder.Length == path.Length ? '?' : '&')
                .Append(Uri.EscapeDataString(name))
                .Append('=')
                .Append(Uri.EscapeDataString(value));
        }

        return builder?.ToString() ?? path;
    }
}
