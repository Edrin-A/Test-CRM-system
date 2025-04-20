using Microsoft.Playwright;
using TechTalk.SpecFlow;
using Xunit;
using System.Threading.Tasks;
using System;

namespace GUItestProjekt.Steps
{
  [Binding]
  public class SupportChatSteps
  {
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IBrowserContext _context;
    private IPage _page;
    private string _supportMessage;

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
      _supportMessage = "Support-meddelande från automatiserat test " + DateTime.Now.Ticks;
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
      await _page.WaitForSelectorAsync("text=Logga in");

      // Logga in som support
      await _page.FillAsync("input[name='username']", "support1");
      await _page.FillAsync("input[name='password']", "testpostmanflowsupport123");
      await _page.ClickAsync("button.SigninButton-signin");

      // Vänta på omdirigeringen efter inloggning
      await _page.WaitForURLAsync("http://localhost:3000/homes");
      await _page.WaitForTimeoutAsync(1000); // Lägg till en liten väntetid för att säkerställa att allt laddas
    }

    [Given(@"I open the first ticket's chat")]
    public async Task GivenIOpenTheFirstTicketsChat()
    {
      // Navigera till ärendesidan via Dashboard
      // Hitta "Ärenden" i sidofältet och klicka på det
      await _page.ClickAsync("text='Ärenden'");

      // Vänta på att ärendelistan laddas - med längre timeout
      await _page.WaitForURLAsync("http://localhost:3000/arenden", new PageWaitForURLOptions
      {
        Timeout = 60000 // 60 sekunder timeout
      });

      // Vänta även på att containern laddas för att säkerställa att sidan är redo
      await _page.WaitForSelectorAsync(".tickets-container", new PageWaitForSelectorOptions
      {
        Timeout = 60000 // 60 sekunder timeout
      });

      // Vänta på att första ärendet laddas
      await _page.WaitForSelectorAsync(".ticket-item", new PageWaitForSelectorOptions
      {
        Timeout = 60000 // 60 sekunder timeout
      });

      // Hitta och spara länken till chatten för det första ärendet
      string chatLink = await _page.EvalOnSelectorAsync<string>(".ticket-item .chat-link", "el => el.href");

      Console.WriteLine($"Hittade chattlänk: {chatLink}");

      // Öppna en ny flik med chatten
      var newPage = await _context.NewPageAsync();

      // Gå till chatten via den nya fliken
      await newPage.GotoAsync(chatLink);

      // Byt referenser till den nya fliken
      _page = newPage;

      // Vänta på att chattsidan laddas
      await _page.WaitForSelectorAsync(".chat-container", new PageWaitForSelectorOptions
      {
        Timeout = 60000 // 60 sekunder timeout
      });
    }

    [When(@"I type a message as support")]
    public async Task WhenITypeAMessageAsSupport()
    {
      // Skriv in meddelande i chattfältet
      await _page.FillAsync(".message-input", _supportMessage);
    }

    [When(@"I click the support send button")]
    public async Task WhenIClickTheSupportSendButton()
    {
      // Klicka på skicka knappen
      await _page.ClickAsync(".send-button-ChatPage");

      // Vänta lite så att meddelandet hinner skickas
      await _page.WaitForTimeoutAsync(1000);
    }

    [Then(@"I should see my support message in the chat")]
    public async Task ThenIShouldSeeMySupportMessageInTheChat()
    {
      // Vänta på att meddelandet visas i chatten
      await _page.WaitForSelectorAsync(".messages-list");

      // Kontrollera att meddelandet finns i chatten
      var messageExists = await _page.QuerySelectorAsync($".message-content:has-text(\"{_supportMessage}\")") != null;
      Assert.True(messageExists, $"Meddelandet '{_supportMessage}' hittades inte i chatten");

      // Kontrollera att meddelandet visas som ett support-meddelande (staff-message class)
      var isStaffMessage = await _page.QuerySelectorAsync($".staff-message .message-content:has-text(\"{_supportMessage}\")") != null;
      Assert.True(isStaffMessage, "Meddelandet visas inte som ett support-meddelande");
    }
  }
}
