#!/usr/bin/env bash
# Restores the freshly packed Discloud.Net package into a brand-new console project and builds it for every
# supported target framework. This proves the package can be consumed from a feed, that the bundled DTOs assembly
# resolves, and that the README usage patterns compile against the public API.
#
# Usage: eng/verify-package.sh <package-directory> <version>
set -euo pipefail

package_dir="$(cd "$1" && pwd)"
version="$2"
if command -v cygpath > /dev/null; then
  # Git Bash on Windows: NuGet needs a native path.
  package_dir="$(cygpath -m "$package_dir")"
fi
work_dir="$(mktemp -d)"
trap 'rm -rf "$work_dir"' EXIT

cat > "$work_dir/nuget.config" <<EOF
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="$package_dir" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
EOF

cat > "$work_dir/Consumer.csproj" <<EOF
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFrameworks>net8.0;net10.0</TargetFrameworks>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <PublishAot>false</PublishAot>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Discloud.Net" Version="$version" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="8.0.1" />
  </ItemGroup>
</Project>
EOF

cat > "$work_dir/Program.cs" <<'EOF'
using Discloud;
using Discloud.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// Dependency injection with the "Discloud" configuration section.
var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?> { ["Discloud:Token"] = "placeholder" })
    .Build();
var services = new ServiceCollection();
services.AddDiscloudClient(configuration.GetSection("Discloud"));
await using var provider = services.BuildServiceProvider();
IDiscloudClient injected = provider.GetRequiredService<IDiscloudClient>();

// Direct instantiation.
using var direct = new DiscloudClient(new DiscloudClientOptions { Token = "placeholder", Timeout = TimeSpan.FromMinutes(5) });

// Public API surface used in the README. Nothing is awaited: no request is sent.
Func<Task> calls = async () =>
{
    User user = await injected.User.GetAsync();
    IReadOnlyList<App> apps = await injected.Apps.GetAllAsync();
    AppStartResponse started = await injected.Apps.StartAsync("app-id");
    await injected.AppModerators.AddAsync("app-id", "user-id", [AppPermission.StartApp, AppPermission.LogsApp]);
    await injected.Databases.CreateAsync(new CreateDatabaseRequest
    {
        DisplayName = "db",
        Engine = new DatabaseEngineSettings { Type = DatabaseEngine.Postgres, Version = "17" },
        TotalRam = 512,
    });
    await injected.Teams.AddMemberAsync("team-id", "user-id", TeamRole.Developer);
    Console.WriteLine($"{user.Username} {apps.Count} {started.Message}");
};

try
{
    await direct.User.GetAsync(new CancellationToken(canceled: true));
}
catch (OperationCanceledException)
{
    Console.WriteLine("Package consumed successfully.");
}
catch (DiscloudException ex)
{
    Console.WriteLine(ex.Message);
}

GC.KeepAlive(calls);
EOF

dotnet build "$work_dir/Consumer.csproj" -c Release --configfile "$work_dir/nuget.config"
dotnet run --project "$work_dir/Consumer.csproj" -c Release --no-build --framework net8.0
dotnet run --project "$work_dir/Consumer.csproj" -c Release --no-build --framework net10.0
