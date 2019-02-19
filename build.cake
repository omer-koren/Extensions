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


		Information($"Version from GitVersion is {packageVersion}");
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
                IncludeSource = true,
                IncludeSymbols = true,
                ArgumentCustomization = args => args.Append($"/p:PackageVersion={packageVersion}"),
            });
    });

var nugetFeed = EnvironmentVariable("NUGET_FEED") ?? "https://api.nuget.org/v3/index.json";
var nugetApiKey = EnvironmentVariable("NUGET_API_KEY");

Task("NuGetPush")

.Does(() =>
{
    
    foreach(var nupkg in System.IO.Directory.GetFiles(artifactsDirectory,"*.symbols.nupkg"))
    {
        Information($"Pushing '{nupkg}' to {nugetFeed}");

        if(!string.IsNullOrWhiteSpace(nugetApiKey))
        {
            DotNetCoreNuGetPush(nupkg,new DotNetCoreNuGetPushSettings
                    {
                        Source = nugetFeed,
                        ApiKey = nugetApiKey
                    });
            
        }
        
    }
   
});
Task("Default")
    .IsDependentOn("Clean")
	.IsDependentOn("GitVersion")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("Pack")
    .IsDependentOn("NuGetPush");


RunTarget(target);