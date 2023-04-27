using System;
using System.IO;
using System.Drawing;
using System.Configuration;
using System.Collections.Specialized;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Status = AventStack.ExtentReports.Status;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TestContext = NUnit.Framework.TestContext;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace CS_TestAutomation.Framework.Core
{
    public class BaseTest
    {
        public IWebDriver driver;
        public static String mainReportFolder = Path.Combine(Directory.GetParent(Directory.GetParent(
            Environment.CurrentDirectory).FullName).FullName, "ExtentReports");
        public String reportFolder = "";
        public NameValueCollection googleSite = (NameValueCollection)ConfigurationManager.GetSection("googleSite");
        public NameValueCollection browser = (NameValueCollection)ConfigurationManager.GetSection("browser");
        public static String dateTime = DateTime.Now.ToString("yyyy-MM-dd HH-mm-tt");
        public static String extentFolder = @"./../ExtentReports/";
        public static ExtentReports extent = new ExtentReports();
        public static ExtentTest test;
        public static bool OneRunFlag = true;

        public IWebDriver GetWebDriver()
        {
            if (browser["webBrowser"] == "Chrome")
            {
                ChromeOptions options = new ChromeOptions();
                options.AddUserProfilePreference("download.default_directory", reportFolder + "\\TestDownloadFolder");
                if (browser["isHeadless"] == "True") { options.AddArguments("--headless=new", "--no-sandbox"); }
                driver = new ChromeDriver(options);
            }
            else if (browser["webBrowser"] == "Firefox")
            {
                FirefoxOptions options = new FirefoxOptions();
                if (browser["isHeadless"] == "True") { options.AddArguments("--headless"); }
                driver = new FirefoxDriver(options);
            }
            else if (browser["webBrowser"] == "MSEdge")
            {
                EdgeOptions options = new EdgeOptions();
                if (browser["isHeadless"] == "True") { options.AddArguments("--headless"); }
                driver = new EdgeDriver(options);
            }
            return driver;
        }

        private String ReportTitle()
        { return dateTime + " - " + TestEnvironment() + " - Automation Report - " + browser["webBrowser"]; }

        private String TestEnvironment()
        {
            String env = "Development";
            //if (     env["base_url"].Contains(env["dev"]))     { env = "Development"; }
            //else if (env["base_url"].Contains(env["staging"])) { env = "Staging"; }
            //else if (env["base_url"].Contains(env["prod"]))    { env = "Production"; }
            return env;
        }

        public MediaEntityModelProvider GetErrorScreenshot()
        {
            string screenshotName = TestContext.CurrentContext.Test.Name + ".png";
            string filePath = mainReportFolder + "\\" + ReportTitle() + "\\Screenshots\\";
            Screenshot file = ((ITakesScreenshot)driver).GetScreenshot();
            file.SaveAsFile(filePath + screenshotName, ScreenshotImageFormat.Png);
            var screenshot = MediaEntityBuilder.CreateScreenCaptureFromPath("Screenshots\\" + screenshotName).Build();
            return screenshot;
        }

        [OneTimeSetUp]
        public void RunBeforeSuite()
        {
            if (OneRunFlag)
            {   //This method is SUPPOSED to be run ONCE only. (This is my hacky way around NUnits stupid framework.)
                Directory.CreateDirectory(mainReportFolder);
                reportFolder = mainReportFolder + "\\" + ReportTitle();
                Directory.CreateDirectory(reportFolder);
                Directory.CreateDirectory(reportFolder + "\\Screenshots");
                String reportFile = reportFolder + "\\" + ReportTitle() + ".html";
                ExtentHtmlReporter htmlReporter = new ExtentHtmlReporter(reportFile);
                htmlReporter.LoadConfig(Environment.CurrentDirectory + "\\Framework\\extent-config.xml");
                extent.AttachReporter(htmlReporter);
                extent.AddTestRunnerLogs(reportFile);
                OneRunFlag = false;
            }
        }

        [SetUp]
        public void RunBeforeEachTest()
        {
            test = extent.CreateTest(TestContext.CurrentContext.Test.Name);
            GetWebDriver();
            driver.Manage().Cookies.DeleteAllCookies();
            driver.Manage().Window.Size = new Size(1920, 1080);
            driver.Navigate().GoToUrl(googleSite["base_url"].ToString());
        }

        [TearDown]
        public void RunAfterEachTest()
        {
            string description = TestContext.CurrentContext.Test.Properties.Get("Description").ToString();
            switch (TestContext.CurrentContext.Result.Outcome.Status)
            {
                case TestStatus.Failed:
                    test.Log(Status.Fail, TestContext.CurrentContext.Result.StackTrace);
                    test.Log(Status.Fail, TestContext.CurrentContext.Result.Message);
                    test.Log(Status.Fail, description, GetErrorScreenshot());
                    break;
                case TestStatus.Passed:
                    test.Log(Status.Pass, description);
                    break;
                case TestStatus.Skipped:
                    test.Log(Status.Skip, description);
                    break;
                default:
                    test.Log(Status.Info, description);
                    break;
            }
            extent.Flush();
            driver.Quit();
        }

        [OneTimeTearDown]
        public void RunAfterSuite()
        {
            extent.Flush();
            driver.Quit();
        }
    }
}