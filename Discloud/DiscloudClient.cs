using System.Net;
using Discloud.Clients;
using Discloud.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Discloud;

/// <summary>
/// The default implementation of <see cref="IDiscloudClient"/>.
/// </summary>
/// <remarks>
/// <para>
/// Create one instance and reuse it for the lifetime of the application: it is thread-safe. When created with a
/// token or <see cref="DiscloudClientOptions"/>, the client owns its <see cref="HttpClient"/> and disposes it in
/// <see cref="Dispose"/>. When created with an existing <see cref="HttpClient"/> (as dependency injection does), the
/// caller keeps ownership of it.
/// </para>
/// <para>
/// In applications that use dependency injection, prefer <c>services.AddDiscloudClient(...)</c>, which uses
/// <see cref="IHttpClientFactory"/>.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// using var client = new DiscloudClient(Environment.GetEnvironmentVariable("DISCLOUD_TOKEN")!);
/// var user = await client.User.GetAsync();
/// </code>
/// </example>
public sealed class DiscloudClient : IDiscloudClient, IDisposable
{
    private static readonly TimeSpan PooledConnectionLifetime = TimeSpan.FromMinutes(2);

    private readonly HttpClient? _ownedHttpClient;

    /// <summary>
    /// Initializes a new client for the public Discloud API with default options.
    /// </summary>
    /// <param name="token">The Discloud API token.</param>
    /// <exception cref="ArgumentException"><paramref name="token"/> is null, empty or whitespace.</exception>
    public DiscloudClient(string token)
        : this(new DiscloudClientOptions { Token = token })
    {
    }

    /// <summary>
    /// Initializes a new client that owns its <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="options">The client options. They are read once; later changes have no effect.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="options"/> is invalid, for example when the token is missing.</exception>
    public DiscloudClient(DiscloudClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        DiscloudClientOptionsValidator.ThrowIfInvalid(options, nameof(options));

        var handler = new SocketsHttpHandler
        {
            // Recycle connections periodically so DNS changes are picked up by long-lived clients.
            PooledConnectionLifetime = PooledConnectionLifetime,
            AutomaticDecompression = DecompressionMethods.All,
        };
        _ownedHttpClient = new HttpClient(handler, disposeHandler: true) { Timeout = options.Timeout };

        var connection = new DiscloudConnection(_ownedHttpClient, options);
        (User, Apps, AppModerators, TeamApps, Explorer, Subdomains, CustomDomains, Snapshots, EnvironmentVariables, Projects, Databases, Teams) =
            CreateClients(connection);
    }

    /// <summary>
    /// Initializes a new client that sends requests through an existing <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="httpClient">
    /// The HTTP client to use, typically created by <see cref="IHttpClientFactory"/>. It is not disposed by this
    /// client, and its <see cref="HttpClient.BaseAddress"/> and default headers are not used or modified.
    /// </param>
    /// <param name="options">The client options. They are read once; later changes have no effect.</param>
    /// <exception cref="ArgumentNullException"><paramref name="httpClient"/> or <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The options are invalid, for example when the token is missing.</exception>
    [ActivatorUtilitiesConstructor]
    public DiscloudClient(HttpClient httpClient, IOptions<DiscloudClientOptions> options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);

        var value = options.Value;
        DiscloudClientOptionsValidator.ThrowIfInvalid(value, nameof(options));

        var connection = new DiscloudConnection(httpClient, value);
        (User, Apps, AppModerators, TeamApps, Explorer, Subdomains, CustomDomains, Snapshots, EnvironmentVariables, Projects, Databases, Teams) =
            CreateClients(connection);
    }

    /// <inheritdoc/>
    public IUserClient User { get; }

    /// <inheritdoc/>
    public IAppsClient Apps { get; }

    /// <inheritdoc/>
    public IAppModeratorsClient AppModerators { get; }

    /// <inheritdoc/>
    public ITeamAppsClient TeamApps { get; }

    /// <inheritdoc/>
    public IExplorerClient Explorer { get; }

    /// <inheritdoc/>
    public ISubdomainsClient Subdomains { get; }

    /// <inheritdoc/>
    public ICustomDomainsClient CustomDomains { get; }

    /// <inheritdoc/>
    public ISnapshotsClient Snapshots { get; }

    /// <inheritdoc/>
    public IEnvironmentVariablesClient EnvironmentVariables { get; }

    /// <inheritdoc/>
    public IProjectsClient Projects { get; }

    /// <inheritdoc/>
    public IDatabasesClient Databases { get; }

    /// <inheritdoc/>
    public ITeamsClient Teams { get; }

    /// <summary>
    /// Disposes the <see cref="HttpClient"/> created by this client, if any. An <see cref="HttpClient"/> passed to the
    /// constructor is left untouched.
    /// </summary>
    public void Dispose() => _ownedHttpClient?.Dispose();

    private static (IUserClient, IAppsClient, IAppModeratorsClient, ITeamAppsClient, IExplorerClient, ISubdomainsClient,
        ICustomDomainsClient, ISnapshotsClient, IEnvironmentVariablesClient, IProjectsClient, IDatabasesClient, ITeamsClient)
        CreateClients(DiscloudConnection connection)
        => (new UserClient(connection),
            new AppsClient(connection),
            new AppModeratorsClient(connection),
            new TeamAppsClient(connection),
            new ExplorerClient(connection),
            new SubdomainsClient(connection),
            new CustomDomainsClient(connection),
            new SnapshotsClient(connection),
            new EnvironmentVariablesClient(connection),
            new ProjectsClient(connection),
            new DatabasesClient(connection),
            new TeamsClient(connection));
}
