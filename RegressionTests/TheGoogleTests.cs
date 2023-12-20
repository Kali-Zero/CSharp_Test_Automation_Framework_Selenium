using AventStack.ExtentReports;
using NUnit.Framework;
using CSharp_Selenium_Test_Automation.PageObjects;
using CS_TestAutomation.Framework.Core;

namespace CSharp_Selenium_Test_Automation
{
    [TestFixture]
    public class TheGoogleTests : BaseTest
    {
        public GooglePageObjects googlePageObjects = new GooglePageObjects();

        [Test][Description("00. Does the Google page do a thing?")]
        public void IsAboutLinkDisplayed()
        {
            Assert.That(googlePageObjects.About(driver).Displayed, "This means it failed.");
            test.Log(Status.Pass, "This means it passed.");
        }

        [Test][Description("01. This test should pass.")]
        public void PassTest()
        {
            Assert.That(!driver.Url.ToString().Contains("shenannigans"), "Test 1 Message.");
            test.Log(Status.Pass, "Test 1 Log Message.");
        }

        [Test][Description("02. This test should fail.")]
        public void FailTest1()
        {
            Assert.That(!driver.Url.ToString().Contains("shenannigans"), "Test 2a Message.");
            test.Log(Status.Pass, "Test 2a Log Message.");
            Assert.That(driver.Url.ToString().Contains("shenannigans"), "Test 2b Message.");
            test.Log(Status.Pass, "Test 2b Log Message.");
        }

        [Test][Description("03. This test should pass.")]
        public void PassTestTwo()
        {
            Assert.That(!driver.Url.ToString().Contains("shenannigans"), "Test 3 Message.");
            test.Log(Status.Pass, "Test 3 Log Message.");
        }

    }
}