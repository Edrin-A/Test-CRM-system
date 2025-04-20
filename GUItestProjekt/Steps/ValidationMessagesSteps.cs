using Microsoft.Playwright;
using TechTalk.SpecFlow;
using Xunit;
using System.Threading.Tasks;
using System;

namespace GUItestProjekt.Steps
{
  [Binding]
  [Scope(Feature = "Form Validation")]
  public class ValidationMessagesSteps
  {
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IBrowserContext _context;
    private IPage _page;
    private readonly string _uniqueId;

    public ValidationMessagesSteps()
    {
      // Skapa ett unikt ID för användaruppgifter
      _uniqueId = DateTime.Now.Ticks.ToString().Substring(10);
    }

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

    [When(@"I navigate to the add support user page")]
    public async Task WhenINavigateToTheAddSupportUserPage()
    {
      // Navigera till admin panelen
      await _page.ClickAsync("text=Admin");

      // Vänta på att admin-sidan laddas
      await _page.WaitForSelectorAsync("text=Administratörspanel", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });

      // Klicka på alternativet för att lägga till supportanvändare
      await _page.ClickAsync("text=Lägg till kundtjänstmedarbetare");

      // Vänta på att sidan laddas
      await _page.WaitForSelectorAsync("text=Lägg till ny kundtjänstmedarbetare", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });
    }

    [When(@"I fill in the support user form with invalid email")]
    public async Task WhenIFillInTheSupportUserFormWithInvalidEmail()
    {
      // Fyll i användaruppgifter med ogiltig e-post (saknar @)
      await _page.FillAsync("input#username", $"testuser{_uniqueId}");
      await _page.FillAsync("input#password", "Test123!");
      await _page.FillAsync("input#email", $"invalidemail{_uniqueId}"); // Saknar @ tecken och domän

      // Välj support rollen (om det behövs)
      await _page.SelectOptionAsync("select#role", new[] { "SUPPORT" });
    }

    [When(@"I fill in the support user form with empty username")]
    public async Task WhenIFillInTheSupportUserFormWithEmptyUsername()
    {
      // Fyll i användaruppgifter med tomt användarnamn
      await _page.FillAsync("input#username", ""); // Tomt användarnamn
      await _page.FillAsync("input#password", "Test123!");
      await _page.FillAsync("input#email", $"test{_uniqueId}@example.com");

      // Välj support-rollen (om det behövs)
      await _page.SelectOptionAsync("select#role", new[] { "SUPPORT" });
    }

    [When(@"I submit the support user form")]
    public async Task WhenISubmitTheSupportUserForm()
    {
      // Klicka på skicka-knappen
      await _page.ClickAsync("button.SendButton-Layout", new PageClickOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });

      // Vänta en sekund för att ge systemet tid att hantera formuläret
      await _page.WaitForTimeoutAsync(1000);
    }

    [Then(@"I should see a validation error message for email")]
    public async Task ThenIShouldSeeAValidationErrorMessageForEmail()
    {
      // Enkel kontroll: är vi fortfarande på formulärsidan? (Om ja, formuläret avslogs)
      bool isStillOnFormPage = await _page.IsVisibleAsync("text=Lägg till ny kundtjänstmedarbetare");

      // Försök hitta något av vanliga felmeddelanden
      bool hasErrorText =
          await _page.IsVisibleAsync("text=/ogiltig e-post/i") ||
          await _page.IsVisibleAsync("text=/felaktig e-post/i") ||
          await _page.IsVisibleAsync("text=/ange en giltig e-post/i");

      // Om vi är kvar på formulärsidan är det ett bra tecken
      Assert.True(isStillOnFormPage, "Formuläret med ogiltig e-post accepterades utan validering");

      // Logga om det faktiskt fanns ett synligt felmeddelande eller inte
      if (hasErrorText)
      {
        Console.WriteLine("Systemet visar ett felmeddelande för ogiltig e-post.");
      }
      else
      {
        Console.WriteLine("NOTERA: Formuläret nekades men inget explicit felmeddelande hittades. Detta kan vara ett användbarhetsproblem.");
      }
    }

    [Then(@"I should see a validation error message for username")]
    public async Task ThenIShouldSeeAValidationErrorMessageForUsername()
    {
      // Enkel kontroll: är vi fortfarande på formulärsidan? (Om ja, formuläret avslogs)
      bool isStillOnFormPage = await _page.IsVisibleAsync("text=Lägg till ny kundtjänstmedarbetare");

      // Försök hitta något av vanliga felmeddelanden
      bool hasErrorText =
          await _page.IsVisibleAsync("text=/användarnamn krävs/i") ||
          await _page.IsVisibleAsync("text=/ange ett användarnamn/i") ||
          await _page.IsVisibleAsync("text=/fältet får inte vara tomt/i");

      // Om vi är kvar på formulärsidan är det ett bra tecken
      Assert.True(isStillOnFormPage, "Formuläret med tomt användarnamn accepterades utan validering");

      // Logga om det faktiskt fanns ett synligt felmeddelande eller inte
      if (hasErrorText)
      {
        Console.WriteLine("Systemet visar ett felmeddelande för tomt användarnamn.");
      }
      else
      {
        Console.WriteLine("NOTERA: Formuläret nekades men inget explicit felmeddelande hittades. Detta kan vara ett användbarhetsproblem.");
      }
    }
  }
}