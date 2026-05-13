using Newtonsoft.Json;

namespace WikipediaAutomationProject.Api;

public class WikipediaContract
{
    public Parse Parse { get; set; }
}

public class Parse
{
    public string Title { get; set; }
    public int Pageid { get; set; }
    public Text Text { get; set; }
}

public class Text
{
    [JsonProperty("*")]
    public string Asterisk {get; set; }
}