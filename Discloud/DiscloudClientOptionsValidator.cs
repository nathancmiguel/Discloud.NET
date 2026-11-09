using Microsoft.Extensions.Options;

namespace Discloud;

/// <summary>
/// Validates <see cref="DiscloudClientOptions"/> both for dependency injection and for direct instantiation.
/// </summary>
internal sealed class DiscloudClientOptionsValidator : IValidateOptions<DiscloudClientOptions>
{
    public ValidateOptionsResult Validate(string? name, DiscloudClientOptions options)
    {
        var error = GetError(options);
        return error is null ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(error);
    }

    /// <summary>
    /// Throws when the options cannot be used to call the API.
    /// </summary>
    /// <exception cref="ArgumentException">The options are invalid.</exception>
    public static void ThrowIfInvalid(DiscloudClientOptions options, string paramName)
    {
        var error = GetError(options);
        if (error is not null)
        {
            throw new ArgumentException(error, paramName);
        }
    }

    private static string? GetError(DiscloudClientOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Token))
        {
            return $"The Discloud API token is not configured. Set '{DiscloudClientOptions.SectionName}:Token' in configuration " +
                   $"(for example with user secrets or the '{DiscloudClientOptions.SectionName}__Token' environment variable) " +
                   "or assign DiscloudClientOptions.Token.";
        }

        if (options.BaseAddress is null || !options.BaseAddress.IsAbsoluteUri ||
            (options.BaseAddress.Scheme != Uri.UriSchemeHttps && options.BaseAddress.Scheme != Uri.UriSchemeHttp))
        {
            return "DiscloudClientOptions.BaseAddress must be an absolute HTTP or HTTPS address.";
        }

        if (options.Timeout <= TimeSpan.Zero && options.Timeout != System.Threading.Timeout.InfiniteTimeSpan)
        {
            return "DiscloudClientOptions.Timeout must be positive or Timeout.InfiniteTimeSpan.";
        }

        return null;
    }
}
