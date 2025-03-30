using InfoDroplets.ResultExporter.Models;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace InfoDroplets.ResultExporterApp.Tester
{
    [TestFixture]
    internal class LogProcessorTester
    {
        [Test, TestCaseSource(typeof(FilterABTestData), nameof(FilterABTestData.TestCases))]
        public void FilterABFilesTester(string TestCaseName, string[] paths, int? expectedFileCount, Type expectedException)
        {
            var lp = new LogProcessor();

            if (expectedException != null)
            {
                Assert.Throws(expectedException, () => lp.FilterABFiles(ref paths));
            }
            else
            {
                lp.FilterABFiles(ref paths);
                Assert.That(expectedFileCount, Is.EqualTo(paths.Count()));
            }
        }

        [Test, TestCaseSource(typeof(GetDropletNumberTestData), nameof(GetDropletNumberTestData.TestCases))]
        public void GetDropletNumberTester(string testCaseName, string path, int? expectedDropletNumber, Type expectedException)
        {
            var lp = new LogProcessor();

            if (expectedException != null)
            {
                Assert.Throws(expectedException, () => lp.GetDropletNumber(path));
            } 
            else
            {
                lp.GetDropletNumber(path);
                Assert.That(lp.DropletNumber, Is.EqualTo(expectedDropletNumber));
            }
        }

        [Test, TestCaseSource(typeof(ProcessFilesTestData), nameof(ProcessFilesTestData.TestCases))]
        public void ProcessFilesTester(string testCaseName, List<string[]> allFilesLines, int expectedLogSegmentCount, int expectedBreakSegmentCount, Type expectedException)
        {
            var lp = new LogProcessor();

            if (expectedException != null)
            {
                Assert.Throws(expectedException, () => lp.ProcessFiles(allFilesLines));
            }
            else
            {
                lp.ProcessFiles(allFilesLines);
                Assert.That(lp.LogCollection.Count, Is.EqualTo(expectedLogSegmentCount));
                Assert.That(lp.BreakCollection.Count, Is.EqualTo(expectedBreakSegmentCount));
            }
        }

        [Test, TestCaseSource(typeof(GetMapCenterPosTestData), nameof(GetMapCenterPosTestData.TestCases))]
        public void GetMapCenterPosTester(string testCaseName, List<string[]> allFilesLines, LogEntry? expectedCenterPos, Type expectedException)
        {
            var lp = new LogProcessor();
            lp.ProcessFiles(allFilesLines);

            if (expectedException != null)
            {
                Assert.Throws(expectedException, () => lp.GetMapCenterPos());
            }
            else
            {
                LogEntry centerPos = lp.GetMapCenterPos();
                Assert.That(centerPos, Is.EqualTo(expectedCenterPos));
            }
        }

        [Test, TestCaseSource(typeof(GetElevationLimitTestData), nameof(GetElevationLimitTestData.TestCases))]
        public void GetElevationLimitTester(string testCaseName, List<string[]> allFilesLines, int expectedElevationLimit, Type expectedException)
        {
            var lp = new LogProcessor();
            lp.ProcessFiles(allFilesLines);

            if (expectedException != null)
            {
                Assert.Throws(expectedException, () => lp.GetElevationLimit());
            }
            else
            {
                int elevationLimit = lp.GetElevationLimit();
                Assert.That(elevationLimit, Is.EqualTo(expectedElevationLimit));
            }
        }
    }

    internal class FilterABTestData
    {
        static string[] emptySdArray = new string[] { };
        static string[] ABLogsArray = new string[]
        {
                @"D:\UNI\_Szakdolgozat\TestData\L8_V0a.txt",
                @"D:\UNI\_Szakdolgozat\TestData\L8_V0b.txt",
                @"D:\UNI\_Szakdolgozat\TestData\L8_V1a.txt",
                @"D:\UNI\_Szakdolgozat\TestData\L8_V1b.txt",
                @"D:\UNI\_Szakdolgozat\TestData\L8_V2a.txt",
                @"D:\UNI\_Szakdolgozat\TestData\L8_V0b.txt"
        };
        static string[] ALogsArray = new string[]
        {
                @"D:\UNI\_Szakdolgozat\TestData\L8_V0a.txt",
                @"D:\UNI\_Szakdolgozat\TestData\L8_V1a.txt",
                @"D:\UNI\_Szakdolgozat\TestData\L8_V2a.txt"
        };
        static string[] BLogsArray = new string[]
        {
                @"D:\UNI\_Szakdolgozat\TestData\L8_V0b.txt",
                @"D:\UNI\_Szakdolgozat\TestData\L8_V1b.txt",
                @"D:\UNI\_Szakdolgozat\TestData\L8_V2b.txt"
        };
        static string[] RadioArray = new string[]
        {
                @"D:\UNI\_Szakdolgozat\TestData\R8_V0a.txt",
                @"D:\UNI\_Szakdolgozat\TestData\R8_V0b.txt",
                @"D:\UNI\_Szakdolgozat\TestData\R8_V1a.txt",
                @"D:\UNI\_Szakdolgozat\TestData\R8_V1b.txt",
                @"D:\UNI\_Szakdolgozat\TestData\R8_V2a.txt",
                @"D:\UNI\_Szakdolgozat\TestData\R8_V2b.txt"
        };
        static string[] RadioAndLogsArray = new string[]
        {
                @"D:\UNI\_Szakdolgozat\TestData\L8_V0a.txt",
                @"D:\UNI\_Szakdolgozat\TestData\L8_V0b.txt",
                @"D:\UNI\_Szakdolgozat\TestData\L8_V1a.txt",
                @"D:\UNI\_Szakdolgozat\TestData\L8_V1b.txt",
                @"D:\UNI\_Szakdolgozat\TestData\L8_V2a.txt",
                @"D:\UNI\_Szakdolgozat\TestData\L8_V2b.txt",
                @"D:\UNI\_Szakdolgozat\TestData\R8_V0a.txt",
                @"D:\UNI\_Szakdolgozat\TestData\R8_V0b.txt",
                @"D:\UNI\_Szakdolgozat\TestData\R8_V1a.txt",
                @"D:\UNI\_Szakdolgozat\TestData\R8_V1b.txt",
                @"D:\UNI\_Szakdolgozat\TestData\R8_V2a.txt",
                @"D:\UNI\_Szakdolgozat\TestData\R8_V0b.txt"
        };
        static string[] RandomFolderContentArray = new string[]
        {
            @"C:\Users\Matyi\Documents\powerRename.txt",
            @"C:\Users\Matyi\Documents\alma.png",
            @"C:\Users\Matyi\Documents\tiredboi.me",
        };

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { "RadioAndLogsArray", RadioAndLogsArray, 3, null }; //expected input
            yield return new object[] { "ABLogsArray", ABLogsArray, 3, null }; //realistic input
            yield return new object[] { "ALogsArray", ALogsArray, 3, null }; //realistic input
            yield return new object[] { "BLogsArray", BLogsArray, 3, null }; //realistic input
            yield return new object[] { "RadioArray", RadioArray, null, typeof(Exception) }; //expected error
            yield return new object[] { "emptySdArray", emptySdArray, null, typeof(Exception) }; //expected error
            yield return new object[] { "RandomFolderContentArray", RandomFolderContentArray, null, typeof(Exception) }; //expected error
        }
    }

    internal class GetDropletNumberTestData
    {
        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { "ValidAPath", @"D:\UNI\_Szakdolgozat\TestData\L8_V0a.txt", 8, null }; //expected input
            yield return new object[] { "ValidBPath", @"D:\UNI\_Szakdolgozat\TestData\L8_V0b.txt", 8, null }; //expected input
            yield return new object[] { "Valid2DigitVersionPath", @"D:\UNI\_Szakdolgozat\TestData\L1_V21a.txt", 1, null }; //expected input
            yield return new object[] { "Valid2DigitIdPath", @"D:\UNI\_Szakdolgozat\TestData\L10_V21a.txt", 10, null }; //expected input
            yield return new object[] { "Invalid3DigitIdAPath", @"D:\UNI\_Szakdolgozat\TestData\L100_V21a.txt", null, typeof(Exception) }; //expected to fail (too long id)
            yield return new object[] { "ValidRadioFilePath", @"D:\UNI\_Szakdolgozat\TestData\R10_V0a.txt", 10, null }; //expected to fail (radio)
            yield return new object[] { "InvalidRandomFilePath", @"D:\UNI\_Szakdolgozat\TestData\kicskacsa.quack", null, typeof(Exception) }; //expected to fail (not log)
        }
    }

    internal static class ProcessFilesTestData
    {
        static List<string[]> SingleFileNullContent = new List<string[]>
        {
            { "Developed by Soprobotics.\r\nCsepp number:;8\r\nCsepp version:;HighAlt Field Test\r\nRX:;D1\r\nTX:;D2\r\nGPS baud rate:;9600\r\nGPS Nav Mode:;6;Airborne under 1 g\r\nFile version:;0TinyGPS++ libary version:;1.0.3\r\n__________________________\r\nCseppID;Satellites;Time (GMT0);Latitude;Longitude;Altitude\r\n________;________;________;________;________;________\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;9:16:59;0.000000;0.000000;0.000000\r\n8;0;9:17:1;0.000000;0.000000;0.000000\r\n8;0;9:17:3;0.000000;0.000000;0.000000\r\n8;0;9:17:4;0.000000;0.000000;0.000000\r\n8;0;9:17:6;0.000000;0.000000;0.000000\r\n8;0;9:17:7;0.000000;0.000000;0.000000\r\n8;0;9:17:9;0.000000;0.000000;0.000000\r\n8;0;9:17:11;0.000000;0.000000;0.000000\r\n8;0;9:17:12;0.000000;0.000000;0.000000\r\n8;0;9:17:14;0.000000;0.000000;0.000000\r\n8;0;9:17:15;0.000000;0.000000;0.000000\r\n8;0;9:17:17;0.000000;0.000000;0.000000\r\n8;0;9:17:19;0.000000;0.000000;0.000000\r\n8;0;9:17:20;0.000000;0.000000;0.000000\r\n8;0;9:17:22;0.000000;0.000000;0.000000\r\n8;0;9:17:23;0.000000;0.000000;0.000000".Split("\r\n") }
        };
        static List<string[]> SingleFileRealContent = new List<string[]>
        {
            { "Developed by Soprobotics.\r\nCsepp number:;8\r\nCsepp version:; HighAlt Field Test\r\nRX:; D1\r\nTX:; D2\r\nGPS baud rate:;9600\r\nGPS Nav Mode:;6;Airborne under 1 g\r\nFile version:;0TinyGPS++ libary version:;1.0.3\r\n__________________________\r\nCseppID; Satellites;Time(GMT0); Latitude;Longitude;Altitude\r\n________;________;________;________;________;________\r\n8;6;9:20:40;1.11;1.22;1.33\r\n8;6;9:20:42;2.11;2.22;2.33\r\n8;6;9:20:42;3.11;3.22;3.33\r\n8;6;9:20:42;4.11;4.22;4.33".Split("\r\n") }
        };
        static List<string[]> MultipleFilesNullContent = new List<string[]>
        {
            { "Developed by Soprobotics.\r\nCsepp number:;8\r\nCsepp version:;HighAlt Field Test\r\nRX:;D1\r\nTX:;D2\r\nGPS baud rate:;9600\r\nGPS Nav Mode:;6;Airborne under 1 g\r\nFile version:;0TinyGPS++ libary version:;1.0.3\r\n__________________________\r\nCseppID;Satellites;Time (GMT0);Latitude;Longitude;Altitude\r\n________;________;________;________;________;________\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;9:16:59;0.000000;0.000000;0.000000\r\n8;0;9:17:1;0.000000;0.000000;0.000000\r\n8;0;9:17:3;0.000000;0.000000;0.000000\r\n8;0;9:17:4;0.000000;0.000000;0.000000\r\n8;0;9:17:6;0.000000;0.000000;0.000000\r\n8;0;9:17:7;0.000000;0.000000;0.000000\r\n8;0;9:17:9;0.000000;0.000000;0.000000\r\n8;0;9:17:11;0.000000;0.000000;0.000000\r\n8;0;9:17:12;0.000000;0.000000;0.000000\r\n8;0;9:17:14;0.000000;0.000000;0.000000\r\n8;0;9:17:15;0.000000;0.000000;0.000000\r\n8;0;9:17:17;0.000000;0.000000;0.000000\r\n8;0;9:17:19;0.000000;0.000000;0.000000\r\n8;0;9:17:20;0.000000;0.000000;0.000000\r\n8;0;9:17:22;0.000000;0.000000;0.000000\r\n8;0;9:17:23;0.000000;0.000000;0.000000".Split("\r\n") },
            { "Developed by Soprobotics.\r\nCsepp number:;8\r\nCsepp version:;HighAlt Field Test\r\nRX:;D1\r\nTX:;D2\r\nGPS baud rate:;9600\r\nGPS Nav Mode:;6;Airborne under 1 g\r\nFile version:;0TinyGPS++ libary version:;1.0.3\r\n__________________________\r\nCseppID;Satellites;Time (GMT0);Latitude;Longitude;Altitude\r\n________;________;________;________;________;________\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;9:16:59;0.000000;0.000000;0.000000\r\n8;0;9:17:1;0.000000;0.000000;0.000000\r\n8;0;9:17:3;0.000000;0.000000;0.000000\r\n8;0;9:17:4;0.000000;0.000000;0.000000\r\n8;0;9:17:6;0.000000;0.000000;0.000000\r\n8;0;9:17:7;0.000000;0.000000;0.000000\r\n8;0;9:17:9;0.000000;0.000000;0.000000\r\n8;0;9:17:11;0.000000;0.000000;0.000000\r\n8;0;9:17:12;0.000000;0.000000;0.000000\r\n8;0;9:17:14;0.000000;0.000000;0.000000\r\n8;0;9:17:15;0.000000;0.000000;0.000000\r\n8;0;9:17:17;0.000000;0.000000;0.000000\r\n8;0;9:17:19;0.000000;0.000000;0.000000\r\n8;0;9:17:20;0.000000;0.000000;0.000000\r\n8;0;9:17:22;0.000000;0.000000;0.000000\r\n8;0;9:17:23;0.000000;0.000000;0.000000".Split("\r\n") },
            { "Developed by Soprobotics.\r\nCsepp number:;8\r\nCsepp version:;HighAlt Field Test\r\nRX:;D1\r\nTX:;D2\r\nGPS baud rate:;9600\r\nGPS Nav Mode:;6;Airborne under 1 g\r\nFile version:;0TinyGPS++ libary version:;1.0.3\r\n__________________________\r\nCseppID;Satellites;Time (GMT0);Latitude;Longitude;Altitude\r\n________;________;________;________;________;________\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;9:16:59;0.000000;0.000000;0.000000\r\n8;0;9:17:1;0.000000;0.000000;0.000000\r\n8;0;9:17:3;0.000000;0.000000;0.000000\r\n8;0;9:17:4;0.000000;0.000000;0.000000\r\n8;0;9:17:6;0.000000;0.000000;0.000000\r\n8;0;9:17:7;0.000000;0.000000;0.000000\r\n8;0;9:17:9;0.000000;0.000000;0.000000\r\n8;0;9:17:11;0.000000;0.000000;0.000000\r\n8;0;9:17:12;0.000000;0.000000;0.000000\r\n8;0;9:17:14;0.000000;0.000000;0.000000\r\n8;0;9:17:15;0.000000;0.000000;0.000000\r\n8;0;9:17:17;0.000000;0.000000;0.000000\r\n8;0;9:17:19;0.000000;0.000000;0.000000\r\n8;0;9:17:20;0.000000;0.000000;0.000000\r\n8;0;9:17:22;0.000000;0.000000;0.000000\r\n8;0;9:17:23;0.000000;0.000000;0.000000".Split("\r\n") },
            { "Developed by Soprobotics.\r\nCsepp number:;8\r\nCsepp version:;HighAlt Field Test\r\nRX:;D1\r\nTX:;D2\r\nGPS baud rate:;9600\r\nGPS Nav Mode:;6;Airborne under 1 g\r\nFile version:;0TinyGPS++ libary version:;1.0.3\r\n__________________________\r\nCseppID;Satellites;Time (GMT0);Latitude;Longitude;Altitude\r\n________;________;________;________;________;________\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;0:0:0;0.000000;0.000000;0.000000\r\n8;0;9:16:59;0.000000;0.000000;0.000000\r\n8;0;9:17:1;0.000000;0.000000;0.000000\r\n8;0;9:17:3;0.000000;0.000000;0.000000\r\n8;0;9:17:4;0.000000;0.000000;0.000000\r\n8;0;9:17:6;0.000000;0.000000;0.000000\r\n8;0;9:17:7;0.000000;0.000000;0.000000\r\n8;0;9:17:9;0.000000;0.000000;0.000000\r\n8;0;9:17:11;0.000000;0.000000;0.000000\r\n8;0;9:17:12;0.000000;0.000000;0.000000\r\n8;0;9:17:14;0.000000;0.000000;0.000000\r\n8;0;9:17:15;0.000000;0.000000;0.000000\r\n8;0;9:17:17;0.000000;0.000000;0.000000\r\n8;0;9:17:19;0.000000;0.000000;0.000000\r\n8;0;9:17:20;0.000000;0.000000;0.000000\r\n8;0;9:17:22;0.000000;0.000000;0.000000\r\n8;0;9:17:23;0.000000;0.000000;0.000000".Split("\r\n") }
        };
        static List<string[]> MultipleFilesRealContent = new List<string[]>
        {
            { "Developed by Soprobotics.\r\nCsepp number:;8\r\nCsepp version:;HighAlt Field Test\r\nRX:;D1\r\nTX:;D2\r\nGPS baud rate:;9600\r\nGPS Nav Mode:;6;Airborne under 1 g\r\nFile version:;0TinyGPS++ libary version:;1.0.3\r\n__________________________\r\nCseppID;Satellites;Time (GMT0);Latitude;Longitude;Altitude\r\n________;________;________;________;________;________\r\n8;1;0:0:0;1.11;1.22;1.33\r\n8;1;0:0:0;2.11;2.22;2.33\r\n8;1;0:0:0;3.11;3.22;3.33".Split("\r\n") },        
            { "Developed by Soprobotics.\r\nCsepp number:;8\r\nCsepp version:;HighAlt Field Test\r\nRX:;D1\r\nTX:;D2\r\nGPS baud rate:;9600\r\nGPS Nav Mode:;6;Airborne under 1 g\r\nFile version:;0TinyGPS++ libary version:;1.0.3\r\n__________________________\r\nCseppID;Satellites;Time (GMT0);Latitude;Longitude;Altitude\r\n________;________;________;________;________;________\r\n8;2;0:0:0;1.11;1.22;1.33\r\n8;2;0:0:0;2.11;2.22;2.33\r\n8;2;0:0:0;3.11;3.22;3.33".Split("\r\n") },        
            { "Developed by Soprobotics.\r\nCsepp number:;8\r\nCsepp version:;HighAlt Field Test\r\nRX:;D1\r\nTX:;D2\r\nGPS baud rate:;9600\r\nGPS Nav Mode:;6;Airborne under 1 g\r\nFile version:;0TinyGPS++ libary version:;1.0.3\r\n__________________________\r\nCseppID;Satellites;Time (GMT0);Latitude;Longitude;Altitude\r\n________;________;________;________;________;________\r\n8;3;0:0:0;1.11;1.22;1.33\r\n8;3;0:0:0;2.11;2.22;2.33\r\n8;3;0:0:0;3.11;3.22;3.33".Split("\r\n") },        
            { "Developed by Soprobotics.\r\nCsepp number:;8\r\nCsepp version:;HighAlt Field Test\r\nRX:;D1\r\nTX:;D2\r\nGPS baud rate:;9600\r\nGPS Nav Mode:;6;Airborne under 1 g\r\nFile version:;0TinyGPS++ libary version:;1.0.3\r\n__________________________\r\nCseppID;Satellites;Time (GMT0);Latitude;Longitude;Altitude\r\n________;________;________;________;________;________\r\n8;4;0:0:0;1.11;1.22;4.33\r\n8;4;0:0:0;2.11;2.22;2.33\r\n8;4;0:0:0;3.11;3.22;3.33".Split("\r\n") },        
        };
        static List<string[]> MultipleFilesNullAndRealContent = new List<string[]>
        {
            { "Developed by Soprobotics.\r\nCsepp number:;8\r\nCsepp version:;HighAlt Field Test\r\nRX:;D1\r\nTX:;D2\r\nGPS baud rate:;9600\r\nGPS Nav Mode:;6;Airborne under 1 g\r\nFile version:;0TinyGPS++ libary version:;1.0.3\r\n__________________________\r\nCseppID;Satellites;Time (GMT0);Latitude;Longitude;Altitude\r\n________;________;________;________;________;________\r\n8;1;0:0:0;1.11;1.22;1.33\r\n8;1;0:0:0;2.11;2.22;2.33\r\n8;1;0:0:0;3.11;3.22;3.33".Split("\r\n") },
            { "Developed by Soprobotics.\r\nCsepp number:;8\r\nCsepp version:;HighAlt Field Test\r\nRX:;D1\r\nTX:;D2\r\nGPS baud rate:;9600\r\nGPS Nav Mode:;6;Airborne under 1 g\r\nFile version:;0TinyGPS++ libary version:;1.0.3\r\n__________________________\r\nCseppID;Satellites;Time (GMT0);Latitude;Longitude;Altitude\r\n________;________;________;________;________;________\r\n8;2;0:0:0;1.11;1.22;1.33\r\n8;2;0:0:0;2.11;2.22;2.33\r\n8;2;0:0:0;3.11;3.22;3.33".Split("\r\n") },
            { "Developed by Soprobotics.\r\nCsepp number:;8\r\nCsepp version:;HighAlt Field Test\r\nRX:;D1\r\nTX:;D2\r\nGPS baud rate:;9600\r\nGPS Nav Mode:;6;Airborne under 1 g\r\nFile version:;0TinyGPS++ libary version:;1.0.3\r\n__________________________\r\nCseppID;Satellites;Time (GMT0);Latitude;Longitude;Altitude\r\n________;________;________;________;________;________\r\n8;3;0:0:0;0.00;0.00;0.00\r\n8;3;0:0:0;0.00;0.00;0.00\r\n8;3;0:0:0;0.00;0.00;0.00".Split("\r\n") },
            { "Developed by Soprobotics.\r\nCsepp number:;8\r\nCsepp version:;HighAlt Field Test\r\nRX:;D1\r\nTX:;D2\r\nGPS baud rate:;9600\r\nGPS Nav Mode:;6;Airborne under 1 g\r\nFile version:;0TinyGPS++ libary version:;1.0.3\r\n__________________________\r\nCseppID;Satellites;Time (GMT0);Latitude;Longitude;Altitude\r\n________;________;________;________;________;________\r\n8;4;0:0:0;1.11;1.22;4.33\r\n8;4;0:0:0;2.11;2.22;2.33\r\n8;4;0:0:0;3.11;3.22;3.33".Split("\r\n") },
        };

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { "SingleFileRealContent", SingleFileRealContent, 1, 0, null }; //expected input
            yield return new object[] { "SingleFileNullContent", SingleFileNullContent, 0, 0, typeof(Exception) }; //expected input
            yield return new object[] { "MultipleFilesNullContent", MultipleFilesNullContent, 0, 0, typeof(Exception) }; //expected input
            yield return new object[] { "MultipleFilesRealContent", MultipleFilesRealContent, 4, 3, null }; //expected input
            yield return new object[] { "MultipleFilesNullAndRealContent", MultipleFilesNullAndRealContent, 3, 2, null }; //expected errinputor
        }
    }

    internal static class GetMapCenterPosTestData
    {
        static List<string[]> ValidLogEntryPairActualContent = new List<string[]>
        {
            { "Developed by Soprobotics.\r\nCsepp number:;1\r\nCsepp version:;HighAlt Field Test\r\nRX:;D1\r\nTX:;D2\r\nGPS baud rate:;9600\r\nGPS Nav Mode:;6;Airborne under 1 g\r\nFile version:;0TinyGPS++ libary version:;1.0.3\r\n__________________________\r\nCseppID;Satellites;Time (GMT0);Latitude;Longitude;Altitude\r\n________;________;________;________;________;________\r\n0;6;9:20:40;46.791625;17.190160;5.000000\r\n0;6;9:20:40;47.759089;20.247051;10.000000".Split("\r\n") }
        };
        static List<string[]> ValidLogEntryCollectionContent = new List<string[]>
        {
            { "Developed by Soprobotics.\r\nRX:;D1\r\nTX:;D2\r\nGPS baud rate:;9600\r\nGPS Nav Mode:;6;Airborne under 1 g\r\nFile version:;0TinyGPS++ libary version:;1.0.3\r\n__________________________\r\nCseppID;Satellites;Time (GMT0);Latitude;Longitude;Altitude\r\n________;________;________;________;________;________\r\n0;6;9:20:40;46.791625;17.190160;0.000000\r\n0;6;9:20:40;46.781625;17.200160;0.000000 \r\n0;6;9:20:40;46.771625;17.210160;0.000000\r\n0;6;9:20:40;46.761625;17.220160;0.000000\r\n0;6;9:20:40;47.759089;20.247051;0.000000".Split("\r\n") }
        };
        static List<string[]> InvalidLogEntryPairContent = new List<string[]>
        {
            { "Developed by Soprobotics.\r\nCsepp number:;1\r\nCsepp version:;HighAlt Field Test\r\nRX:;D1\r\nTX:;D2\r\nGPS baud rate:;9600\r\nGPS Nav Mode:;6;Airborne under 1 g\r\nFile version:;0TinyGPS++ libary version:;1.0.3\r\n__________________________\r\nCseppID;Satellites;Time (GMT0);Latitude;Longitude;Altitude\r\n________;________;________;________;________;________\r\n0;6;9:20:40;68.787318;144.514913;182.300003\r\n0;6;9:20:40;66.178240;154.600043;182.300003".Split("\r\n") }
        };

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { "ValidLogEntryPairActual", ValidLogEntryPairActualContent, new LogEntry(47.275357, 18.7186055, 0), null };
            yield return new object[] { "ValidLogEntryCollection", ValidLogEntryCollectionContent, new LogEntry(47.260357, 18.7186055, 0), null };
            yield return new object[] { "InvalidLogEntryPair", InvalidLogEntryPairContent, null, typeof(ArgumentOutOfRangeException) };
        }
    }

    internal static class GetElevationLimitTestData
    {
        static List<string[]> SinglePositiveElevationContent = new List<string[]>
        {
            { "Developed by Soprobotics.\r\nCsepp number:;1\r\nCsepp version:;HighAlt Field Test\r\nRX:;D1\r\nTX:;D2\r\nGPS baud rate:;9600\r\nGPS Nav Mode:;6;Airborne under 1 g\r\nFile version:;0TinyGPS++ libary version:;1.0.3\r\n__________________________\r\nCseppID;Satellites;Time (GMT0);Latitude;Longitude;Altitude\r\n________;________;________;________;________;________\r\n0;6;9:20:40;46.791625;17.190160;5.000000".Split("\r\n") }
        };
        static List<string[]> SingleNegativeElevationContent = new List<string[]>
        {
            { "Developed by Soprobotics.\r\nCsepp number:;1\r\nCsepp version:;HighAlt Field Test\r\nRX:;D1\r\nTX:;D2\r\nGPS baud rate:;9600\r\nGPS Nav Mode:;6;Airborne under 1 g\r\nFile version:;0TinyGPS++ libary version:;1.0.3\r\n__________________________\r\nCseppID;Satellites;Time (GMT0);Latitude;Longitude;Altitude\r\n________;________;________;________;________;________\r\n0;6;9:20:40;46.791625;17.190160;-5.000000".Split("\r\n") }
        };
        static List<string[]> MultipleElevationsContent = new List<string[]>
        {
            { "Developed by Soprobotics.\r\nCsepp number:;1\r\nCsepp version:;HighAlt Field Test\r\nRX:;D1\r\nTX:;D2\r\nGPS baud rate:;9600\r\nGPS Nav Mode:;6;Airborne under 1 g\r\nFile version:;0TinyGPS++ libary version:;1.0.3\r\n__________________________\r\nCseppID;Satellites;Time (GMT0);Latitude;Longitude;Altitude\r\n________;________;________;________;________;________\r\n0;6;9:20:40;46.791625;17.190160;5.000000\r\n0;6;9:20:40;46.791625;17.190160;5.600000\r\n0;6;9:20:40;46.791625;17.190160;50.000000\r\n0;6;9:20:40;46.791625;17.190160;500.600000".Split("\r\n") }
        };
        static List<string[]> UnrealisticElevationContent = new List<string[]>
        {
            { "Developed by Soprobotics.\r\nCsepp number:;1\r\nCsepp version:;HighAlt Field Test\r\nRX:;D1\r\nTX:;D2\r\nGPS baud rate:;9600\r\nGPS Nav Mode:;6;Airborne under 1 g\r\nFile version:;0TinyGPS++ libary version:;1.0.3\r\n__________________________\r\nCseppID;Satellites;Time (GMT0);Latitude;Longitude;Altitude\r\n________;________;________;________;________;________\r\n0;6;9:20:40;46.791625;17.190160;5.000000\r\n0;6;9:20:40;46.791625;17.190160;5.600000\r\n0;6;9:20:40;46.791625;17.190160;50.000000\r\n0;6;9:20:40;46.791625;17.190160;500000.600000".Split("\r\n") }
        };

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { "SinglePositiveElevationContent", SinglePositiveElevationContent, 6, null };
            yield return new object[] { "SingleNegativeElevationContent", SingleNegativeElevationContent, -5, null };
            yield return new object[] { "MultipleElevationsContent", MultipleElevationsContent, 551, null };
            yield return new object[] { "UnrealisticElevationContent", UnrealisticElevationContent, null, typeof(Exception) };
        }
    }

}
