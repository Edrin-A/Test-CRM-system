using Microsoft.Playwright;
using TechTalk.SpecFlow;
using Xunit;
using System.Threading.Tasks;
using System;

namespace GUItestProjekt.Steps
{
  [Binding]
  [Scope(Feature = "Ticket Status Update")] // så inloggning inte krockar med de andra metoderna
  public class TicketStatusUpdateSteps
  {
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IBrowserContext _context;
    private IPage _page;
    private string _originalStatus;
    private int _firstTicketIndex = 0;

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
      // Återställ ärende till ursprunglig status om möjligt
      try
      {
        if (!string.IsNullOrEmpty(_originalStatus))
        {
          var statusSelect = await _page.QuerySelectorAllAsync(".ticket-item .status-select");
          if (statusSelect.Count > _firstTicketIndex)
          {
            await statusSelect[_firstTicketIndex].SelectOptionAsync(_originalStatus);
            await _page.WaitForTimeoutAsync(500);
          }
        }
      }
      catch
      {
        // Ignorera fel vid återställning
      }

      await _browser.CloseAsync();
      _playwright.Dispose();
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

      // Kontrollera att det finns ärenden
      var tickets = await _page.QuerySelectorAllAsync(".ticket-item");
      Assert.True(tickets.Count > 0, "Inga ärenden hittades");

      // Spara originalstatus från första ärendet
      var statusSelects = await _page.QuerySelectorAllAsync(".ticket-item .status-select");
      _originalStatus = await statusSelects[_firstTicketIndex].EvaluateAsync<string>("el => el.value");
      Console.WriteLine($"Ursprunglig status: {_originalStatus}");
    }

    [When(@"I change the status of the first ticket to ""(.*)""")]
    public async Task WhenIChangeTheStatusOfTheFirstTicketTo(string newStatus)
    {
      // Välj ett ärende för uppdatering (använder det första för enkelhetens skull)
      var statusSelects = await _page.QuerySelectorAllAsync(".ticket-item .status-select");
      Assert.True(statusSelects.Count > 0, "Inga ärenden med statusval hittades");

      // Ändra status
      await statusSelects[_firstTicketIndex].SelectOptionAsync(newStatus);

      // Vänta på att API anrop ska slutföras
      await _page.WaitForTimeoutAsync(1000);
    }

    [Then(@"the ticket status should be updated to ""(.*)""")]
    public async Task ThenTheTicketStatusShouldBeUpdatedTo(string expectedStatus)
    {
      // Ladda om sidan för att verifiera att statusen verkligen uppdaterades i databasen
      await _page.ReloadAsync();
      await _page.WaitForSelectorAsync(".tickets-container", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });

      // Hämta status igen från samma ärende
      var statusSelects = await _page.QuerySelectorAllAsync(".ticket-item .status-select");
      var currentStatus = await statusSelects[_firstTicketIndex].EvaluateAsync<string>("el => el.value");

      // Verifiera att statusen har ändrats
      Assert.Equal(expectedStatus, currentStatus);
    }
  }
}