namespace Philiprehberger.TextDiff;

/// <summary>
/// Options that control how text comparison is performed.
/// </summary>
public class DiffOptions
{
    /// <summary>
    /// Gets or sets whether to ignore leading and trailing whitespace differences.
    /// Defaults to <c>false</c>.
    /// </summary>
    public bool IgnoreWhitespace { get; set; }

    /// <summary>
    /// Gets or sets whether to ignore case differences.
    /// Defaults to <c>false</c>.
    /// </summary>
    public bool IgnoreCase { get; set; }

    /// <summary>
    /// Gets or sets whether to ignore blank lines when computing the diff.
    /// Defaults to <c>false</c>.
    /// </summary>
    public bool IgnoreBlankLines { get; set; }

    /// <summary>
    /// Gets or sets the diff granularity mode.
    /// Defaults to <see cref="DiffMode.Line"/>.
    /// </summary>
    public DiffMode Mode { get; set; } = DiffMode.Line;

    /// <summary>
    /// Gets the default options with no ignore flags and line-level diffing.
    /// </summary>
    public static DiffOptions Default => new();
}
