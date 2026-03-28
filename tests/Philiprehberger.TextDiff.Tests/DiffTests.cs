using Xunit;
using Philiprehberger.TextDiff;

namespace Philiprehberger.TextDiff.Tests;

public class DiffTests
{
    [Fact]
    public void Compare_IdenticalTexts_ReturnsNoChanges()
    {
        var result = Diff.Compare("hello\nworld", "hello\nworld");

        Assert.False(result.HasChanges);
        Assert.Equal(0, result.AddedCount);
        Assert.Equal(0, result.RemovedCount);
        Assert.Equal(2, result.UnchangedCount);
    }

    [Fact]
    public void Compare_AddedLine_CountsCorrectly()
    {
        var result = Diff.Compare("a\nb", "a\nb\nc");

        Assert.True(result.HasChanges);
        Assert.Equal(1, result.AddedCount);
        Assert.Equal(0, result.RemovedCount);
    }

    [Fact]
    public void Compare_RemovedLine_CountsCorrectly()
    {
        var result = Diff.Compare("a\nb\nc", "a\nc");

        Assert.True(result.HasChanges);
        Assert.Equal(0, result.AddedCount);
        Assert.Equal(1, result.RemovedCount);
    }

    [Fact]
    public void Compare_ModifiedLine_ShowsRemoveAndAdd()
    {
        var result = Diff.Compare("hello\nworld", "hello\nearth");

        Assert.True(result.HasChanges);
        Assert.Equal(1, result.AddedCount);
        Assert.Equal(1, result.RemovedCount);
    }

    [Fact]
    public void Compare_EmptyTexts_ReturnsNoChanges()
    {
        var result = Diff.Compare("", "");

        Assert.False(result.HasChanges);
        Assert.Equal(0, result.AddedCount);
        Assert.Equal(0, result.RemovedCount);
        Assert.Equal(0, result.UnchangedCount);
    }

    [Fact]
    public void Lines_ReturnsCorrectLineNumbers()
    {
        var lines = Diff.Lines("a\nb\nc", "a\nc");

        Assert.Equal(3, lines.Count);
        Assert.Equal(DiffLineType.Unchanged, lines[0].Type);
        Assert.Equal(1, lines[0].OldLineNumber);
        Assert.Equal(1, lines[0].NewLineNumber);
        Assert.Equal(DiffLineType.Removed, lines[1].Type);
        Assert.Equal(2, lines[1].OldLineNumber);
        Assert.Null(lines[1].NewLineNumber);
        Assert.Equal(DiffLineType.Unchanged, lines[2].Type);
    }

    [Fact]
    public void Unified_ProducesCorrectFormat()
    {
        var unified = Diff.Unified("alpha\nbeta\ngamma", "alpha\nbeta\ndelta", "old.txt", "new.txt", context: 2);

        Assert.Contains("--- old.txt", unified);
        Assert.Contains("+++ new.txt", unified);
        Assert.Contains("-gamma", unified);
        Assert.Contains("+delta", unified);
    }
}
