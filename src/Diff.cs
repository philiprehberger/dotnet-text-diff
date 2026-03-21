namespace Philiprehberger.TextDiff;

/// <summary>
/// Provides static methods for comparing two text strings line by line
/// using Myers' diff algorithm.
/// </summary>
public static class Diff
{
    /// <summary>
    /// Compares two texts and returns a <see cref="DiffResult"/> containing
    /// the individual diff lines and summary statistics.
    /// </summary>
    /// <param name="oldText">The original text.</param>
    /// <param name="newText">The modified text.</param>
    /// <returns>A <see cref="DiffResult"/> describing the differences between the two texts.</returns>
    public static DiffResult Compare(string oldText, string newText)
    {
        var diffLines = Lines(oldText, newText);

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

        return new DiffResult(diffLines, added, removed, unchanged, added > 0 || removed > 0);
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
    /// <returns>A unified diff string, or an empty string if the texts are identical.</returns>
    public static string Unified(string oldText, string newText, string oldLabel = "old", string newLabel = "new", int context = 3)
    {
        var diffLines = Lines(oldText, newText);
        return UnifiedDiffFormatter.Format(diffLines, oldLabel, newLabel, context);
    }

    /// <summary>
    /// Compares two texts and returns the individual diff lines with
    /// change types and line numbers.
    /// </summary>
    /// <param name="oldText">The original text.</param>
    /// <param name="newText">The modified text.</param>
    /// <returns>A read-only list of <see cref="DiffLine"/> entries describing each line's change status.</returns>
    public static IReadOnlyList<DiffLine> Lines(string oldText, string newText)
    {
        var oldLines = SplitLines(oldText);
        var newLines = SplitLines(newText);

        var edits = MyersAlgorithm.ComputeEdits(oldLines, newLines);

        var result = new List<DiffLine>(edits.Count);
        var oldLineNumber = 1;
        var newLineNumber = 1;

        foreach (var edit in edits)
        {
            switch (edit.Type)
            {
                case MyersAlgorithm.EditType.Equal:
                    result.Add(new DiffLine(DiffLineType.Unchanged, edit.Content, oldLineNumber, newLineNumber));
                    oldLineNumber++;
                    newLineNumber++;
                    break;
                case MyersAlgorithm.EditType.Delete:
                    result.Add(new DiffLine(DiffLineType.Removed, edit.Content, oldLineNumber, null));
                    oldLineNumber++;
                    break;
                case MyersAlgorithm.EditType.Insert:
                    result.Add(new DiffLine(DiffLineType.Added, edit.Content, null, newLineNumber));
                    newLineNumber++;
                    break;
            }
        }

        return result;
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
