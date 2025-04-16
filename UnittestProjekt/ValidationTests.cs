using System;
using Xunit;
using System.Text.RegularExpressions;

namespace UnittestProjekt
{

  public class ValidationTests
  {

    // Testar den enklaste formen av epostvalidering
    // Epostadresser måste innehålla @

    [Fact]
    public void Email_Should_Contain_AtSymbol()
    {
      // Setup
      string validEmail = "test@example.com";
      string invalidEmail = "testexample.com";

      // Test
      Assert.Contains("@", validEmail);
      Assert.DoesNotContain("@", invalidEmail);
    }



    // E-postadresser måste följa det grundläggande formatet [text]@[domain].[tld]
    // Verifierar att olika felaktiga format upptäcks
    [Fact]
    public void Email_Should_Match_EmailPattern()
    {
      // Setup
      string validEmail = "test@example.com";
      string invalidEmail1 = "testexample.com";
      string invalidEmail2 = "test@";
      string invalidEmail3 = "@example.com";

      // Pattern för grundläggande e-postvalidering
      string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

      // Test
      Assert.Matches(pattern, validEmail);
      Assert.DoesNotMatch(pattern, invalidEmail1);
      Assert.DoesNotMatch(pattern, invalidEmail2);
      Assert.DoesNotMatch(pattern, invalidEmail3);
    }
  }
}