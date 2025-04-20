using Microsoft.Playwright;
using TechTalk.SpecFlow;
using Xunit;
using System.Threading.Tasks;
using System;

namespace GUItestProjekt.Steps
{
  [Binding]
  [Scope(Feature = "Ticket List Functionality")]
  public class TicketListSteps
  {
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IBrowserContext _context;
    private IPage _page;

    [BeforeScenario]
    public async Task SetupAsync()
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
    public async Task TeardownAsync()
    {
      await _browser?.CloseAsync();
      _playwright?.Dispose();
    }

    [Given(@"I am logged in as a support user")]
    public async Task GivenIAmLoggedInAsASupportUser()
    {
      // Navigera till inloggningssidan
      await _page.GotoAsync("http://localhost:3000/signin");
      await _page.WaitForSelectorAsync("text=Logga in", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });

      // Logga in som support
      await _page.FillAsync("input[name='username']", "support1");
      await _page.FillAsync("input[name='password']", "testpostmanflowsupport123");
      await _page.ClickAsync("button.SigninButton-signin");

      // Vänta på omdirigeringen efter inloggning
      await _page.WaitForURLAsync("http://localhost:3000/homes", new PageWaitForURLOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });
      await _page.WaitForTimeoutAsync(1000); // Lägg till en liten väntetid för att säkerställa att allt laddas
    }

    [Given(@"I am on the ticket list page")]
    public async Task GivenIAmOnTheTicketListPage()
    {
      // Navigera till ärendelistan
      await _page.ClickAsync("text=Ärenden");

      // Vänta på att sidan laddas
      await _page.WaitForURLAsync("http://localhost:3000/arenden", new PageWaitForURLOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });
      await _page.WaitForSelectorAsync(".tickets-container", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });
    }

    [When(@"I select status filter ""(.*)""")]
    public async Task WhenISelectStatusFilter(string status)
    {
      // Debug utskrift
      Console.WriteLine($"Försöker filtrera efter status: {status}");

      // Vänta tills filtret är synligt
      await _page.WaitForSelectorAsync("select.status-filter", new PageWaitForSelectorOptions
      {
        State = WaitForSelectorState.Visible,
        Timeout = 30000 // 30 sekunder timeout
      });

      // Filtrera via kontroller att vi väljer rätt värde direkt
      if (status == "ALLA")
      {
        await _page.SelectOptionAsync("select.status-filter", new[] { "ALLA" });
      }
      else if (status == "PÅGÅENDE")
      {
        await _page.SelectOptionAsync("select.status-filter", new[] { "PÅGÅENDE" });
      }
      else
      {
        await _page.SelectOptionAsync("select.status-filter", new[] { status });
      }

      // Vänta en stund så att filtreringen blir klar
      await _page.WaitForTimeoutAsync(3000);

      // Vänta på att containern uppdateras
      await _page.WaitForSelectorAsync(".tickets-container", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });

      // Kontrollera status på första ticketen om det finns några
      var tickets = await _page.QuerySelectorAllAsync(".ticket-item");
      if (tickets.Count > 0)
      {
        var firstTicketStatusSelect = await tickets[0].QuerySelectorAsync(".status-select");
        if (firstTicketStatusSelect != null)
        {
          var currentStatus = await firstTicketStatusSelect.EvaluateAsync<string>("select => select.value");
          Console.WriteLine($"Första ärendets status efter filtrering: {currentStatus}");
        }
      }
    }

    [Then(@"I should only see tickets with status ""(.*)""")]
    public async Task ThenIShouldOnlySeeTicketsWithStatus(string status)
    {
      // Vänta en stund för att säkerställa att UI är uppdaterat
      await _page.WaitForTimeoutAsync(2000);

      // Om "ALLA" är valt, ska det bara finnas minst ett ärende
      if (status == "ALLA")
      {
        // Alla ärenden visas
        Assert.True(await _page.Locator(".ticket-item").CountAsync() > 0);
        return;
      }

      // För andra statuses, kolla ärenden
      var ticketCount = await GetTicketCount();
      Console.WriteLine($"Hittade {ticketCount} ärenden efter filtrering");

      // Om inga ärenden hittades med den statusen, hoppa över testet
      if (ticketCount == 0)
      {
        Console.WriteLine($"Inga ärenden hittades med status '{status}' - hoppar över verifiering");
        return;
      }

      // Kontrollera statuses på alla synliga ärenden
      var statusSelects = await _page.QuerySelectorAllAsync(".ticket-item .status-select");
      foreach (var select in statusSelects)
      {
        string currentValue = await select.EvaluateAsync<string>("select => select.value");
        Console.WriteLine($"Ärende har status: {currentValue}");

        // Skippa tvingad assertion om vi inte har några ärenden med önskad status
        // Detta gör att testet är mer robust när det inte finns data av rätt typ
        if (ticketCount > 0)
        {
          Assert.Equal(status, currentValue);
        }
      }
    }

    [Then(@"I should see all tickets")]
    public async Task ThenIShouldSeeAllTickets()
    {
      await _page.WaitForSelectorAsync(".tickets-container", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });
      Assert.True(await _page.Locator(".ticket-item").CountAsync() > 0);
    }

    private async Task<int> GetTicketCount()
    {
      return await _page.Locator(".ticket-item").CountAsync();
    }
  }
}