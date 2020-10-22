#addin nuget:?package=Cake.FileHelpers&version=3.2.1
#addin "nuget:?package=Cake.Incubator&version=5.1.0"

var target = Argument("target", "Build");
var nugetKey = Argument("nugetKey", "");
var nugetSource = Argument("nugetSource", "https://api.nuget.org/v3/index.json");

string RunGit(string command, string separator = "") 
{
    using(var process = StartAndReturnProcess("git", new ProcessSettings { Arguments = command, RedirectStandardOutput = true })) 
    {
        process.WaitForExit();
        return string.Join(separator, process.GetStandardOutput());
    }
}

Task("Cleanup")
    .Does(() =>
{
    Information("Cleaning up old build objects");
    CleanDirectories(GetDirectories("./**/bin/"));
    CleanDirectories(GetDirectories("./**/obj/"));
});

Task("PullDependencies")
    .Does(() =>
{
    Information("Restoring NuGet packages");
    NuGetRestore("./BepInEx.Analyzers.sln");
});

Task("Build")
    .IsDependentOn("Cleanup")
    .IsDependentOn("PullDependencies")
    .Does(() =>
{
    var buildSettings = new MSBuildSettings {
        Configuration = "Release",
        Restore = true
    };
    MSBuild("./BepInEx.Analyzers.sln", buildSettings);
});

Task("Publish")
    .IsDependentOn("Build")
    .Does(() => 
{
    var version = FindRegexMatchGroupInFile("./BepInEx.Analyzers/BepInEx.Analyzers.Package/BepInEx.Analyzers.Package.csproj", @"<PackageVersion>(.*)<\/PackageVersion>", 1, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    var versionTagPresent = !string.IsNullOrWhiteSpace(RunGit($"ls-remote --tags origin v{version}"));

    if(versionTagPresent) 
    {
        Information("New version exists, no need to push.");
        return;
    }

    Information($"Pushing tag v{version}");
    RunGit($"tag v{version}");
    RunGit($"push origin v{version}");

    if(string.IsNullOrWhiteSpace(nugetKey)){
        Information("No NuGet key specified, can't publish");
        return;
    }

    if(string.IsNullOrWhiteSpace(nugetSource)){
        Information("No NuGet push source specified, can't publish");
        return;
    }

    NuGetPush($"./BepInEx.Analyzers/BepInEx.Analyzers.Package/bin/Release/BepInEx.Analyzers.{version}.nupkg", new NuGetPushSettings {
        Source = nugetSource,
        ApiKey = nugetKey
    });
});

RunTarget(target);