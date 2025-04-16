using Microsoft.Playwright;
using TechTalk.SpecFlow;
using Xunit;
using System.Threading.Tasks;
using System;

namespace GUItestProjekt.Steps
{
  [Binding]
  [Scope(Feature = "Admin User Login")]
  public class AdminLoginSteps
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

    [Given(@"I am on the admin login page")]
    public async Task GivenIAmOnTheAdminLoginPage()
    {
      await _page.GotoAsync("http://localhost:3000/signin");
      await _page.WaitForSelectorAsync("text=Logga in", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });
    }

    [When(@"I enter admin user credentials")]
    public async Task WhenIEnterAdminUserCredentials()
    {
      // Använd användarnamn och lösenord för en admin-användare
      await _page.FillAsync("input[name='username']", "admin1");
      await _page.FillAsync("input[name='password']", "admin1");
    }

    [When(@"I click the admin login button")]
    public async Task WhenIClickTheAdminLoginButton()
    {
      await _page.ClickAsync("button.SigninButton-signin");
    }

    [Then(@"I should be logged in as admin user")]
    public async Task ThenIShouldBeLoggedInAsAdminUser()
    {
      // Vänta på omdirigeringen efter inloggning
      await _page.WaitForURLAsync("http://localhost:3000/homes", new PageWaitForURLOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });
    }

    [Then(@"I should see the admin dashboard")]
    public async Task ThenIShouldSeeTheAdminDashboard()
    {
      // Verifiera att dashboard element visas
      await _page.WaitForSelectorAsync(".background-hela-sidan", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });

      // Kontrollera att sidmenyalternativ för admin användare visas
      bool hasNavigation = await _page.IsVisibleAsync("text=Dashboard");
      bool hasAdminPanel = await _page.IsVisibleAsync("text=Admin");

      Assert.True(hasNavigation, "Dashboard-navigation hittades inte");
      Assert.True(hasAdminPanel, "Admin-panelen hittades inte");

      // Verifiera att admin användaren har rätt behörighet genom att klicka på Admin knappen
      await _page.ClickAsync("text=Admin");

      // Vänta på att admins idan laddas och att vi ser administratörspanelen
      await _page.WaitForSelectorAsync("text=Administratörspanel", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });

      // Verifiera att vi ser menyalternativen i admin-panelen
      bool hasAddSupportOption = await _page.IsVisibleAsync("text=Lägg till kundtjänstmedarbetare");
      bool hasManageSupportOption = await _page.IsVisibleAsync("text=Hantera kundtjänstmedarbetare");
      bool hasManageProductsOption = await _page.IsVisibleAsync("text=Hantera produkter/tjänster");

      Assert.True(hasAddSupportOption, "Alternativ för att lägga till kundtjänstmedarbetare hittades inte");
      Assert.True(hasManageSupportOption, "Alternativ för att hantera kundtjänstmedarbetare hittades inte");
      Assert.True(hasManageProductsOption, "Alternativ för att hantera produkter/tjänster hittades inte");
    }
  }
}