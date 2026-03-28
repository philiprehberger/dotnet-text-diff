namespace Philiprehberger.TextDiff;

/// <summary>
/// Provides static methods for comparing two text strings line by line
/// using Myers' diff algorithm, with support for word-level and character-level
/// diffing, and configurable ignore options.
/// </summary>
public static class Diff
{
    /// <summary>
    /// Compares two texts and returns a <see cref="DiffResult"/> containing
    /// the individual diff lines, summary statistics, and detailed <see cref="DiffStatistics"/>.
    /// </summary>
    /// <param name="oldText">The original text.</param>
    /// <param name="newText">The modified text.</param>
    /// <param name="options">Optional diff options controlling comparison behavior.</param>
    /// <returns>A <see cref="DiffResult"/> describing the differences between the two texts.</returns>
    public static DiffResult Compare(string oldText, string newText, DiffOptions? options = null)
    {
        options ??= DiffOptions.Default;
        var diffLines = Lines(oldText, newText, options);

        var added = 0;
        var removed = 0;
        var unchanged = 0;

        foreach (var line in diffLines)
        {
            switch (line.Type)
            {
                case DiffLineType.Added:
                    added++;
                    break;
                case DiffLineType.Removed:
                    removed++;
                    break;
                case DiffLineType.Unchanged:
                    unchanged++;
                    break;
            }
        }

        var modifications = CountModifications(diffLines);
        var totalLines = Math.Max(added + removed + unchanged, 1);
        var changePercentage = (double)(added + removed) / totalLines * 100.0;

        var statistics = new DiffStatistics(added, removed, modifications, Math.Round(changePercentage, 2));

        return new DiffResult(diffLines, added, removed, unchanged, added > 0 || removed > 0, statistics);
    }

    /// <summary>
    /// Produces a unified diff string comparing two texts, with standard
    /// <c>---</c>, <c>+++</c>, and <c>@@</c> headers.
    /// </summary>
    /// <param name="oldText">The original text.</param>
    /// <param name="newText">The modified text.</param>
    /// <param name="oldLabel">The label for the old text in the diff header. Defaults to <c>"old"</c>.</param>
    /// <param name="newLabel">The label for the new text in the diff header. Defaults to <c>"new"</c>.</param>
    /// <param name="context">The number of unchanged context lines to show around each change. Defaults to <c>3</c>.</param>
    /// <param name="options">Optional diff options controlling comparison behavior.</param>
    /// <returns>A unified diff string, or an empty string if the texts are identical.</returns>
    public static string Unified(string oldText, string newText, string oldLabel = "old", string newLabel = "new", int context = 3, DiffOptions? options = null)
    {
        options ??= DiffOptions.Default;
        var diffLines = Lines(oldText, newText, options);
        return UnifiedDiffFormatter.Format(diffLines, oldLabel, newLabel, context);
    }

    /// <summary>
    /// Compares two texts and returns the individual diff lines with
    /// change types and line numbers.
    /// </summary>
    /// <param name="oldText">The original text.</param>
    /// <param name="newText">The modified text.</param>
    /// <param name="options">Optional diff options controlling comparison behavior.</param>
    /// <returns>A read-only list of <see cref="DiffLine"/> entries describing each line's change status.</returns>
    public static IReadOnlyList<DiffLine> Lines(string oldText, string newText, DiffOptions? options = null)
    {
        options ??= DiffOptions.Default;

        var oldLines = SplitLines(oldText);
        var newLines = SplitLines(newText);

        if (options.IgnoreBlankLines)
        {
            oldLines = oldLines.Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            newLines = newLines.Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
        }

        var oldCompare = NormalizeForComparison(oldLines, options);
        var newCompare = NormalizeForComparison(newLines, options);

        var edits = MyersAlgorithm.ComputeEdits(oldCompare, newCompare);

        var result = new List<DiffLine>(edits.Count);
        var oldLineNumber = 1;
        var newLineNumber = 1;
        var oldIndex = 0;
        var newIndex = 0;

        foreach (var edit in edits)
        {
            switch (edit.Type)
            {
                case MyersAlgorithm.EditType.Equal:
                    var equalContent = oldLines[oldIndex];
                    result.Add(new DiffLine(DiffLineType.Unchanged, equalContent, oldLineNumber, newLineNumber));
                    oldLineNumber++;
                    newLineNumber++;
                    oldIndex++;
                    newIndex++;
                    break;
                case MyersAlgorithm.EditType.Delete:
                    result.Add(new DiffLine(DiffLineType.Removed, oldLines[oldIndex], oldLineNumber, null));
                    oldLineNumber++;
                    oldIndex++;
                    break;
                case MyersAlgorithm.EditType.Insert:
                    result.Add(new DiffLine(DiffLineType.Added, newLines[newIndex], null, newLineNumber));
                    newLineNumber++;
                    newIndex++;
                    break;
            }
        }

        return result;
    }

    private static string[] NormalizeForComparison(string[] lines, DiffOptions options)
    {
        if (!options.IgnoreWhitespace && !options.IgnoreCase)
        {
            return lines;
        }

        return lines.Select(l =>
        {
            var normalized = l;
            if (options.IgnoreWhitespace)
            {
                normalized = normalized.Trim();
            }
            if (options.IgnoreCase)
            {
                normalized = normalized.ToUpperInvariant();
            }
            return normalized;
        }).ToArray();
    }

    /// <summary>
    /// Counts the number of modifications by identifying paired remove/add sequences.
    /// A modification is a consecutive sequence of removed lines followed by added lines.
    /// </summary>
    private static int CountModifications(IReadOnlyList<DiffLine> lines)
    {
        var modifications = 0;
        var i = 0;

        while (i < lines.Count)
        {
            if (lines[i].Type == DiffLineType.Removed)
            {
                var removedCount = 0;
                while (i < lines.Count && lines[i].Type == DiffLineType.Removed)
                {
                    removedCount++;
                    i++;
                }

                var addedCount = 0;
                while (i < lines.Count && lines[i].Type == DiffLineType.Added)
                {
                    addedCount++;
                    i++;
                }

                if (addedCount > 0)
                {
                    modifications += Math.Min(removedCount, addedCount);
                }
            }
            else
            {
                i++;
            }
        }

        return modifications;
    }

    private static string[] SplitLines(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return [];
        }

        return text.Split('\n')
            .Select(l => l.TrimEnd('\r'))
            .ToArray();
    }
}
