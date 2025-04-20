using Microsoft.Playwright;
using TechTalk.SpecFlow;
using Xunit;
using System.Threading.Tasks;
using System;

namespace GUItestProjekt.Steps
{
  [Binding]
  [Scope(Feature = "Admin Panel Verification")]
  public class AdminPanelSteps
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
        Headless = true,
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

    [Given(@"I am logged in as an admin user")]
    public async Task GivenIAmLoggedInAsAnAdminUser()
    {
      // Navigera till inloggningssidan
      await _page.GotoAsync("http://localhost:3000/signin");
      await _page.WaitForSelectorAsync("text=Logga in", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });

      // Logga in som admin
      await _page.FillAsync("input[name='username']", "admin1");
      await _page.FillAsync("input[name='password']", "admin1");
      await _page.ClickAsync("button.SigninButton-signin");

      // Vänta på omdirigeringen efter inloggning
      await _page.WaitForURLAsync("http://localhost:3000/homes", new PageWaitForURLOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });
      await _page.WaitForTimeoutAsync(1000); // Vänta lite extra för att säkerställa att allt laddas klart
    }

    [When(@"I navigate to the admin panel")]
    public async Task WhenINavigateToTheAdminPanel()
    {
      // Klicka på Admin länken
      await _page.ClickAsync("text=Admin");

      // Vänta på att admin sidan laddas
      await _page.WaitForSelectorAsync("text=Administratörspanel", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });
    }

    [Then(@"I should see all admin panel options")]
    public async Task ThenIShouldSeeAllAdminPanelOptions()
    {
      // Verifiera att alla förväntade alternativ finns i admin panelen
      var menuOptions = new[]
      {
        "Lägg till kundtjänstmedarbetare",
        "Hantera kundtjänstmedarbetare",
        "Hantera produkter/tjänster"
      };

      foreach (var option in menuOptions)
      {
        bool isVisible = await _page.IsVisibleAsync($"text={option}");
        Assert.True(isVisible, $"Alternativet '{option}' hittades inte i admin-panelen");
      }

      // Verifiera att Tillbaka knappen finns
      bool hasBackButton = await _page.IsVisibleAsync("text=Tillbaka till huvudmenyn");
      Assert.True(hasBackButton, "Tillbaka-knappen hittades inte");
    }

    [Then(@"I should be able to navigate between admin functions")]
    public async Task ThenIShouldBeAbleToNavigateBetweenAdminFunctions()
    {
      // Testa navigering till "Lägg till kundtjänstmedarbetare"
      await _page.ClickAsync("text=Lägg till kundtjänstmedarbetare");
      await _page.WaitForSelectorAsync("text=Lägg till ny kundtjänstmedarbetare", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });

      // Testa att gå tillbaka till huvudmenyn
      await _page.ClickAsync("text=Tillbaka till menyn");
      await _page.WaitForSelectorAsync("text=Administratörspanel", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });

      // Testa navigering till "Hantera kundtjänstmedarbetare"
      await _page.ClickAsync("text=Hantera kundtjänstmedarbetare");
      await _page.WaitForSelectorAsync("text=Hantera kundtjänstmedarbetare", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });

      // Testa att gå tillbaka till huvudmenyn
      await _page.ClickAsync("text=Tillbaka till menyn");
      await _page.WaitForSelectorAsync("text=Administratörspanel", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });

      // Testa navigering till "Hantera produkter/tjänster"
      await _page.ClickAsync("text=Hantera produkter/tjänster");
      await _page.WaitForSelectorAsync("text=Hantera produkter", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });

      // Testa att gå tillbaka till huvudmenyn
      await _page.ClickAsync("text=Tillbaka till menyn");
      await _page.WaitForSelectorAsync("text=Administratörspanel", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });

      // Testa att gå tillbaka till Dashboard från admin-panelen
      await _page.ClickAsync("text=Tillbaka till huvudmenyn");
      await _page.WaitForURLAsync("http://localhost:3000/homes", new PageWaitForURLOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });
    }
  }
}