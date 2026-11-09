using System.Globalization;
using System.Net;
using Discloud;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Registers <see cref="IDiscloudClient"/> with dependency injection.
/// </summary>
/// <remarks>
/// The client is registered as a typed client of <see cref="IHttpClientFactory"/>, so connection pooling and handler
/// lifetimes are managed by the factory. Every method returns the <see cref="IHttpClientBuilder"/>, which can add
/// delegating handlers (for example a resilience or logging handler) or replace the primary handler.
/// Both <see cref="IDiscloudClient"/> and <see cref="DiscloudClient"/> can be injected.
/// </remarks>
public static class DiscloudServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Discloud client and binds <see cref="DiscloudClientOptions"/> from the <c>Discloud</c> section of
    /// the application <see cref="IConfiguration"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The HTTP client builder, to customize the HTTP pipeline.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// The token is validated when the client is first resolved: an <see cref="OptionsValidationException"/> is thrown
    /// if <c>Discloud:Token</c> is missing.
    /// </remarks>
    /// <example>
    /// <code>
    /// // appsettings.json: { "Discloud": { "Token": "" } }, with the value supplied by user secrets
    /// // or the Discloud__Token environment variable.
    /// builder.Services.AddDiscloudClient();
    /// </code>
    /// </example>
    public static IHttpClientBuilder AddDiscloudClient(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddOptions<DiscloudClientOptions>()
            .Configure<IConfiguration>((options, configuration) => Bind(configuration.GetSection(DiscloudClientOptions.SectionName), options));
        return AddCore(services);
    }

    /// <summary>
    /// Registers the Discloud client and binds <see cref="DiscloudClientOptions"/> from a configuration section.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration section holding the options, typically <c>Configuration.GetSection("Discloud")</c>.</param>
    /// <returns>The HTTP client builder, to customize the HTTP pipeline.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> or <paramref name="configuration"/> is <see langword="null"/>.</exception>
    /// <example>
    /// <code>
    /// builder.Services.AddDiscloudClient(builder.Configuration.GetSection("Discloud"));
    /// </code>
    /// </example>
    public static IHttpClientBuilder AddDiscloudClient(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        services.AddOptions<DiscloudClientOptions>().Configure(options => Bind(configuration, options));
        return AddCore(services);
    }

    /// <summary>
    /// Registers the Discloud client and configures <see cref="DiscloudClientOptions"/> in code, for applications that
    /// do not use <c>appsettings.json</c>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configures the options.</param>
    /// <returns>The HTTP client builder, to customize the HTTP pipeline.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> or <paramref name="configure"/> is <see langword="null"/>.</exception>
    /// <example>
    /// <code>
    /// services.AddDiscloudClient(options => options.Token = secretStore.Get("discloud-token"));
    /// </code>
    /// </example>
    public static IHttpClientBuilder AddDiscloudClient(this IServiceCollection services, Action<DiscloudClientOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);
        services.AddOptions<DiscloudClientOptions>().Configure(configure);
        return AddCore(services);
    }

    /// <summary>
    /// Copies the supported keys of a configuration section to the options. Binding is explicit rather than
    /// reflection-based so the SDK stays compatible with trimming and Native AOT.
    /// </summary>
    private static void Bind(IConfiguration section, DiscloudClientOptions options)
    {
        if (section[nameof(DiscloudClientOptions.Token)] is { } token)
        {
            options.Token = token;
        }

        if (section[nameof(DiscloudClientOptions.BaseAddress)] is { Length: > 0 } baseAddress)
        {
            options.BaseAddress = Uri.TryCreate(baseAddress, UriKind.Absolute, out var uri)
                ? uri
                : throw new InvalidOperationException($"The configuration value '{section.Path()}:BaseAddress' is not an absolute URI.");
        }

        if (section[nameof(DiscloudClientOptions.Timeout)] is { Length: > 0 } timeout)
        {
            options.Timeout = TimeSpan.TryParse(timeout, CultureInfo.InvariantCulture, out var value)
                ? value
                : throw new InvalidOperationException($"The configuration value '{section.Path()}:Timeout' is not a valid TimeSpan (for example 00:02:00).");
        }
    }

    private static string Path(this IConfiguration configuration)
        => configuration is IConfigurationSection section ? section.Path : DiscloudClientOptions.SectionName;

    private static IHttpClientBuilder AddCore(IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<DiscloudClientOptions>, DiscloudClientOptionsValidator>());

        var builder = services
            .AddHttpClient<IDiscloudClient, DiscloudClient>((provider, httpClient) =>
            {
                httpClient.Timeout = provider.GetRequiredService<IOptions<DiscloudClientOptions>>().Value.Timeout;
            })
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler { AutomaticDecompression = DecompressionMethods.All });

        services.TryAddTransient(provider => (DiscloudClient)provider.GetRequiredService<IDiscloudClient>());
        return builder;
    }
}
