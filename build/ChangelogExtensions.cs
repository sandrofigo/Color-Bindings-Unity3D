using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NuGet.Versioning;
using Nuke.Common.ChangeLog;
using Nuke.Common.IO;
using Nuke.Common.Utilities.Collections;

public static class ChangelogExtensions
{
    public static ReleaseNotes GetLatestReleaseNotes(AbsolutePath changelogFile)
    {
        var releaseNotes = ReadVersionedReleaseNotes(changelogFile);

        return releaseNotes.IsEmpty() ? null : releaseNotes.MaxBy(n => n.Version);
    }

    /// <summary>
    /// Reads all versioned release sections of a Keep a Changelog file.
    /// Replaces ChangelogTasks.ReadChangelog, which since NUKE 7.1 ends a section at its first empty line.
    /// </summary>
    static IReadOnlyList<ReleaseNotes> ReadVersionedReleaseNotes(AbsolutePath changelogFile)
    {
        var lines = changelogFile.ReadAllLines().ToList();
        var headIndices = lines.Select((line, index) => (line, index)).Where(x => x.line.StartsWith("## ")).Select(x => x.index).ToList();
        var result = new List<ReleaseNotes>();

        for (int i = 0; i < headIndices.Count; i++)
        {
            int startIndex = headIndices[i];
            int endIndex = i + 1 < headIndices.Count ? headIndices[i + 1] - 1 : lines.Count - 1;
            string caption = lines[startIndex].TrimStart('#', ' ', '[').Split(' ').First().TrimEnd(']');

            if (!NuGetVersion.TryParse(caption, out var version))
                continue;

            // Exclude link reference definitions (e.g. "[1.0.0]: https://...") at the end of the file
            var notes = lines.Skip(startIndex + 1).Take(endIndex - startIndex).Where(l => !Regex.IsMatch(l, @"^\[[^\]]+\]:\s")).ToList();

            if (notes.Any(n => !string.IsNullOrWhiteSpace(n)))
                result.Add(new ReleaseNotes(version, notes.AsReadOnly(), startIndex, endIndex));
        }

        return result.AsReadOnly();
    }
}
