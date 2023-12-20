using NUnit.Framework;
using AventStack.ExtentReports;
using CS_TestAutomation.Framework.Core;

namespace CS_TAF_v1.RegressionTests
{
    internal class TestClassB : BaseTest
    {
        [Test][Description("01. Unit Test B.")]
        public void UnitTestB()
        {
            Assert.That(1, Is.EqualTo(1), "Error, these are not equal!");
            test.Log(Status.Pass, "Unit Test B Passes.");
        }
    }
}