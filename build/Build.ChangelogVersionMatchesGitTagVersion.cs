using System.Linq;
using KeepAChangelog.IO;
using NuGet.Versioning;
using Nuke.Common;
using Nuke.Common.IO;
using Serilog;

partial class Build
{
    Target ChangelogVersionMatchesGitTagVersion => _ => _
        .OnlyWhenStatic(() => GitRepository.CurrentCommitHasVersionTag())
        .Executes(() =>
        {
            SemanticVersion versionFromChangelog = GetLatestVersionFromChangelog(ChangelogFile);
            SemanticVersion versionFromGitTag = GitRepository.GetLatestVersionTagOnCurrentCommit();

            Assert.True(versionFromChangelog == versionFromGitTag, $"The version {versionFromChangelog} from the changelog does not match the version {versionFromGitTag} from the git tag!");

            Log.Information("Changelog version and git tag version match");
        });

    static SemanticVersion GetLatestVersionFromChangelog(AbsolutePath pathToChangelogFile)
    {
        Release latestRelease = GetLatestChangelogRelease(pathToChangelogFile);

        return latestRelease == null ? null : SemanticVersion.Parse(latestRelease.Version);
    }

    static Release GetLatestChangelogRelease(AbsolutePath pathToChangelogFile)
    {
        Assert.True(pathToChangelogFile != null, "No path has been provided!");

        return Changelog.FromFile(pathToChangelogFile)
            .Releases
            .Where(r => r.IsReleased)
            .MaxBy(r => SemanticVersion.Parse(r.Version));
    }
}
