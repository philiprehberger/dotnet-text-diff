namespace Philiprehberger.TextDiff;

/// <summary>
/// Formats a list of <see cref="DiffLine"/> entries into unified diff format
/// with standard <c>---</c>, <c>+++</c>, and <c>@@</c> headers.
/// </summary>
internal static class UnifiedDiffFormatter
{
    /// <summary>
    /// Formats diff lines as a unified diff string.
    /// </summary>
    /// <param name="lines">The diff lines to format.</param>
    /// <param name="oldLabel">The label for the old file in the <c>---</c> header.</param>
    /// <param name="newLabel">The label for the new file in the <c>+++</c> header.</param>
    /// <param name="context">The number of unchanged context lines to include around each change.</param>
    /// <returns>A unified diff string, or an empty string if there are no changes.</returns>
    internal static string Format(IReadOnlyList<DiffLine> lines, string oldLabel, string newLabel, int context)
    {
        if (lines.Count == 0)
        {
            return string.Empty;
        }

        // Find ranges of changed lines, expanded by context.
        var hunks = BuildHunks(lines, context);

        if (hunks.Count == 0)
        {
            return string.Empty;
        }

        var builder = new System.Text.StringBuilder();
        builder.AppendLine($"--- {oldLabel}");
        builder.AppendLine($"+++ {newLabel}");

        foreach (var hunk in hunks)
        {
            var oldStart = GetOldStart(lines, hunk.Start);
            var newStart = GetNewStart(lines, hunk.Start);
            var oldCount = 0;
            var newCount = 0;

            for (var i = hunk.Start; i <= hunk.End; i++)
            {
                var line = lines[i];
                switch (line.Type)
                {
                    case DiffLineType.Removed:
                        oldCount++;
                        break;
                    case DiffLineType.Added:
                        newCount++;
                        break;
                    case DiffLineType.Unchanged:
                        oldCount++;
                        newCount++;
                        break;
                }
            }

            builder.AppendLine($"@@ -{oldStart},{oldCount} +{newStart},{newCount} @@");

            for (var i = hunk.Start; i <= hunk.End; i++)
            {
                var line = lines[i];
                var prefix = line.Type switch
                {
                    DiffLineType.Added => "+",
                    DiffLineType.Removed => "-",
                    _ => " "
                };
                builder.AppendLine($"{prefix}{line.Content}");
            }
        }

        // Remove trailing newline for clean output.
        var result = builder.ToString();
        return result.TrimEnd('\r', '\n');
    }

    private static int GetOldStart(IReadOnlyList<DiffLine> lines, int index)
    {
        var line = lines[index];
        return line.OldLineNumber ?? FindNearestOldLine(lines, index);
    }

    private static int GetNewStart(IReadOnlyList<DiffLine> lines, int index)
    {
        var line = lines[index];
        return line.NewLineNumber ?? FindNearestNewLine(lines, index);
    }

    private static int FindNearestOldLine(IReadOnlyList<DiffLine> lines, int index)
    {
        // Search backward for the nearest line with an old line number.
        for (var i = index - 1; i >= 0; i--)
        {
            if (lines[i].OldLineNumber.HasValue)
            {
                return lines[i].OldLineNumber!.Value + 1;
            }
        }

        return 1;
    }

    private static int FindNearestNewLine(IReadOnlyList<DiffLine> lines, int index)
    {
        // Search backward for the nearest line with a new line number.
        for (var i = index - 1; i >= 0; i--)
        {
            if (lines[i].NewLineNumber.HasValue)
            {
                return lines[i].NewLineNumber!.Value + 1;
            }
        }

        return 1;
    }

    private record Hunk(int Start, int End);

    private static List<Hunk> BuildHunks(IReadOnlyList<DiffLine> lines, int context)
    {
        // Find indices of changed lines.
        var changedIndices = new List<int>();
        for (var i = 0; i < lines.Count; i++)
        {
            if (lines[i].Type != DiffLineType.Unchanged)
            {
                changedIndices.Add(i);
            }
        }

        if (changedIndices.Count == 0)
        {
            return [];
        }

        var hunks = new List<Hunk>();
        var start = Math.Max(0, changedIndices[0] - context);
        var end = Math.Min(lines.Count - 1, changedIndices[0] + context);

        for (var i = 1; i < changedIndices.Count; i++)
        {
            var changeStart = Math.Max(0, changedIndices[i] - context);
            var changeEnd = Math.Min(lines.Count - 1, changedIndices[i] + context);

            if (changeStart <= end + 1)
            {
                // Merge overlapping or adjacent hunks.
                end = changeEnd;
            }
            else
            {
                hunks.Add(new Hunk(start, end));
                start = changeStart;
                end = changeEnd;
            }
        }

        hunks.Add(new Hunk(start, end));
        return hunks;
    }
}
