namespace Philiprehberger.TextDiff;

/// <summary>
/// Implements Myers' diff algorithm to compute the shortest edit script
/// between two sequences of lines.
/// </summary>
internal static class MyersAlgorithm
{
    /// <summary>
    /// Represents a single edit operation in the diff.
    /// </summary>
    internal enum EditType
    {
        /// <summary>The line is present in both sequences.</summary>
        Equal,

        /// <summary>The line was inserted (present only in the new sequence).</summary>
        Insert,

        /// <summary>The line was deleted (present only in the old sequence).</summary>
        Delete
    }

    /// <summary>
    /// Represents a single edit operation with its type and associated line content.
    /// </summary>
    /// <param name="Type">The type of edit.</param>
    /// <param name="Content">The line content.</param>
    internal record Edit(EditType Type, string Content);

    /// <summary>
    /// Computes the shortest edit script between two arrays of lines using Myers' algorithm.
    /// </summary>
    /// <param name="oldLines">The original lines.</param>
    /// <param name="newLines">The modified lines.</param>
    /// <returns>A list of edit operations that transform <paramref name="oldLines"/> into <paramref name="newLines"/>.</returns>
    internal static List<Edit> ComputeEdits(string[] oldLines, string[] newLines)
    {
        var n = oldLines.Length;
        var m = newLines.Length;
        var max = n + m;

        // V stores the furthest-reaching x for each diagonal k.
        // Offset by max so negative diagonals can be indexed.
        var v = new int[2 * max + 1];
        var trace = new List<int[]>();

        for (var d = 0; d <= max; d++)
        {
            var snapshot = new int[v.Length];
            Array.Copy(v, snapshot, v.Length);
            trace.Add(snapshot);

            for (var k = -d; k <= d; k += 2)
            {
                int x;
                if (k == -d || (k != d && v[k - 1 + max] < v[k + 1 + max]))
                {
                    x = v[k + 1 + max]; // move down (insert)
                }
                else
                {
                    x = v[k - 1 + max] + 1; // move right (delete)
                }

                var y = x - k;

                // Follow diagonal (equal lines)
                while (x < n && y < m && oldLines[x] == newLines[y])
                {
                    x++;
                    y++;
                }

                v[k + max] = x;

                if (x >= n && y >= m)
                {
                    return Backtrack(trace, oldLines, newLines, max);
                }
            }
        }

        // Fallback — should not be reached for valid inputs.
        return Backtrack(trace, oldLines, newLines, max);
    }

    private static List<Edit> Backtrack(List<int[]> trace, string[] oldLines, string[] newLines, int max)
    {
        var edits = new List<Edit>();
        var x = oldLines.Length;
        var y = newLines.Length;

        for (var d = trace.Count - 1; d >= 0; d--)
        {
            var v = trace[d];
            var k = x - y;

            int prevK;
            if (k == -d || (k != d && v[k - 1 + max] < v[k + 1 + max]))
            {
                prevK = k + 1;
            }
            else
            {
                prevK = k - 1;
            }

            var prevX = v[prevK + max];
            var prevY = prevX - prevK;

            // Trace diagonal (equal lines)
            while (x > prevX && y > prevY)
            {
                x--;
                y--;
                edits.Add(new Edit(EditType.Equal, oldLines[x]));
            }

            if (d > 0)
            {
                if (x == prevX)
                {
                    // Insert
                    y--;
                    edits.Add(new Edit(EditType.Insert, newLines[y]));
                }
                else
                {
                    // Delete
                    x--;
                    edits.Add(new Edit(EditType.Delete, oldLines[x]));
                }
            }
        }

        edits.Reverse();
        return edits;
    }
}
