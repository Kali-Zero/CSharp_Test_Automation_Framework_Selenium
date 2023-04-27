C# Selenium Test Automation Framework
This is a Work-In-Progress project using:
.NET Framework 4.8 (will update to 6... at some point)
NUnit - https://nunit.org/
Selenium - https://www.seleniumhq.org/docs/
Extent Reports 3 - https://www.extentreports.com/docs/versions/3/net/index.html#basic-usage
A hot garbage test suite maintained by Calista.Sullivan@gmail.com

Note on Extent Reports:
I used 3 instead of 4 because it had no dependances and I wanted to keep the framework 'light'
After 3, it starts requiring a LOT of dependancy libraries.

The report will be output to the ExtentReport folder in the main project, NOT the bin folder.
I didn't want results to be overridden after each run.
Will periodically save them elsewhere until I figure out a better option.

Current Issues:
Duplicate Screenshots appear in the report if a test fails