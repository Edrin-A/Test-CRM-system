Feature: Ticket List Functionality
    Test viewing and filtering tickets as a support user

Scenario: Filter tickets by status
    Given I am logged in as a support user
    And I am on the ticket list page
    When I select status filter "PÅGÅENDE"
    Then I should only see tickets with status "PÅGÅENDE"
    When I select status filter "ALLA"
    Then I should see all tickets 