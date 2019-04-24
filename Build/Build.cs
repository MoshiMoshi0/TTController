using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "Source";
    AbsolutePath PluginsDirectory => RootDirectory / "Plugins";
    AbsolutePath ThirdPartyDirectory => RootDirectory / "ThirdParty";
    AbsolutePath ArtifactsDirectory => RootDirectory / "Build" / "artifacts";

    AbsolutePath ServiceBinPath => SourceDirectory / "TTController.Service" / "bin" / Configuration;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            // Clean source folders but skip the service project as we are cleaning it manually later
            SourceDirectory.GlobDirectories("**/obj").ForEach(DeleteDirectory);
            SourceDirectory.GlobDirectories("**/bin")
                .Where(f => !f.Parent.ToString().EndsWith("TTController.Service"))
                .ForEach(DeleteDirectory);

            PluginsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            ThirdPartyDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);

            // Clean service bin path but leave 'config.json' file
            ServiceBinPath.GlobDirectories("*").ForEach(DeleteDirectory);
            ServiceBinPath.GlobFiles("*").Where(f => !f.ToString().EndsWith("config.json")).ForEach(DeleteFile);

            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.GetNormalizedAssemblyVersion())
                .SetFileVersion(GitVersion.GetNormalizedFileVersion())
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());

            // Copy plugin dlls to service bin path      
            var dllBlacklist = new [] { "TTController.Common.dll", "OpenHardwareMonitorLib.dll", "HidLibrary.dll", "Newtonsoft.Json.dll" };
            Solution.GetProjects("TTController.Plugin.*")
                .ForEach(p =>
                {
                    CopyDirectoryRecursively(p.Directory / "bin" / Configuration,
                                             ServiceBinPath / "Plugins" / Path.GetFileName(p.Directory.Parent) / Path.GetFileName(p.Directory),
                                             DirectoryExistsPolicy.Merge,
                                             FileExistsPolicy.OverwriteIfNewer,
                                             null,
                                             f => dllBlacklist.Contains(f.Name) || Path.GetExtension(f.Name) != ".dll");
                });
        });
}
