using Microsoft.Playwright;
using TechTalk.SpecFlow;
using Xunit;

namespace GUItestProjekt.Steps
{
  [Binding]
  public class CreateTicketSteps
  {
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IPage _page;

    [BeforeScenario]
    public async Task Setup()
    {
      _playwright = await Playwright.CreateAsync();
      _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = false, SlowMo = 300 });
      _page = await _browser.NewPageAsync();
    }

    [AfterScenario]
    public async Task Teardown()
    {
      await _browser.CloseAsync();
      _playwright.Dispose();
    }

    [Given(@"I am on the homepage")]
    public async Task GivenIAmOnTheHomepage()
    {
      await _page.GotoAsync("http://localhost:3000/");
      // Vänta på att sidan laddas
      await _page.WaitForSelectorAsync("text=Välj företag");
    }

    [When(@"I fill in the form with valid information")]
    public async Task WhenIFillInTheFormWithValidInformation()
    {
      // Klicka på Godisfabriken AB-knappen om den finns
      if (await _page.IsVisibleAsync("text=Godisfabriken AB"))
      {
        await _page.ClickAsync("text=Godisfabriken AB");
      }
      else
      {
        await _page.GotoAsync("http://localhost:3000/godisfabrikenab");
      }

      // Vänta på att formuläret laddas
      await _page.WaitForSelectorAsync("select#product");

      // Välj en produkt från dropdown
      await _page.SelectOptionAsync("select#product", new SelectOptionValue { Index = 1 });

      // Fyll i e-post
      await _page.FillAsync("input#email", $"test{DateTime.Now.Ticks}@example.com");

      // Fyll i ämne
      await _page.FillAsync("input#subject", "Test ärende från automatiserat test");

      // Fyll i meddelande
      await _page.FillAsync("textarea#message", "Detta är ett testmeddelande skapat av ett automatiserat test. Vänligen ignorera.");
    }

    [When(@"I submit the form")]
    public async Task WhenISubmitTheForm()
    {
      // Klicka på Skicka knappen
      await _page.ClickAsync("button.SendButton-Layout");
    }

    [Then(@"I should see a confirmation message")]
    public async Task ThenIShouldSeeAConfirmationMessage()
    {
      // Vänta på bekräftelsemeddelandet
      await _page.WaitForSelectorAsync("text=Du har nu skickat in ditt ärende");

      // Verifiera att bekräftelsemeddelandet visas
      var confirmationMessage = await _page.IsVisibleAsync("text=Du har nu skickat in ditt ärende");
      Assert.True(confirmationMessage, "Bekräftelsemeddelandet visas inte");
    }
  }
}