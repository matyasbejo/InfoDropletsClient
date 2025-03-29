using InfoDroplets.ResultExporterApp;
using NUnit.Framework;

namespace InfoDroplets.ResultExporterApp.Tester
{
    [TestFixture]
    public class LogProcessorTester
    {
        #region FilterAB tester

        [Test, TestCaseSource(typeof(FilterABTestData), nameof(FilterABTestData.TestCases))]
        public void FilterABFilesTester(string[] paths, int? expectedFileCount, Type expectedException)
        {
            var lp = new LogProcessor(paths);
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
    }

    public class FilterABTestData
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
            yield return new object[] { RadioAndLogsArray, 3, null }; //expected input
            yield return new object[] { ABLogsArray, 3, null }; //realistic input
            yield return new object[] { ALogsArray, 3, null }; //realistic input
            yield return new object[] { BLogsArray, 3, null }; //realistic input
            yield return new object[] { RadioArray, null, typeof(Exception) }; //expected error
            yield return new object[] { emptySdArray, null, typeof(Exception) }; //expected error
            yield return new object[] { RandomFolderContentArray, null, typeof(Exception) }; //expected error
        }
    }

    #
}