using Microsoft.Playwright;
using TechTalk.SpecFlow;
using Xunit;
using System.Threading.Tasks;
using System;

namespace GUItestProjekt.Steps
{
  [Binding]
  [Scope(Feature = "Support User Login")]
  public class SupportLoginSteps
  {
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IBrowserContext _context;
    private IPage _page;

    [BeforeScenario]
    public async Task Setup()
    {
      _playwright = await Playwright.CreateAsync();
      _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
      {
        Headless = false,
        SlowMo = 200
      });
      _context = await _browser.NewContextAsync();
      _page = await _context.NewPageAsync();
    }

    [AfterScenario]
    public async Task Teardown()
    {
      await _browser.CloseAsync();
      _playwright.Dispose();
    }

    [Given(@"I am on the support login page")]
    public async Task GivenIAmOnTheSupportLoginPage()
    {
      await _page.GotoAsync("http://localhost:3000/signin");
      await _page.WaitForSelectorAsync("text=Logga in", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });
    }

    [When(@"I enter support user credentials")]
    public async Task WhenIEnterSupportUserCredentials()
    {
      // Använd användarnamn och lösenord för en support användare
      await _page.FillAsync("input[name='username']", "support1");
      await _page.FillAsync("input[name='password']", "testpostmanflowsupport123");
    }

    [When(@"I click the support login button")]
    public async Task WhenIClickTheSupportLoginButton()
    {
      await _page.ClickAsync("button.SigninButton-signin");
    }

    [Then(@"I should be logged in as support user")]
    public async Task ThenIShouldBeLoggedInAsSupportUser()
    {
      // Vänta på omdirigeringen efter inloggning
      await _page.WaitForURLAsync("http://localhost:3000/homes", new PageWaitForURLOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });
    }

    [Then(@"I should see the support dashboard")]
    public async Task ThenIShouldSeeTheSupportDashboard()
    {
      // Verifiera att Dashboard-element visas
      await _page.WaitForSelectorAsync(".background-hela-sidan", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });

      // Kontrollera att sidmenyalternativ för support användare visas
      bool hasNavigation = await _page.IsVisibleAsync("text=Dashboard");
      bool hasTicketsMenu = await _page.IsVisibleAsync("text=Ärenden");

      Assert.True(hasNavigation, "Dashboard-navigation hittades inte");
      Assert.True(hasTicketsMenu, "Ärenden-menyn hittades inte");
    }
  }
}