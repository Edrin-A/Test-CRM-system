Feature: Form Validation
    Test form validation messages

Scenario: Validation message for invalid email
    Given I am logged in as an admin user
    When I navigate to the add support user page
    And I fill in the support user form with invalid email
    And I submit the support user form
    Then I should see a validation error message for email

Scenario: Validation message for empty username
    Given I am logged in as an admin user
    When I navigate to the add support user page
    And I fill in the support user form with empty username
    And I submit the support user form
    Then I should see a validation error message for username 