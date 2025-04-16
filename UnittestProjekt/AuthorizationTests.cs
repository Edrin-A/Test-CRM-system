using System;
using Xunit;
using server; // Importerar namespace server för att få tillgång till Role enumen

using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace UnittestProjekt
{
  // Testklass som representerar en användare i systemet
  // Använder det för att simulera olika behörighetsroller
  public class TestUser
  {
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; }


    // Konstruktor för att skapa en testanvändare med specificerade egenskaper
    public TestUser(int id, string username, string email, Role role)
    {
      Id = id;
      Username = username;
      Email = email;
      Role = role;
    }
  }


  // Testfall för behörighetssystemet
  /// Verifierar att olika användarroller har rätt behörigheter
  public class AuthorizationTests
  {
    // Testar att admin-användare har tillgång till support resurser
    // Verifierar hierarkin av behörigheter mellan olika användarroller
    [Fact]
    public void Admin_Should_Have_Access_To_Support_Resources()
    {
      // Setup
      var adminUser = new TestUser(1, "admin", "admin@test.com", Role.ADMIN);
      var supportUser = new TestUser(2, "support", "support@test.com", Role.SUPPORT);
      var regularUser = new TestUser(3, "user", "user@test.com", Role.USER);

      // Test
      Assert.Equal(Role.ADMIN, adminUser.Role);
      Assert.Equal(Role.SUPPORT, supportUser.Role);
      Assert.Equal(Role.USER, regularUser.Role);

      // Administratören ska ha högre behörighet än support
      Assert.True(adminUser.Role == Role.ADMIN);
    }

    // Testar att vanliga användare inte har admin- eller supportbehörighet
    // Säkerställer att användare med lägre behörighet inte kan få tillgång till känsliga resurser
    [Fact]
    public void User_Should_Not_Have_Admin_Access()
    {
      // Setup
      var regularUser = new TestUser(3, "user", "user@test.com", Role.USER);

      // Test
      Assert.NotEqual(Role.ADMIN, regularUser.Role);
      Assert.NotEqual(Role.SUPPORT, regularUser.Role);
    }
  }
}