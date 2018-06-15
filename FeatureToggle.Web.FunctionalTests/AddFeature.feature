Feature: AddNew
	In order to work with features toggles
	As a feature admin
	I want to use a web site to manage the configured toggles

Scenario: Adding a new configuration shows a modal form
    Given I browsed the index page of features 
    When I click the add button
    Then I will see a modal popup
	And the form will allow modifying the feature name
	And the form will allow modifying the feature value
	And the feature name will have no value
	And the feature value will have no value

Scenario: Adding a new configuration adds a value to the list
    Given I browsed the index page of features 
    And I clicked the add button
	And I enter the feature name as NewFeature
	And I enter the feature value as NewValue
	When I click the Submit button
	Then the modal popup will not be visible
	And I will see the features list
	And it will contain 5 features
