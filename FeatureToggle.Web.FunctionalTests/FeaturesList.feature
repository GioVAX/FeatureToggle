Feature: FeaturesList
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
	And the page will have a add button