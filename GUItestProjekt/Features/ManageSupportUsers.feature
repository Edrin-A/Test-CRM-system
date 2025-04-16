Feature: Manage Support Users
    Test managing existing support users as admin

Scenario: View existing support users
    Given I am logged in as an admin user
    When I navigate to the manage support users page
    Then I should see the list of existing support users
    And I should see user management options 