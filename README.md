# Philiprehberger.TextDiff

[![CI](https://github.com/philiprehberger/dotnet-text-diff/actions/workflows/ci.yml/badge.svg)](https://github.com/philiprehberger/dotnet-text-diff/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Philiprehberger.TextDiff.svg)](https://www.nuget.org/packages/Philiprehberger.TextDiff)
[![License](https://img.shields.io/github/license/philiprehberger/dotnet-text-diff)](LICENSE)
[![Sponsor](https://img.shields.io/badge/sponsor-GitHub%20Sponsors-ec6cb9)](https://github.com/sponsors/philiprehberger)

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

### Compare Strings

```csharp
using Philiprehberger.TextDiff;

var result = Diff.Compare("line one\nline two\nline three", "line one\nline 2\nline three");

foreach (var line in result.Lines)
{
    var prefix = line.Type switch
    {
        DiffLineType.Added => "+",
        DiffLineType.Removed => "-",
        _ => " "
    };
    Console.WriteLine($"{prefix} {line.Content}");
}
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

### Structured DiffResult

```csharp
using Philiprehberger.TextDiff;

var result = Diff.Compare("a\nb\nc", "a\nc");

Console.WriteLine($"Has changes: {result.HasChanges}");
Console.WriteLine($"Added: {result.AddedCount}");
Console.WriteLine($"Removed: {result.RemovedCount}");
Console.WriteLine($"Unchanged: {result.UnchangedCount}");
```

## API

### `Diff`

| Method | Description |
|--------|-------------|
| `Compare(oldText, newText)` | Compares two texts and returns a `DiffResult` with lines and statistics |
| `Unified(oldText, newText, oldLabel?, newLabel?, context?)` | Produces a unified diff string with `---`, `+++`, and `@@` headers |
| `Lines(oldText, newText)` | Returns a list of `DiffLine` entries with change types and line numbers |

### `DiffResult`

| Property | Type | Description |
|----------|------|-------------|
| `Lines` | `IReadOnlyList<DiffLine>` | The individual diff lines |
| `AddedCount` | `int` | Number of added lines |
| `RemovedCount` | `int` | Number of removed lines |
| `UnchangedCount` | `int` | Number of unchanged lines |
| `HasChanges` | `bool` | Whether the texts differ |

### `DiffLine`

| Property | Type | Description |
|----------|------|-------------|
| `Type` | `DiffLineType` | `Added`, `Removed`, or `Unchanged` |
| `Content` | `string` | The text content of the line |
| `OldLineNumber` | `int?` | 1-based line number in the old text |
| `NewLineNumber` | `int?` | 1-based line number in the new text |

## Development

```bash
dotnet build src/Philiprehberger.TextDiff.csproj --configuration Release
```

## License

MIT
