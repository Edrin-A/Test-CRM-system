Feature: Chat Access Via Token
    Test customer access to ticket via chat token

Scenario: Access chat with valid token
    Given I have created a new ticket
    When I follow the chat token link
    Then I should see the chat interface
    And I should see my initial message 