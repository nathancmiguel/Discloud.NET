using System.Text.Json;
using Discloud.DTOs;

namespace Discloud.Tests;

/// <summary>
/// Tests of the DTO types and converters in isolation.
/// </summary>
public sealed class DtoSerializationTests
{
    private static readonly DiscloudJsonContext Json = DiscloudJsonContext.Default;

    [Fact]
    public void String_values_compare_by_value_and_convert_from_strings()
    {
        TeamRole fromString = "admin";

        Assert.Equal(TeamRole.Admin, fromString);
        Assert.True(TeamRole.Admin == TeamRole.FromString("admin"));
        Assert.True(TeamRole.Admin != TeamRole.Viewer);
        Assert.Equal("admin", TeamRole.Admin.ToString());
        Assert.Equal(TeamRole.Admin.GetHashCode(), new TeamRole("admin").GetHashCode());
        Assert.NotEqual(TeamRole.Admin, new TeamRole("Admin"));
    }

    [Fact]
    public void Default_string_value_is_empty()
    {
        Assert.Equal(string.Empty, default(AppPermission).Value);
        Assert.Throws<ArgumentException>(() => new AppPermission(" "));
        Assert.ThrowsAny<ArgumentException>(() => new AppPermission(null!));
    }

    [Fact]
    public void String_values_round_trip_as_json_strings()
    {
        var json = JsonSerializer.Serialize(new UpdateTeamMemberRequest(TeamRole.Developer), Json.UpdateTeamMemberRequest);

        Assert.Equal("""{"role":"developer"}""", json);
    }

    [Fact]
    public void String_value_rejects_non_string_tokens()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize("""{"status":"ok","data":{"role":5}}""", Json.TeamMemberResponse));
    }

    [Fact]
    public void Request_records_omit_nulls_and_use_api_names()
    {
        var json = JsonSerializer.Serialize(
            new RestoreDatabaseRequest
            {
                BackupId = "1",
                TotalRam = 512,
                Engine = new DatabaseRestoreEngineSettings { Version = "16" },
            },
            Json.RestoreDatabaseRequest);

        Assert.Equal("""{"backupId":"1","totalRam":512,"db":{"version":"16"}}""", json);
    }

    [Fact]
    public void Create_project_request_uses_api_names()
    {
        var json = JsonSerializer.Serialize(
            new CreateProjectRequest { Name = "P", TeamId = "t", FromAppIds = ["1", "2"] },
            Json.CreateProjectRequest);

        Assert.Equal("""{"name":"P","teamID":"t","fromAppIDs":["1","2"]}""", json);
    }

    [Theory]
    [InlineData("\"abc\"", "abc")]
    [InlineData("123", "123")]
    [InlineData("20240101120000", "20240101120000")]
    [InlineData("1.5", "1.5")]
    public void String_or_number_converter_reads_both(string token, string expected)
    {
        var response = JsonSerializer.Deserialize("{\"data\":{\"backupId\":" + token + "}}", Json.DatabaseBackupCreatedResponse);

        Assert.Equal(expected, response!.Data!.BackupId);
    }

    [Theory]
    [InlineData("\"\"", new string[0])]
    [InlineData("\"ffmpeg, java\"", new[] { "ffmpeg", "java" })]
    [InlineData("[\"canvas\"]", new[] { "canvas" })]
    [InlineData("[]", new string[0])]
    public void String_or_array_converter_reads_both(string token, string[] expected)
    {
        var response = JsonSerializer.Deserialize("{\"app\":{\"apts\":" + token + "}}", Json.UploadResponse);

        Assert.Equal(expected, response!.App!.Apts);
    }

    [Fact]
    public void Single_or_array_converter_handles_null()
    {
        var response = JsonSerializer.Deserialize("""{"apps":null}""", Json.AppResponse);

        Assert.Null(response!.Apps);
    }

    [Fact]
    public void Missing_properties_deserialize_as_null()
    {
        var response = JsonSerializer.Deserialize("""{}""", Json.DatabaseResponse);

        Assert.NotNull(response);
        Assert.Null(response.Status);
        Assert.Null(response.Database);
    }

    [Fact]
    public void Numbers_sent_as_strings_are_accepted()
    {
        var response = JsonSerializer.Deserialize("""{"user":{"totalRamMb":"512"}}""", Json.UserResponse);

        Assert.Equal(512, response!.User!.TotalRamMb);
    }

    [Fact]
    public void Error_body_reads_code_and_data()
    {
        var error = JsonSerializer.Deserialize(
            """{"status":"error","code":"SNAPSHOT:APP_BUSY","message":"busy","data":{"a":1}}""", Json.ApiErrorResponse);

        Assert.Equal("SNAPSHOT:APP_BUSY", error!.Code);
        Assert.Equal(1, error.Data!.Value.GetProperty("a").GetInt32());
    }
}
