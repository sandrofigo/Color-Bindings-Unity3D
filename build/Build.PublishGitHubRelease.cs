using System;
using System.Linq;
using NuGet.Versioning;
using Nuke.Common;
using Nuke.Common.ChangeLog;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Utilities.Collections;
using Octokit;
using Serilog;

partial class Build
{
    Target PublishGitHubRelease => _ => _
        .OnlyWhenStatic(() => GitRepository.CurrentCommitHasVersionTag())
        .OnlyWhenStatic(() => IsServerBuild)
        .DependsOn(UnityPackageVersionMatchesGitTagVersion)
        .DependsOn(ChangelogVersionMatchesGitTagVersion)
        .Executes(async () =>
        {
            Assert.True(ChangelogFile != null, "No path has been provided!");


            SemanticVersion version = GitRepository.GetLatestVersionTagOnCurrentCommit();

            var releaseDraft = new NewRelease($"v{version}")
            {
                Draft = true,
                Name = $"v{version}",
                Prerelease = version.IsPrerelease,
                Body = GetGitHubChangelogBody()
            };

            string owner = GitRepository.GetGitHubOwner();
            string name = GitRepository.GetGitHubName();

            var credentials = new Credentials(GitHubActions.Instance.Token);
            GitHubTasks.GitHubClient = new GitHubClient(new ProductHeaderValue(nameof(NukeBuild)), new Octokit.Internal.InMemoryCredentialStore(credentials));

            Log.Information("Creating GitHub release...");

            Release release = await GitHubTasks.GitHubClient.Repository.Release.Create(owner, name, releaseDraft);

            await GitHubTasks.GitHubClient.Repository.Release.Edit(owner, name, release.Id, new ReleaseUpdate { Draft = false });
        });

    string GetGitHubChangelogBody()
    {
        ChangeLog changelog = ChangelogTasks.ReadChangelog(ChangelogFile);
        ReleaseNotes latestReleaseNotes = changelog.GetLatestReleaseNotes();
        var trimmedNotes = latestReleaseNotes.Notes.SkipUntil(n => !string.IsNullOrWhiteSpace(n)).Reverse().SkipUntil(n => !string.IsNullOrWhiteSpace(n)).Reverse();

        return string.Join(Environment.NewLine, trimmedNotes);
    }
}