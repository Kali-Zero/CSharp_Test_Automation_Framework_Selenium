Selenium Test Automation Framework==============================

	A hot garbage automation suite created and maintained by Calista Sullivan
	This is a Work-In-Progress project using:
	.NET Framework 4.8
	NUnit - https://nunit.org/
	Selenium - https://www.seleniumhq.org/docs/
	Extent Reports 3 - https://www.extentreports.com/docs/versions/3/net/index.html

Nuget Package Notes=============================================
	
	Do NOT update: Extent Reports (current working version: 3.1.0)
	Do NOT update: Selenium.WebDriver (current working version: 4.16.2)
	Do NOT update: Selenium.Support (current working version: 4.16.2)

	Selenium gets periodic updates, but a lot of the time they are unstable
	Extent Reports gets updates occasionally, but require a LOT of dependencies,
	     and those dependencies have their own dependancies. In short, it adds
		 too many variables to the testing suite.

Extent Reports==================================================

	The report will be output to the ExtentReport folder in the main project, NOT the bin folder.
	I find it's easier to organize the reports this way.