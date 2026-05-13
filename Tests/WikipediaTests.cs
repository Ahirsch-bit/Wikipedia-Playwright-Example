using WikipediaAutomationProject.Api;
using WikipediaAutomationProject.Pages;
using WikipediaAutomationProject.Services;

namespace WikipediaAutomationProject.Tests;

public class WikipediaTests:BaseTest
{
    [Test]
    public async Task TestWordCountAsync()
    {
        var wikipediaPage = new TestAutomationWikipdediaPage(Page);
        await wikipediaPage.GoToAsync("wiki/Test_automation");
        var content = await wikipediaPage.GetTDDSectionText();
        
        var wordCounter = new WordCounter();
  
        var wordCounts = wordCounter.CountWords(TextNormalizer.NormalizeText(content));
        foreach (var word in wordCounts)
        {
            Console.WriteLine($"The word '{word.Key}' appears {word.Value} times.");
        }

        var client = new WikipediaApiClient();
        var apiContent = await client.GetPageContentAsync();
        var apiWordCounts = wordCounter.CountWords(TextNormalizer.NormalizeText(apiContent));
        foreach (var word in apiWordCounts)        {
            Console.WriteLine($"[API] The word '{word.Key}' appears {word.Value} times.");
        }
        
        Assert.That(apiWordCounts, Is.EquivalentTo(wordCounts), "The word counts from the UI and API should match.");
    }
}