# Philiprehberger.TextDiff

[![CI](https://github.com/philiprehberger/dotnet-text-diff/actions/workflows/ci.yml/badge.svg)](https://github.com/philiprehberger/dotnet-text-diff/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Philiprehberger.TextDiff.svg)](https://www.nuget.org/packages/Philiprehberger.TextDiff)
[![Last updated](https://img.shields.io/github/last-commit/philiprehberger/dotnet-text-diff)](https://github.com/philiprehberger/dotnet-text-diff/commits/main)

Line-by-line text diff using Myers' algorithm with unified diff output and structured DiffResult objects.

## Installation

```bash
dotnet add package Philiprehberger.TextDiff
```

## Usage

```csharp
using Philiprehberger.TextDiff;

var result = Diff.Compare("hello\nworld", "hello\nearth");
Console.WriteLine($"Changes: {result.HasChanges}, Added: {result.AddedCount}, Removed: {result.RemovedCount}");
```

### Unified Diff Output

```csharp
using Philiprehberger.TextDiff;

var oldText = "alpha\nbeta\ngamma\ndelta";
var newText = "alpha\nbeta\nepsilon\ndelta";

var unified = Diff.Unified(oldText, newText, "file-v1.txt", "file-v2.txt", context: 2);
Console.WriteLine(unified);
// --- file-v1.txt
// +++ file-v2.txt
// @@ -1,4 +1,4 @@
//  alpha
//  beta
// -gamma
// +epsilon
//  delta
```

### Diff Statistics

```csharp
using Philiprehberger.TextDiff;

var result = Diff.Compare("a\nb\nc", "a\nx\nc");

Console.WriteLine($"Additions: {result.Statistics.Additions}");
Console.WriteLine($"Deletions: {result.Statistics.Deletions}");
Console.WriteLine($"Modifications: {result.Statistics.Modifications}");
Console.WriteLine($"Change %: {result.Statistics.ChangePercentage}");
```

### Diff Options

```csharp
using Philiprehberger.TextDiff;

var options = new DiffOptions
{
    IgnoreWhitespace = true,
    IgnoreCase = true,
    IgnoreBlankLines = true
};

var result = Diff.Compare("  Hello  \n\nWorld", "hello\nworld", options);
Console.WriteLine($"Has changes: {result.HasChanges}"); // False
```

### Character-Level Inline Diff

```csharp
using Philiprehberger.TextDiff;

var changes = InlineDiff.ComputeCharacterDiff("hello world", "hello earth");

foreach (var change in changes)
{
    var marker = change.Type switch
    {
        ChangeType.Equal => " ",
        ChangeType.Delete => "-",
        ChangeType.Insert => "+"
    };
    Console.Write($"[{marker}{change.Text}]");
}
// [ hello ][- ][+e][-w][-o][-r][-l][-d][+a][+r][+t][+h]
```

### Word-Level Diff

```csharp
using Philiprehberger.TextDiff;

var changes = InlineDiff.ComputeWordDiff("the quick brown fox", "the slow brown cat");

foreach (var change in changes)
{
    if (change.Type == ChangeType.Delete)
        Console.Write($"[-{change.Text}]");
    else if (change.Type == ChangeType.Insert)
        Console.Write($"[+{change.Text}]");
    else
        Console.Write(change.Text);
}
// the [-quick][+slow] brown [-fox][+cat]
```

## API

### `Diff`

| Method | Description |
|--------|-------------|
| `Compare(oldText, newText, options?)` | Compares two texts and returns a `DiffResult` with lines, counts, and statistics |
| `Unified(oldText, newText, oldLabel?, newLabel?, context?, options?)` | Produces a unified diff string with `---`, `+++`, and `@@` headers |
| `Lines(oldText, newText, options?)` | Returns a list of `DiffLine` entries with change types and line numbers |

### `DiffResult`

| Property | Type | Description |
|----------|------|-------------|
| `Lines` | `IReadOnlyList<DiffLine>` | The individual diff lines |
| `AddedCount` | `int` | Number of added lines |
| `RemovedCount` | `int` | Number of removed lines |
| `UnchangedCount` | `int` | Number of unchanged lines |
| `HasChanges` | `bool` | Whether the texts differ |
| `Statistics` | `DiffStatistics` | Detailed diff statistics |

### `DiffLine`

| Property | Type | Description |
|----------|------|-------------|
| `Type` | `DiffLineType` | `Added`, `Removed`, or `Unchanged` |
| `Content` | `string` | The text content of the line |
| `OldLineNumber` | `int?` | 1-based line number in the old text |
| `NewLineNumber` | `int?` | 1-based line number in the new text |

### `DiffStatistics`

| Property | Type | Description |
|----------|------|-------------|
| `Additions` | `int` | Number of added lines |
| `Deletions` | `int` | Number of deleted lines |
| `Modifications` | `int` | Number of modified lines (paired remove/add sequences) |
| `ChangePercentage` | `double` | Percentage of lines that changed (0.0 to 100.0) |

### `DiffOptions`

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IgnoreWhitespace` | `bool` | `false` | Ignore leading and trailing whitespace |
| `IgnoreCase` | `bool` | `false` | Ignore case differences |
| `IgnoreBlankLines` | `bool` | `false` | Skip blank lines when comparing |
| `Mode` | `DiffMode` | `Line` | Diff granularity: `Line`, `Word`, or `Character` |

### `InlineDiff`

| Method | Description |
|--------|-------------|
| `ComputeCharacterDiff(oldText, newText)` | Character-level diff returning `InlineChange` segments |
| `ComputeWordDiff(oldText, newText)` | Word-level diff returning `InlineChange` segments |

### `InlineChange`

| Property | Type | Description |
|----------|------|-------------|
| `Type` | `ChangeType` | `Equal`, `Insert`, or `Delete` |
| `Text` | `string` | The text content of the segment |

### `DiffMode`

| Value | Description |
|-------|-------------|
| `Line` | Compare text line by line |
| `Word` | Compare text word by word |
| `Character` | Compare text character by character |

## Development

```bash
dotnet build src/Philiprehberger.TextDiff.csproj --configuration Release
```

## Support

If you find this project useful:

⭐ [Star the repo](https://github.com/philiprehberger/dotnet-text-diff)

🐛 [Report issues](https://github.com/philiprehberger/dotnet-text-diff/issues?q=is%3Aissue+is%3Aopen+label%3Abug)

💡 [Suggest features](https://github.com/philiprehberger/dotnet-text-diff/issues?q=is%3Aissue+is%3Aopen+label%3Aenhancement)

❤️ [Sponsor development](https://github.com/sponsors/philiprehberger)

🌐 [All Open Source Projects](https://philiprehberger.com/open-source-packages)

💻 [GitHub Profile](https://github.com/philiprehberger)

🔗 [LinkedIn Profile](https://www.linkedin.com/in/philiprehberger)

## License

[MIT](LICENSE)
