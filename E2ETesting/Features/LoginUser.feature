Feature: LoginUser

    Test the login functionality with valid credentials

@tag1
Scenario: User logs in with valid email and password
    Given I am on the login page
    When I fill in the email field with "john@email.com"
    And I fill in the password field with "Ohj2fjbi!"
    And I click the login button
    Then I should be redirected to the home page

Scenario: User logs in with invalid password
    Given I am on the login page
    When I fill in the email field with "john@email.com"
    And I fill in the password field with "wrongpassword"
    And I click the login button
    Then I should stay on the login page 
    And I should see an error message

Scenario: User books a study room successfully
    Given I am on the login page
    When I fill in the email field with "john@email.com"
    And I fill in the password field with "Ohj2fjbi!"
    And I click the login button
    Then I should be redirected to the home page    
    When I navigate to the booking page
    And I select a study room
    And I select a date 
    And I submit the booking
    Then I should be redirected to the MyBookings page
    And I should see my new booking listed

    