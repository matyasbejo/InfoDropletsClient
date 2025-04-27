using InfoDroplets.Logic;
using InfoDroplets.Models;
using InfoDroplets.Repository;
using InfoDroplets.Utils.Interfaces;
using NUnit.Framework;

namespace InfoDroplets.Tests
{
    [TestFixture]
    public class DropletLogicTests
    {
        [Test, TestCaseSource(typeof(HaversineDistanceKmTestData), nameof(HaversineDistanceKmTestData.TestCases))]
        public void HaversineDistanceKmTester(string TestCaseName, IGpsPos pos1, IGpsPos pos2, double expectedDistance, double expectedRange, bool expectedToPass)
        {
            if (expectedToPass)
            {
                double calculatedDistance = DropletLogic.Distance2DHaversineKm(pos1, pos2);
                Assert.That(Math.Abs((calculatedDistance - expectedDistance)) <= expectedRange);
            }
            else
            {
                Assert.Throws(typeof(NullReferenceException), () => DropletLogic.Distance2DHaversineKm(pos1, pos2));
            }
        }
        
        [Test, TestCaseSource(typeof(Distance3DKmTestData), nameof(Distance3DKmTestData.TestCases))]
        public void Distance3DKm(string TestCaseName, IGpsPos pos1, IGpsPos pos2, double expectedDistance, double expectedRange, bool expectedToPass)
        {
            if (expectedToPass)
            {
                double calculatedDistance = DropletLogic.Distance3DKm(pos1, pos2);
                Assert.That(Math.Abs((calculatedDistance - expectedDistance)) <= expectedRange);
            }
            else
            {
                Assert.Throws(typeof(NullReferenceException), () => DropletLogic.Distance3DKm(pos1, pos2));
            }
        }
    }

    internal class HaversineDistanceKmTestData
    {
        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { "1ValidSameHeight", new GpsPos(47.683784, 16.589251, 100), new GpsPos(47.680594, 16.577495, 100), 0.9497, 0.01, true}; //expected success
            yield return new object[] { "2ValidDifferentHeight", new GpsPos(47.683784, 16.589251, 100), new GpsPos(47.680594, 16.577495, 1000), 0.9497, 0.01, true}; //expected success
            yield return new object[] { "3ValidZeroHeight", new GpsPos(47.683784, 16.589251, 0), new GpsPos(47.680594, 16.577495, 0), 0.9497, 0.01, true}; //expected success
            yield return new object[] { "4ValidSamePosition", new GpsPos(47.683784, 16.589251, 0), new GpsPos(47.683784, 16.589251, 0), 0, 0, true}; //expected success
            yield return new object[] { "5InvalidMissingFirstValue", null, new GpsPos(47.683784, 16.589251, 0), 0, 0, false}; //expected fail
            yield return new object[] { "6InvalidMissingSecondValue", new GpsPos(47.683784, 16.589251, 0), null, 0, 0, false}; //expected fail
            yield return new object[] { "7InvalidMissingValues", null, null, 0, 0, false}; //expected fail
        }
    }
    
    internal class Distance3DKmTestData
    {
        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { "1ValidDifferentPosSameHeight", new GpsPos(47.683784, 16.589251, 100), new GpsPos(47.680594, 16.577495, 100), 0.9497, 0.01, true}; //expected success
            yield return new object[] { "3ValidDifferentPosDifferentHeight", new GpsPos(47.683784, 16.589251, 100), new GpsPos(47.680594, 16.577495, 1000), 1.3, 0.01, true}; //expected success
            yield return new object[] { "2ValidSamePosDifferentHeight", new GpsPos(47.683784, 16.589251, 100), new GpsPos(47.683784, 16.589251, 1000), 0.9, 0, true }; //expected success
            yield return new object[] { "4ValidZeroHeight", new GpsPos(47.683784, 16.589251, 0), new GpsPos(47.680594, 16.577495, 0), 0.9497, 0.01, true}; //expected success
            yield return new object[] { "5ValidSamePosition", new GpsPos(47.683784, 16.589251, 0), new GpsPos(47.683784, 16.589251, 0), 0, 0, true}; //expected success
            yield return new object[] { "6InvalidMissingFirstValue", null, new GpsPos(47.683784, 16.589251, 0), 0, 0, false}; //expected fail
            yield return new object[] { "7InvalidMissingSecondValue", new GpsPos(47.683784, 16.589251, 0), null, 0, 0, false}; //expected fail
            yield return new object[] { "8InvalidMissingValues", null, null, 0, 0, false}; //expected fail
        }
    }
}

