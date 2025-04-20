using Microsoft.Playwright;
using TechTalk.SpecFlow;
using Xunit;
using System.Text.Json;

namespace GUItestProjekt.Steps
{
  [Binding]
  public class ChatFunctionSteps
  {
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IPage _page;
    private string _chatToken;
    private string _initialMessage;
    private string _newMessage;

    [BeforeScenario]
    public async Task Setup()
    {
      _playwright = await Playwright.CreateAsync();
      _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = true, SlowMo = 300 });
      _page = await _browser.NewPageAsync();
      _initialMessage = "Initial message from test " + DateTime.Now.Ticks;
      _newMessage = "Follow-up message from test " + DateTime.Now.Ticks;
    }

    [AfterScenario]
    public async Task Teardown()
    {
      await _browser.CloseAsync();
      _playwright.Dispose();
    }

    [Given(@"I have created a new ticket and have access to the chat")]
    public async Task GivenIHaveCreatedANewTicketAndHaveAccessToTheChat()
    {
      // Registrera en händelseavlyssnare för att fånga API-svar innan vi navigerar
      await _page.RouteAsync("**/api/form", async route =>
      {
        // Fortsätt med begäran men fånga svaret
        var response = await route.FetchAsync();
        if (response.Ok)
        {
          string body = await response.TextAsync();
          try
          {
            // Försök parsa JSON-svaret
            var jsonDoc = JsonDocument.Parse(body);
            if (jsonDoc.RootElement.TryGetProperty("chatToken", out var tokenElement))
            {
              _chatToken = tokenElement.GetString();
              Console.WriteLine($"Fångat chat-token: {_chatToken}");
            }
          }
          catch (JsonException ex)
          {
            Console.WriteLine($"Kunde inte parsa JSON: {ex.Message}");
          }
        }
        await route.FulfillAsync(new()
        {
          Response = response,
        });
      });

      // Gå till förstasidan
      await _page.GotoAsync("http://localhost:3000/");
      await _page.WaitForSelectorAsync("text=Välj företag");

      // Klicka på Godisfabriken AB
      await _page.ClickAsync("text=Godisfabriken AB");

      // Vänta på att formuläret laddas
      await _page.WaitForSelectorAsync("select#product");

      // Välj produkt
      await _page.SelectOptionAsync("select#product", new SelectOptionValue { Index = 1 });

      // Fyll i formuläret
      string email = $"test{DateTime.Now.Ticks}@example.com";
      await _page.FillAsync("input#email", email);
      await _page.FillAsync("input#subject", "Test ärende för chattfunktion");
      await _page.FillAsync("textarea#message", _initialMessage);

      // Skicka formuläret
      await _page.ClickAsync("button.SendButton-Layout");

      // Vänta på bekräftelsemeddelandet
      await _page.WaitForSelectorAsync("text=Du har nu skickat in ditt ärende");

      // Vänta lite för att se till att API-svaret har hunnit fångas
      await _page.WaitForTimeoutAsync(1000);

      // Om vi inte hittar chat-token, använd fallback
      if (string.IsNullOrEmpty(_chatToken))
      {
        Console.WriteLine("Kunde inte fånga chat-token automatiskt. Använder fallback.");
        _chatToken = "b1e06b03-9fcb-4f0c-bb36-94c9777ef3a9"; // Ersätt med ett giltigt token
        Console.WriteLine($"VARNING: Använder fallback chat-token: {_chatToken}");
      }

      // Navigera till chat-sidan med token
      await _page.GotoAsync($"http://localhost:3000/chat/{_chatToken}");

      // Vänta på att chat-gränssnittet laddas
      await _page.WaitForSelectorAsync(".chat-container");

      // Vänta på att meddelanden laddas
      await _page.WaitForSelectorAsync(".messages-list");

      // Verifiera att det initiala meddelandet visas
      bool hasInitialMessage = await _page.IsVisibleAsync($"text={_initialMessage}");
      Assert.True(hasInitialMessage, "Det initiala meddelandet visas inte i chatten");
    }

    [When(@"I type a message in the chat input field")]
    public async Task WhenITypeAMessageInTheChatInputField()
    {
      await _page.FillAsync(".message-input", _newMessage);
    }

    [When(@"I click the send button")]
    public async Task WhenIClickTheSendButton()
    {
      await _page.ClickAsync(".send-button-ChatPage");

      // Vänta lite för att låta meddelandet skickas och visas
      await _page.WaitForTimeoutAsync(1000);
    }

    [Then(@"I should see my message in the chat history")]
    public async Task ThenIShouldSeeMyMessageInTheChatHistory()
    {
      // Vänta på att meddelandet läggs till i listan
      await _page.WaitForSelectorAsync($"text={_newMessage}");

      // Verifiera att meddelandet visas
      bool hasNewMessage = await _page.IsVisibleAsync($"text={_newMessage}");

      Assert.True(hasNewMessage, "Det nya meddelandet visas inte i chatten");

      // Extra verifiering - kontrollera att meddelandet har rätt styling för användare
      var messageElement = await _page.QuerySelectorAsync($"text={_newMessage}");
      var parentClasses = await _page.EvaluateAsync<string>("el => el.closest('.message').className", messageElement);

      // Använd en enkel strängkontroll istället för Assert.Contains som förväntar sig en samling
      Assert.True(parentClasses.Contains("user-message"), "Meddelandet har inte korrekt styling för användarmeddelande");
    }
  }
}