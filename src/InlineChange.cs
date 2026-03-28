namespace Philiprehberger.TextDiff;

/// <summary>
/// Represents a segment within a character-level or word-level inline diff of a single line.
/// </summary>
/// <param name="Type">The type of change for this segment.</param>
/// <param name="Text">The text content of the segment.</param>
public record InlineChange(ChangeType Type, string Text);

/// <summary>
/// Represents the type of change for an inline segment.
/// </summary>
public enum ChangeType
{
    /// <summary>The segment is equal in both old and new text.</summary>
    Equal,

    /// <summary>The segment was inserted in the new text.</summary>
    Insert,

    /// <summary>The segment was deleted from the old text.</summary>
    Delete
}
