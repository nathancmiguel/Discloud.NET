using System.Text.Json.Serialization;

namespace Discloud.DTOs;

/// <summary>
/// Compile-time generated JSON metadata for every type exchanged with the Discloud API.
/// </summary>
/// <remarks>
/// Source generation avoids runtime reflection, which keeps serialization fast on first use and makes the SDK
/// compatible with trimming and Native AOT. Requests omit <see langword="null"/> properties so optional fields are
/// only sent when set.
/// </remarks>
[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    NumberHandling = JsonNumberHandling.AllowReadingFromString)]
// Common
[JsonSerializable(typeof(DiscloudResponse))]
[JsonSerializable(typeof(ApiErrorResponse))]
// User
[JsonSerializable(typeof(UserResponse))]
[JsonSerializable(typeof(VscodeUserResponse))]
[JsonSerializable(typeof(LocaleResponse))]
// Applications
[JsonSerializable(typeof(App))]
[JsonSerializable(typeof(AppStatus))]
[JsonSerializable(typeof(AppLogs))]
[JsonSerializable(typeof(AppBackup))]
[JsonSerializable(typeof(UploadResponse))]
[JsonSerializable(typeof(AppResponse))]
[JsonSerializable(typeof(AppStatusResponse))]
[JsonSerializable(typeof(AppLogsResponse))]
[JsonSerializable(typeof(AppBackupResponse))]
[JsonSerializable(typeof(AppStartResponse))]
[JsonSerializable(typeof(AppRestartResponse))]
[JsonSerializable(typeof(AppStopResponse))]
[JsonSerializable(typeof(AppDeleteResponse))]
[JsonSerializable(typeof(AppCommitResponse))]
[JsonSerializable(typeof(AppConsoleResponse))]
[JsonSerializable(typeof(ExecResponse))]
[JsonSerializable(typeof(UpdateAppProfileRequest))]
[JsonSerializable(typeof(AppRamUpdateRequest))]
[JsonSerializable(typeof(AppAptRequest))]
[JsonSerializable(typeof(AppConsoleRequest))]
[JsonSerializable(typeof(ExecRequest))]
// Moderators and team apps
[JsonSerializable(typeof(AppModeratorRequest))]
[JsonSerializable(typeof(AppModeratorListResponse))]
[JsonSerializable(typeof(AppModeratorResponse))]
[JsonSerializable(typeof(TeamAppListResponse))]
// File explorer
[JsonSerializable(typeof(ExplorerListResponse))]
[JsonSerializable(typeof(ExplorerOpenResponse))]
[JsonSerializable(typeof(ExplorerOperationResponse))]
[JsonSerializable(typeof(ExplorerCreateRequest))]
[JsonSerializable(typeof(ExplorerEditRequest))]
// Domains
[JsonSerializable(typeof(SubdomainResponse))]
[JsonSerializable(typeof(SubdomainListResponse))]
[JsonSerializable(typeof(CustomDomainResponse))]
[JsonSerializable(typeof(CustomDomainListResponse))]
[JsonSerializable(typeof(CustomDomainMoveResponse))]
[JsonSerializable(typeof(CustomDomainRemoveResponse))]
[JsonSerializable(typeof(CustomDomainCreateRequest))]
[JsonSerializable(typeof(CustomDomainMoveRequest))]
// Snapshots
[JsonSerializable(typeof(SnapshotListResponse))]
[JsonSerializable(typeof(SnapshotDownloadResponse))]
[JsonSerializable(typeof(SnapshotDeleteResponse))]
[JsonSerializable(typeof(AppSnapshotListResponse))]
[JsonSerializable(typeof(SnapshotCreateResponse))]
// Environment variables
[JsonSerializable(typeof(EnvVarsRequest))]
[JsonSerializable(typeof(EnvVarsResponse))]
[JsonSerializable(typeof(EnvVarsDeleteResponse))]
// Projects
[JsonSerializable(typeof(CreateProjectRequest))]
[JsonSerializable(typeof(UpdateProjectRequest))]
[JsonSerializable(typeof(SetProjectTeamRequest))]
[JsonSerializable(typeof(AddAppToProjectRequest))]
[JsonSerializable(typeof(ProjectResponse))]
[JsonSerializable(typeof(ProjectListResponse))]
[JsonSerializable(typeof(ProjectDetailsResponse))]
[JsonSerializable(typeof(ProjectAppListResponse))]
[JsonSerializable(typeof(ProjectAppLinkResponse))]
[JsonSerializable(typeof(ProjectMigrationResponse))]
// Databases
[JsonSerializable(typeof(CreateDatabaseRequest))]
[JsonSerializable(typeof(UpdateDatabaseRequest))]
[JsonSerializable(typeof(RestoreDatabaseRequest))]
[JsonSerializable(typeof(DatabaseBackupRequest))]
[JsonSerializable(typeof(DatabasePresetListResponse))]
[JsonSerializable(typeof(DatabaseNamesResponse))]
[JsonSerializable(typeof(DatabaseListResponse))]
[JsonSerializable(typeof(DatabaseResponse))]
[JsonSerializable(typeof(DatabaseCreatedResponse))]
[JsonSerializable(typeof(DatabaseUpdateResponse))]
[JsonSerializable(typeof(DatabaseBackupCreatedResponse))]
[JsonSerializable(typeof(DatabaseBackupListResponse))]
[JsonSerializable(typeof(DatabaseDownloadResponse))]
[JsonSerializable(typeof(DatabaseLogsResponse))]
// Teams
[JsonSerializable(typeof(CreateTeamRequest))]
[JsonSerializable(typeof(UpdateTeamRequest))]
[JsonSerializable(typeof(AddTeamMemberRequest))]
[JsonSerializable(typeof(UpdateTeamMemberRequest))]
[JsonSerializable(typeof(TeamResponse))]
[JsonSerializable(typeof(TeamListResponse))]
[JsonSerializable(typeof(TeamsAppListResponse))]
[JsonSerializable(typeof(TeamRolePermissionsResponse))]
[JsonSerializable(typeof(TeamDetailsResponse))]
[JsonSerializable(typeof(TeamMemberResponse))]
[JsonSerializable(typeof(TeamMemberListResponse))]
internal sealed partial class DiscloudJsonContext : JsonSerializerContext;
