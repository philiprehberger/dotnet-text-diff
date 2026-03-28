using Xunit;
using Philiprehberger.TextDiff;

namespace Philiprehberger.TextDiff.Tests;

public class UnifiedDiffFormatterTests
{
    [Fact]
    public void Format_IdenticalTexts_ReturnsEmpty()
    {
        var unified = Diff.Unified("same\ntext", "same\ntext");

        Assert.Equal(string.Empty, unified);
    }

    [Fact]
    public void Format_SingleLineChange_ProducesValidUnified()
    {
        var unified = Diff.Unified("alpha\nbeta", "alpha\ngamma");

        Assert.Contains("--- old", unified);
        Assert.Contains("+++ new", unified);
        Assert.Contains("@@", unified);
        Assert.Contains("-beta", unified);
        Assert.Contains("+gamma", unified);
    }

    [Fact]
    public void Format_CustomLabels_UsesLabels()
    {
        var unified = Diff.Unified("a", "b", "source.txt", "target.txt");

        Assert.Contains("--- source.txt", unified);
        Assert.Contains("+++ target.txt", unified);
    }

    [Fact]
    public void Format_ContextLines_IncludesContext()
    {
        var unified = Diff.Unified("a\nb\nc\nd\ne", "a\nb\nx\nd\ne", context: 1);

        Assert.Contains(" b", unified);
        Assert.Contains(" d", unified);
        Assert.Contains("-c", unified);
        Assert.Contains("+x", unified);
    }

    [Fact]
    public void Format_MultipleChanges_ProducesMultipleHunks()
    {
        var old = "a\nb\nc\nd\ne\nf\ng\nh\ni\nj";
        var @new = "a\nB\nc\nd\ne\nf\ng\nH\ni\nj";
        var unified = Diff.Unified(old, @new, context: 0);

        var hunkCount = unified.Split("@@").Length - 1;
        Assert.True(hunkCount >= 2, $"Expected at least 2 @@ markers but found {hunkCount / 2} hunks");
    }
}
