using AventStack.ExtentReports;
using CS_TestAutomation.Framework.Core;
using NUnit.Framework;

namespace CS_TAF_v1.RegressionTests
{
    internal class TestClassA : BaseTest
    {
        [Test][Description("01. Unit Test A.")]
        public void UnitTestA()
        {
            Assert.AreEqual(1, 1);
            test.Log(Status.Pass, "Unit Test A Passes.");
        }
    }
}