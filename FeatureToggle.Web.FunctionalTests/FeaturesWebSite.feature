Feature: FeaturesWebSite
	In order to work with features toggles
	As a feature admin
	I want to use a web site to manage the configured toggles

Scenario: View the list of configurations
	Given I have access to the web site
	When I browse the index page of features
	Then I will see the features list
	And it will contain 4 features
	And it will contain 4 edit buttons
	And it will contain 4 delete buttons

Scenario: Deleting a configuration shows a confirmation popup
    Given I browsed the index page of features 
    When I click the delete button of the FeatureToggle.Color feature
    Then I will see a confirmation delete popup
	And the popup message will reference the FeatureToggle.Color feature
	And the popup will have a Yes button
	And the popup will have a Cancel button

Scenario: Delete a configuration
    Given I browsed the index page of features 
    And I clicked the delete button of the FeatureToggle.Color feature
	When I click Yes in the confirmation dialog
    Then I will see the features list 
    And it will contain 3 features
	And it will not contain the FeatureToggle.Color feature
