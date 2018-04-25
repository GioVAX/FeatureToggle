using System;
using System.IO;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TechTalk.SpecFlow;
using Xunit;

namespace FeatureToggle.Web.FunctionalTests.IndexPageSteps
{

    public static class IndexPageModel
    {
        public static By FeaturesListBy => By.CssSelector("#featuresList");
        public static By FeatureListTableRowsBy => By.CssSelector("#featuresList tbody tr");
        public static By FeatureListTableEditIconsBy => By.CssSelector("#featuresList tbody tr td:last-child > .btn.glyphicon-pencil");
        public static By FeatureListTableDeleteIconsBy => By.CssSelector("#featuresList tbody tr td:last-child > .btn.glyphicon-remove");
        public static By FeatureListTableListedFeatures => By.CssSelector("#featuresList tbody tr td:first-child");
        public static By EditFeaturePopupForm => By.CssSelector("#featureForm form");
        public static By EditPopupFeatureNameEdit => By.CssSelector("#featureForm form input[name=Feature]");
        public static By EditPopupFeatureValueEdit => By.CssSelector("#featureForm form input[name=Value]");

        public static By FeatureListTableDeleteIconForFeatureBy(string featureName, string action) =>
            By.CssSelector($"#featuresList tbody tr.{featureName.Replace('.', '_')} td:last-child > .btn.{action}");
    }

    [Binding]
    public class IndexPage : IDisposable
    {
        private readonly IWebDriver _driver;
        private const string BaseUrl = "http://localhost:51847/";
        private const string WebApplicationRelativePath = @"..\..\..\FeatureToggle";

        public IndexPage()
        {
            File.Copy(
                Path.Combine(WebApplicationRelativePath, "Features_Test.Json"),
                Path.Combine(WebApplicationRelativePath, "Features.json"),
                true);

            _driver = new ChromeDriver();
        }
        public void Dispose()
        {
            try
            {
                var alert = _driver?.SwitchTo().Alert();
                alert?.Dismiss();
            }
            catch { }

            _driver?.Quit();
            _driver?.Dispose();
        }

        [Given(@"I have access to the web site")]
        public void GivenIHaveAccessToTheWebSite()
        {
        }

        [Given(@"I browsed the index page of features")]
        [When(@"I browse the index page of features")]
        public void BrowseTheIndexPageOfFeatures()
        {
            _driver.Navigate().GoToUrl(BaseUrl);
        }

        [Then(@"I will see the features list")]
        public void ThenIWillSeeFeatures()
        {
            Assert.True(_driver.FindElement(IndexPageModel.FeaturesListBy).Displayed);
        }

        [Then(@"it will contain (.*) features")]
        public void ThenItWillContainFeatures(int featuresNumber)
        {
            var featureRows = _driver.FindElements(IndexPageModel.FeatureListTableRowsBy);
            Assert.Equal(featuresNumber, featureRows.Count);
        }

        [Then(@"it will contain (.*) edit buttons")]
        public void ThenEachFeatureHasTheEditButton(int iconCount)
        {
            var featureRows = _driver.FindElements(IndexPageModel.FeatureListTableEditIconsBy);
            Assert.Equal(iconCount, featureRows.Count);
        }

        [Then(@"it will contain (.*) delete buttons")]
        public void ThenItWillContainDeleteButtons(int iconCount)
        {
            var featureRows = _driver.FindElements(IndexPageModel.FeatureListTableDeleteIconsBy);
            Assert.Equal(iconCount, featureRows.Count);
        }

        [Given(@"I clicked the (.*) button of the (.*) feature")]
        [When(@"I click the (.*) button of the (.*) feature")]
        public void WhenIClickAnActionButtonForFeature(string action, string featureName)
        {
            var button = _driver.FindElement(IndexPageModel.FeatureListTableDeleteIconForFeatureBy(featureName, action));
            button.Click();
        }

        [Then(@"I will see a modal popup to modify the configuration")]
        public void ThenIWillSeeAModalPopupToModifyTheConfiguration()
        {
            var form = _driver.FindElement(IndexPageModel.EditFeaturePopupForm);
            Assert.NotNull(form);
        }

        [Then(@"the feature name will be (.*)")]
        public void ThenTheFeatureNameWillBe(string featureName)
        {
            var featureTextBox = _driver.FindElement(IndexPageModel.EditPopupFeatureNameEdit);
            Assert.Equal(featureName, featureTextBox.GetAttribute("value"));
        }

        [Then(@"the feature value will be (.*)")]
        public void ThenTheFeatureValueWillBe(string featureValue)
        {
            var featureTextBox = _driver.FindElement(IndexPageModel.EditPopupFeatureValueEdit);
            Assert.Equal(featureValue, featureTextBox.GetAttribute("value"));
        }

        [Then(@"the form method will be (.*)")]
        public void ThenTheFormMethodWillBePost(string expectedMethod)
        {
            var form = _driver.FindElement(IndexPageModel.EditFeaturePopupForm);
            var actualMethod = form.GetAttribute("method");
            Assert.Equal(expectedMethod, actualMethod);
        }

        [Then(@"the form will not allow modifying the feature name")]
        public void ThenTheFormWillNotAllowModifyingTheFeatureName()
        {
            var nameTextbox = _driver.FindElement(IndexPageModel.EditPopupFeatureNameEdit);
            Assert.False(nameTextbox.Enabled);
        }

        [Then(@"the form will allow modifying the feature value")]
        public void ThenTheFormWillAllowModifyingTheFeatureValue()
        {
            var nameTextbox = _driver.FindElement(IndexPageModel.EditPopupFeatureValueEdit);
            Assert.True(nameTextbox.Enabled);
        }

        [Then(@"it will not contain the (.*) feature")]
        public void ThenItWillNotContainTheRemovedFeature(string removedFeature)
        {
            var cells = _driver.FindElements(IndexPageModel.FeatureListTableListedFeatures);
            Assert.DoesNotContain(removedFeature, cells.Select(c => c.Text));
        }

        [Then(@"I will see a confirmation (.*) popup")]
        public void ThenIWillSeeAConfirmationDeletePopup(string action2Confirm)
        {
            var alert = _driver.SwitchTo().Alert();
            var text = alert.Text;

            Assert.Contains(action2Confirm, text);
        }

        [Then(@"the popup message will reference the (.*) feature")]
        public void ThenThePopupMessageWillReferenceTheFeature(string featureName)
        {
            var alert = _driver.SwitchTo().Alert();
            var text = alert.Text;

            Assert.Contains(featureName, text);
        }

        [Then(@"the popup will have a (.*) button")]
        public void ThenThePopupWillHaveAButton(string buttonText)
        {

        }

        [When(@"I click Yes in the confirmation dialog")]
        public void WhenIClickYesInTheConfirmationDialog()
        {
            _driver.SwitchTo().Alert().Accept();
        }

        [Given(@"I modified the feature value to (.*)")]
        public void GivenIModifiedTheFeatureValueTo( string newFeatureValue)
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"I click the Submit button")]
        public void WhenIClickTheSubmitButton()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the feature value of (.*) will be (.*)")]
        public void ThenTheFeatureValueOfFeatureWillBe(string featureName, string featureValue)
        {
            ScenarioContext.Current.Pending();
        }
    }
}
