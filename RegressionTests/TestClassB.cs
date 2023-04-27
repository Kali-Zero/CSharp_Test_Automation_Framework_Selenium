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
            Assert.AreEqual(1, 1);
            test.Log(Status.Pass, "Unit Test B Passes.");
        }
    }
}