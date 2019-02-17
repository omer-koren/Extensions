var target = Argument("Target", "Default");
var configuration = Argument("Configuration", "Release");

var artifactsDirectory = Directory("./artifacts");

string packageVersion = null;

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(artifactsDirectory);
    });

Task("GitVersion")
	.Does(() =>
	{
		var result = GitVersion();
		packageVersion = result.NuGetVersionV2;
	});

Task("Restore")
    .Does(() =>
    {
        DotNetCoreRestore();
    });
 

 Task("Build")
    .Does(() =>
    {
        DotNetCoreBuild( 
            ".",
            new DotNetCoreBuildSettings()
            {
                Configuration = configuration,
                ArgumentCustomization = args => args.Append($"--no-restore /p:Version={packageVersion}"),
            });
    });
 

Task("Pack")
    .Does(() =>
    {
        DotNetCorePack(
            ".",
            new DotNetCorePackSettings()
            {
                Configuration = configuration,
                OutputDirectory = artifactsDirectory,
                NoBuild = true,
                ArgumentCustomization = args => args.Append($"/p:PackageVersion={packageVersion}"),
            });
    });


Task("Default")
    .IsDependentOn("Clean")
	.IsDependentOn("GitVersion")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("Pack");


RunTarget(target);