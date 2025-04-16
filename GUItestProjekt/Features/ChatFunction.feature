Feature: Chat Functionality
    Test sending messages in the chat interface

Scenario: Send message in chat
    Given I have created a new ticket and have access to the chat
    When I type a message in the chat input field
    And I click the send button
    Then I should see my message in the chat history 