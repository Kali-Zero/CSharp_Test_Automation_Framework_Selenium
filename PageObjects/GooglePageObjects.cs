using OpenQA.Selenium;

namespace CSharp_Selenium_Test_Automation.PageObjects
{
    public class GooglePageObjects
    {
        //Top 'Nav Bar' Items
        public IWebElement About(IWebDriver driver) { return driver.FindElement(By.XPath("//*[text()='About']")); }
        public IWebElement Store(IWebDriver driver) { return driver.FindElement(By.XPath("//*[text()='Store']")); }
        public IWebElement Gmail(IWebDriver driver) { return driver.FindElement(By.XPath("//*[text()='Gmail']")); }
        public IWebElement Images(IWebDriver driver) { return driver.FindElement(By.XPath("//*[text()='Images']")); }
        public IWebElement Apps(IWebDriver driver) { return driver.FindElement(By.XPath("//*[@title='Google apps']")); }
        public IWebElement SignIn(IWebDriver driver) { return driver.FindElement(By.XPath("//*[@role='navigation']//*[text()='Sign in']")); }

        //Apps menu elements
        public IWebElement AppsAccount(IWebDriver driver) { return driver.FindElement(By.XPath(
            "//*[@aria-label='Google apps']//*[text()='Account']")); }

        //Bottom 'Footer' Items
    }
}
