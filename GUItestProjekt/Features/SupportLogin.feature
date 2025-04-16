Feature: Support User Login
    Test login functionality for support users

Scenario: Login as support user
    Given I am on the support login page
    When I enter support user credentials
    And I click the support login button
    Then I should be logged in as support user
    And I should see the support dashboard 