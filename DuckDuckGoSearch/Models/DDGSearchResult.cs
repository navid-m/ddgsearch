namespace DuckDuckGoSearch.Models
{
    public class DDGSearchResult
    {
        public required string Link { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
    }
}
