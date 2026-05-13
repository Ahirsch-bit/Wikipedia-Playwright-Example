using Microsoft.Playwright;

namespace WikipediaAutomationProject.Tests;

public abstract class BaseTest
{
    public IPage Page { get; private set; }
    public IBrowser Browser { get; private set; }

    [SetUp]
    public async Task InitializeAsync()
    {
        Page = await CreatePageAsync();
    }
    
    [TearDown]
    public async Task CleanupAsync()
    {
        await Browser.CloseAsync();
    }
    
    private async Task<IPage> CreatePageAsync()
    {
        var playwright = await Playwright.CreateAsync(); 
        Browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });
        var context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            BaseURL = "https://wikipedia.org"
        });
        return await context.NewPageAsync();
    }
}