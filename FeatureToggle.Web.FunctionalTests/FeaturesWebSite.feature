Feature: FeaturesWebSite
	In order to work with features toggles
	As a feature admin
	I want to use a web site to manage the configured toggles

@mytag
Scenario: View the list of configured features
	Given I have access to the web site
	When I browse the index page of features
	Then I will see 4 features
