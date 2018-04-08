using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using TechTalk.SpecFlow;
using Xunit;

namespace FeatureToggle.Web.FunctionalTests.IndexPageSteps
{

    public class IndexPageModel
    {
        public static By FeaturesListBy => By.CssSelector("#featuresList");
        public static By FeatureListTableRowsBy => By.CssSelector("#featuresList tbody tr");
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
            Assert.Equal(featuresNumber, _driver.FindElements(IndexPageModel.FeatureListTableRowsBy).Count);
        }
    }
}
