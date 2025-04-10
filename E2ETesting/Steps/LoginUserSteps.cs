using Microsoft.Playwright;
using TechTalk.SpecFlow;
using Xunit;

namespace E2ETesting.Steps;

[Binding]
public class LoginUserSteps
{
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IBrowserContext _context;
    private IPage _page;

    [BeforeScenario]
    public async Task Setup()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = false, SlowMo = 300 });
        _context = await _browser.NewContextAsync();
        _page = await _context.NewPageAsync();
    }

    [AfterScenario]
    public async Task Teardown()
    {
        await _browser.CloseAsync();
        _playwright.Dispose();
    }

    [Given("I am on the login page")]
    public async Task GivenIAmOnTheLoginPage()
    {
        await _page.GotoAsync("https://localhost:7086/Identity/Account/Login");

    }

    [When(@"I fill in the email field with ""(.*)""")]
    public async Task WhenIFillInTheEmailFieldWith(string email)
    {
        await _page.FillAsync("input[name='Input.Email']", email);
    }

    [When(@"I fill in the password field with ""(.*)""")]
    public async Task WhenIFillInThePasswordFieldWith(string password)
    {
        await _page.FillAsync("input[name='Input.Password']", password);
    }
    
    [When(@"I click the login button")]
    public async Task WhenIClickTheLoginButton()
    {
        await _page.ClickAsync("button[type='submit']");
    }
    [Then(@"I should be redirected to the home page")]
    public async Task ThenIShouldBeRedirectedToTheHomePage()
    {
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        var url = _page.Url;
        Assert.Contains("/Home", url);
    }
}


