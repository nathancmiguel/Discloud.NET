using System.Text.Json.Nodes;
using Discloud.DTOs;
using Discloud.Tests.Infrastructure;

namespace Discloud.Tests;

/// <summary>
/// One case per OpenAPI operation (97 in total). Each case checks the HTTP method, the route and query string, the
/// JSON request body, authentication, and that the documented response shape is deserialized without error.
/// </summary>
public sealed class EndpointCoverageTests
{
    private const string Ok = """{"status":"ok","message":"done"}""";

    public sealed record Endpoint(
        string OperationId,
        Func<IDiscloudClient, Task> Call,
        string Method,
        string PathAndQuery,
        string? Body,
        string Response)
    {
        public override string ToString() => OperationId;
    }

    private static readonly Dictionary<string, string> Vars = new() { ["KEY"] = "value" };

    public static readonly TheoryData<Endpoint> Endpoints =
    [
        // User
        new Endpoint("getCurrentUser", c => c.User.GetAsync(), "GET", "/v2/user", null, """{"status":"ok","user":{"userID":"1"}}"""),
        new Endpoint("updateUserLocale", c => c.User.SetLocaleAsync(DiscloudLocale.EnglishUnitedStates), "PUT", "/v2/locale/en-US", null, """{"status":"ok","locale":"en-US"}"""),
        new Endpoint("getVscodeUser", c => c.User.GetVscodeDataAsync(), "GET", "/v2/vscode", null, """{"status":"ok","user":{"userID":"1"}}"""),

        // Applications
        new Endpoint("uploadApplication", c => c.Apps.UploadAsync(new MemoryStream([1, 2, 3]), "app.zip"), "POST", "/v2/upload", null, """{"status":"ok","app":{"id":"1"}}"""),
        new Endpoint("getAppInfo", c => c.Apps.GetAsync("123"), "GET", "/v2/app/123", null, """{"status":"ok","apps":{"id":"123"}}"""),
        new Endpoint("getAppInfo (all)", c => c.Apps.GetAllAsync(), "GET", "/v2/app/all", null, """{"status":"ok","apps":[{"id":"123"}]}"""),
        new Endpoint("getAppStatus", c => c.Apps.GetStatusAsync("123"), "GET", "/v2/app/123/status", null, """{"status":"ok","apps":{"id":"123"}}"""),
        new Endpoint("getAppStatus (all)", c => c.Apps.GetAllStatusesAsync(), "GET", "/v2/app/all/status", null, """{"status":"ok","apps":[{"id":"123"}]}"""),
        new Endpoint("getAppLogs", c => c.Apps.GetLogsAsync("123"), "GET", "/v2/app/123/logs", null, """{"status":"ok","apps":{"id":"123"}}"""),
        new Endpoint("getAppLogs (all)", c => c.Apps.GetAllLogsAsync(), "GET", "/v2/app/all/logs", null, """{"status":"ok","apps":[]}"""),
        new Endpoint("getAppBackup", c => c.Apps.GetBackupAsync("123"), "GET", "/v2/app/123/backup", null, """{"status":"ok","backups":{"id":"123"}}"""),
        new Endpoint("getAppBackup (all)", c => c.Apps.GetAllBackupsAsync(), "GET", "/v2/app/all/backup", null, """{"status":"ok","backups":[]}"""),
        new Endpoint("startApp", c => c.Apps.StartAsync("123"), "PUT", "/v2/app/123/start", null, Ok),
        new Endpoint("startApp (all)", c => c.Apps.StartAllAsync(), "PUT", "/v2/app/all/start", null, Ok),
        new Endpoint("restartApp", c => c.Apps.RestartAsync("123"), "PUT", "/v2/app/123/restart", null, Ok),
        new Endpoint("restartApp (all)", c => c.Apps.RestartAllAsync(), "PUT", "/v2/app/all/restart", null, Ok),
        new Endpoint("stopApp", c => c.Apps.StopAsync("123"), "PUT", "/v2/app/123/stop", null, Ok),
        new Endpoint("stopApp (all)", c => c.Apps.StopAllAsync(), "PUT", "/v2/app/all/stop", null, Ok),
        new Endpoint("updateAppRam", c => c.Apps.SetRamAsync("123", 200), "PUT", "/v2/app/123/ram", """{"ramMB":200}""", Ok),
        new Endpoint("commitAppChanges", c => c.Apps.CommitAsync("123", new MemoryStream([1]), "app.zip"), "PUT", "/v2/app/123/commit", null, Ok),
        new Endpoint("updateAppProfile", c => c.Apps.UpdateProfileAsync("123", new UpdateAppProfileRequest { Name = "Bot" }), "PUT", "/v2/app/123/profile", """{"name":"Bot"}""", Ok),
        new Endpoint("installAppApt", c => c.Apps.InstallAptAsync("123", [AptPackage.Ffmpeg, AptPackage.Java]), "PUT", "/v2/app/123/apt", """{"apt":"ffmpeg,java"}""", Ok),
        new Endpoint("uninstallAppApt", c => c.Apps.UninstallAptAsync("123", [AptPackage.Ffmpeg]), "DELETE", "/v2/app/123/apt", """{"apt":"ffmpeg"}""", Ok),
        new Endpoint("runAppConsoleCommand", c => c.Apps.RunConsoleCommandAsync("123", "ls"), "PUT", "/v2/app/123/console", """{"command":"ls"}""", """{"status":"ok","apps":{"id":"123","shell":{"stdout":"x"}}}"""),
        new Endpoint("execCommand", c => c.Apps.ExecAsync("123", "ls -la"), "PUT", "/v2/app/123/exec", """{"cmd":"ls -la"}""", """{"status":"ok","exec":{"stdout":"","exitCode":0}}"""),
        new Endpoint("deleteApp", c => c.Apps.DeleteAsync("123"), "DELETE", "/v2/app/123/delete", null, Ok),
        new Endpoint("deleteApp (all)", c => c.Apps.DeleteAllAsync(), "DELETE", "/v2/app/all/delete", null, Ok),

        // File explorer
        new Endpoint("listExplorer", c => c.Explorer.ListAsync("123", "/root/app"), "GET", "/v2/app/123/explorer?cPath=%2Froot%2Fapp", null, """{"status":"ok","path":"/root/app","dir":[]}"""),
        new Endpoint("createExplorer", c => c.Explorer.CreateAsync("123", "/root/app/a.js", ExplorerItemType.File), "POST", "/v2/app/123/explorer", """{"cPath":"/root/app/a.js","typeFile":"file"}""", """{"status":"ok","path":"/root/app/a.js"}"""),
        new Endpoint("openExplorer", c => c.Explorer.ReadFileAsync("123", "/root/app/a.js"), "GET", "/v2/app/123/explorer/open?cPath=%2Froot%2Fapp%2Fa.js", null, """{"status":"ok","content":"x"}"""),
        new Endpoint("editExplorer", c => c.Explorer.WriteFileAsync("123", "/root/app/a.js", "x"), "PUT", "/v2/app/123/explorer/edit", """{"cPath":"/root/app/a.js","fileContent":"x"}""", """{"status":"ok","path":"/root/app/a.js"}"""),

        // App moderators
        new Endpoint("getAppModerators", c => c.AppModerators.ListAsync("123"), "GET", "/v2/app/123/team", null, """{"status":"ok","team":[]}"""),
        new Endpoint("addAppModerator", c => c.AppModerators.AddAsync("123", "9", [AppPermission.StartApp]), "POST", "/v2/app/123/team", """{"modID":"9","perms":["start_app"]}""", """{"status":"ok","app":{"appID":"123","modID":"9"}}"""),
        new Endpoint("editAppModerator", c => c.AppModerators.UpdateAsync("123", "9", [AppPermission.LogsApp, AppPermission.StatusApp]), "PUT", "/v2/app/123/team", """{"modID":"9","perms":["logs_app","status_app"]}""", """{"status":"ok","app":{"modID":"9"}}"""),
        new Endpoint("removeAppModerator", c => c.AppModerators.RemoveAsync("123", "9"), "DELETE", "/v2/app/123/team/9", null, Ok),

        // Team apps
        new Endpoint("listTeamApps", c => c.TeamApps.ListAsync(), "GET", "/v2/team", null, """{"status":"ok","apps":[]}"""),
        new Endpoint("getTeamAppStatus", c => c.TeamApps.GetStatusAsync("123"), "GET", "/v2/team/123/status", null, """{"status":"ok","apps":{"id":"123"}}"""),
        new Endpoint("getTeamAppLogs", c => c.TeamApps.GetLogsAsync("123"), "GET", "/v2/team/123/logs", null, """{"status":"ok","apps":{"id":"123"}}"""),
        new Endpoint("getTeamAppLogs (all)", c => c.TeamApps.GetAllLogsAsync(), "GET", "/v2/team/all/logs", null, """{"status":"ok","apps":[]}"""),
        new Endpoint("getTeamAppBackup", c => c.TeamApps.GetBackupAsync("123"), "GET", "/v2/team/123/backup", null, """{"status":"ok","backups":{"id":"123"}}"""),
        new Endpoint("getTeamAppBackup (all)", c => c.TeamApps.GetAllBackupsAsync(), "GET", "/v2/team/all/backup", null, """{"status":"ok","backups":[]}"""),
        new Endpoint("startTeamApp", c => c.TeamApps.StartAsync("123"), "PUT", "/v2/team/123/start", null, Ok),
        new Endpoint("startTeamApp (all)", c => c.TeamApps.StartAllAsync(), "PUT", "/v2/team/all/start", null, Ok),
        new Endpoint("restartTeamApp", c => c.TeamApps.RestartAsync("123"), "PUT", "/v2/team/123/restart", null, Ok),
        new Endpoint("restartTeamApp (all)", c => c.TeamApps.RestartAllAsync(), "PUT", "/v2/team/all/restart", null, Ok),
        new Endpoint("stopTeamApp", c => c.TeamApps.StopAsync("123"), "PUT", "/v2/team/123/stop", null, Ok),
        new Endpoint("stopTeamApp (all)", c => c.TeamApps.StopAllAsync(), "PUT", "/v2/team/all/stop", null, Ok),
        new Endpoint("updateTeamAppRam", c => c.TeamApps.SetRamAsync("123", 300), "PUT", "/v2/team/123/ram", """{"ramMB":300}""", Ok),
        new Endpoint("commitTeamAppChanges", c => c.TeamApps.CommitAsync("123", new MemoryStream([1]), "app.zip"), "PUT", "/v2/team/123/commit", null, Ok),

        // Subdomains
        new Endpoint("listSubdomains", c => c.Subdomains.ListAsync(), "GET", "/v2/subdomain/all", null, """{"status":"ok","subdomains":[]}"""),
        new Endpoint("getSubdomain", c => c.Subdomains.GetAsync("my-sub"), "GET", "/v2/subdomain/my-sub", null, """{"status":"ok","subdomain":{"id":"my-sub"}}"""),
        new Endpoint("createSubdomain", c => c.Subdomains.CreateAsync("my-sub"), "POST", "/v2/subdomain/my-sub", null, """{"status":"ok","subdomain":{"id":"my-sub"}}"""),
        new Endpoint("deleteSubdomain", c => c.Subdomains.DeleteAsync("my-sub"), "DELETE", "/v2/subdomain/my-sub", null, Ok),

        // Custom domains
        new Endpoint("listCustomDomains", c => c.CustomDomains.ListAsync(), "GET", "/v2/customdomain/all", null, """{"status":"ok","domains":[]}"""),
        new Endpoint("getCustomDomain", c => c.CustomDomains.GetAsync("example.com"), "GET", "/v2/customdomain/example.com", null, """{"status":"ok","domain":{"id":"example.com"}}"""),
        new Endpoint("createCustomDomain", c => c.CustomDomains.CreateAsync("example.com", "123"), "POST", "/v2/customdomain/create", """{"domainName":"example.com","appID":"123"}""", """{"status":"ok","domain":{"id":"example.com"}}"""),
        new Endpoint("editCustomDomain", c => c.CustomDomains.MoveAsync("example.com", "456"), "PUT", "/v2/customdomain/example.com/edit", """{"newAppID":"456"}""", """{"status":"ok","migrated":false,"domain":"example.com"}"""),
        new Endpoint("removeCustomDomain", c => c.CustomDomains.RemoveAsync("example.com"), "DELETE", "/v2/customdomain/example.com/remove", null, """{"status":"ok","domain":"example.com"}"""),
        new Endpoint("verifyCustomDomain", c => c.CustomDomains.VerifyAsync("example.com"), "GET", "/v2/customdomain/example.com/verify", null, """{"status":"ok","domain":{"id":"example.com","valid":true}}"""),

        // Snapshots
        new Endpoint("listSnapshots", c => c.Snapshots.ListAsync(new SnapshotListQuery { Page = 2, Limit = 10, Summary = true, AppId = "app-1" }), "GET", "/v2/snapshot?page=2&limit=10&summary=true&appID=app-1", null, """{"status":"ok","versions":[]}"""),
        new Endpoint("getSnapshotDownloadUrl", c => c.Snapshots.GetDownloadUrlAsync("app-1", "20260420-153000"), "GET", "/v2/snapshot/app-1/versions/20260420-153000", null, """{"status":"ok","download":{"url":"https://x"}}"""),
        new Endpoint("deleteSnapshot", c => c.Snapshots.DeleteAsync("app-1", "20260420-153000"), "DELETE", "/v2/snapshot/app-1/versions/20260420-153000", null, """{"status":"ok","deleted":"key"}"""),
        new Endpoint("listAppSnapshots", c => c.Snapshots.ListVersionsAsync("app-1"), "GET", "/v2/snapshot/app-1", null, """{"status":"ok","versions":[]}"""),
        new Endpoint("createSnapshot", c => c.Snapshots.CreateAsync("app-1"), "POST", "/v2/snapshot/app-1", null, """{"status":"ok","snapshot":{"version":"20260420-153000"}}"""),

        // Environment variables
        new Endpoint("getAccountEnvVars", c => c.EnvironmentVariables.GetForAccountAsync(), "GET", "/v2/env/account", null, """{"status":"ok","data":{"vars":{}}}"""),
        new Endpoint("setAccountEnvVars", c => c.EnvironmentVariables.SetForAccountAsync(Vars), "PUT", "/v2/env/account", """{"vars":{"KEY":"value"}}""", """{"status":"ok","data":{"vars":{"KEY":"value"}}}"""),
        new Endpoint("deleteAccountEnvVars", c => c.EnvironmentVariables.DeleteForAccountAsync(), "DELETE", "/v2/env/account", null, """{"status":"ok","data":{"removed":1}}"""),
        new Endpoint("getProjectEnvVars", c => c.EnvironmentVariables.GetForProjectAsync("p1"), "GET", "/v2/env/project/p1", null, """{"status":"ok","data":{"vars":{}}}"""),
        new Endpoint("setProjectEnvVars", c => c.EnvironmentVariables.SetForProjectAsync("p1", Vars), "PUT", "/v2/env/project/p1", """{"vars":{"KEY":"value"}}""", """{"status":"ok","data":{"vars":{}}}"""),
        new Endpoint("deleteProjectEnvVars", c => c.EnvironmentVariables.DeleteForProjectAsync("p1"), "DELETE", "/v2/env/project/p1", null, """{"status":"ok","data":{"removed":1}}"""),
        new Endpoint("getAppEnvVars", c => c.EnvironmentVariables.GetForAppAsync("123"), "GET", "/v2/env/app/123", null, """{"status":"ok","data":{"vars":{}}}"""),
        new Endpoint("setAppEnvVars", c => c.EnvironmentVariables.SetForAppAsync("123", Vars), "PUT", "/v2/env/app/123", """{"vars":{"KEY":"value"}}""", """{"status":"ok","data":{"vars":{}}}"""),
        new Endpoint("deleteAppEnvVars", c => c.EnvironmentVariables.DeleteForAppAsync("123"), "DELETE", "/v2/env/app/123", null, """{"status":"ok","data":{"removed":0}}"""),

        // Projects
        new Endpoint("createProject", c => c.Projects.CreateAsync(new CreateProjectRequest { Name = "P", Description = "D" }), "POST", "/v2/projects", """{"name":"P","description":"D"}""", """{"status":"ok","data":{"_id":"p1"}}"""),
        new Endpoint("listProjects", c => c.Projects.ListAsync(), "GET", "/v2/projects", null, """{"status":"ok","data":[]}"""),
        new Endpoint("getProject", c => c.Projects.GetAsync("p1"), "GET", "/v2/projects/p1", null, """{"status":"ok","data":{"apps":[]}}"""),
        new Endpoint("updateProject", c => c.Projects.UpdateAsync("p1", new UpdateProjectRequest { Name = "R" }), "PATCH", "/v2/projects/p1", """{"name":"R"}""", """{"status":"ok","data":{"_id":"p1"}}"""),
        new Endpoint("deleteProject", c => c.Projects.DeleteAsync("p1"), "DELETE", "/v2/projects/p1", null, Ok),
        new Endpoint("setProjectTeam", c => c.Projects.SetTeamAsync("p1", "t1"), "PATCH", "/v2/projects/p1/team", """{"teamID":"t1"}""", """{"status":"ok","data":{"_id":"p1"}}"""),
        new Endpoint("projectAvailableApps", c => c.Projects.ListAvailableAppsAsync("p1"), "GET", "/v2/projects/p1/apps", null, """{"status":"ok","data":[]}"""),
        new Endpoint("addAppToProject", c => c.Projects.AddAppAsync("p1", "123"), "PUT", "/v2/projects/p1/apps/123", """{"migrate":false}""", """{"status":"ok","data":{"_id":"123"}}"""),
        new Endpoint("removeAppFromProject", c => c.Projects.RemoveAppAsync("p1", "123"), "DELETE", "/v2/projects/p1/apps/123", null, Ok),

        // Databases
        new Endpoint("listDatabasePresets", c => c.Databases.ListPresetsAsync(), "GET", "/v2/database/presets", null, """{"status":"ok","data":{"databases":[]}}"""),
        new Endpoint("randomDatabaseNames", c => c.Databases.GetNameSuggestionsAsync(DatabaseEngine.Postgres), "GET", "/v2/database/presets/random-names?type=postgres", null, """{"status":"ok","data":{"suggestedUser":"u"}}"""),
        new Endpoint("listDatabases", c => c.Databases.ListAsync(), "GET", "/v2/database", null, """{"status":"ok","data":[]}"""),
        new Endpoint("createDatabase", c => c.Databases.CreateAsync(new CreateDatabaseRequest { DisplayName = "DB", Engine = new DatabaseEngineSettings { Type = DatabaseEngine.Postgres, Version = "17" }, TotalRam = 512 }), "POST", "/v2/database", """{"displayName":"DB","db":{"type":"postgres","version":"17"},"totalRam":512}""", """{"status":"ok","id":"d1"}"""),
        new Endpoint("getDatabase", c => c.Databases.GetAsync("d1"), "GET", "/v2/database/d1", null, """{"status":"ok","database":{"_id":"d1"}}"""),
        new Endpoint("updateDatabase", c => c.Databases.UpdateAsync("d1", new UpdateDatabaseRequest { TotalRam = 1024 }), "PATCH", "/v2/database/d1", """{"totalRam":1024}""", """{"status":"ok","rebuilding":true}"""),
        new Endpoint("deleteDatabase", c => c.Databases.DeleteAsync("d1"), "DELETE", "/v2/database/d1", null, Ok),
        new Endpoint("startDatabase", c => c.Databases.StartAsync("d1"), "PUT", "/v2/database/d1/start", null, Ok),
        new Endpoint("stopDatabase", c => c.Databases.StopAsync("d1"), "PUT", "/v2/database/d1/stop", null, Ok),
        new Endpoint("restartDatabase", c => c.Databases.RestartAsync("d1"), "PUT", "/v2/database/d1/restart", null, Ok),
        new Endpoint("backupDatabase", c => c.Databases.CreateBackupAsync("d1", gzip: false), "POST", "/v2/database/d1/backup", """{"gzip":false}""", """{"status":"ok","data":{"backupId":20240101120000}}"""),
        new Endpoint("listDatabaseBackups", c => c.Databases.ListBackupsAsync("d1", new DatabaseBackupListQuery { Limit = 5, Cursor = "c/1", Detail = DatabaseBackupDetail.Full }), "GET", "/v2/database/d1/backups?limit=5&cursor=c%2F1&detail=full", null, """{"status":"ok","data":{"backups":[],"nextCursor":null}}"""),
        new Endpoint("downloadDatabaseBackup", c => c.Databases.GetBackupDownloadUrlAsync("d1", "20240101120000", 7), "GET", "/v2/database/d1/backups/20240101120000/download?expiresInDays=7", null, """{"status":"ok","data":{"downloadUrl":"https://x"}}"""),
        new Endpoint("restoreDatabaseBackup", c => c.Databases.RestoreAsync("d1", new RestoreDatabaseRequest { BackupId = "20240101120000", TotalRam = 512 }), "POST", "/v2/database/d1/restore", """{"backupId":"20240101120000","totalRam":512}""", """{"status":"ok","id":"d2"}"""),
        new Endpoint("getDatabaseLogs", c => c.Databases.GetLogsAsync("d1"), "GET", "/v2/database/d1/logs", null, """{"status":"ok","app":{"_id":"d1","terminal":{"url":null}}}"""),
        new Endpoint("deleteDatabaseLogs", c => c.Databases.DeleteLogsAsync("d1"), "DELETE", "/v2/database/d1/logs", null, """{"status":"ok"}"""),

        // Teams
        new Endpoint("createTeam", c => c.Teams.CreateAsync(new CreateTeamRequest { Name = "T" }), "POST", "/v2/teams", """{"name":"T"}""", """{"status":"ok","data":{"_id":"t1"}}"""),
        new Endpoint("listTeams", c => c.Teams.ListAsync(), "GET", "/v2/teams", null, """{"status":"ok","data":[]}"""),
        new Endpoint("listTeamsApps", c => c.Teams.ListAppsAsync(), "GET", "/v2/teams/apps", null, """{"status":"ok","data":[]}"""),
        new Endpoint("teamRolePermissions", c => c.Teams.GetRolePermissionsAsync(TeamRole.Admin), "GET", "/v2/teams/roles/admin/permissions", null, """{"status":"ok","data":{"role":"admin","permissions":[]}}"""),
        new Endpoint("getTeam", c => c.Teams.GetAsync("t1"), "GET", "/v2/teams/t1", null, """{"status":"ok","data":{"team":{"_id":"t1"}}}"""),
        new Endpoint("updateTeam", c => c.Teams.UpdateAsync("t1", new UpdateTeamRequest { Description = "D" }), "PATCH", "/v2/teams/t1", """{"description":"D"}""", """{"status":"ok","data":{"_id":"t1"}}"""),
        new Endpoint("deleteTeam", c => c.Teams.DeleteAsync("t1"), "DELETE", "/v2/teams/t1", null, Ok),
        new Endpoint("addTeamMember", c => c.Teams.AddMemberAsync("t1", "u1", TeamRole.Developer), "POST", "/v2/teams/t1/members", """{"userID":"u1","role":"developer"}""", """{"status":"ok","data":{"userID":"u1","role":"developer"}}"""),
        new Endpoint("listTeamMembers", c => c.Teams.ListMembersAsync("t1"), "GET", "/v2/teams/t1/members", null, """{"status":"ok","data":[]}"""),
        new Endpoint("updateTeamMember", c => c.Teams.UpdateMemberAsync("t1", "u1", TeamRole.Admin), "PATCH", "/v2/teams/t1/members/u1", """{"role":"admin"}""", """{"status":"ok","data":{"userID":"u1","role":"admin"}}"""),
        new Endpoint("removeTeamMember", c => c.Teams.RemoveMemberAsync("t1", "u1"), "DELETE", "/v2/teams/t1/members/u1", null, Ok),
        new Endpoint("leaveTeam", c => c.Teams.LeaveAsync("t1"), "POST", "/v2/teams/t1/leave", null, Ok),
    ];

    [Theory]
    [MemberData(nameof(Endpoints))]
    public async Task Endpoint_sends_documented_request_and_reads_response(Endpoint endpoint)
    {
        using var client = TestClient.Create(endpoint.Response, out var handler);

        await endpoint.Call(client);

        var request = Assert.Single(handler.Requests);
        Assert.Equal(endpoint.Method, request.Method.Method);
        Assert.Equal(endpoint.PathAndQuery, request.PathAndQuery);
        Assert.Equal("api.discloud.app", request.Uri.Host);
        Assert.Equal(TestClient.Token, request.Header("api-token"));

        if (endpoint.Body is not null)
        {
            Assert.StartsWith("application/json", request.ContentType);
            Assert.True(
                JsonNode.DeepEquals(JsonNode.Parse(endpoint.Body), JsonNode.Parse(request.Body!)),
                $"Expected body {endpoint.Body} but was {request.Body}");
        }
        else if (!request.ContentType?.StartsWith("multipart/form-data", StringComparison.Ordinal) ?? true)
        {
            Assert.Null(request.Body);
        }
    }

    [Fact]
    public void Every_OpenAPI_operation_has_a_case()
    {
        var specPath = Path.Combine(AppContext.BaseDirectory, "openapi.yaml");
        var operationIds = File.ReadLines(specPath)
            .Select(line => line.Trim())
            .Where(line => line.StartsWith("operationId:", StringComparison.Ordinal))
            .Select(line => line["operationId:".Length..].Trim())
            .ToHashSet();

        var covered = Endpoints
            .Select(row => ((Endpoint)row.Data).OperationId.Split(' ')[0])
            .ToHashSet();

        Assert.Equal(97, operationIds.Count);
        Assert.Empty(operationIds.Except(covered));
        Assert.Empty(covered.Except(operationIds));
    }
}
