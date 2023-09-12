using System;
using System.Linq;
using NuGet.Versioning;
using Nuke.Common;
using Nuke.Common.ChangeLog;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitHubActions.Configuration;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using Octokit;
using Serilog;
using Spectre.Console;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

[GitHubActions("tests",
    GitHubActionsImage.UbuntuLatest,
    OnPushBranches = new[] { "main" },
    InvokedTargets = new[] { nameof(Test) },
    PublishCondition = "always()",
    EnableGitHubToken = true,
    JobConcurrencyCancelInProgress = true
)]
[GitHubActions("publish",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = new[] { nameof(Publish) },
    PublishCondition = "always()",
    EnableGitHubToken = true,
    OnPushTags = new[] { "v[0-9]+.[0-9]+.[0-9]+" }
)]
partial class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Test);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [GitRepository] GitRepository GitRepository;
    [Parameter] AbsolutePath UnityPackageFile;
    [Parameter] AbsolutePath ChangelogFile;

    Target Clean => _ => _
        .Executes(() =>
        {
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .DependsOn(CheckForUnityMetaFiles)
        .Executes(() =>
        {
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
        });

    Target Publish => _ => _
        .DependsOn(Test)
        .Triggers(PublishGitHubRelease);

    Target Doctor => _ => _
        .DependsOn(Publish)
        .Executes(() =>
        {
            LogFancyStatus(SucceededTargets.Contains(CheckForUnityMetaFiles), "Unity .meta files exist on current commit");
            LogFancyStatus(GitRepository.CurrentCommitHasVersionTag(), "Git version tag exists");
            LogFancyStatus(SucceededTargets.Contains(UnityPackageVersionMatchesGitTagVersion), "Unity package version matches git tag version");
            LogFancyStatus(SucceededTargets.Contains(ChangelogVersionMatchesGitTagVersion), "Changelog version matches git tag version");

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine(":notebook: Changelog Content:");
            AnsiConsole.MarkupLine($"[gray]{GetGitHubChangelogBody()}[/]");

            return;
            void LogFancyStatus(bool value, string text) => AnsiConsole.MarkupLine($"[{(value ? "green" : "red")}]{(value ? ":check_mark:" : ":multiply:")} {text}[/]");
        });
}