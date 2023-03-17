using OpenQA.Selenium;

namespace CS_TAF_v1.PageObjects
{
    public class GooglePageObjects
    {
        public IWebElement About(IWebDriver driver) { return driver.FindElement(By.XPath("//*[text()='About']")); }
    }
}
