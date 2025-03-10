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
using NUnit.Framework.Internal;


namespace CS_TestAutomation.Framework.Core
{
    public class BaseTest
    {
        public IWebDriver driver;
        public static String mainReportFolder = Path.Combine(Directory.GetParent(Directory.GetParent(
            Environment.CurrentDirectory).FullName).FullName, "ExtentReports");
        public static String reportFolder = "";
        public static NameValueCollection BaseConfig = (NameValueCollection)ConfigurationManager.GetSection("BaseConfig");
        public static NameValueCollection GoogleTest = (NameValueCollection)ConfigurationManager.GetSection("GoogleTest");
        public static NameValueCollection EnvConfig;
        public static String dateTime = DateTime.Now.ToString("yyyy-MM-dd HH-mm-tt");
        public static String extentFolder = @"./../ExtentReports/";
        public static ExtentReports extent = new ExtentReports();
        public static ExtentTest test;
        public static bool OneRunFlag = true;
        public static int WebDriverWaitTime = 60;  //One Minute Timer
        public static string AutoFileType = "";

        public void SetEnvConfig()
        {  //Overwrites EnvConfig to whatever environment you select
            if (BaseConfig["testSuite"].Contains("Google")) { EnvConfig = GoogleTest; }
            //Placeholder!
            else { EnvConfig = BaseConfig; }
            //else if (BaseConfig["base_url"].Contains("SomeOtherConfig")) { EnvConfig = SomeOtherConfig; }
        }

        //Connection Strings: Ignore this. These are for fancy people with databases only, no looky!
        //This is useful if you want to generate test data from an existing database.
        //You'll probably have to create some sprocs for it if you want to go that route.
        //I used it for awhile, but decided that generating test data on the fly was a more flexible approach.
        //public string MagicDataConnStuff()
        //{
        //    string dataConn = @"server=" + EnvConfig["server"] + ";"
        //                     + "database=" + EnvConfig["database"] + ";"
                                //Add Crytography for EXTRA fun
        //                     + "user=" + Cryptography.Decrypt(EnvConfig["CryptoUser"]) + ";"
        //                     + "password=" + EnvConfig["CryptoPassword"] + ";"
        //                     + "Application Name=" + EnvConfig["ApplicationName"] + ";"
        //                     + "Connection Timeout=" + EnvConfig["ConnectionTimeout"] + ";";
        //    return dataConn;
        //}

        public IWebDriver GetWebDriver()
        {
            if (BaseConfig["webBrowser"] == "Chrome")
            {
                ChromeOptions options = new ChromeOptions();
                options.AddUserProfilePreference("download.default_directory", reportFolder + "\\TestDownloadFolder");
                if (BaseConfig["isHeadless"] == "True") { options.AddArguments("--headless=new", "--no-sandbox"); }
                driver = new ChromeDriver(options);
            }
            else if (BaseConfig["webBrowser"] == "Firefox")
            {
                FirefoxOptions options = new FirefoxOptions();
                if (BaseConfig["isHeadless"] == "True") { options.AddArguments("--headless"); }
                driver = new FirefoxDriver(options);
            }
            else if (BaseConfig["webBrowser"] == "MSEdge")
            {
                EdgeOptions options = new EdgeOptions();
                if (BaseConfig["isHeadless"] == "True") { options.AddArguments("--headless"); }
                driver = new EdgeDriver(options);
            }
            return driver;
        }

        private String ReportTitle()
        {
            return dateTime + " - " + EnvConfig["env"] + " - Automation Report - " + EnvConfig["webBrowser"];
        }

        public MediaEntityModelProvider GetErrorScreenshot()
        {
            string screenshotName = TestContext.CurrentContext.Test.Name + ".png";
            string filePath = mainReportFolder + "\\" + ReportTitle() + "\\Screenshots\\";
            Screenshot file = ((ITakesScreenshot)driver).GetScreenshot();
            file.SaveAsFile(filePath + screenshotName);
            var screenshot = MediaEntityBuilder.CreateScreenCaptureFromPath("Screenshots\\" + screenshotName).Build();
            return screenshot;
        }

        [OneTimeSetUp]
        public void RunBeforeSuite()
        {
            SetEnvConfig();
            if (OneRunFlag)
            {   //This method is SUPPOSED to be run ONCE only. (This is a hacky way around NUnits stupid framework.)
                Directory.CreateDirectory(mainReportFolder);
                reportFolder = mainReportFolder + "\\" + ReportTitle();
                Directory.CreateDirectory(reportFolder);
                Directory.CreateDirectory(reportFolder + "\\Screenshots");
                Directory.CreateDirectory(reportFolder + "\\AutoGeneratedFiles");
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
            driver.Navigate().GoToUrl(BaseConfig["base_url"].ToString());
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
            if (driver != null)
            {
                try { driver.Close(); }
                catch { }
                driver.Quit();
            }
        }
    }
}