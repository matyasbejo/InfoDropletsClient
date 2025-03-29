using InfoDroplets.ResultExporterApp;
using NUnit.Framework;

namespace InfoDroplets.ResultExporterApp.Tester
{
    [TestFixture]
    internal class LogProcessorTester
    {
        [Test, TestCaseSource(typeof(FilterABTestData), nameof(FilterABTestData.TestCases))]
        public void FilterABFilesTester(string[] paths, int? expectedFileCount, Type expectedException)
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
        public void GetDropletNumberTester(string path, int? expectedDropletNumber, Type expectedException)
        {
            var lp = new LogProcessor();

            if (expectedException != null)
            {
                Assert.Throws(expectedException, () => lp.GetDropletNumber(path));
            } 
            else
            {
                Assert.That(lp.GetDropletNumber(path), Is.EqualTo(expectedDropletNumber));
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
            yield return new object[] { RadioAndLogsArray, 3, null }; //expected input
            yield return new object[] { ABLogsArray, 3, null }; //realistic input
            yield return new object[] { ALogsArray, 3, null }; //realistic input
            yield return new object[] { BLogsArray, 3, null }; //realistic input
            yield return new object[] { RadioArray, null, typeof(Exception) }; //expected error
            yield return new object[] { emptySdArray, null, typeof(Exception) }; //expected error
            yield return new object[] { RandomFolderContentArray, null, typeof(Exception) }; //expected error
        }
    }

    internal class GetDropletNumberTestData
    {
        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { @"D:\UNI\_Szakdolgozat\TestData\L8_V0a.txt", 8, null }; //expected input
            yield return new object[] { @"D:\UNI\_Szakdolgozat\TestData\L8_V0b.txt", 8, null }; //expected input
            yield return new object[] { @"D:\UNI\_Szakdolgozat\TestData\L1_V21a.txt", 1, null }; //expected input
            yield return new object[] { @"D:\UNI\_Szakdolgozat\TestData\L10_V21a.txt", 10, null }; //expected input
            yield return new object[] { @"D:\UNI\_Szakdolgozat\TestData\L100_V21a.txt", null, typeof(Exception) }; //expected to fail (too long id)
            yield return new object[] { @"D:\UNI\_Szakdolgozat\TestData\R10_V0a.txt", null, typeof(Exception) }; //expected to fail (radio)
            yield return new object[] { @"D:\UNI\_Szakdolgozat\TestData\kicskacsa.quack", null, typeof(Exception) }; //expected to fail (not log)
        }
    }
}
