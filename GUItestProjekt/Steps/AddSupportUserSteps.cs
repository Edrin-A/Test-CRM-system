using Microsoft.Playwright;
using TechTalk.SpecFlow;
using Xunit;
using System.Threading.Tasks;
using System;

namespace GUItestProjekt.Steps
{
  [Binding]
  [Scope(Feature = "Add Support User")]
  public class AddSupportUserSteps
  {
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IBrowserContext _context;
    private IPage _page;
    private string _uniqueUsername;
    private string _uniqueEmail;

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

      // Skapa unika värden för användare
      var uniqueId = DateTime.Now.Ticks.ToString().Substring(10);
      _uniqueUsername = $"testuser{uniqueId}";
      _uniqueEmail = $"test{uniqueId}@example.com";
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
      // Navigera till admin-panelen
      await _page.ClickAsync("text=Admin");

      // Vänta på att admin sidan laddas
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

    [When(@"I fill in the support user form")]
    public async Task WhenIFillInTheSupportUserForm()
    {
      // Fyll i användaruppgifter
      await _page.FillAsync("input#username", _uniqueUsername);
      await _page.FillAsync("input#password", "Test123!");
      await _page.FillAsync("input#email", _uniqueEmail);

      // är förmodligen redan vald som standard
      await _page.SelectOptionAsync("select#role", new[] { "SUPPORT" });

      // Logga vad vi skapar för användare (för felsökning)
      Console.WriteLine($"Skapar supportanvändare: {_uniqueUsername} med e-post: {_uniqueEmail}");
    }

    [When(@"I submit the support user form")]
    public async Task WhenISubmitTheSupportUserForm()
    {
      // Klicka på skicka knappen
      await _page.ClickAsync("button.SendButton-Layout", new PageClickOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });
    }

    [Then(@"I should see a support user success message")]
    public async Task ThenIShouldSeeASupportUserSuccessMessage()
    {
      // Vänta på att bekräftelsemeddelandet visas
      await _page.WaitForSelectorAsync(".success-message", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });

      // Verifiera att rätt bekräftelsemeddelande visas
      bool hasSuccessMessage = await _page.IsVisibleAsync("text=Du har nu registrerat en ny kundtjänstmedarbetare");
      Assert.True(hasSuccessMessage, "Bekräftelsemeddelandet för skapad supportanvändare visas inte");

      // Verifiera att e-post bekräftelse visas
      bool hasEmailConfirmation = await _page.IsVisibleAsync("text=E-post har nu skickats till denna person");
      Assert.True(hasEmailConfirmation, "Bekräftelse om skickad e-post visas inte");

      // Klicka på tillbakaknappen
      await _page.ClickAsync("text=Tillbaka till menyn");

      // Verifiera att vi kommer tillbaka till admin-panelen
      await _page.WaitForSelectorAsync("text=Administratörspanel", new PageWaitForSelectorOptions
      {
        Timeout = 30000 // 30 sekunder timeout
      });
    }
  }
}