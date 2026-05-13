using HtmlAgilityPack;
using Newtonsoft.Json;

namespace WikipediaAutomationProject.Api;

public class WikipediaApiClient
{
    private HttpClient _httpClient;

    public WikipediaApiClient()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://en.wikipedia.org/w/api.php")
        };
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "MyAppName/1.0 (myemail@example.com)");
    }
    
    public async Task<string> GetPageContentAsync()
    {
        var response = await _httpClient.GetAsync($"?action=parse&format=json&page=Test_automation&prop=text&section=8");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var contract = JsonConvert.DeserializeObject<WikipediaContract>(content);
        return ExtractVisibleText(contract.Parse.Text.Asterisk);
    }
    
    private string ExtractVisibleText(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // Remove unwanted nodes
        doc.DocumentNode.SelectNodes("//script|//style|//sup|//ol|//div[@class='mw-references-wrap']")
            .ToList()
            .ForEach(n => n.Remove());

        var text = doc.DocumentNode.InnerText;

        return HtmlEntity.DeEntitize(text);
    }
}