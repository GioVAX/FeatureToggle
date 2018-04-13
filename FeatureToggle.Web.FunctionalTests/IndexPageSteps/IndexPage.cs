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
        public static By FeatureListTableEditIconsBy => By.CssSelector("#featuresList tbody tr td:last-child > .glyphicon-pencil");
        public static By FeatureListTableDeleteIconsBy => By.CssSelector("#featuresList tbody tr td:last-child > .glyphicon-remove");
    }

    [Binding]
    public class IndexPage : IDisposable
    {
        private readonly IWebDriver _driver;
        private const string BaseUrl = "http://localhost:51847/";
        public const string WebApplicationRelativePath = @"..\..\..\FeatureToggle";

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
            _driver?.Quit();
            _driver?.Dispose();
        }

        [Given(@"I have access to the web site")]
        public void GivenIHaveAccessToTheWebSite()
        {
        }

        [Given(@"I browse the index page of features")]
        [When(@"I browse the index page of features")]
        public void WhenIBrowseTheIndexPageOfFeatures()
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

        [When(@"I click the delete button")]
        public void WhenIClickTheDeleteButton()
        {
            var button = _driver.FindElements(IndexPageModel.FeatureListTableDeleteIconsBy).First();
            button.Click();
        }
    }
}
