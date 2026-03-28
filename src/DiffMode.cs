namespace Philiprehberger.TextDiff;

/// <summary>
/// Specifies the granularity at which text is compared.
/// </summary>
public enum DiffMode
{
    /// <summary>Compare text line by line.</summary>
    Line,

    /// <summary>Compare text word by word within each line.</summary>
    Word,

    /// <summary>Compare text character by character within each line.</summary>
    Character
}
