Feature: Create Ticket
    Test creating a new ticket

Scenario: Submit new ticket
    Given I am on the homepage
    When I fill in the form with valid information
    And I submit the form
    Then I should see a confirmation message 