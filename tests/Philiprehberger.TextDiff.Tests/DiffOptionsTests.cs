using Xunit;
using Philiprehberger.TextDiff;

namespace Philiprehberger.TextDiff.Tests;

public class DiffOptionsTests
{
    [Fact]
    public void IgnoreWhitespace_TreatsLeadingTrailingSpacesAsEqual()
    {
        var options = new DiffOptions { IgnoreWhitespace = true };
        var result = Diff.Compare("  hello  \nworld", "hello\nworld", options);

        Assert.False(result.HasChanges);
    }

    [Fact]
    public void IgnoreCase_TreatsDifferentCaseAsEqual()
    {
        var options = new DiffOptions { IgnoreCase = true };
        var result = Diff.Compare("Hello\nWorld", "hello\nworld", options);

        Assert.False(result.HasChanges);
    }

    [Fact]
    public void IgnoreBlankLines_SkipsEmptyLines()
    {
        var options = new DiffOptions { IgnoreBlankLines = true };
        var result = Diff.Compare("a\n\nb", "a\nb", options);

        Assert.False(result.HasChanges);
    }

    [Fact]
    public void IgnoreBlankLines_WithMultipleBlanks_StillMatches()
    {
        var options = new DiffOptions { IgnoreBlankLines = true };
        var result = Diff.Compare("a\n\n\n\nb", "a\nb", options);

        Assert.False(result.HasChanges);
    }

    [Fact]
    public void IgnoreCase_StillDetectsDifferentContent()
    {
        var options = new DiffOptions { IgnoreCase = true };
        var result = Diff.Compare("hello", "world", options);

        Assert.True(result.HasChanges);
    }

    [Fact]
    public void CombinedOptions_IgnoreWhitespaceAndCase()
    {
        var options = new DiffOptions { IgnoreWhitespace = true, IgnoreCase = true };
        var result = Diff.Compare("  Hello  ", "hello", options);

        Assert.False(result.HasChanges);
    }

    [Fact]
    public void Default_Options_HasAllFlagsDisabled()
    {
        var options = DiffOptions.Default;

        Assert.False(options.IgnoreWhitespace);
        Assert.False(options.IgnoreCase);
        Assert.False(options.IgnoreBlankLines);
        Assert.Equal(DiffMode.Line, options.Mode);
    }
}
