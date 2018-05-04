Feature: EditFeature
	In order to work with features toggles
	As a feature admin
	I want to use a web site to manage the configured toggles

Scenario: Editing a configuration shows a modal form
    Given I browsed the index page of features 
    When I click the edit button of the FeatureToggle.Color feature
    Then I will see a modal popup to modify the configuration
	And the feature name will be FeatureToggle.Color
	And the feature value will be Red
	And the form method will be post
	And the form will not allow modifying the feature name
	And the form will allow modifying the feature value

Scenario: Editing a configuration changes the feature value
    Given I browsed the index page of features 
    And I clicked the edit button of the FeatureToggle.Color feature
	And I modified the feature value to Yellow
	When I click the Submit button
	Then the modal popup will not be visible
	Then I will see the features list
	And the feature value of FeatureToggle.Color will be Yellow