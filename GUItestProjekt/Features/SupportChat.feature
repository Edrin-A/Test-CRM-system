Feature: Support Chat Functionality
    Test sending messages as a support user

Scenario: Send message as support
    Given I am logged in as a support user
    And I open the first ticket's chat
    When I type a message as support
    And I click the support send button
    Then I should see my support message in the chat 