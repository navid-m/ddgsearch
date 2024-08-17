namespace DuckDuckGoSearch.Tests
{
    public class SearchTest
    {
        [Test]
        public void SearchAsyncTest()
        {
            var searchResults = DuckDuckGoSearch.SearchAsync("Pizza").Result;
            Console.WriteLine($"Got {searchResults.Count} results:\n");
            foreach (var result in searchResults)
            {
                Console.WriteLine(result.Title);
            }
            Assert.That(searchResults, Is.Not.Empty);
        }

        [Test]
        public void SearchAsyncAllAttribsTest()
        {
            foreach (var result in DuckDuckGoSearch.SearchAsync("Who cares").Result)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(result.Title, Is.Not.Null);
                    Assert.That(result.Link, Is.Not.Null);
                });
            }
        }
    }
}
