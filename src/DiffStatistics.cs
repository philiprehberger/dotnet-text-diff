namespace Philiprehberger.TextDiff;

/// <summary>
/// Provides summary statistics for a diff result, including the number of
/// additions, deletions, modifications, and the overall change percentage.
/// </summary>
/// <param name="Additions">The number of added lines.</param>
/// <param name="Deletions">The number of deleted lines.</param>
/// <param name="Modifications">The number of modified lines (paired add/remove sequences).</param>
/// <param name="ChangePercentage">The percentage of lines that changed, from 0.0 to 100.0.</param>
public record DiffStatistics(
    int Additions,
    int Deletions,
    int Modifications,
    double ChangePercentage);
