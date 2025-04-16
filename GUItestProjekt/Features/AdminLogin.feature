Feature: Admin User Login
    Test login functionality for admin users

Scenario: Login as admin user
    Given I am on the admin login page
    When I enter admin user credentials
    And I click the admin login button
    Then I should be logged in as admin user
    And I should see the admin dashboard