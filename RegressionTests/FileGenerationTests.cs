using CS_TestAutomation.Framework;
using CS_TestAutomation.Framework.Core;
using NUnit.Framework;

namespace CS_TestAutomation.RegressionTests
{

    //This is just the file generation stuff. I'll put the freaky sql stuff in later.
    //I'll have to set up a database first, add the connection string garbage, and blah, blah, blah
    //Ok, so I forgot my heroku login, sue me.

    class FileGenerationTests : BaseTest
    {
        public TestDataGenerator TestDataGenerator = new TestDataGenerator();
        public static string fileLocation = "";

        [Test][Description("Can we generate a geolocation file?")]
        public void GeolocationGeneratorTest()
        {
            AutoFileType = "TestDataFileType1";
            //Create the File
            TestDataGenerator.FileWriter();
            //Now open it and make sure it's not empty!
            TestDataGenerator.DoesFileHaveData();
        }

        [Test][Description("Can we generate a junk file?")]
        public void JunkFileGeneratorTest()
        {
            AutoFileType = "TestDataFileType2";
            TestDataGenerator.FileWriter();
            TestDataGenerator.DoesFileHaveData();
        }
    }
}
