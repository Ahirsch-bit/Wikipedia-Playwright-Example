using Microsoft.Playwright;

namespace WikipediaAutomationProject.Pages;

public abstract class BasePage
{
    protected readonly IPage Page;

    public BasePage(IPage page)
    {
        Page = page;
    }
    
    public async Task GoToAsync(string url)
    {
        await Page.GotoAsync(url);
    }
}