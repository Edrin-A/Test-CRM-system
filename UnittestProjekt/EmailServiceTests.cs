using System;
using System.Threading.Tasks;
using Xunit;
using Moq;

namespace UnittestProjekt
{
  // Interface för eposttjänsten som används för mocking i tester
  // Definierar metoden för att skicka e-post asynkront
  public interface IEmailService
  {
    Task SendEmailAsync(string to, string subject, string body);
  }

  // Record-klass som representerar SMTP-inställningarna för e-posttjänsten
  // Innehåller alla nödvändiga konfigurationsparametrar
  public record EmailSettings(
    string SmtpServer,
    int SmtpPort,
    string FromEmail,
    string Password
  );


  // Record-klass som representerar en begäran om att skicka e-post
  // Innehåller mottagare, ämne och innehåll
  public record EmailRequest(
    string To,
    string Subject,
    string Body
  );


  // Testfall för eposttjänsten
  // Verifierar att epostinställningar och epostförfrågningar fungerar korrekt
  public class EmailServiceTests
  {
    // Testar att EmailSettings klassen har alla nödvändiga egenskaper och att de kan sättas korrekt
    [Fact]
    public void EmailSettings_Should_Have_Required_Properties()
    {
      // Setup
      var settings = new EmailSettings(
          "smtp.example.com",
          587,
          "test@example.com",
          "password123"
      );

      // Test
      Assert.Equal("smtp.example.com", settings.SmtpServer);
      Assert.Equal(587, settings.SmtpPort);
      Assert.Equal("test@example.com", settings.FromEmail);
      Assert.Equal("password123", settings.Password);
    }


    // Testar att EmailRequest klassen har alla nödvändiga egenskaper och att de kan sättas korrekt
    [Fact]
    public void EmailRequest_Should_Have_Required_Properties()
    {
      // Setup
      var request = new EmailRequest(
          "recipient@example.com",
          "Test Subject",
          "<p>Test Body</p>"
      );

      // Test
      Assert.Equal("recipient@example.com", request.To);
      Assert.Equal("Test Subject", request.Subject);
      Assert.Equal("<p>Test Body</p>", request.Body);
    }
  }
}