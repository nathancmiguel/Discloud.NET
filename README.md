# Discloud.NET

A strongly typed, asynchronous .NET client for the [Discloud](https://discloud.com/) API (v2).

`Discloud.Net` wraps every operation of the Discloud API contract (97 operations) in an idiomatic C# API: no
hand-written HTTP requests, authentication headers, JSON handling or error parsing.

## Features

- **Complete API coverage**: applications, moderators, team apps, file explorer, subdomains, custom domains,
  snapshots, environment variables, projects, managed databases and teams. See the [API reference](#api-reference).
- **Strongly typed**: immutable `record` DTOs, nullable annotations, typed values for roles, permissions, engines
  and locales.
- **Async throughout**: every method is `async`, ends in `Async` and accepts a `CancellationToken`.
- **Dependency injection**: `services.AddDiscloudClient(...)` on top of `IHttpClientFactory`, with
  `appsettings.json`, user secrets and environment variable support.
- **Clear errors**: a small exception hierarchy separating API errors, network failures and invalid responses.
- **Lightweight**: `System.Text.Json` source generation (trimming and Native AOT compatible), streaming
  deserialization, and only two dependencies (`Microsoft.Extensions.Http` and
  `Microsoft.Extensions.Options.ConfigurationExtensions`).

## Supported .NET versions

| Target framework | Status |
| --- | --- |
| .NET 10 (`net10.0`) | Supported (LTS) |
| .NET 8 (`net8.0`) | Supported (LTS) |

.NET 9 and later non-LTS releases use the `net8.0` build. .NET Framework and .NET Standard are not supported.

## Installation

```bash
dotnet add package Discloud.Net
```

## Authentication

Every request is authenticated with your Discloud API token, sent in the `api-token` header. Obtain a token from
your Discloud account as described in the Discloud documentation.

> **Never commit a real token to source control.** Use [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets)
> during development, and environment variables or a secret manager (Azure Key Vault, AWS Secrets Manager, …) in
> production. The SDK never writes the token to logs, exception messages, `ToString()` output or serialized options.

### appsettings.json

The SDK reads the `Discloud` configuration section:

```json
{
  "Discloud": {
    "Token": ""
  }
}
```

Leave the value empty in `appsettings.json` and supply it from a safer source.

User secrets (development):

```bash
dotnet user-secrets init
```

```bash
dotnet user-secrets set "Discloud:Token" "your-api-token"
```

Environment variable (production; `__` maps to the `:` section separator):

```bash
export Discloud__Token=your-api-token
```

The section also accepts optional `BaseAddress` (default `https://api.discloud.app/v2/`) and `Timeout`
(default `00:01:40`) keys.

## Quick start

### Dependency injection (ASP.NET Core, Worker Service, generic host)

```csharp
using Discloud;

var builder = WebApplication.CreateBuilder(args);

// Binds the "Discloud" section of the configuration (appsettings.json, user secrets, environment variables).
builder.Services.AddDiscloudClient(builder.Configuration.GetSection("Discloud"));

var app = builder.Build();

app.MapGet("/apps", async (IDiscloudClient discloud, CancellationToken ct) =>
    await discloud.Apps.GetAllAsync(ct));

app.Run();
```

Other registration options:

```csharp
// Reads the "Discloud" section from the IConfiguration registered in the container.
builder.Services.AddDiscloudClient();

// Configures the options in code, for applications that do not use appsettings.json.
builder.Services.AddDiscloudClient(options =>
{
    options.Token = Environment.GetEnvironmentVariable("DISCLOUD_TOKEN");
    options.Timeout = TimeSpan.FromMinutes(5);
});
```

Each overload returns an `IHttpClientBuilder`, so the HTTP pipeline can be extended, for example with a logging or
resilience handler. Inject either `IDiscloudClient` or `DiscloudClient`. A missing token fails with an
`OptionsValidationException` when the client is first resolved.

### Direct instantiation

```csharp
using Discloud;

using var discloud = new DiscloudClient(Environment.GetEnvironmentVariable("DISCLOUD_TOKEN")!);

var user = await discloud.User.GetAsync();
Console.WriteLine($"{user.Username}: {user.RamUsedMb}/{user.TotalRamMb} MB ({user.Plan})");
```

Create one `DiscloudClient` and reuse it: it is thread-safe and pools connections. A client created from a token or
`DiscloudClientOptions` owns its `HttpClient` and disposes it in `Dispose()`. You can also pass your own
`HttpClient`, which the client never disposes or modifies:

```csharp
using Microsoft.Extensions.Options;

var discloud = new DiscloudClient(httpClient, Options.Create(new DiscloudClientOptions { Token = token }));
```

## Usage examples

All examples use an `IDiscloudClient` named `discloud`. Models live in the `Discloud.DTOs` namespace.

```csharp
using Discloud;
using Discloud.DTOs;
```

### Applications

```csharp
// List and inspect.
IReadOnlyList<App> apps = await discloud.Apps.GetAllAsync();
App app = await discloud.Apps.GetAsync("1234567890");
AppStatus status = await discloud.Apps.GetStatusAsync(app.Id!);
Console.WriteLine($"{app.Name}: {status.Cpu} CPU, {status.Memory}");

AppLogs logs = await discloud.Apps.GetLogsAsync(app.Id!);
Console.WriteLine(logs.Terminal?.Small);

// Lifecycle. Methods ending in "All" act on every application.
await discloud.Apps.RestartAsync(app.Id!);
AppStopResponse stopped = await discloud.Apps.StopAllAsync();
Console.WriteLine($"Stopped: {string.Join(", ", stopped.Apps?.Stopped ?? [])}");

// Upload a new application, or commit a new version. The stream is not disposed by the SDK.
await using (var zip = File.OpenRead("bot.zip"))
{
    UploadResponse uploaded = await discloud.Apps.UploadAsync(zip, "bot.zip");
    Console.WriteLine($"Created {uploaded.App?.Id}");
}

await using (var zip = File.OpenRead("bot.zip"))
{
    await discloud.Apps.CommitAsync(app.Id!, zip, "bot.zip");
}

// Configuration.
await discloud.Apps.SetRamAsync(app.Id!, 256);
await discloud.Apps.UpdateProfileAsync(app.Id!, new UpdateAppProfileRequest { Name = "My Bot" });
await discloud.Apps.InstallAptAsync(app.Id!, [AptPackage.Ffmpeg, AptPackage.Java]);

// Run commands.
ExecResponse exec = await discloud.Apps.ExecAsync(app.Id!, "ls -la");
Console.WriteLine(exec.Exec?.Stdout);

AppConsoleResult console = await discloud.Apps.RunConsoleCommandAsync(app.Id!, "ls");
Console.WriteLine(console.Shell?.Stdout);
```

### Moderators and team apps

```csharp
// Moderators of your applications.
await discloud.AppModerators.AddAsync("1234567890", "197651928518230016",
    [AppPermission.StartApp, AppPermission.StopApp, AppPermission.LogsApp]);
IReadOnlyList<AppModerator> moderators = await discloud.AppModerators.ListAsync("1234567890");

// Applications of other users that you moderate.
foreach (TeamApp teamApp in await discloud.TeamApps.ListAsync())
{
    Console.WriteLine($"{teamApp.Name} by {teamApp.Owner?.Name}");
}

await discloud.TeamApps.RestartAsync("1745287471226");
```

### File explorer

```csharp
ExplorerListResponse listing = await discloud.Explorer.ListAsync("1234567890", "/root/app");
foreach (ExplorerItem item in listing.Items ?? [])
{
    Console.WriteLine($"{(item.Type == ExplorerItemType.Directory ? "[dir]" : item.Size + " B")} {item.Name}");
}

ExplorerOpenResponse file = await discloud.Explorer.ReadFileAsync("1234567890", "/root/app/index.js");
await discloud.Explorer.WriteFileAsync("1234567890", "/root/app/index.js", file.Content + "\n// edited");
await discloud.Explorer.CreateAsync("1234567890", "/root/app/logs", ExplorerItemType.Directory);
```

### Domains

```csharp
Subdomain subdomain = await discloud.Subdomains.CreateAsync("my-bot");

CustomDomain domain = await discloud.CustomDomains.CreateAsync("example.com", "1234567890");
foreach (DnsInstruction record in domain.Instructions ?? [])
{
    Console.WriteLine($"{record.Type} {record.Name} -> {record.Content}");
}

CustomDomain verified = await discloud.CustomDomains.VerifyAsync("example.com");
Console.WriteLine(verified.Valid == true ? "DNS OK" : verified.Message);
```

### Snapshots

```csharp
CreatedSnapshot snapshot = await discloud.Snapshots.CreateAsync("1234567890");
IReadOnlyList<SnapshotVersion> versions = await discloud.Snapshots.ListVersionsAsync("1234567890");
SnapshotDownload download = await discloud.Snapshots.GetDownloadUrlAsync("1234567890", snapshot.Version!);
Console.WriteLine($"{download.Url} (expires {download.ExpiresAt})");

SnapshotListResponse page = await discloud.Snapshots.ListAsync(new SnapshotListQuery { Page = 1, Limit = 20 });
```

### Environment variables

Each `Set…` method replaces the whole set of variables of its scope (account, project or application).

```csharp
var vars = new Dictionary<string, string>(await discloud.EnvironmentVariables.GetForAppAsync("1234567890"))
{
    ["NODE_ENV"] = "production",
};
await discloud.EnvironmentVariables.SetForAppAsync("1234567890", vars);

int removed = await discloud.EnvironmentVariables.DeleteForProjectAsync("proj1719010867306");
```

### Projects and teams

```csharp
Team team = await discloud.Teams.CreateAsync(new CreateTeamRequest { Name = "Backend" });
await discloud.Teams.AddMemberAsync(team.Id!, "123456789012345678", TeamRole.Developer);

Project project = await discloud.Projects.CreateAsync(new CreateProjectRequest { Name = "Production", TeamId = team.Id });

AddAppToProjectResult added = await discloud.Projects.AddAppAsync(project.Id!, "1234567890", migrate: true);
if (added.IsMigrationStarted)
{
    Console.WriteLine($"Migrating from {added.Migration!.From} to {added.Migration.To}");
}
```

### Managed databases

Provisioning is asynchronous: create, then poll until the database is online.

```csharp
DatabasePreset postgres = (await discloud.Databases.ListPresetsAsync())
    .First(p => p.Type == DatabaseEngine.Postgres);

DatabaseCreatedResponse created = await discloud.Databases.CreateAsync(new CreateDatabaseRequest
{
    DisplayName = "Staging",
    Engine = new DatabaseEngineSettings { Type = DatabaseEngine.Postgres, Version = postgres.Versions![^1] },
    TotalRam = 512,
    Vars = new Dictionary<string, string>
    {
        ["POSTGRES_DB"] = postgres.SuggestedDbName!,
        ["POSTGRES_USER"] = postgres.SuggestedUser!,
        ["POSTGRES_PASSWORD"] = postgres.SuggestedPassword!,
    },
});

Database database;
do
{
    await Task.Delay(TimeSpan.FromSeconds(5));
    database = await discloud.Databases.GetAsync(created.Id!);
}
while (database.Online != true);

DatabaseBackup backup = await discloud.Databases.CreateBackupAsync(database.Id!);
DatabaseBackupPage backups = await discloud.Databases.ListBackupsAsync(database.Id!, new DatabaseBackupListQuery { Limit = 10 });
```

### Cancellation

Every method accepts a `CancellationToken`. Cancellation surfaces as `OperationCanceledException`, never as a
`DiscloudException`.

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
try
{
    IReadOnlyList<App> all = await discloud.Apps.GetAllAsync(cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Cancelled.");
}
```

The per-request timeout is configured with `DiscloudClientOptions.Timeout` (default 100 seconds). Uploading or
committing large applications may need more.

## Error handling

Failures are raised as exceptions; a returned value always represents a successful response.

| Exception | When |
| --- | --- |
| `DiscloudException` | Base class of every SDK exception. |
| `DiscloudApiException` | The API answered with an error. Exposes `StatusCode`, `ErrorCode`, `ApiMessage` and `ErrorData`. |
| ├ `DiscloudAuthenticationException` | HTTP 401: invalid or missing token. |
| ├ `DiscloudForbiddenException` | HTTP 403: missing permission or plan. |
| ├ `DiscloudNotFoundException` | HTTP 404: the resource does not exist. |
| ├ `DiscloudConflictException` | HTTP 409: name taken, resource busy, cluster mismatch. |
| └ `DiscloudRateLimitException` | HTTP 429: too many requests or another operation running. Exposes `RetryAfter`. |
| `DiscloudNetworkException` | The API could not be reached, the connection dropped, or the request timed out (`IsTimeout`). |
| `DiscloudInvalidResponseException` | The API answered successfully with an empty, malformed or incomplete body. |

Some Discloud routes answer with HTTP 200 and `"status": "error"` (for example starting an application that is
already online). The SDK reports those as `DiscloudApiException` with `StatusCode == HttpStatusCode.OK`.

```csharp
try
{
    await discloud.Snapshots.CreateAsync("1234567890");
}
catch (DiscloudConflictException ex) when (ex.ErrorCode == "SNAPSHOT:APP_BUSY")
{
    Console.WriteLine("The application is busy; try again later.");
}
catch (DiscloudForbiddenException ex)
{
    Console.WriteLine($"Not allowed: {ex.ApiMessage}");
}
catch (DiscloudApiException ex)
{
    Console.WriteLine($"HTTP {(int)ex.StatusCode} {ex.ErrorCode}: {ex.ApiMessage}");
}
catch (DiscloudNetworkException ex) when (ex.IsTimeout)
{
    Console.WriteLine("The request timed out.");
}
```

Exceptions never contain the token or raw response bodies (which may hold secrets such as environment variables).

## Working with DTOs

- **Responses** are immutable `sealed record` types. Every property is nullable because the API contract marks no
  response field as required and different routes return different subsets of fields.
- **Requests** are records with `init` properties; required fields use the C# `required` modifier, and `null`
  optional properties are not sent.
- **Typed values** such as `TeamRole`, `AppPermission`, `AptPackage`, `DatabaseEngine`, `DiscloudLocale` and
  `ExplorerItemType` are string-backed structs: compare them with their static properties, and unknown values sent by
  the API are preserved instead of failing deserialization. Integer codes (`AppType`, `SubdomainStatus`) are enums.
- Operations that return a single resource return the resource directly (`Task<App>`); operations that report an
  action outcome return the response record (`Task<AppStartResponse>`, or `Task<DiscloudResponse>` with `Status` and
  `Message`).
- Records support non-destructive mutation: `var renamed = request with { Name = "New name" };`.

## API reference

Every operation of [`openapi/openapi.yaml`](openapi/openapi.yaml) maps to one SDK method. Operations accepting the
special `all` identifier have a separate `…All…` method.

### User — `discloud.User`

| Operation | HTTP | Method |
| --- | --- | --- |
| getCurrentUser | `GET /user` | `GetAsync()` → `User` |
| updateUserLocale | `PUT /locale/{locale}` | `SetLocaleAsync(locale)` → `DiscloudLocale` |
| getVscodeUser | `GET /vscode` | `GetVscodeDataAsync()` → `VscodeUser` |

### Applications — `discloud.Apps`

| Operation | HTTP | Method |
| --- | --- | --- |
| uploadApplication | `POST /upload` | `UploadAsync(zip, fileName, projectId?)` → `UploadResponse` |
| getAppInfo | `GET /app/{appID}` | `GetAsync(appId)` → `App`, `GetAllAsync()` → `IReadOnlyList<App>` |
| getAppStatus | `GET /app/{appID}/status` | `GetStatusAsync(appId)`, `GetAllStatusesAsync()` |
| getAppLogs | `GET /app/{appID}/logs` | `GetLogsAsync(appId)`, `GetAllLogsAsync()` |
| getAppBackup | `GET /app/{appID}/backup` | `GetBackupAsync(appId)`, `GetAllBackupsAsync()` |
| startApp | `PUT /app/{appID}/start` | `StartAsync(appId)`, `StartAllAsync()` → `AppStartResponse` |
| restartApp | `PUT /app/{appID}/restart` | `RestartAsync(appId)`, `RestartAllAsync()` → `AppRestartResponse` |
| stopApp | `PUT /app/{appID}/stop` | `StopAsync(appId)`, `StopAllAsync()` → `AppStopResponse` |
| updateAppRam | `PUT /app/{appID}/ram` | `SetRamAsync(appId, ramMb)` |
| commitAppChanges | `PUT /app/{appID}/commit` | `CommitAsync(appId, zip, fileName)` → `AppCommitResponse` |
| updateAppProfile | `PUT /app/{appID}/profile` | `UpdateProfileAsync(appId, request)` |
| installAppApt | `PUT /app/{appID}/apt` | `InstallAptAsync(appId, packages)` |
| uninstallAppApt | `DELETE /app/{appID}/apt` | `UninstallAptAsync(appId, packages)` |
| runAppConsoleCommand | `PUT /app/{appID}/console` | `RunConsoleCommandAsync(appId, command)` → `AppConsoleResult` |
| execCommand | `PUT /app/{appID}/exec` | `ExecAsync(appId, command)` → `ExecResponse` |
| deleteApp | `DELETE /app/{appID}/delete` | `DeleteAsync(appId)`, `DeleteAllAsync()` → `AppDeleteResponse` |

### App moderators — `discloud.AppModerators`

| Operation | HTTP | Method |
| --- | --- | --- |
| getAppModerators | `GET /app/{appID}/team` | `ListAsync(appId)` → `IReadOnlyList<AppModerator>` |
| addAppModerator | `POST /app/{appID}/team` | `AddAsync(appId, moderatorId, permissions)` → `AppModerator` |
| editAppModerator | `PUT /app/{appID}/team` | `UpdateAsync(appId, moderatorId, permissions)` → `AppModerator` |
| removeAppModerator | `DELETE /app/{appID}/team/{modID}` | `RemoveAsync(appId, moderatorId)` |

### Team apps (apps you moderate) — `discloud.TeamApps`

| Operation | HTTP | Method |
| --- | --- | --- |
| listTeamApps | `GET /team` | `ListAsync()` → `IReadOnlyList<TeamApp>` |
| getTeamAppStatus | `GET /team/{appID}/status` | `GetStatusAsync(appId)` |
| getTeamAppLogs | `GET /team/{appID}/logs` | `GetLogsAsync(appId)`, `GetAllLogsAsync()` |
| getTeamAppBackup | `GET /team/{appID}/backup` | `GetBackupAsync(appId)`, `GetAllBackupsAsync()` |
| startTeamApp | `PUT /team/{appID}/start` | `StartAsync(appId)`, `StartAllAsync()` |
| restartTeamApp | `PUT /team/{appID}/restart` | `RestartAsync(appId)`, `RestartAllAsync()` |
| stopTeamApp | `PUT /team/{appID}/stop` | `StopAsync(appId)`, `StopAllAsync()` |
| updateTeamAppRam | `PUT /team/{appID}/ram` | `SetRamAsync(appId, ramMb)` |
| commitTeamAppChanges | `PUT /team/{appID}/commit` | `CommitAsync(appId, zip, fileName)` |

### File explorer — `discloud.Explorer`

| Operation | HTTP | Method |
| --- | --- | --- |
| listExplorer | `GET /app/{appID}/explorer` | `ListAsync(appId, path?)` → `ExplorerListResponse` |
| createExplorer | `POST /app/{appID}/explorer` | `CreateAsync(appId, path, type)` |
| openExplorer | `GET /app/{appID}/explorer/open` | `ReadFileAsync(appId, path)` → `ExplorerOpenResponse` |
| editExplorer | `PUT /app/{appID}/explorer/edit` | `WriteFileAsync(appId, path, content)` |

### Subdomains — `discloud.Subdomains`

| Operation | HTTP | Method |
| --- | --- | --- |
| listSubdomains | `GET /subdomain/all` | `ListAsync()` |
| getSubdomain | `GET /subdomain/{subdomainName}` | `GetAsync(name)` |
| createSubdomain | `POST /subdomain/{subdomainName}` | `CreateAsync(name)` |
| deleteSubdomain | `DELETE /subdomain/{subdomainName}` | `DeleteAsync(name)` |

### Custom domains — `discloud.CustomDomains`

| Operation | HTTP | Method |
| --- | --- | --- |
| listCustomDomains | `GET /customdomain/all` | `ListAsync()` |
| getCustomDomain | `GET /customdomain/{domainName}` | `GetAsync(domain)` |
| createCustomDomain | `POST /customdomain/create` | `CreateAsync(domain, appId)` |
| editCustomDomain | `PUT /customdomain/{domainName}/edit` | `MoveAsync(domain, newAppId)` |
| removeCustomDomain | `DELETE /customdomain/{domainName}/remove` | `RemoveAsync(domain)` |
| verifyCustomDomain | `GET /customdomain/{domainName}/verify` | `VerifyAsync(domain)` |

### Snapshots — `discloud.Snapshots`

| Operation | HTTP | Method |
| --- | --- | --- |
| listSnapshots | `GET /snapshot` | `ListAsync(query?)` → `SnapshotListResponse` |
| listAppSnapshots | `GET /snapshot/{appID}` | `ListVersionsAsync(appId)` |
| createSnapshot | `POST /snapshot/{appID}` | `CreateAsync(appId)` → `CreatedSnapshot` |
| getSnapshotDownloadUrl | `GET /snapshot/{appID}/versions/{version}` | `GetDownloadUrlAsync(appId, version)` |
| deleteSnapshot | `DELETE /snapshot/{appID}/versions/{version}` | `DeleteAsync(appId, version)` |

### Environment variables — `discloud.EnvironmentVariables`

| Operation | HTTP | Method |
| --- | --- | --- |
| getAccountEnvVars / setAccountEnvVars / deleteAccountEnvVars | `GET/PUT/DELETE /env/account` | `GetForAccountAsync()`, `SetForAccountAsync(vars)`, `DeleteForAccountAsync()` |
| getProjectEnvVars / setProjectEnvVars / deleteProjectEnvVars | `GET/PUT/DELETE /env/project/{projectID}` | `GetForProjectAsync(id)`, `SetForProjectAsync(id, vars)`, `DeleteForProjectAsync(id)` |
| getAppEnvVars / setAppEnvVars / deleteAppEnvVars | `GET/PUT/DELETE /env/app/{appID}` | `GetForAppAsync(id)`, `SetForAppAsync(id, vars)`, `DeleteForAppAsync(id)` |

### Projects — `discloud.Projects`

| Operation | HTTP | Method |
| --- | --- | --- |
| createProject | `POST /projects` | `CreateAsync(request)` → `Project` |
| listProjects | `GET /projects` | `ListAsync()` |
| getProject | `GET /projects/{projectID}` | `GetAsync(projectId)` → `ProjectDetails` |
| updateProject | `PATCH /projects/{projectID}` | `UpdateAsync(projectId, request)` |
| deleteProject | `DELETE /projects/{projectID}` | `DeleteAsync(projectId)` |
| setProjectTeam | `PATCH /projects/{projectID}/team` | `SetTeamAsync(projectId, teamId?)` |
| projectAvailableApps | `GET /projects/{projectID}/apps` | `ListAvailableAppsAsync(projectId)` |
| addAppToProject | `PUT /projects/{projectID}/apps/{appID}` | `AddAppAsync(projectId, appId, migrate)` → `AddAppToProjectResult` |
| removeAppFromProject | `DELETE /projects/{projectID}/apps/{appID}` | `RemoveAppAsync(projectId, appId)` |

### Managed databases — `discloud.Databases`

| Operation | HTTP | Method |
| --- | --- | --- |
| listDatabasePresets | `GET /database/presets` | `ListPresetsAsync()` |
| randomDatabaseNames | `GET /database/presets/random-names` | `GetNameSuggestionsAsync(engine?)` |
| listDatabases | `GET /database` | `ListAsync()` |
| createDatabase | `POST /database` | `CreateAsync(request)` → `DatabaseCreatedResponse` |
| getDatabase | `GET /database/{dbID}` | `GetAsync(databaseId)` → `Database` |
| updateDatabase | `PATCH /database/{dbID}` | `UpdateAsync(databaseId, request)` |
| deleteDatabase | `DELETE /database/{dbID}` | `DeleteAsync(databaseId)` |
| startDatabase / stopDatabase / restartDatabase | `PUT /database/{dbID}/start`, `/stop`, `/restart` | `StartAsync`, `StopAsync`, `RestartAsync` |
| backupDatabase | `POST /database/{dbID}/backup` | `CreateBackupAsync(databaseId, gzip?)` |
| listDatabaseBackups | `GET /database/{dbID}/backups` | `ListBackupsAsync(databaseId, query?)` |
| downloadDatabaseBackup | `GET /database/{dbID}/backups/{backupId}/download` | `GetBackupDownloadUrlAsync(databaseId, backupId, expiresInDays?)` |
| restoreDatabaseBackup | `POST /database/{dbID}/restore` | `RestoreAsync(databaseId, request)` |
| getDatabaseLogs | `GET /database/{dbID}/logs` | `GetLogsAsync(databaseId)` |
| deleteDatabaseLogs | `DELETE /database/{dbID}/logs` | `DeleteLogsAsync(databaseId)` |

### Teams — `discloud.Teams`

| Operation | HTTP | Method |
| --- | --- | --- |
| createTeam | `POST /teams` | `CreateAsync(request)` → `Team` |
| listTeams | `GET /teams` | `ListAsync()` |
| listTeamsApps | `GET /teams/apps` | `ListAppsAsync()` |
| teamRolePermissions | `GET /teams/roles/{role}/permissions` | `GetRolePermissionsAsync(role)` |
| getTeam | `GET /teams/{teamID}` | `GetAsync(teamId)` → `TeamDetails` |
| updateTeam | `PATCH /teams/{teamID}` | `UpdateAsync(teamId, request)` |
| deleteTeam | `DELETE /teams/{teamID}` | `DeleteAsync(teamId)` |
| addTeamMember | `POST /teams/{teamID}/members` | `AddMemberAsync(teamId, userId, role)` |
| listTeamMembers | `GET /teams/{teamID}/members` | `ListMembersAsync(teamId)` |
| updateTeamMember | `PATCH /teams/{teamID}/members/{memberUserID}` | `UpdateMemberAsync(teamId, userId, role)` |
| removeTeamMember | `DELETE /teams/{teamID}/members/{memberUserID}` | `RemoveMemberAsync(teamId, userId)` |
| leaveTeam | `POST /teams/{teamID}/leave` | `LeaveAsync(teamId)` |

## Known limitations

These follow from the API contract in `openapi/openapi.yaml`:

- A few objects have no documented schema and are exposed as raw `JsonElement`: `ProjectDetails.Team`,
  `Database.Permissions`, `SnapshotListResponse.Backups` items and `DiscloudApiException.ErrorData`.
- The API contract documents that a successful commit may time out without returning a response; raise
  `DiscloudClientOptions.Timeout` for large applications.
- The SDK does not retry failed requests. Add a resilience handler through the `IHttpClientBuilder` returned by
  `AddDiscloudClient` if you need retries; be careful not to retry non-idempotent operations such as uploads.
- The API contract quirks are normalized: misspelled keys (`stoped`, `removealled`) map to `Stopped` and `Removed`,
  backup IDs sent as numbers become strings, and `apts` sent as an empty string becomes an empty list.

## Development

Requirements: the .NET 10 SDK (see `global.json`) and the .NET 8 runtime to run the `net8.0` tests.

```bash
dotnet restore
```

```bash
dotnet build -c Release
```

```bash
dotnet test -c Release
```

```bash
dotnet format --verify-no-changes
```

```bash
dotnet pack Discloud/Discloud.csproj -c Release -o artifacts
```

Repository layout:

| Path | Contents |
| --- | --- |
| `Discloud/` | The `Discloud.Net` package: client, options, HTTP pipeline, exceptions and DI registration. |
| `DTOs/` | Request and response models and their JSON serialization (shipped inside the same package). |
| `Discloud.Tests/` | xUnit tests. HTTP is simulated with an in-process handler; no request reaches the API. |
| `openapi/openapi.yaml` | The API contract the SDK implements. |
| `eng/verify-package.sh` | Builds a throwaway consumer project against a packed `.nupkg`. |

Integration tests call the real API with read-only operations. They are skipped unless `DISCLOUD_TOKEN` is set:

```bash
DISCLOUD_TOKEN=your-api-token dotnet test -c Release
```

### Continuous integration and releases

- `.github/workflows/ci.yml` runs on pushes and pull requests: restore, format check, Release build, tests on
  Linux and Windows, then pack, package validation and a consumer smoke test. Nothing is published.
- `.github/workflows/release.yml` runs when a tag such as `v1.0.0` or `v1.1.0-preview.1` is pushed: it validates and
  packs the version from the tag, then publishes it to NuGet with Trusted Publishing (a short-lived key obtained
  through GitHub OIDC; no API key is stored). It needs a trusted publishing policy on nuget.org and the `NUGET_USER`
  secret in the `nuget` environment.

### Contributing

Issues and pull requests are welcome. Please keep the SDK aligned with `openapi/openapi.yaml`, add a case to
`EndpointCoverageTests` for any new operation, document public APIs, and make sure `dotnet format --verify-no-changes`
and `dotnet test` pass.

## License

[MIT](LICENSE) © 2026 Nathan Miguel
