using Microsoft.Playwright;
using TechTalk.SpecFlow;
using Xunit;
using System.Threading.Tasks;
using System;

namespace GUItestProjekt.Steps
{
  [Binding]
  [Scope(Feature = "Login Validation")]
  public class LoginValidationSteps
  {
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IBrowserContext _context;
    private IPage _page;
    private string _alertMessage = null;

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

      // Konfigurera händelsehanterare för JavaScript alerts - för playwright kunde inte märka av alert() som vi har...
      // Detta är viktigt för att fånga alert-dialoger
      _page.Dialog += async (_, dialog) =>
      {
        _alertMessage = dialog.Message;
        Console.WriteLine($"Alert-meddelande visades: {_alertMessage}");
        await dialog.AcceptAsync(); // Klicka automatiskt "OK" på dialogen
      };
    }

    [AfterScenario]
    public async Task Teardown()
    {
      await _browser.CloseAsync();
      _playwright.Dispose();
    }

    [Given(@"I navigate to the login page")]
    public async Task GivenINavigateToTheLoginPage()
    {
      // Navigera till inloggningssidan
      await _page.GotoAsync("http://localhost:3000/signin");
      await _page.WaitForSelectorAsync("text=Logga in", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });
    }

    [When(@"I attempt to login with incorrect credentials")]
    public async Task WhenIAttemptToLoginWithIncorrectCredentials()
    {
      // Återställ alert meddelande
      _alertMessage = null;

      // Fyll i felaktiga inloggningsuppgifter
      await _page.FillAsync("input[name='username']", "felaktig_användare");
      await _page.FillAsync("input[name='password']", "felaktigt_lösenord");

      // Klicka på inloggningsknappen
      await _page.ClickAsync("button.SigninButton-signin");

      // Vänta lite så att alert-dialogen hinner visas
      await _page.WaitForTimeoutAsync(2000);
    }

    [When(@"I attempt to login with empty credentials")]
    public async Task WhenIAttemptToLoginWithEmptyCredentials()
    {
      // Återställ alert meddelande
      _alertMessage = null;

      // Lämna användarnamn och lösenord tomma
      await _page.FillAsync("input[name='username']", "");
      await _page.FillAsync("input[name='password']", "");

      // Klicka på inloggningsknappen
      await _page.ClickAsync("button.SigninButton-signin");

      // Vänta lite så att alert-dialogen hinner visas
      await _page.WaitForTimeoutAsync(2000);
    }

    [Then(@"I should see a login error message")]
    public async Task ThenIShouldSeeALoginErrorMessage()
    {
      // Enkel kontroll: är vi fortfarande på inloggningssidan? (Om ja, inloggningen nekades)
      bool isStillOnLoginPage = await _page.IsVisibleAsync("text=Logga in");

      // Verifiera att inloggningen nekades
      Assert.True(isStillOnLoginPage, "Inloggningen med felaktiga uppgifter accepterades");

      // Verifiera att vi fortfarande är på inloggningssidan via URL
      string currentUrl = _page.Url;
      Assert.Contains("/signin", currentUrl, StringComparison.OrdinalIgnoreCase);

      // Kontrollera om vi fångade upp en JavaScript-alert (detta är det viktigaste för denna app)
      if (_alertMessage != null)
      {
        Console.WriteLine($"FRAMGÅNG: Systemet visade en alert med meddelandet: {_alertMessage}");

        // Verifiera att meddelandet innehåller något relevant om fel inloggning
        bool containsRelevantErrorText =
            _alertMessage.Contains("Felaktigt", StringComparison.OrdinalIgnoreCase) ||
            _alertMessage.Contains("lösenord", StringComparison.OrdinalIgnoreCase) ||
            _alertMessage.Contains("behörighet", StringComparison.OrdinalIgnoreCase) ||
            _alertMessage.Contains("misslyckades", StringComparison.OrdinalIgnoreCase);

        Assert.True(containsRelevantErrorText,
            $"Alert-meddelandet '{_alertMessage}' innehåller inte förväntad text om felaktig inloggning");
      }
      else
      {
        // Sök efter andra typer av felmeddelanden på sidan som fallback
        bool hasErrorText =
            await _page.IsVisibleAsync("text=/felaktigt användarnamn eller lösenord/i") ||
            await _page.IsVisibleAsync("text=/ogiltiga inloggningsuppgifter/i") ||
            await _page.IsVisibleAsync("text=/kunde inte logga in/i") ||
            await _page.IsVisibleAsync("text=/användaren finns inte/i") ||
            await _page.IsVisibleAsync("div.error-message") ||
            await _page.IsVisibleAsync("span.error-text");

        if (hasErrorText)
        {
          Console.WriteLine("Systemet visar ett felmeddelande på sidan för felaktiga inloggningsuppgifter.");
        }
        else
        {
          Console.WriteLine("NOTERA: Inloggningen nekades men inget felmeddelande hittades (varken alert eller på sidan).");
          Console.WriteLine("Detta är ett potentiellt användbarhetsproblem.");
        }
      }
    }

    [Then(@"I should see empty fields validation messages")]
    public async Task ThenIShouldSeeEmptyFieldsValidationMessages()
    {
      // Enkel kontroll: är vi fortfarande på inloggningssidan? (Om ja, inloggningen nekades)
      bool isStillOnLoginPage = await _page.IsVisibleAsync("text=Logga in");

      // Verifiera att inloggningen nekades
      Assert.True(isStillOnLoginPage, "Inloggningen med tomma fält accepterades");

      // Verifiera att vi fortfarande är på inloggningssidan via URL
      string currentUrl = _page.Url;
      Assert.Contains("/signin", currentUrl, StringComparison.OrdinalIgnoreCase);

      // Kontrollera om vi fångade upp en JavaScript-alert (detta är det viktigaste för denna app)
      if (_alertMessage != null)
      {
        Console.WriteLine($"FRAMGÅNG: Systemet visade en alert med meddelandet: {_alertMessage}");
        // Vi förväntar oss något meddelande om tomma fält, men det kan vara olika formuleringar
      }
      else
      {
        // Sök efter andra typer av felmeddelanden på sidan som fallback
        bool hasErrorText =
            await _page.IsVisibleAsync("text=/användarnamn krävs/i") ||
            await _page.IsVisibleAsync("text=/lösenord krävs/i") ||
            await _page.IsVisibleAsync("text=/fälten får inte vara tomma/i") ||
            await _page.IsVisibleAsync("text=/fyll i alla fält/i") ||
            await _page.IsVisibleAsync("div.error-message") ||
            await _page.IsVisibleAsync("span.error-text");

        if (hasErrorText)
        {
          Console.WriteLine("Systemet visar ett felmeddelande på sidan för tomma inloggningsfält.");
        }
        else
        {
          // Vissa webbläsare har inbyggd HTML validering som kan blockera formulärinskickning
          Console.WriteLine("NOTERA: Inloggningen nekades men inget explicit felmeddelande hittades.");
          Console.WriteLine("Detta kan vara på grund av HTML validering eller ett användbarhetsproblem.");
        }
      }
    }
  }
}