Feature: Ticket Status Update
    Test updating ticket status as a support user

Scenario: Update ticket status
    Given I am logged in as a support user
    And I am on the ticket list page
    When I change the status of the first ticket to "PÅGÅENDE"
    Then the ticket status should be updated to "PÅGÅENDE" 