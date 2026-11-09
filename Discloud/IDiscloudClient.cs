using Discloud.Clients;

namespace Discloud;

/// <summary>
/// The entry point of the Discloud API. Operations are grouped by resource.
/// </summary>
/// <remarks>
/// <para>
/// Every operation is asynchronous, accepts a <see cref="CancellationToken"/> and is safe to call concurrently.
/// Failures are reported with exceptions derived from <see cref="DiscloudException"/>; a returned value always
/// represents a successful response.
/// </para>
/// <para>
/// Inject this interface when the client is registered with <c>AddDiscloudClient</c>, or create a
/// <see cref="DiscloudClient"/> directly.
/// </para>
/// </remarks>
public interface IDiscloudClient
{
    /// <summary>Gets the operations on the authenticated account.</summary>
    IUserClient User { get; }

    /// <summary>Gets the operations on the applications you own.</summary>
    IAppsClient Apps { get; }

    /// <summary>Gets the operations on the moderators of the applications you own.</summary>
    IAppModeratorsClient AppModerators { get; }

    /// <summary>Gets the operations on applications owned by other users that you moderate.</summary>
    ITeamAppsClient TeamApps { get; }

    /// <summary>Gets the file explorer of your applications.</summary>
    IExplorerClient Explorer { get; }

    /// <summary>Gets the operations on <c>*.discloud.app</c> subdomains.</summary>
    ISubdomainsClient Subdomains { get; }

    /// <summary>Gets the operations on custom domains.</summary>
    ICustomDomainsClient CustomDomains { get; }

    /// <summary>Gets the operations on versioned application backups (snapshots).</summary>
    ISnapshotsClient Snapshots { get; }

    /// <summary>Gets the operations on account, project and application environment variables.</summary>
    IEnvironmentVariablesClient EnvironmentVariables { get; }

    /// <summary>Gets the operations on projects.</summary>
    IProjectsClient Projects { get; }

    /// <summary>Gets the operations on managed databases.</summary>
    IDatabasesClient Databases { get; }

    /// <summary>Gets the operations on teams and their members.</summary>
    ITeamsClient Teams { get; }
}
