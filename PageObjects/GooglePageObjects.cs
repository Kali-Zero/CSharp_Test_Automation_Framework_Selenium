using OpenQA.Selenium;

namespace CSharp_Selenium_Test_Automation.PageObjects
{
    public class GooglePageObjects
    {
        public IWebElement About(IWebDriver driver) { return driver.FindElement(By.XPath("//*[text()='About']")); }
    }
}
