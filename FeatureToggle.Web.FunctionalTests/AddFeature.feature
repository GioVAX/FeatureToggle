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
