using Microsoft.Playwright;
using TechTalk.SpecFlow;
using Xunit;
using System.Threading.Tasks;
using System;

namespace GUItestProjekt.Steps
{
  [Binding]
  public class CustomerChatSteps
  {
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IPage _page;
    private string _customerMessage;

    [BeforeScenario]
    public async Task SetupAsync()
    {
      _playwright = await Playwright.CreateAsync();
      _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
      {
        Headless = true,
        SlowMo = 200
      });
      _page = await _browser.NewPageAsync();
      _customerMessage = "Kundmeddelande från automatiserat test " + DateTime.Now.Ticks;
    }

    [AfterScenario]
    public async Task TeardownAsync()
    {
      await _browser?.CloseAsync();
      _playwright?.Dispose();
    }

    [Given(@"I navigate to the customer chat page")]
    public async Task GivenINavigateToTheCustomerChatPage()
    {
      await _page.ClickAsync("text=Chatta med oss");
      await _page.WaitForURLAsync("http://localhost:3000/chat/customer");
      await _page.WaitForSelectorAsync(".customer-chat-container");
    }

    [When(@"I enter a customer message")]
    public async Task WhenIEnterACustomerMessage()
    {
      await _page.FillAsync("input.customer-chat-input", _customerMessage);
    }

    [When(@"I send the customer message")]
    public async Task WhenISendTheCustomerMessage()
    {
      await _page.ClickAsync("button.customer-send-button");
      await _page.WaitForTimeoutAsync(500); // Vänta på att meddelandet skickas
    }

    [Then(@"the customer message should appear in the chat")]
    public async Task ThenTheCustomerMessageShouldAppearInTheChat()
    {
      var messageExists = await _page.QuerySelectorAsync($".message-content:has-text(\"{_customerMessage}\")") != null;
      Assert.True(messageExists, $"Meddelandet '{_customerMessage}' hittades inte i chatten");
    }
  }
}