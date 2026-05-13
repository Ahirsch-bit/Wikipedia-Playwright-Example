using Microsoft.Playwright;

namespace WikipediaAutomationProject.Pages;

public class TestAutomationWikipdediaPage: BasePage
{
    private ILocator TestDrivenDevelopmentHeader =>
        Page.Locator("id=Test-driven_development");

    private ILocator TestDrivenDevelopmentBody =>
        TestDrivenDevelopmentHeader.Locator("xpath=following::p[1]");
    
    public TestAutomationWikipdediaPage(IPage page) : base(page)
    {
    }

    public async Task<string> GetTDDSectionText()
    {
        var headerText = await TestDrivenDevelopmentHeader.InnerTextAsync();
        var bodyText = await TestDrivenDevelopmentBody.InnerTextAsync();
        return $"{headerText} {bodyText}";
    }
}