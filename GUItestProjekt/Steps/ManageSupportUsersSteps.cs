using Microsoft.Playwright;
using TechTalk.SpecFlow;
using Xunit;
using System.Threading.Tasks;
using System;

namespace GUItestProjekt.Steps
{
  [Binding]
  [Scope(Feature = "Manage Support Users")]
  public class ManageSupportUsersSteps
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

    [When(@"I navigate to the manage support users page")]
    public async Task WhenINavigateToTheManageSupportUsersPage()
    {
      // Navigera till admin panelen
      await _page.ClickAsync("text=Admin");

      // Vänta på att admin sidan laddas
      await _page.WaitForSelectorAsync("text=Administratörspanel", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });

      // Klicka på alternativet för att hantera supportanvändare
      await _page.ClickAsync("text=Hantera kundtjänstmedarbetare");

      // Vänta på att sidan laddas - huvudrubriken innehåller "Hantera kundtjänstmedarbetare för [företagsnamn]"
      await _page.WaitForSelectorAsync("h2:has-text('Hantera kundtjänstmedarbetare för')", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });
    }

    [Then(@"I should see the list of existing support users")]
    public async Task ThenIShouldSeeTheListOfExistingSupportUsers()
    {
      // Vänta på att tabellen med användare laddas
      await _page.WaitForSelectorAsync(".admin-table", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });

      // Kontrollera att tabellen har data eller ett meddelande om att den är tom
      bool hasTable = await _page.IsVisibleAsync(".admin-table");
      bool hasEmptyMessage = await _page.IsVisibleAsync("text=Inga kundtjänstmedarbetare hittades");

      // Antingen ska tabellen vara synlig eller ett tomt meddelande visas
      Assert.True(hasTable || hasEmptyMessage, "Varken tabellen eller ett tomt meddelande visas");

      if (hasTable)
      {
        // Kontrollera att tabellen har rätt kolumner baserat på den faktiska JSX-filen
        bool hasUsernameColumn = await _page.IsVisibleAsync("th:has-text('Användarnamn')");
        bool hasEmailColumn = await _page.IsVisibleAsync("th:has-text('Email')");
        bool hasCompanyColumn = await _page.IsVisibleAsync("th:has-text('Företag')");
        bool hasActionsColumn = await _page.IsVisibleAsync("th:has-text('Åtgärder')");

        Assert.True(hasUsernameColumn, "Användarnamn-kolumnen saknas");
        Assert.True(hasEmailColumn, "Email-kolumnen saknas");
        Assert.True(hasCompanyColumn, "Företag-kolumnen saknas");
        Assert.True(hasActionsColumn, "Åtgärder-kolumnen saknas");
      }
    }

    [Then(@"I should see user management options")]
    public async Task ThenIShouldSeeUserManagementOptions()
    {
      // Kontrollera att det finns knappar för att redigera och ta bort användare
      // Vi hittar dessa knappar endast om det finns användare i tabellen
      bool hasTable = await _page.IsVisibleAsync(".admin-table");
      bool hasUsersRows = await _page.IsVisibleAsync(".admin-table tbody tr");

      if (hasTable && hasUsersRows)
      {
        bool hasEditButton = await _page.IsVisibleAsync(".EditButton-Table") ||
                            await _page.IsVisibleAsync("button:has-text('Redigera')");

        bool hasDeleteButton = await _page.IsVisibleAsync(".DeleteButton-Table") ||
                              await _page.IsVisibleAsync("button:has-text('Ta bort')");

        Assert.True(hasEditButton, "Redigera-knappen hittades inte");
        Assert.True(hasDeleteButton, "Ta bort-knappen hittades inte");
      }

      // Verifiera att tillbakaknappen finns oavsett om det finns användare eller inte
      bool hasBackButton = await _page.IsVisibleAsync("button:has-text('Tillbaka till menyn')");
      Assert.True(hasBackButton, "Tillbaka-knappen hittades inte");

      // Klicka på tillbakaknappen för att återgå till admin-panelen
      await _page.ClickAsync("button:has-text('Tillbaka till menyn')");

      // Verifiera att vi kommer tillbaka till admin-panelen
      await _page.WaitForSelectorAsync("text=Administratörspanel", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });
    }
  }
}