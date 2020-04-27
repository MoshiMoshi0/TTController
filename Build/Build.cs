using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build")]
    readonly Configuration Configuration = Configuration.Debug;

    [Solution] readonly Solution Solution;
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
            if (Directory.Exists(ServiceBinPath))
            {
                ServiceBinPath.GlobDirectories("*").ForEach(DeleteDirectory);
                ServiceBinPath.GlobFiles("*").Where(f => !f.ToString().EndsWith("config.json")).ForEach(DeleteFile);
            }

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
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());

            // Copy plugin files to service bin path      
            var fileBlacklist = new[] { "TTController.Common", "LibreHardwareMonitorLib", "HidLibrary", "Newtonsoft.Json" };
            var extensionWhitelist = Configuration == Configuration.Debug ? new[] { ".pdb", ".dll" } : new[] { ".dll" };
            Solution.GetProjects("TTController.Plugin.*")
                .ForEach(p =>
                {
                    CopyDirectoryRecursively(p.Directory / "bin" / Configuration,
                                             ServiceBinPath / "Plugins" / Path.GetFileName(p.Directory.Parent) / Path.GetFileName(p.Directory),
                                             DirectoryExistsPolicy.Merge,
                                             FileExistsPolicy.OverwriteIfNewer,
                                             null,
                                             f => fileBlacklist.Contains(Path.GetFileNameWithoutExtension(f.Name)) || !extensionWhitelist.Contains(Path.GetExtension(f.Name)));
                });

            CopyDirectoryRecursively(PluginsDirectory / "Devices", ServiceBinPath / "Plugins" / "Devices", DirectoryExistsPolicy.Merge, FileExistsPolicy.OverwriteIfNewer);
        });

    Target Pack => _ => _
        .DependsOn(Clean)
        .DependsOn(Compile)
        .Executes(() =>
        {
            var files = Directory.EnumerateFiles(ServiceBinPath, "*", SearchOption.AllDirectories)
                .Where(f => Path.GetFileName(f) != "config.json"
                         && Path.GetExtension(f) != "InstallState");

            if (Configuration != Configuration.Debug)
                files = files.Where(f => Path.GetExtension(f) != ".pdb");

            ZipFiles(ArtifactsDirectory / $"TTController_{GitVersion.AssemblySemVer}{GitVersion.PreReleaseTagWithDash}.{GitVersion.Sha}.zip", ServiceBinPath, files);
        });

    private static void ZipFiles(string outFile, string workingDirectory, IEnumerable<string> files)
    {
        using (var zip = ZipFile.Open(outFile, ZipArchiveMode.Create))
            foreach (var file in files)
                zip.CreateEntryFromFile(file, Path.GetRelativePath(workingDirectory, file), CompressionLevel.Optimal);
    }
}
