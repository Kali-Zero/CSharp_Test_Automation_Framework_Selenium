using CSharp_Selenium_Test_Automation.PageObjects;
using CS_TestAutomation.Framework.Core;
using AventStack.ExtentReports;
using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenQA.Selenium.Support.UI;
using System;
using SeleniumExtras.WaitHelpers;

namespace CSharp_Selenium_Test_Automation
{
    [TestFixture, Order(1)]
    public class TheGoogleTests : BaseTest
    {
        public GooglePageObjects googlePageObjects = new GooglePageObjects();

        [Test, Order(1)] [Description ("Test 1: Does the 'About' link bring the user to the 'About' page?")]
        public void DoesTheAboutLinkWork()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WebDriverWaitTime));
            Assert.That(googlePageObjects.About(driver).Displayed);
            test.Log(Status.Pass, "'About' link is displayed.");
            wait.Until(ExpectedConditions.ElementToBeClickable(googlePageObjects.About(driver))).Click();
            wait.Until(ExpectedConditions.UrlContains(GoogleTest["about_url"]));
            Assert.That(driver.Url.ToString().Contains(GoogleTest["about_url"]), "'About' URL is incorrect!");
            test.Log(Status.Pass, "'About' URL is correct.");
        }

        [Test, Order(2)][Description("Test 2: Does the 'Store' link bring the user to the 'Store' page?")]
        public void DoesStoreLinkWork()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WebDriverWaitTime));
            Assert.That(googlePageObjects.Store(driver).Displayed);
            test.Log(Status.Pass, "'Store' link is displayed.");
            wait.Until(ExpectedConditions.ElementToBeClickable(googlePageObjects.Store(driver))).Click();
            wait.Until(ExpectedConditions.UrlContains(GoogleTest["store_url"]));
            Assert.That(driver.Url.ToString().Contains(GoogleTest["store_url"]), "'Store' URL is incorrect!");
            test.Log(Status.Pass, "'Store' URL is correct.");
        }

        [Test, Order(3)][Description("Test 3: Does the 'Gmail' link bring the user to the 'Gmail' page?")]
        public void DoesGmailLinkWork()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WebDriverWaitTime));
            Assert.That(googlePageObjects.Gmail(driver).Displayed);
            test.Log(Status.Pass, "'Gmail' link is displayed.");
            wait.Until(ExpectedConditions.ElementToBeClickable(googlePageObjects.Gmail(driver))).Click();
            try { wait.Until(ExpectedConditions.UrlContains(GoogleTest["logged_in_gmail_url"])); }
            catch { }
            try { wait.Until(ExpectedConditions.UrlContains(GoogleTest["not_logged_in_gmail_url"])); }
            catch { }
            Assert.That(driver.Url.Contains(GoogleTest["logged_in_gmail_url"]) ||
                        driver.Url.Contains(GoogleTest["not_logged_in_gmail_url"]),
                        "'Gmail' URL is incorrect!");
            test.Log(Status.Pass, "'Gmail' URL is correct.");
        }

        [Test, Order(4)][Description("Test 4: Does the 'Images' link bring the user to the 'Images' page?")]
        public void DoesImagesLinkWork()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WebDriverWaitTime));
            Assert.That(googlePageObjects.Images(driver).Displayed);
            test.Log(Status.Pass, "'Images' link is displayed.");
            wait.Until(ExpectedConditions.ElementToBeClickable(googlePageObjects.Images(driver))).Click();
            Assert.That(driver.Url.Contains(GoogleTest["images_url"]), "'Images' URL is incorrect!");
            test.Log(Status.Pass, "'Images' URL is correct.");
        }

        //This is currently set to fail on purpose (As an example of a fail on the Extent Report)
        [Test, Order(5)][Description("Test 5: Does the 'Apps' link open the 'Apps' modal? " +
                "Note: This test is set to fail so you can see error messaging and screenshots.")]
        public void DoesAppsLinkWork()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WebDriverWaitTime));
            Assert.That(googlePageObjects.Apps(driver).Displayed);
            test.Log(Status.Pass, "'Apps' link is displayed.");
            wait.Until(ExpectedConditions.ElementToBeClickable(googlePageObjects.Apps(driver))).Click();
            driver.SwitchTo().ActiveElement();
            Assert.That(googlePageObjects.AppsAccount(driver).Displayed);
            test.Log(Status.Pass, "This should not appear in report because it failed.");
        }

        [Test, Order(6)][Description("Test 6: Does the 'Sign In' button bring the user to the 'Sign In' page?")]
        public void DoesSignInButtonWork()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(WebDriverWaitTime));
            Assert.That(googlePageObjects.SignIn(driver).Displayed);
            test.Log(Status.Pass, "'Sign In Button' link is displayed.");
            wait.Until(ExpectedConditions.ElementToBeClickable(googlePageObjects.SignIn(driver))).Click();
            Assert.That(driver.Url.Contains(GoogleTest["account_url"]), "'Account' URL is incorrect!");
            test.Log(Status.Pass, "Account text is displayed.");
        }

        [Test, Order(7)][Description("Test 7: This test is set to be skipped.")]
        public void SkipTestExample()
        {
            test.Log(Status.Skip, "This is an example of a skipped test.");
        }

    }

}