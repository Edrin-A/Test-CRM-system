using Microsoft.Playwright;
using TechTalk.SpecFlow;
using Xunit;

namespace GUItestProjekt.Steps
{
  [Binding]
  public class LoginSteps
  {
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IPage _page;

    [BeforeScenario]
    public async Task Setup()
    {
      _playwright = await Playwright.CreateAsync();
      _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = false, SlowMo = 200 });
      _page = await _browser.NewPageAsync();
    }

    [AfterScenario]
    public async Task Teardown()
    {
      await _browser.CloseAsync();
      _playwright.Dispose();
    }

    [Given(@"I am on the login page")]
    public async Task GivenIAmOnTheLoginPage()
    {
      await _page.GotoAsync("http://localhost:3000/signin");
      await _page.WaitForSelectorAsync("text=Logga in");
    }

    [When(@"I enter username ""(.*)"" and password ""(.*)""")]
    public async Task WhenIEnterUsernameAndPassword(string username, string password)
    {
      await _page.FillAsync("input[name='username']", username);
      await _page.FillAsync("input[name='password']", password);
    }

    [When(@"I click the login button")]
    public async Task WhenIClickTheLoginButton()
    {
      await _page.ClickAsync("button.SigninButton-signin");
    }

    [Then(@"I should be logged in successfully")]
    public async Task ThenIShouldBeLoggedInSuccessfully()
    {
      await _page.WaitForURLAsync("http://localhost:3000/homes");
      var isLoggedIn = await _page.IsVisibleAsync("text=Ärenden");
      Assert.True(isLoggedIn);
    }
  }
}