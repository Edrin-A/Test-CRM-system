using System;
using Xunit;
using server; // Importera namespace server för att få tillgång till Role enum

namespace UnittestProjekt
{

  // Testfall för ärendehanteringssystemet
  // Verifierar att ärenden (tickets) fungerar korrekt med olika statusvärden och behörigheter

  public class TicketTests
  {
    // Testar att alla ärendestatusvärden är giltiga enumvärden
    // Verifierar också att ogiltiga statusvärden inte existerar i enumen
    [Fact]
    public void Ticket_Status_Should_Be_Valid_Enum_Value()
    {
      // Setup
      // Kontrollera att alla statusvärden är giltiga enumvärden
      Assert.True(Enum.IsDefined(typeof(TicketStatus), "NY"));
      Assert.True(Enum.IsDefined(typeof(TicketStatus), "PÅGÅENDE"));
      Assert.True(Enum.IsDefined(typeof(TicketStatus), "LÖST"));
      Assert.True(Enum.IsDefined(typeof(TicketStatus), "STÄNGD"));

      // Test
      //Ogiltigt värde bör inte existera
      Assert.False(Enum.IsDefined(typeof(TicketStatus), "AVBRUTEN"));
    }


    // Testar att ett nyskapat ärende har statusen NY som standardvärde
    [Fact]
    public void Ticket_Should_Have_Default_Status_New()
    {
      // Setup
      var ticket = new TestTicket();

      // Test
      Assert.Equal(TicketStatus.NY, ticket.Status);
    }


    // Testar att stängda ärenden inte accepterar nya meddelanden från vanliga användare
    // men att supportpersonal fortfarande kan skicka meddelanden
    [Fact]
    public void Closed_Ticket_Should_Not_Accept_New_Messages()
    {
      // Setup
      var ticket = new TestTicket();
      ticket.Status = TicketStatus.STÄNGD;

      // Test
      Assert.False(ticket.CanAddMessage(Role.USER));
      Assert.True(ticket.CanAddMessage(Role.SUPPORT)); // Support kan fortfarande skicka
    }
  }

  // Enum för att representera olika statusar som ett ärende kan ha
  public enum TicketStatus
  {
    NY,
    PÅGÅENDE,
    LÖST,
    STÄNGD
  }


  // Testklass som representerar ett ärende i systemet
  // Använder det för att simulera ärendehantering och meddelandebehörigheter
  public class TestTicket
  {
    // Status för ärendet, standardvärde är NY
    public TicketStatus Status { get; set; } = TicketStatus.NY;


    // Kontrollerar om en användare med specifik roll kan lägga till meddelanden i ärendet
    public bool CanAddMessage(Role senderType)
    {
      // Implementera logik för att kontrollera om meddelanden kan läggas till baserat på status
      if (Status == TicketStatus.STÄNGD && senderType == Role.USER)
      {
        return false;
      }
      return true;
    }
  }
}