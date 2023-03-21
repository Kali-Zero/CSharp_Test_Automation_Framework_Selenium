using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Configuration;
using System.Collections.Specialized;
using System;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports;
using NUnit.Framework;
using System.IO;
using TestContext = NUnit.Framework.TestContext;
using NUnit.Framework.Interfaces;
using System.Drawing;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace CSharp_Selenium_Test_Automation.Framework.Core
{
    public class BaseTest
    {
        public IWebDriver driver;
        public string mainReportFolder = Path.Combine(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).FullName).FullName, "ExtentReports");
        public NameValueCollection googleSite = (NameValueCollection)ConfigurationManager.GetSection("googleSite");
        public NameValueCollection browser = (NameValueCollection)ConfigurationManager.GetSection("browser");
        public String dateTime = DateTime.Now.ToString("yyyy-MM-dd HH-mm-tt");
        public String extentFolder = @"./../ExtentReports/";
        public ExtentReports extent = new ExtentReports();
        public ExtentTest test;

        private IWebDriver GetChromeDriver()
        {
            if (driver == null)
            {
                ChromeOptions options = new ChromeOptions();
                if (browser["isHeadless"] == "True") { options.AddArguments("--headless"); }
                driver = new ChromeDriver(options);
                return driver;
            }
            else { return driver; }
        }

        private IWebDriver GetFirefoxDriver()
        {
            if (driver == null)
            {
                FirefoxOptions options = new FirefoxOptions();
                if (browser["isHeadless"] == "True") { options.AddArguments("--headless"); }
                driver = new FirefoxDriver(options);
                return driver;
            }
            else { return driver; }
        }

        private IWebDriver GetEdgeDriver()
        {
            if (driver == null)
            {
                EdgeOptions options = new EdgeOptions();
                if (browser["isHeadless"] == "True") { options.AddArguments("--headless"); }
                driver = new EdgeDriver(options);
                return driver;
            }
            else { return driver; }
        }

        private String ReportTitle()
        { return dateTime + " - Automation Report - Chrome"; }

        public MediaEntityModelProvider GetScreenshot()
        {
            string filePath = mainReportFolder + "\\" + ReportTitle() + "\\Screenshots\\";
            string screenshotName = TestContext.CurrentContext.Test.Name + ".png";
            Screenshot file = ((ITakesScreenshot)driver).GetScreenshot();
            file.SaveAsFile(filePath + screenshotName, ScreenshotImageFormat.Png);
            var screenshot = MediaEntityBuilder.CreateScreenCaptureFromPath("Screenshots/" + screenshotName).Build();
            return screenshot;
        }


        [OneTimeSetUp]
        public void RunBeforeSuite()
        {
            Directory.CreateDirectory(mainReportFolder);
            String reportFolder = mainReportFolder + "\\" + ReportTitle();
            Directory.CreateDirectory(reportFolder + "\\Screenshots");
            String reportFile = reportFolder + "\\" + ReportTitle() + ".html";
            ExtentHtmlReporter htmlReporter = new ExtentHtmlReporter(reportFile);
            htmlReporter.LoadConfig(Environment.CurrentDirectory + "\\Framework\\extent-config.xml");
            extent.AttachReporter(htmlReporter);
            extent.AddTestRunnerLogs(reportFile);
        }

        [SetUp]
        public void RunBeforeEachMethod()
        {
            test = extent.CreateTest(TestContext.CurrentContext.Test.Name);
            if (browser["webBrowser"] == "Chrome") { GetChromeDriver(); }
            else if (browser["webBrowser"] == "Firefox") { GetFirefoxDriver(); }
            else if (browser["webBrowser"] == "MSEdge") { GetEdgeDriver(); }
            driver.Manage().Cookies.DeleteAllCookies();
            driver.Manage().Window.Size = new Size(1920, 1080);
            driver.Navigate().GoToUrl(googleSite["base_url"].ToString());
        }

        [TearDown]
        public void RunAfterEachMethod()
        {
            string description = TestContext.CurrentContext.Test.Properties.Get("Description").ToString();

            switch (TestContext.CurrentContext.Result.Outcome.Status)
            {
                case TestStatus.Failed:
                    test.Log(Status.Fail, TestContext.CurrentContext.Result.StackTrace);
                    test.Log(Status.Fail, TestContext.CurrentContext.Result.Message);
                    test.Log(Status.Fail, description, GetScreenshot());
                    break;
                case TestStatus.Passed:
                    test.Log(Status.Pass, description);
                    break;
                default:
                    test.Log(Status.Info, description);
                    break;
            }

            extent.Flush();
            //This error appears if I quit the webDriver here:
            //System.ObjectDisposedException : Cannot access a disposed object.
            //Object name: 'System.Net.Http.HttpClient'.
            //driver?.Quit();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            extent.Flush();
            driver?.Quit();
        }
    }
}