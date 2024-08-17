using DuckDuckGoSearch.Models;
using HtmlAgilityPack;
using System.Collections.Concurrent;

namespace DuckDuckGoSearch;

public static class DuckDuckGoSearch
{
    private const string BASE_URL = "https://duckduckgo.com/html";
    private static readonly HttpClient client = new();

    static DuckDuckGoSearch()
    {
        client.DefaultRequestHeaders.UserAgent.ParseAdd(GetAgent());
    }

    public static async Task<SearchResults> SearchAsync(string query)
    {
        try
        {
            var response = await client.GetAsync($"{BASE_URL}?q={query}");
            if (response.IsSuccessStatusCode)
            {
                var html = await response.Content.ReadAsStringAsync();
                return ParseHtml(html);
            }
            else
            {
                throw new Exception(
                    $"Failed to retrieve search results. Status code: {response.StatusCode}"
                );
            }
        }
        catch (HttpRequestException e)
        {
            throw new Exception(
                $"Internet connection is required to query DDG.\nSpecifics:\n{e.Message}"
            );
        }
    }

    private static string RemoveGarbage(string uri) =>
        Uri.UnescapeDataString(uri.Replace("//duckduckgo.com/l/?uddg=", "").Split('&')[0]);

    private static SearchResults ParseHtml(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var results = new ConcurrentBag<SearchResult>();
        var nodes = doc.DocumentNode.SelectNodes("//div[@class='links_main links_deep result__body']");

        if (nodes != null)
        {
            Parallel.ForEach(nodes, result =>
            {
                var link = result.SelectSingleNode(".//a[@class='result__a']")?.GetAttributeValue("href", string.Empty);
                var title = result.SelectSingleNode(".//h2[@class='result__title']")?.InnerText?.Trim();
                if (title != null && title.Contains("Ad clicks are managed by Microsoft's ad network"))
                {
                    return;
                }
                if (!string.IsNullOrEmpty(link) && !string.IsNullOrEmpty(title))
                {
                    results.Add(
                        new SearchResult
                        {
                            Link = RemoveGarbage(link),
                            Title = title,
                            Description = result.SelectSingleNode(".//a[@class='result__snippet']")?
                                                .InnerText?
                                                .Trim()
                        }
                    );
                }
            });
        }

        var searchResults = new SearchResults();
        foreach (var result in results)
        {
            searchResults.Add(result);
        }

        if (searchResults.Count == 0)
        {
            throw new Exception("You got ratelimited, or there are no results for the provided query.");
        }
        return searchResults;
    }

    private static readonly List<string> AgentsList =
    [
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36",
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36",
        "Mozilla/5.0 (Linux; Android 10; SM-G975F) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Mobile Safari/537.36",
        "Mozilla/5.0 (iPhone; CPU iPhone OS 13_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
        "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko"
    ];

    private static string GetAgent() => AgentsList[new Random().Next(AgentsList.Count)];
}
