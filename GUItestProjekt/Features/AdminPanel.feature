Feature: Admin Panel Verification
    Test admin panel functionality and navigation

Scenario: Verify admin panel functionalities
    Given I am logged in as an admin user
    When I navigate to the admin panel
    Then I should see all admin panel options
    And I should be able to navigate between admin functions 