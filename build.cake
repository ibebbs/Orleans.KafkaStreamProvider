var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var buildDir = Directory("./src/Build/bin") + Directory(configuration);
var packageDir = Directory("./src/Build/packages") + Directory(configuration);
var solution = "./src/Orleans.KafkaStreamProvider.sln";

Task("Clean")
    .Does(() =>{
        CleanDirectory(buildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() => {
        NuGetRestore(solution);
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() => {
      MSBuild(solution, settings => 
        settings.SetConfiguration(configuration));  
});

Task("Package")
    .IsDependentOn("Build")
    .Does(() => { 
        var settings = new NuGetPackSettings
        {
            OutputDirectory = packageDir,
            IncludeReferencedProjects = true,
            Properties = new Dictionary<string, string>
            {
                { "Configuration", "Release" }
            }
        };
        NuGetPack("./src/Orleans.KafkaStreamProvider/Orleans.KafkaStreamProvider.csproj", settings);
});

Task("Default")
    .IsDependentOn("Build");

RunTarget(target);