Feature: Login Validation
    Test error messages during login

Scenario: Error message for incorrect login credentials
    Given I navigate to the login page
    When I attempt to login with incorrect credentials
    Then I should see a login error message

Scenario: Error message for empty login fields
    Given I navigate to the login page
    When I attempt to login with empty credentials
    Then I should see empty fields validation messages 