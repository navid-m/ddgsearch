# DuckDuckGoSearch

Library to retrieve results from duckduckgo, given some string query.

Each returned SearchResult object contains the title, description, and URL.

## Example Usage

```csharp
var searchResults = await DDGClient.SearchAsync("Some query");
Console.WriteLine($"Got {searchResults.Count} results:");
foreach (var result in searchResults)
{
    Console.WriteLine(result.Title);
}
```
