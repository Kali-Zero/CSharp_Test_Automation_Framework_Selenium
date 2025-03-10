﻿using AventStack.ExtentReports;
using CS_TestAutomation.Framework.Core;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using NUnit.Framework;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_TestAutomation.Framework
{
    class TestDataGenerator : BaseTest
    {
        public static Random random = new Random();

        //Move some of these to config file?
        public StringBuilder dataWriter = new StringBuilder();
        public string filename = "";
        public string header = "";
        public string dataRows = "";
        public string defaultPoint = "";
        public double defaultLat = 0;
        public double defaultLng = 0;
        public int AddXHours = 0;
        public bool FirstRun = true;
        public Random rnd = new Random();
        public int monthIterator = 1;
        public int hourIterator = -1;
        public int baseDataValue = 5000;
        public string rngLng = "";
        public string rngLat = "";
        public static string fileLocation = "";
        public ZipArchiveEntry ZipFile;

        public String FileLocation()
        {
            if (AutoFileType == "TestDataFileType1") 
            { 
                fileLocation = reportFolder + "\\AutoGeneratedFiles\\TestDataFileType1.csv"; 
            }
            else if (AutoFileType == "TestDataFileTypeTwo") 
            { 
                fileLocation = reportFolder + "\\AutoGeneratedFiles\\TestDataFileType2.csv"; 
            }
            return fileLocation;
        }

        public void FileWriter()
        {
            string filepath = reportFolder + "\\AutoGeneratedFiles";
            CreateFileNamesAndHeader();
            var writer = new StreamWriter(filename);
            dataWriter.Append(header + "\n").ToString();
            CreateExampleLocationDataRows();
            dataWriter.Append(dataRows).ToString();
            File.WriteAllText(filepath + "\\" + filename, dataWriter.ToString());
            writer.Close();
            Assert.That(File.Exists(filename));
            test.Log(Status.Pass, "File successfully created: " + filename);
        }

        //This will come in handy when you need to unzip a file first. Not really useful here though.
        //Still good for reference,though... might add other archive file types later (7zip, rar, etc...)
        public string IfZippedThenUnZip()
        {
            var downloads = new DirectoryInfo(reportFolder + "\\AutoGeneratedFiles");
            var zippedDownloads = new DirectoryInfo(reportFolder + "\\AutoGeneratedFiles\\ZippedFiles");
            String filename = downloads.GetFiles().OrderByDescending(file => file.LastWriteTime).First().Name;
            if (filename.EndsWith(".zip"))
            {
                using (ZipArchive archive = System.IO.Compression.ZipFile.OpenRead(reportFolder + "\\AutoGeneratedFiles" + "\\" + filename))
                {
                    foreach (ZipArchiveEntry ZipFile in archive.Entries)
                    {
                        if (!File.Exists(Path.Combine(reportFolder + "\\AutoGeneratedFiles", ZipFile.FullName)))
                        {   // Make sure file doesn't already exist ( Certain reports use the same files )
                            ZipFile.ExtractToFile(Path.Combine(reportFolder + "\\AutoGeneratedFiles", ZipFile.FullName));
                        }
                        else
                        {   // Rename duplicate unzipped file
                            ZipFile.ExtractToFile(Path.Combine(reportFolder + "\\AutoGeneratedFiles", dateTime + " - " + ZipFile.FullName));
                        }
                    }
                }
                //Moves the zipped files into their own file... because I like it that way.
                var ZippedFile = downloads.GetFiles().Where(file => file.FullName.EndsWith(".zip"))
                    .OrderByDescending(file => file.LastWriteTime).First();
                if (!File.Exists(downloads + "\\ZippedFiles\\" + ZippedFile.Name))
                {
                    File.Move(downloads + "\\" + ZippedFile.Name,
                        downloads + "\\ZippedFiles\\" + ZippedFile.Name);
                }
                else
                {
                    File.Move(downloads + "\\" + ZippedFile.Name,
                        downloads + "\\ZippedFiles\\" + dateTime + " - " + ZippedFile.Name);
                }

                filename = downloads.GetFiles().OrderByDescending(file => file.LastWriteTime).First().Name;
                test.Log(Status.Info, "Unzipped Test File: " + filename);
            }
            return filename;
        }

        //Yeah, it's a bit hackey, but it's better than nothing ;-)
        public void DoesFileHaveData()  //DoesFileHaveData(string filename)
        {
            bool DataOrNoData = false;
            IfZippedThenUnZip();
            var downloads = new DirectoryInfo(reportFolder + "\\AutoGeneratedFiles");
            filename = downloads.GetFiles().OrderByDescending(file => file.LastWriteTime).First().FullName;

            //This line feels wrong, but I can't tell you why?
            string CheckFileExt = Path.GetExtension(Path.Combine(downloads.FullName, filename)).ToLower();

            if (CheckFileExt == ".csv")
            {
                string[] lines = File.ReadAllLines(Path.Combine(downloads.FullName, filename));
                test.Log(Status.Info, "Number of rows = " + lines.Length);
                if (lines.Length >= 2) { DataOrNoData = true; }
            }
            else if (CheckFileExt == ".xlsx")
            {
                using (SpreadsheetDocument testFile = SpreadsheetDocument.Open(filename, false))
                {
                    WorkbookPart workbookPart = testFile.WorkbookPart;
                    Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault();
                    Worksheet firstWorksheet = ((WorksheetPart)workbookPart.GetPartById(sheet.Id)).Worksheet;
                    WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                    Cell cell = worksheetPart.Worksheet.Descendants<Cell>().
                        Where(c => c.CellReference == "B2").FirstOrDefault();
                    string value1 = cell.InnerText.ToString();
                    if (value1 != null & cell.InnerText != "") { DataOrNoData = true; }
                    test.Log(Status.Info, "Cell B2 = " + value1);
                }
            }
            //Future options:
            //else if (filename.EndsWith(".pdf")) { }
            //Check the file for data
            Assert.That(DataOrNoData, "File does not contain data! Booooo!");
            test.Log(Status.Pass, "File contains data. *YaY*");
        }

        public string CreateRandomDataValue()
        {
            //Keep the data variation low, to avoid outliers.
            //This is set to plus or minus 1%, plus some decimals for a little extra variation.
            //This seems like a weirdly specific thing to do, unless you work in data science and analytics. ;-)
            if (rnd.Next(0, 2) == 0) { baseDataValue = baseDataValue + (baseDataValue * 1 / 100); }
            else { baseDataValue = baseDataValue - (baseDataValue * 1 / 100); }
            return baseDataValue.ToString() + "." + rnd.Next(0, 9999).ToString();
        }

        //This is for cartography stuff.
        public string GenerateDefaultGeometryPoint(double geoOffset)
        {
            //These values may need a little tweaking to keep it in the continental US...
            //Latitude  needs to be between -90 and 90, Longitude needs to be between -180 and 180
            //Order is: Long, THEN Lat - I don't know why. 
            if (FirstRun)
            {
                rngLng = rnd.Next(-120, -110).ToString() +
                    "." + rnd.Next(000000, 999999).ToString();
                rngLat = rnd.Next(20, 40).ToString() +
                    "." + rnd.Next(000000, 999999).ToString();
                //Generate "DEFAULTPOINT":
                defaultLng = double.Parse(rngLng) + geoOffset;
                defaultLat = double.Parse(rngLat) + geoOffset;
                defaultPoint = defaultLng.ToString() + " " + defaultLat.ToString();
                FirstRun = false;
            }
            else
            {
                defaultLng = double.Parse(rngLng) + geoOffset;
                defaultLat = double.Parse(rngLat) + geoOffset;
                defaultPoint = defaultLng.ToString() + " " + defaultLat.ToString();
            }
            return "DefaultPoint: " + defaultPoint;
        }

        //Also cartography stuff. Note to self: this makes a simple square; add more points later to make it a hexagon.
        public string GenerateExpandedGeometriesPoints(double geoOffset)
        {   //Ordered counterclockwise because reasons
            string northeast = (defaultLng + 1 + geoOffset).ToString() + " " + (defaultLat - 1 + geoOffset).ToString();
            string southeast = (defaultLng - 1 + geoOffset).ToString() + " " + (defaultLat - 1 + geoOffset).ToString();
            string northwest = (defaultLng + 1 + geoOffset).ToString() + " " + (defaultLat + 1 + geoOffset).ToString();
            string southwest = (defaultLng - 1 + geoOffset).ToString() + " " + (defaultLat + 1 + geoOffset).ToString();
            return "\"POLYGON ((" + northeast + ", " + southeast + ", " + southwest + ", " + northwest + ", " + northeast + "))\"";
        }

        public void CreateFileNamesAndHeader()
        {
            if (AutoFileType == "TestDataFileType1")
            {
                filename = "TestDataFileType1.csv";
                header = "Point Name,Point Info,Default Location,Geometry";
            }
            else if (AutoFileType == "TestDataFileType2")
            {
                filename = "TestDataFileType2.csv";
                header = "Column1,Column2,Column3,Column4,Column5,Column6,Column7,Column8";
            }
        }

        public string DateTimeIterator()
        {
            //Start with a dateTimeStamp, and iterate it, incrementing by one hour
            //for a total of ONE year (8760 total iterations)
            DateTime startDateTime = new DateTime(2021, 01, 01, 00, 00, 00);
            DateTime currentIteration = startDateTime.AddHours(AddXHours);
            AddXHours++;

            if (AutoFileType == "TestDataFileType1")
            {
                return currentIteration.ToString("MM/dd/yyyy HH:mm tt");
            }
            else if (AutoFileType == "TestDataFileType2")
            {
                //Iterate 1->12 with 24 hours in each
                hourIterator++;
                if (hourIterator > 23)
                {
                    monthIterator++;
                    hourIterator = 0;
                }
                if (monthIterator > 12 && hourIterator < 23)
                {
                    monthIterator = 1;
                    hourIterator = 0;
                }
                return monthIterator.ToString() + "," + hourIterator.ToString();
            }
            else 
            { 
                return "No Timestamp for you!"; 
            }
        }

        public void CreateExampleLocationDataRows() 
        {
            if (AutoFileType == "TestDataFileType1")
            {
                //Maybe I'll make this a for loop to add more data points? Ponder it! Or don't. I'm not the boss of you.
                //Creates connected geometry points
                dataRows =
                    "DataPoint1" + "," + "DataPoint1Info" + "," +
                    GenerateDefaultGeometryPoint(0.0) + "," + GenerateExpandedGeometriesPoints(0.0) + "\n" +
                    "DataPoint2" + "," + "DataPoint2Info" + "," +
                    GenerateDefaultGeometryPoint(0.1) + "," + GenerateExpandedGeometriesPoints(0.1) + "\n" +
                    "DataPoint3" + "," + "DataPoint3Info" + "," +
                    GenerateDefaultGeometryPoint(0.2) + "," + GenerateExpandedGeometriesPoints(0.2) + "\n" +
                    "DataPoint4" + "," + "DataPoint4Info" + "," +
                    GenerateDefaultGeometryPoint(0.3) + "," + GenerateExpandedGeometriesPoints(0.3) + "\n" +
                    "DataPoint5" + "," + "DataPoint5Info" + "," +
                    GenerateDefaultGeometryPoint(0.4) + "," + GenerateExpandedGeometriesPoints(0.4);
            }
            else
            {
                AssembleTwentryRowsOfGarbage();
            }

        }

        public string GenerateDefaultGeometryPoints(double geoOffset)
        {
            //These values still need tweaking to keep it on land in America...
            //Latitude  needs to be between -90 and 90, Longitude needs to be between -180 and 180
            //Order is: Long, THEN Lat
            if (FirstRun)
            {
                rngLng = rnd.Next(-120, -110).ToString() +
                    "." + rnd.Next(000000, 999999).ToString();
                rngLat = rnd.Next(20, 40).ToString() +
                    "." + rnd.Next(000000, 999999).ToString();
                //Generate "DEFAULTPOINT":
                defaultLng = double.Parse(rngLng) + geoOffset;
                defaultLat = double.Parse(rngLat) + geoOffset;
                defaultPoint = defaultLng.ToString() + " " + defaultLat.ToString();
                FirstRun = false;
            }
            else
            {
                defaultLng = double.Parse(rngLng) + geoOffset;
                defaultLat = double.Parse(rngLat) + geoOffset;
                defaultPoint = defaultLng.ToString() + " " + defaultLat.ToString();
            }
            return "\"POINT (" + defaultPoint + ")\"";
        }

        //==================================================================================

        public static string CreateGarbage()
        {   //Creates a garbage data point with an arbitrary length of 10
            const string garbage = "abcdefghijklmnopqrstuvwxyz.ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(garbage, 10).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void AssembleTwentryRowsOfGarbage()
        {
            var stupidData = new StringBuilder();
            string filepath = reportFolder + "\\AutoGeneratedFiles";
            //Determines how many columns of garbage data are needed
            char[] delimiters = new char[] { ',' };
            int headerLength = header.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
            //Assembles 20 rows of garbage data points
            for (int i = 0; i < 20; i++)
            {
                //Creates garbage data for each column of the csv (based on the size of the header)
                for (int g = 0; g < headerLength; g++)
                {
                    if (g < headerLength - 1)
                    {
                        stupidData.Append(string.Format(CreateGarbage().ToString() + ",", "{" + g + "}"));
                    }
                    else
                    {
                        stupidData.Append(string.Format(CreateGarbage().ToString(), "{" + g + "}"));
                    }
                }
                stupidData.Append("\n").ToString();
            }
            dataRows = stupidData.Append("\n").ToString();
        }
    }
}
