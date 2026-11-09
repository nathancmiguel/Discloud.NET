using System.Text.Json;
using Discloud.DTOs;
using Discloud.Tests.Infrastructure;

namespace Discloud.Tests;

/// <summary>
/// Deserializes the examples published in openapi.yaml through the public API.
/// </summary>
public sealed class DeserializationTests
{
    [Fact]
    public async Task User_example()
    {
        using var client = TestClient.Create(
            """
            {"status":"ok","message":"User data were successfully charged","user":{
              "userID":"803658045736878080","totalRamMb":16384,"ramUsedMb":3386,"subdomains":["exemplo"],
              "customdomains":["example.discloud.com"],"apps":["1719010867306"],"username":"exemplo",
              "avatar":"https://cdn.discordapp.com/embed/avatars/0.png","plan":"Safira","locale":"pt-BR",
              "lastDataLeft":{"days":264,"hours":12,"minutes":6,"seconds":56},"planDataEnd":"2026-01-12T22:28:58.189Z"}}
            """, out _);

        var user = await client.User.GetAsync(TestContext.Current.CancellationToken);

        Assert.Equal("803658045736878080", user.UserId);
        Assert.Equal(16384, user.TotalRamMb);
        Assert.Equal(["example.discloud.com"], user.CustomDomains);
        Assert.Equal(264, user.LastDataLeft?.Days);
        Assert.Equal("2026-01-12T22:28:58.189Z", user.PlanDataEnd);
    }

    [Fact]
    public async Task Nullable_user_fields_accept_null()
    {
        using var client = TestClient.Create("""{"status":"ok","user":{"userID":"1","username":null,"avatar":null}}""", out _);

        var user = await client.User.GetAsync(TestContext.Current.CancellationToken);

        Assert.Null(user.Username);
        Assert.Null(user.Avatar);
        Assert.Null(user.Apps);
    }

    [Fact]
    public async Task Locale_is_read_as_a_typed_value()
    {
        using var client = TestClient.Create("""{"status":"ok","locale":"en-US"}""", out _);

        var locale = await client.User.SetLocaleAsync(DiscloudLocale.EnglishUnitedStates, TestContext.Current.CancellationToken);

        Assert.Equal(DiscloudLocale.EnglishUnitedStates, locale);
    }

    [Fact]
    public async Task Single_app_object_is_returned_by_get()
    {
        using var client = TestClient.Create(
            """
            {"status":"ok","apps":{"id":"1745338954955","avatarURL":"https://i.ibb.co/x.png","name":"Temp2 - JS","type":0,
             "online":true,"ramKilled":false,"exitCode":0,"ram":100,"mainFile":"src/index.js","lang":"javascript",
             "autoDeployGit":"no","autoRestart":false,"mods":[],"apts":[],"addedTimestamp":1745338954955}}
            """, out _);

        var app = await client.Apps.GetAsync("1745338954955", TestContext.Current.CancellationToken);

        Assert.Equal("Temp2 - JS", app.Name);
        Assert.Equal(AppType.Bot, app.Type);
        Assert.Equal(1745338954955L, app.AddedTimestamp);
        Assert.Equal("javascript", app.Language);
        Assert.Empty(app.Apts!);
    }

    [Fact]
    public async Task All_apps_accepts_both_array_and_single_object()
    {
        using (var client = TestClient.Create("""{"status":"ok","apps":[{"id":"1","type":1},{"id":"2"}]}""", out _))
        {
            var apps = await client.Apps.GetAllAsync(TestContext.Current.CancellationToken);
            Assert.Equal(["1", "2"], apps.Select(a => a.Id));
            Assert.Equal(AppType.Site, apps[0].Type);
        }

        using (var client = TestClient.Create("""{"status":"ok","apps":{"id":"1"}}""", out _))
        {
            var apps = await client.Apps.GetAllAsync(TestContext.Current.CancellationToken);
            Assert.Single(apps);
        }
    }

    [Fact]
    public async Task Status_example_parses_dates_and_nested_objects()
    {
        using var client = TestClient.Create(
            """
            {"status":"ok","apps":{"id":"1745338954955","container":"Online","cpu":"0.00%","memory":"39.9MB/100MB",
             "ssd":"1.999GB","netIO":{"down":"61KB","up":"16KB"},"last_restart":"30 minutes",
             "startedAt":"2025-04-22T16:24:55.978921501Z"}}
            """, out _);

        var status = await client.Apps.GetStatusAsync("1745338954955", TestContext.Current.CancellationToken);

        Assert.Equal("61KB", status.NetIO?.Down);
        Assert.Equal("30 minutes", status.LastRestart);
        Assert.Equal(new DateTimeOffset(2025, 4, 22, 16, 24, 55, TimeSpan.Zero), status.StartedAt!.Value.AddTicks(-(status.StartedAt.Value.Ticks % TimeSpan.TicksPerSecond)));
    }

    [Fact]
    public async Task Upload_example_maps_empty_apts_string_to_empty_list_and_null_sync_git()
    {
        using var client = TestClient.Create(
            """
            {"status":"ok","statusCode":200,"message":"Your application has been successfully uploaded.",
             "app":{"id":"1745338954955","type":0,"name":"Bot javascript","ram":100,"apts":"","clusterName":"fenrir",
                    "exitCode":0,"online":true,"ramKilled":false,"syncGit":null,"mainFile":"index.js","lang":"javascript",
                    "version":"latest","addedAtTimestamp":1719010867306,"autoRestart":false}}
            """, out _);

        var result = await client.Apps.UploadAsync(new MemoryStream([1]), "a.zip", cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.App);
        Assert.Empty(result.App.Apts!);
        Assert.Null(result.App.SyncGit);
        Assert.Equal("latest", result.App.Version);
    }

    [Fact]
    public async Task Bulk_operation_results_map_misspelled_api_keys()
    {
        using (var client = TestClient.Create("""{"status":"ok","apps":{"stoped":["1"],"alreadyOffline":["2"],"alreadyInProcess":[]}}""", out _))
        {
            var stop = await client.Apps.StopAllAsync(TestContext.Current.CancellationToken);
            Assert.Equal(["1"], stop.Apps?.Stopped);
            Assert.Equal(["2"], stop.Apps?.AlreadyOffline);
        }

        using (var client = TestClient.Create("""{"status":"ok","apps":{"removealled":["1"]}}""", out _))
        {
            var delete = await client.Apps.DeleteAllAsync(TestContext.Current.CancellationToken);
            Assert.Equal(["1"], delete.Apps?.Removed);
        }
    }

    [Fact]
    public async Task Start_response_exposes_app_status()
    {
        using var client = TestClient.Create(
            """{"status":"ok","message":"Its application was connected.","appStatus":{"online":true,"ramKilled":false,"exitCode":0}}""", out _);

        var result = await client.Apps.StartAsync("1", TestContext.Current.CancellationToken);

        Assert.True(result.AppStatus?.Online);
        Assert.Equal("Its application was connected.", result.Message);
    }

    [Fact]
    public async Task Moderator_permissions_are_typed_and_unknown_values_are_preserved()
    {
        using var client = TestClient.Create(
            """{"status":"ok","team":[{"modID":"1","perms":["start_app","future_perm"],"avatar":null,"username":"x"}]}""", out _);

        var moderators = await client.AppModerators.ListAsync("1", TestContext.Current.CancellationToken);

        var perms = moderators[0].Permissions!;
        Assert.Equal(AppPermission.StartApp, perms[0]);
        Assert.Equal("future_perm", perms[1].Value);
    }

    [Fact]
    public async Task Exec_output_can_be_null_when_app_offline()
    {
        using var client = TestClient.Create("""{"status":"ok","statusCode":200,"message":"ok","exec":null}""", out _);

        var result = await client.Apps.ExecAsync("1", "ls", TestContext.Current.CancellationToken);

        Assert.Null(result.Exec);
    }

    [Fact]
    public async Task Explorer_items_are_typed()
    {
        using var client = TestClient.Create(
            """{"status":"ok","path":"/root/app","dir":[{"name":"index.js","type":"file","size":1024},{"name":"src","type":"directory"}]}""", out _);

        var listing = await client.Explorer.ListAsync("1", cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(ExplorerItemType.File, listing.Items![0].Type);
        Assert.Equal(1024, listing.Items[0].Size);
        Assert.Equal(ExplorerItemType.Directory, listing.Items[1].Type);
        Assert.Null(listing.Items[1].Size);
    }

    [Fact]
    public async Task Subdomain_status_is_an_enum()
    {
        using var client = TestClient.Create(
            """{"status":"ok","subdomains":[{"id":"a","userID":"1","status":1,"date":1745100000000},{"id":"b","status":2}]}""", out _);

        var subdomains = await client.Subdomains.ListAsync(TestContext.Current.CancellationToken);

        Assert.Equal(SubdomainStatus.Available, subdomains[0].Status);
        Assert.Equal(SubdomainStatus.InUse, subdomains[1].Status);
    }

    [Fact]
    public async Task Custom_domain_example_with_nested_app_and_instructions()
    {
        using var client = TestClient.Create(
            """
            {"status":"ok","domain":{"id":"example.com","dns_status":"active","date":1745100000000,"isLegacy":false,
             "app":{"id":"1719010867306","name":"example.com","online":true,"type":1},
             "instructions":[{"type":"A","name":"example.com","content":"192.0.2.1"}]}}
            """, out _);

        var domain = await client.CustomDomains.GetAsync("example.com", TestContext.Current.CancellationToken);

        Assert.Equal("active", domain.DnsStatus);
        Assert.Equal(AppType.Site, domain.App?.Type);
        Assert.Equal("192.0.2.1", domain.Instructions![0].Content);
    }

    [Fact]
    public async Task Snapshot_examples()
    {
        using (var client = TestClient.Create(
            """{"status":"ok","app":{"id":"my-app-id"},"versions":[{"version":"20260420-153000","size":204800,"createdAt":"2026-04-20T15:30:00Z"}]}""", out _))
        {
            var list = await client.Snapshots.ListAsync(new SnapshotListQuery { AppId = "my-app-id" }, TestContext.Current.CancellationToken);
            Assert.Equal("my-app-id", list.App?.Id);
            Assert.Equal(new DateTimeOffset(2026, 4, 20, 15, 30, 0, TimeSpan.Zero), list.Versions![0].CreatedAt);
        }

        using (var client = TestClient.Create(
            """{"status":"ok","page":1,"limit":50,"total":3,"backups":[{"anything":true}]}""", out _))
        {
            var page = await client.Snapshots.ListAsync(cancellationToken: TestContext.Current.CancellationToken);
            Assert.Equal(3, page.Total);
            Assert.True(page.Backups![0].GetProperty("anything").GetBoolean());
        }

        using (var client = TestClient.Create(
            """
            {"status":"ok","app":{"id":"my-app-id"},"snapshot":{"version":"20260420-153000","size":"2.5 MB","url":"https://s3/x",
             "allVersions":[{"version":"20260420-153000","size":"2.5 MB","date":"2026-04-20T15:30:00Z"}]}}
            """, out _))
        {
            var created = await client.Snapshots.CreateAsync("my-app-id", TestContext.Current.CancellationToken);
            Assert.Equal("2.5 MB", created.Size);
            Assert.Single(created.AllVersions!);
        }
    }

    [Fact]
    public async Task Environment_variables_are_returned_as_a_dictionary()
    {
        using var client = TestClient.Create("""{"status":"ok","data":{"vars":{"API_KEY":"sk_live_123","NODE_ENV":"production"}}}""", out _);

        var vars = await client.EnvironmentVariables.GetForAccountAsync(TestContext.Current.CancellationToken);

        Assert.Equal("production", vars["NODE_ENV"]);
        Assert.Equal(2, vars.Count);
    }

    [Fact]
    public async Task Project_details_with_null_team()
    {
        using var client = TestClient.Create(
            """{"status":"ok","data":{"project":{"_id":"proj1","name":"My Project","memberRole":"owner"},"team":null,"apps":[{"_id":"1","nameTAG":"My Bot"}]}}""", out _);

        var details = await client.Projects.GetAsync("proj1", TestContext.Current.CancellationToken);

        Assert.Equal("owner", details.Project?.MemberRole);
        Assert.True(details.Team is null || details.Team.Value.ValueKind == JsonValueKind.Null);
        Assert.Equal("My Bot", details.Apps![0].NameTag);
    }

    [Fact]
    public async Task Database_example()
    {
        using var client = TestClient.Create(
            """
            {"status":"ok","message":"Database status loaded.","database":{"_id":"1714500000365db","projectID":null,
             "clusterName":"BOLT","hostname":"pedro042","displayName":"My Postgres staging","dbType":"postgresql",
             "dbEngine":"postgres","dbVersion":"17","extensions":[],"totalRam":512,
             "owner":{"id":"123456789012345678","username":"dev_user","avatar":null},"perms":null,
             "vars":{"POSTGRES_DB":"app_db"},"online":true,"exists":true,"totalUsedRam":187.3,
             "last_restart":"2 minutes","dataVolumeSizeHuman":"1.2G"}}
            """, out _);

        var database = await client.Databases.GetAsync("1714500000365db", TestContext.Current.CancellationToken);

        Assert.Equal(DatabaseEngine.Postgres, database.DbEngine);
        Assert.Equal(187.3, database.TotalUsedRam);
        Assert.Equal("dev_user", database.Owner?.Username);
        Assert.Equal("app_db", database.Vars!["POSTGRES_DB"]);
        Assert.Null(database.ProjectId);
    }

    [Fact]
    public async Task Database_backup_ids_accept_numbers_and_strings()
    {
        using (var client = TestClient.Create("""{"status":"ok","data":{"backupId":20240101120000,"downloadUrl":"https://x"}}""", out _))
        {
            var backup = await client.Databases.CreateBackupAsync("d1", cancellationToken: TestContext.Current.CancellationToken);
            Assert.Equal("20240101120000", backup.BackupId);
        }

        using (var client = TestClient.Create(
            """{"status":"ok","data":{"backups":[{"backupId":"20240101120000","createdAt":"2024-01-01T12:00:00.000Z","size":1048576}],"nextCursor":null}}""", out _))
        {
            var page = await client.Databases.ListBackupsAsync("d1", cancellationToken: TestContext.Current.CancellationToken);
            Assert.Equal("20240101120000", page.Backups![0].BackupId);
            Assert.Equal(1048576, page.Backups[0].Size);
            Assert.Null(page.NextCursor);
        }
    }

    [Fact]
    public async Task Database_presets_example()
    {
        using var client = TestClient.Create(
            """
            {"status":"ok","data":{"databases":[{"type":"postgres","versions":["16","17"],"extensions":["pgvector"],
             "defaultPort":5432,"suggestedDisplayName":"Nova base Brisa","suggestedDbName":"brisa-aurora",
             "suggestedUser":"brisa","suggestedPassword":"a1b2c3d4e5f6",
             "vars":{"required":["POSTGRES_DB"],"optional":[],"fromSuggestions":{"POSTGRES_DB":"suggestedDbName"}}}]}}
            """, out _);

        var presets = await client.Databases.ListPresetsAsync(TestContext.Current.CancellationToken);

        Assert.Equal(DatabaseEngine.Postgres, presets[0].Type);
        Assert.Equal("suggestedDbName", presets[0].Vars?.FromSuggestions?["POSTGRES_DB"]);
    }

    [Fact]
    public async Task Team_members_report_the_owner_role()
    {
        using var client = TestClient.Create(
            """{"status":"ok","data":{"team":{"_id":"t1","name":"My Team"},"members":[{"userID":"1","role":"owner"}],"projects":[{"_id":"p","name":"P"}]}}""", out _);

        var details = await client.Teams.GetAsync("t1", TestContext.Current.CancellationToken);

        Assert.Equal(TeamRole.Owner, details.Members![0].Role);
        Assert.Equal("P", details.Projects![0].Name);
    }
}
