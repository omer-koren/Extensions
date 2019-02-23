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
 

var repoName = EnvironmentVariable("APPVEYOR_REPO_NAME") ?? "owner-name/repo-name";
var repoUrl = $"https://github.com/" + repoName;
var repoBranch = EnvironmentVariable("APPVEYOR_REPO_BRANCH") ??  "master";
var repoCommit = EnvironmentVariable("APPVEYOR_REPO_COMMIT") ?? "sha-1";

var iconUrl = "https://s3.amazonaws.com/koren-extensions/logo.png";

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
                ArgumentCustomization = args => args.Append($"/p:PackageVersion={packageVersion} " +
                                                             $"/p:PackageProjectUrl={repoUrl} " +
                                                             $"/p:RepositoryUrl={repoUrl} " +
                                                             $"/p:RepositoryType=git " +
                                                             $"/p:RepositoryBranch={repoBranch} " +
                                                             $"/p:Authors=\"Omer Koren\" " + 
                                                             $"/p:PackageIconUrl={iconUrl} " + 
                                                             $"/p:RepositoryCommit={repoCommit} "),
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