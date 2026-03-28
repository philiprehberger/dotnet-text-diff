namespace Philiprehberger.TextDiff;

/// <summary>
/// Represents the result of comparing two texts, containing the individual
/// diff lines, summary counts, and statistics.
/// </summary>
/// <param name="Lines">The collection of diff lines produced by the comparison.</param>
/// <param name="AddedCount">The number of lines that were added.</param>
/// <param name="RemovedCount">The number of lines that were removed.</param>
/// <param name="UnchangedCount">The number of lines that are unchanged.</param>
/// <param name="HasChanges">Whether the two texts differ.</param>
/// <param name="Statistics">Detailed diff statistics including modifications and change percentage.</param>
public record DiffResult(
    IReadOnlyList<DiffLine> Lines,
    int AddedCount,
    int RemovedCount,
    int UnchangedCount,
    bool HasChanges,
    DiffStatistics Statistics);
