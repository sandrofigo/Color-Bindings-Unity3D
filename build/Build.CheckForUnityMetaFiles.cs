using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Utilities.Collections;
using Serilog;

partial class Build
{
    [Parameter] AbsolutePath[] CheckForUnityMetaFilesIncludedDirectories;
    [Parameter] AbsolutePath[] CheckForUnityMetaFilesExcludedDirectories;

    Target CheckForUnityMetaFiles => _ => _
        .Executes(() =>
        {
            Log.Information("Checking if all necessary Unity .meta files exist...");

            // Verify that all files and directories have a Unity meta file (if the meta file is missing the file will be ignored when imported into Unity)
            AssertThatUnityMetaFilesExist(CheckForUnityMetaFilesIncludedDirectories, CheckForUnityMetaFilesIncludedDirectories, out int totalDirectoriesChecked, out int totalFilesChecked);

            Log.Information("Checked a total of {Directories} directories and {Files} files", totalDirectoriesChecked, totalFilesChecked);
        });

    /// <summary>
    /// Checks recursively if all files and folders have a Unity meta file.
    /// </summary>
    static void AssertThatUnityMetaFilesExist(AbsolutePath[] includeDirectories, AbsolutePath[] excludeDirectories, out int totalDirectoriesChecked, out int totalFilesChecked)
    {
        excludeDirectories ??= Array.Empty<AbsolutePath>();

        if (includeDirectories.IsNullOrEmpty() || includeDirectories.All(d => !d.DirectoryExists()))
            Assert.Fail("No directories have been provided to check for .meta files or they don't exist!");

        totalDirectoriesChecked = 0;
        totalFilesChecked = 0;

        foreach (AbsolutePath includeDirectory in includeDirectories)
        {
            var directories = includeDirectory.GlobDirectories("**").Where(d => d != includeDirectory);

            foreach (AbsolutePath d in directories)
            {
                if (excludeDirectories.Contains(d))
                    continue;

                Assert.True((d.Parent / (d.Name + ".meta")).FileExists(), $"The directory '{d}' does not have a Unity meta file!");

                totalDirectoriesChecked++;
            }

            var files = includeDirectory.GlobFiles("**/*").Where(f => !f.ToString().EndsWith(".meta"));

            foreach (AbsolutePath f in files)
            {
                if (excludeDirectories.Contains(f.Parent))
                    continue;

                Assert.True((f.Parent / (f.Name + ".meta")).FileExists(), $"The file '{f}' does not have a Unity meta file!");

                totalFilesChecked++;
            }
        }
    }
}