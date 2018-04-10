using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using TechTalk.SpecFlow;
using Xunit;

namespace FeatureToggle.Web.FunctionalTests.IndexPageSteps
{

    public static class IndexPageModel
    {
        public static By FeaturesListBy => By.CssSelector("#featuresList");
        public static By FeatureListTableRowsBy => By.CssSelector("#featuresList tbody tr");
        public static By FeatureListTableEditIconsBy => By.CssSelector("#featuresList tbody tr td:last-child > .glyphicon-pencil");
    }

    [Binding]
    public class IndexPage
    {
        private IWebDriver _driver;
        private const string _baseUrl = "http://localhost:51847/";

        public IndexPage()
        {
            _driver = new ChromeDriver();
        }

        [Given(@"I have access to the web site")]
        public void GivenIHaveAccessToTheWebSite()
        {
        }

        [When(@"I browse the index page of features")]
        public void WhenIBrowseTheIndexPageOfFeatures()
        {
            _driver.Navigate().GoToUrl(_baseUrl);
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
    }
}
