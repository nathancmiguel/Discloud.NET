using Discloud.DTOs;
using Discloud.Http;

namespace Discloud.Clients;

internal sealed class UserClient(DiscloudConnection connection) : IUserClient
{
    private static readonly DiscloudJsonContext Json = DiscloudJsonContext.Default;

    public async Task<User> GetAsync(CancellationToken cancellationToken = default)
    {
        var response = await connection.SendAsync(HttpMethod.Get, "user", Json.UserResponse, cancellationToken).ConfigureAwait(false);
        return DiscloudConnection.Require(response.User, "user");
    }

    public async Task<DiscloudLocale> SetLocaleAsync(DiscloudLocale locale, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locale.Value, nameof(locale));
        var response = await connection.SendAsync(HttpMethod.Put, $"locale/{ApiPath.Segment(locale.Value)}", Json.LocaleResponse, cancellationToken)
            .ConfigureAwait(false);
        return DiscloudConnection.Require(response.Locale, "locale");
    }

    public async Task<VscodeUser> GetVscodeDataAsync(CancellationToken cancellationToken = default)
    {
        var response = await connection.SendAsync(HttpMethod.Get, "vscode", Json.VscodeUserResponse, cancellationToken).ConfigureAwait(false);
        return DiscloudConnection.Require(response.User, "user");
    }
}
