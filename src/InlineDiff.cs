namespace Philiprehberger.TextDiff;

/// <summary>
/// Computes character-level and word-level diffs within a pair of lines.
/// </summary>
public static class InlineDiff
{
    /// <summary>
    /// Computes a character-level diff between two strings.
    /// </summary>
    /// <param name="oldText">The original string.</param>
    /// <param name="newText">The modified string.</param>
    /// <returns>A list of <see cref="InlineChange"/> segments describing character-level differences.</returns>
    public static IReadOnlyList<InlineChange> ComputeCharacterDiff(string oldText, string newText)
    {
        var oldChars = oldText.Select(c => c.ToString()).ToArray();
        var newChars = newText.Select(c => c.ToString()).ToArray();
        return ComputeTokenDiff(oldChars, newChars);
    }

    /// <summary>
    /// Computes a word-level diff between two strings, splitting on word boundaries.
    /// </summary>
    /// <param name="oldText">The original string.</param>
    /// <param name="newText">The modified string.</param>
    /// <returns>A list of <see cref="InlineChange"/> segments describing word-level differences.</returns>
    public static IReadOnlyList<InlineChange> ComputeWordDiff(string oldText, string newText)
    {
        var oldTokens = TokenizeWords(oldText);
        var newTokens = TokenizeWords(newText);
        return ComputeTokenDiff(oldTokens, newTokens);
    }

    private static string[] TokenizeWords(string text)
    {
        var tokens = new List<string>();
        var i = 0;
        while (i < text.Length)
        {
            if (char.IsWhiteSpace(text[i]))
            {
                var start = i;
                while (i < text.Length && char.IsWhiteSpace(text[i]))
                {
                    i++;
                }
                tokens.Add(text[start..i]);
            }
            else if (char.IsLetterOrDigit(text[i]) || text[i] == '_')
            {
                var start = i;
                while (i < text.Length && (char.IsLetterOrDigit(text[i]) || text[i] == '_'))
                {
                    i++;
                }
                tokens.Add(text[start..i]);
            }
            else
            {
                tokens.Add(text[i].ToString());
                i++;
            }
        }
        return tokens.ToArray();
    }

    private static IReadOnlyList<InlineChange> ComputeTokenDiff(string[] oldTokens, string[] newTokens)
    {
        var n = oldTokens.Length;
        var m = newTokens.Length;
        var max = n + m;

        if (max == 0)
        {
            return [];
        }

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
                    x = v[k + 1 + max];
                }
                else
                {
                    x = v[k - 1 + max] + 1;
                }

                var y = x - k;

                while (x < n && y < m && oldTokens[x] == newTokens[y])
                {
                    x++;
                    y++;
                }

                v[k + max] = x;

                if (x >= n && y >= m)
                {
                    return BacktrackTokens(trace, oldTokens, newTokens, max);
                }
            }
        }

        return BacktrackTokens(trace, oldTokens, newTokens, max);
    }

    private static IReadOnlyList<InlineChange> BacktrackTokens(List<int[]> trace, string[] oldTokens, string[] newTokens, int max)
    {
        var edits = new List<InlineChange>();
        var x = oldTokens.Length;
        var y = newTokens.Length;

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

            while (x > prevX && y > prevY)
            {
                x--;
                y--;
                edits.Add(new InlineChange(ChangeType.Equal, oldTokens[x]));
            }

            if (d > 0)
            {
                if (x == prevX)
                {
                    y--;
                    edits.Add(new InlineChange(ChangeType.Insert, newTokens[y]));
                }
                else
                {
                    x--;
                    edits.Add(new InlineChange(ChangeType.Delete, oldTokens[x]));
                }
            }
        }

        edits.Reverse();
        return MergeAdjacentChanges(edits);
    }

    private static IReadOnlyList<InlineChange> MergeAdjacentChanges(List<InlineChange> changes)
    {
        if (changes.Count == 0)
        {
            return [];
        }

        var merged = new List<InlineChange>();
        var currentType = changes[0].Type;
        var currentText = changes[0].Text;

        for (var i = 1; i < changes.Count; i++)
        {
            if (changes[i].Type == currentType)
            {
                currentText += changes[i].Text;
            }
            else
            {
                merged.Add(new InlineChange(currentType, currentText));
                currentType = changes[i].Type;
                currentText = changes[i].Text;
            }
        }

        merged.Add(new InlineChange(currentType, currentText));
        return merged;
    }
}
