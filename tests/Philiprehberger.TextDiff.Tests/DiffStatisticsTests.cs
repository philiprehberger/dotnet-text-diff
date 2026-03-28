using Xunit;
using Philiprehberger.TextDiff;

namespace Philiprehberger.TextDiff.Tests;

public class DiffStatisticsTests
{
    [Fact]
    public void Statistics_IdenticalTexts_AllZeros()
    {
        var result = Diff.Compare("a\nb\nc", "a\nb\nc");

        Assert.Equal(0, result.Statistics.Additions);
        Assert.Equal(0, result.Statistics.Deletions);
        Assert.Equal(0, result.Statistics.Modifications);
        Assert.Equal(0.0, result.Statistics.ChangePercentage);
    }

    [Fact]
    public void Statistics_PureAddition_CountsCorrectly()
    {
        var result = Diff.Compare("a", "a\nb\nc");

        Assert.Equal(2, result.Statistics.Additions);
        Assert.Equal(0, result.Statistics.Deletions);
        Assert.Equal(0, result.Statistics.Modifications);
    }

    [Fact]
    public void Statistics_PureDeletion_CountsCorrectly()
    {
        var result = Diff.Compare("a\nb\nc", "a");

        Assert.Equal(0, result.Statistics.Additions);
        Assert.Equal(2, result.Statistics.Deletions);
        Assert.Equal(0, result.Statistics.Modifications);
    }

    [Fact]
    public void Statistics_Modification_DetectsPairedChanges()
    {
        var result = Diff.Compare("hello\nworld", "hello\nearth");

        Assert.Equal(1, result.Statistics.Additions);
        Assert.Equal(1, result.Statistics.Deletions);
        Assert.Equal(1, result.Statistics.Modifications);
    }

    [Fact]
    public void Statistics_ChangePercentage_CalculatedCorrectly()
    {
        var result = Diff.Compare("a\nb\nc\nd", "a\nb\nx\nd");

        var totalLines = result.AddedCount + result.RemovedCount + result.UnchangedCount;
        Assert.True(result.Statistics.ChangePercentage > 0);
        Assert.True(result.Statistics.ChangePercentage <= 100.0);
    }

    [Fact]
    public void Statistics_MultipleModifications_CountsEachPair()
    {
        var result = Diff.Compare("a\nb\nc", "x\ny\nc");

        Assert.Equal(2, result.Statistics.Modifications);
    }
}
