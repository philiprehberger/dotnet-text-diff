namespace Philiprehberger.TextDiff;

/// <summary>
/// Represents the type of change for a diff line.
/// </summary>
public enum DiffLineType
{
    /// <summary>A line that was added in the new text.</summary>
    Added,

    /// <summary>A line that was removed from the old text.</summary>
    Removed,

    /// <summary>A line that is unchanged between old and new text.</summary>
    Unchanged
}

/// <summary>
/// Represents a single line in a diff result, including its change type,
/// content, and line numbers in the old and new texts.
/// </summary>
/// <param name="Type">The type of change for this line.</param>
/// <param name="Content">The text content of the line.</param>
/// <param name="OldLineNumber">The 1-based line number in the old text, or <c>null</c> if the line was added.</param>
/// <param name="NewLineNumber">The 1-based line number in the new text, or <c>null</c> if the line was removed.</param>
public record DiffLine(
    DiffLineType Type,
    string Content,
    int? OldLineNumber,
    int? NewLineNumber);
