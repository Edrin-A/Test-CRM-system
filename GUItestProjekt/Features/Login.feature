Feature: Login
    Test login functionality

Scenario: Login with valid credentials
    Given I am on the login page
    When I enter username "support1" and password "testpostmanflowsupport123"
    And I click the login button
    Then I should be logged in successfully 