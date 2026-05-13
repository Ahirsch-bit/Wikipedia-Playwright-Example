namespace WikipediaAutomationProject.Services;

public class WordCounter
{
    public Dictionary<string, int> CountWords(string text)
    {
        var words = text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        return words.GroupBy(word => word)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}