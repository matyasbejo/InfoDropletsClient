using InfoDroplets.ResultExporter.Models;
using NUnit.Framework;
using ResultExporterApp;

namespace InfoDroplets.ResultExporterApp.Tester
{
    [TestFixture]
    internal class MapGeneratorTests
    {
        [Test, TestCaseSource(typeof(PrepareValuesTestData), nameof(PrepareValuesTestData.TestCases))]
        public void PrepareValuesTester(string TestCaseName, int deviceId, int yMax, double ctrLng, double ctrLat, List<List<LogEntry>> logCollection, List<List<LogEntry>> breakCollection, Type expectedException)
        {
            MapGenerator re = new MapGenerator();

            if (expectedException != null)
            {
                Assert.Throws(expectedException, () => re.PrepareValues(deviceId, yMax, ctrLng, ctrLat, logCollection, breakCollection));
            }
            else
            {
                var success = re.PrepareValues(deviceId, yMax, ctrLng, ctrLat, logCollection, breakCollection);
                Assert.That(success, Is.True);
            }
        }
    }

    internal class PrepareValuesTestData
    {
        static List<List<LogEntry>> LogCollection = new List<List<LogEntry>>
        {
            new List<LogEntry>
            {
                new LogEntry(1,1,0),
                new LogEntry(2,2,0)
            },
            new List<LogEntry>
            {
                new LogEntry(3,3,0),
                new LogEntry(4,4,0),
            }
        };
        static List<List<LogEntry>> EmptyLogCollection = new List<List<LogEntry>>
        {
        };
        static List<List<LogEntry>> BreakCollection = new List<List<LogEntry>>
        {
            new List<LogEntry>
            {
                new LogEntry(2,2,0),
                new LogEntry(3,3,0),
            }
        };
        static List<List<LogEntry>> EmptyBreakCollection = new List<List<LogEntry>>
        {
        };

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { "ValidAllCollections", 0, 100, 1.1, 1.1, LogCollection, BreakCollection, null}; //expected success
            yield return new object[] { "ValidEmptyBreak", 0, 100, 1.1, 1.1, LogCollection, EmptyBreakCollection, null}; //expected success
            yield return new object[] { "InvalidEmptyLog", 0, 100, 1.1, 1.1, EmptyLogCollection, EmptyBreakCollection, typeof(Exception)}; //expected error
            yield return new object[] { "InvalidNullLog", 0, 100, 1.1, 1, LogCollection, null, typeof(Exception)}; //expected error
            yield return new object[] { "InvalidNullBreak", 0, 100, 1.1, 1, null, BreakCollection, typeof(Exception)}; //expected error
        }
    }
}
