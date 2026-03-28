using Xunit;
using Philiprehberger.TextDiff;

namespace Philiprehberger.TextDiff.Tests;

public class InlineDiffTests
{
    [Fact]
    public void CharacterDiff_IdenticalStrings_ReturnsAllEqual()
    {
        var changes = InlineDiff.ComputeCharacterDiff("hello", "hello");

        Assert.Single(changes);
        Assert.Equal(ChangeType.Equal, changes[0].Type);
        Assert.Equal("hello", changes[0].Text);
    }

    [Fact]
    public void CharacterDiff_SingleCharChange_DetectsChange()
    {
        var changes = InlineDiff.ComputeCharacterDiff("cat", "car");

        Assert.True(changes.Any(c => c.Type == ChangeType.Delete && c.Text == "t"));
        Assert.True(changes.Any(c => c.Type == ChangeType.Insert && c.Text == "r"));
    }

    [Fact]
    public void CharacterDiff_Insertion_DetectsAddedCharacters()
    {
        var changes = InlineDiff.ComputeCharacterDiff("ac", "abc");

        Assert.True(changes.Any(c => c.Type == ChangeType.Insert && c.Text == "b"));
    }

    [Fact]
    public void CharacterDiff_Deletion_DetectsRemovedCharacters()
    {
        var changes = InlineDiff.ComputeCharacterDiff("abc", "ac");

        Assert.True(changes.Any(c => c.Type == ChangeType.Delete && c.Text == "b"));
    }

    [Fact]
    public void CharacterDiff_EmptyStrings_ReturnsEmpty()
    {
        var changes = InlineDiff.ComputeCharacterDiff("", "");

        Assert.Empty(changes);
    }

    [Fact]
    public void WordDiff_IdenticalStrings_ReturnsAllEqual()
    {
        var changes = InlineDiff.ComputeWordDiff("hello world", "hello world");

        Assert.True(changes.All(c => c.Type == ChangeType.Equal));
        Assert.Equal("hello world", string.Concat(changes.Select(c => c.Text)));
    }

    [Fact]
    public void WordDiff_ChangedWord_DetectsChange()
    {
        var changes = InlineDiff.ComputeWordDiff("hello world", "hello earth");

        Assert.True(changes.Any(c => c.Type == ChangeType.Delete && c.Text == "world"));
        Assert.True(changes.Any(c => c.Type == ChangeType.Insert && c.Text == "earth"));
    }

    [Fact]
    public void WordDiff_AddedWord_DetectsInsertion()
    {
        var changes = InlineDiff.ComputeWordDiff("hello world", "hello beautiful world");

        Assert.True(changes.Any(c => c.Type == ChangeType.Insert));
    }

    [Fact]
    public void WordDiff_RemovedWord_DetectsDeletion()
    {
        var changes = InlineDiff.ComputeWordDiff("hello beautiful world", "hello world");

        Assert.True(changes.Any(c => c.Type == ChangeType.Delete));
    }

    [Fact]
    public void WordDiff_PreservesWhitespace()
    {
        var changes = InlineDiff.ComputeWordDiff("a b", "a b");

        var combined = string.Concat(changes.Select(c => c.Text));
        Assert.Equal("a b", combined);
    }
}
