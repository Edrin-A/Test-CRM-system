Feature: Add Support User
    Test creating a new support user as admin

Scenario: Create a new support user
    Given I am logged in as an admin user
    When I navigate to the add support user page
    And I fill in the support user form
    And I submit the support user form
    Then I should see a support user success message