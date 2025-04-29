using InfoDroplets.Logic;
using InfoDroplets.Models;
using InfoDroplets.Repository;
using InfoDroplets.Utils.Enums;
using InfoDroplets.Utils.Interfaces;
using Microsoft.ApplicationInsights.DataContracts;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace InfoDroplets.Tests
{
    [TestFixture]
    public class DropletLogicNonCRUDTests
    {
        [Test, TestCaseSource(typeof(HaversineDistanceKmTestData), nameof(HaversineDistanceKmTestData.TestCases))]
        public void HaversineDistanceKmTester(string TestCaseName, IGpsPos pos1, IGpsPos pos2, double expectedDistance, double expectedRange, Type expectedException)
        {
            if (expectedException == null)
            {
                double calculatedDistance = DropletLogic.Distance2DHaversineKm(pos1, pos2);
                Assert.That(Math.Abs((calculatedDistance - expectedDistance)) <= expectedRange);
            }
            else
            {
                Assert.Throws(expectedException, () => DropletLogic.Distance2DHaversineKm(pos1, pos2));
            }
        }
        
        [Test, TestCaseSource(typeof(Distance3DKmTestData), nameof(Distance3DKmTestData.TestCases))]
        public void Distance3DKmTester(string TestCaseName, IGpsPos pos1, IGpsPos pos2, double expectedDistance, double expectedRange, Type expectedException)
        {
            if (expectedException == null)
            {
                double calculatedDistance = DropletLogic.Distance3DKm(pos1, pos2);
                Assert.That(Math.Abs((calculatedDistance - expectedDistance)) <= expectedRange);
            }
            else
            {
                Assert.Throws(expectedException, () => DropletLogic.Distance3DKm(pos1, pos2));
            }
        }
        
        [Test, TestCaseSource(typeof(GetDirectionTestData), nameof(GetDirectionTestData.TestCases))]
        public void GetDirectionTester(string TestCaseName, List<TrackingEntry> entries, DropletDirection expectedDirection, Type expectedException)
        {
            if (expectedException == null)
            {
                DropletDirection? actualDirection = DropletLogic.GetDirection(entries);
                Assert.That(actualDirection == expectedDirection);
            }
            else
            {
                Assert.Throws(expectedException, () => DropletLogic.GetDirection(entries));
            }
        }
        
        [Test, TestCaseSource(typeof(GenerateCommandTestData), nameof(GenerateCommandTestData.TestCases))]
        public void GenerateCommandTester(string TestCaseName, int? dropletId, RadioCommand? command, string ExpectedOutput, Type expectedException)
        {
            if (expectedException == null)
            {
                string generatedOutput = DropletLogic.GenerateCommand(dropletId,command);
                Assert.That(generatedOutput == ExpectedOutput);
            }
            else
            {
                Assert.Throws(expectedException, () => DropletLogic.GenerateCommand(dropletId,command));
            }
        }
        
        [Test, TestCaseSource(typeof(GetElevationTrendTestData), nameof(GenerateCommandTestData.TestCases))]
        public void GetElevationTrendTester(string TestCaseName, List<TrackingEntry> entries, DropletElevationTrend expectedElevationTrend, Type expectedException)
        {
            if (expectedException == null)
            {
                DropletElevationTrend generatedOutput = DropletLogic.GetElevationTrend(entries);
                Assert.That(generatedOutput == expectedElevationTrend);
            }
            else
            {
                Assert.Throws(expectedException, () => DropletLogic.GetElevationTrend(entries));
            }
        }
        
        [Test, TestCaseSource(typeof(GetSpeedKmHTestData), nameof(GetSpeedKmHTestData.TestCases))]
        public void GetSpeedKmHTester(string TestCaseName, List<TrackingEntry> entries, double expectedSpeed, Type expectedException)
        {
            if (expectedException == null)
            {
                double calculatedSpeed = DropletLogic.GetSpeedKmH(entries);
                Assert.That(calculatedSpeed == expectedSpeed);
            }
            else
            {
                Assert.Throws(expectedException, () => DropletLogic.GetSpeedKmH(entries));
            }
        }
    }

    internal class HaversineDistanceKmTestData
    {
        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { "01ValidSameHeight", new GpsPos(47.683784, 16.589251, 100), new GpsPos(47.680594, 16.577495, 100), 0.9497, 0.01, null}; //expected success
            yield return new object[] { "02ValidDifferentHeight", new GpsPos(47.683784, 16.589251, 100), new GpsPos(47.680594, 16.577495, 1000), 0.9497, 0.01, null}; //expected success
            yield return new object[] { "03ValidZeroHeight", new GpsPos(47.683784, 16.589251, 0), new GpsPos(47.680594, 16.577495, 0), 0.9497, 0.01, null}; //expected success
            yield return new object[] { "04ValidSamePosition", new GpsPos(47.683784, 16.589251, 0), new GpsPos(47.683784, 16.589251, 0), 0, 0, null}; //expected success
            yield return new object[] { "05InvalidMissingFirstValue", null, new GpsPos(47.683784, 16.589251, 0), 0, 0, typeof(NullReferenceException)}; //expected fail
            yield return new object[] { "06InvalidMissingSecondValue", new GpsPos(47.683784, 16.589251, 0), null, 0, 0, typeof(NullReferenceException)}; //expected fail
            yield return new object[] { "07InvalidMissingValues", null, null, 0, 0, typeof(NullReferenceException)}; //expected fail
        }
    }
    internal class Distance3DKmTestData
    {
        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { "01ValidDifferentPosSameHeight", new GpsPos(47.683784, 16.589251, 100), new GpsPos(47.680594, 16.577495, 100), 0.9497, 0.01, null}; //expected success
            yield return new object[] { "03ValidDifferentPosDifferentHeight", new GpsPos(47.683784, 16.589251, 100), new GpsPos(47.680594, 16.577495, 1000), 1.3, 0.01, null}; //expected success
            yield return new object[] { "02ValidSamePosDifferentHeight", new GpsPos(47.683784, 16.589251, 100), new GpsPos(47.683784, 16.589251, 1000), 0.9, 0, null }; //expected success
            yield return new object[] { "04ValidZeroHeight", new GpsPos(47.683784, 16.589251, 0), new GpsPos(47.680594, 16.577495, 0), 0.9497, 0.01, null}; //expected success
            yield return new object[] { "05ValidSamePosition", new GpsPos(47.683784, 16.589251, 0), new GpsPos(47.683784, 16.589251, 0), 0, 0, null}; //expected success
            yield return new object[] { "06InvalidMissingFirstValue", null, new GpsPos(47.683784, 16.589251, 0), 0, 0, typeof(NullReferenceException)}; //expected fail
            yield return new object[] { "07InvalidMissingSecondValue", new GpsPos(47.683784, 16.589251, 0), null, 0, 0, typeof(NullReferenceException)}; //expected fail
            yield return new object[] { "08InvalidMissingValues", null, null, 0, 0, typeof(NullReferenceException)}; //expected fail
        }
    }
    internal class GetDirectionTestData
    {
        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { "01ValidSingleEntryList", SingleEntryList, DropletDirection.Stationary, null }; //expected succes
            yield return new object[] { "02ValidNorthEntryList", NorthEntryList, DropletDirection.North, null }; //expected succes
            yield return new object[] { "03ValidEastEntryList", EastEntryList, DropletDirection.East, null }; //expected succes
            yield return new object[] { "04ValidSouthEntryList", SouthEntryList, DropletDirection.South, null }; //expected succes
            yield return new object[] { "05ValidWestEntryList", WestEntryList, DropletDirection.West, null }; //expected succes
            yield return new object[] { "06ValidNorthEastEntryList", NorthEastEntryList, DropletDirection.NorthEast, null }; //expected succes
            yield return new object[] { "07ValidSouthEastEntryList", SouthEastEntryList, DropletDirection.SouthEast, null }; //expected succes
            yield return new object[] { "08ValidSouthWestEntryList", SouthWestEntryList, DropletDirection.SouthWest, null }; //expected succes
            yield return new object[] { "09ValidNorthWestEntryList", NorthWestEntryList, DropletDirection.NorthWest, null }; //expected succes
            yield return new object[] { "10ValidStationaryEntryList", StationaryEntryList, DropletDirection.Stationary, null }; //expected succes
            yield return new object[] { "11ValidStationaryWithinThresholdEntryList", StationaryWithinThresholdEntryList, DropletDirection.Stationary, null }; //expected succes
            yield return new object[] { "12InvalidStationaryEmptyListEntryList", EmptyEntryList, null, typeof(InvalidOperationException) }; //expected fail
            yield return new object[] { "13InvalidStationaryNullListEntryList", null, null, typeof(ArgumentNullException) }; //expected fail
        }

        static readonly List<TrackingEntry> SingleEntryList = new List<TrackingEntry>
        {
            new TrackingEntry
            {
                Latitude = 47.641597,
                Longitude = 18.697721,
                Elevation = 100
            }
        };

        static readonly List<TrackingEntry> NorthEntryList = new List<TrackingEntry>
        {
            new TrackingEntry
            {
                Latitude = 47.641597,
                Longitude = 0,
                Elevation = 100
            },
            new TrackingEntry
            {
                Latitude = 47.642630,
                Longitude = 0,
                Elevation = 100
            }
        };

        static readonly List<TrackingEntry> NorthEastEntryList = new List<TrackingEntry>
        {
            new TrackingEntry
            {
                Latitude = 47.641597,
                Longitude = 0,
                Elevation = 100
            },
            new TrackingEntry
            {
                Latitude = 47.642630,
                Longitude = 0.03222,
                Elevation = 100
            }
        };

        static readonly List<TrackingEntry> EastEntryList = new List<TrackingEntry>
        {
            new TrackingEntry
            {
                Latitude = 0,
                Longitude = 18.697721,
                Elevation = 100
            },
            new TrackingEntry
            {
                Latitude = 0,
                Longitude = 18.697881,
                Elevation = 100
            }
        };

        static readonly List<TrackingEntry> SouthEastEntryList = new List<TrackingEntry>
        {
            new TrackingEntry
            {
                Latitude = 0.03222,
                Longitude = 18.697721,
                Elevation = 100
            },
            new TrackingEntry
            {
                Latitude = 0,
                Longitude = 18.697881,
                Elevation = 100
            }
        };

        static readonly List<TrackingEntry> SouthEntryList = new List<TrackingEntry>
        {
            new TrackingEntry
            {
                Latitude = 47.641597,
                Longitude = 0,
                Elevation = 100
            },
            new TrackingEntry
            {
                Latitude = 47.640466,
                Longitude = 0,
                Elevation = 100
            }
        };

        static readonly List<TrackingEntry> SouthWestEntryList = new List<TrackingEntry>
        {
            new TrackingEntry
            {
                Latitude = 0.003333,
                Longitude = 18.697721,
                Elevation = 100
            },
            new TrackingEntry
            {
                Latitude = 0,
                Longitude = 18.696678,
                Elevation = 100
            }
        };

        static readonly List<TrackingEntry> WestEntryList = new List<TrackingEntry>
        {
            new TrackingEntry
            {
                Latitude = 0,
                Longitude = 18.697721,
                Elevation = 100
            },
            new TrackingEntry
            {
                Latitude = 0,
                Longitude = 18.696678,
                Elevation = 100
            }
        };

        static readonly List<TrackingEntry> NorthWestEntryList = new List<TrackingEntry>
        {
            new TrackingEntry
            {
                Latitude = 0,
                Longitude = 18.697721,
                Elevation = 100
            },
            new TrackingEntry
            {
                Latitude = 0.003302,
                Longitude = 18.696678,
                Elevation = 100
            }
        };
        
        static readonly List<TrackingEntry> StationaryEntryList = new List<TrackingEntry>
        {
            new TrackingEntry
            {
                Latitude = 20,
                Longitude = 18,
                Elevation = 100
            },
            new TrackingEntry
            {
                Latitude = 20,
                Longitude = 18,
                Elevation = 100
            }
        };

        static readonly List<TrackingEntry> StationaryWithinThresholdEntryList = new List<TrackingEntry>
        {
            new TrackingEntry
            {
                Latitude = 20.00001,
                Longitude = 18.0001,
                Elevation = 100
            },
            new TrackingEntry
            {
                Latitude = 20.00005,
                Longitude = 18.00005,
                Elevation = 100
            }
        };

        static readonly List<TrackingEntry> EmptyEntryList = new List<TrackingEntry>
        {
        };
    }
    internal class GenerateCommandTestData
    {
        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { "01ValidGpsReset", 5, RadioCommand.GpsReset, "R5", null }; //expected success
            yield return new object[] { "02ValidFullReset", 3, RadioCommand.FullReset, "F3", null }; //expected success
            yield return new object[] { "03ValidGetFileVersion", 1, RadioCommand.GetFileVersion, "V1", null }; //expected success
            yield return new object[] { "04ValidPing", 11, RadioCommand.Ping, "P11", null }; //expected success
            yield return new object[] { "05InvalidNullDropletId", null, RadioCommand.GetFileVersion, null, typeof(ArgumentNullException) }; //expected fail
            yield return new object[] { "06InvalidNullRadioCommand", 8, null, null, typeof(ArgumentNullException) }; //expected fail
            yield return new object[] { "07InvalidNullInputs", null, null, null, typeof(ArgumentNullException) }; //expected fail
        }
    }
    internal class GetElevationTrendTestData
    {
        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { "01ValidSingleEntryList", SingleEntryList, DropletElevationTrend.Stationary, null }; //expected succes
            yield return new object[] { "02ValidRisingEntryList", RisingEntryList, DropletElevationTrend.Rising, null }; //expected succes
            yield return new object[] { "02ValidRisingEntryList", DescendingEntryList, DropletElevationTrend.Descending, null }; //expected succes
            yield return new object[] { "03InvalidEmptyEntryList", EmptyEntryList, null, typeof(InvalidOperationException) }; //expected fail
            yield return new object[] { "04InvalidNullEntryList", null, null, typeof(ArgumentNullException) }; //expected fail
        }

        static readonly List<TrackingEntry> SingleEntryList = new List<TrackingEntry>
        {
            new TrackingEntry
            {
                Latitude = 47.641597,
                Longitude = 18.697721,
                Elevation = 100
            }
        };

        static readonly List<TrackingEntry> RisingEntryList = new List<TrackingEntry>
        {
            new TrackingEntry
            {
                Latitude = 47.641597,
                Longitude = 18.697721,
                Elevation = 100
            },
            new TrackingEntry
            {
                Latitude = 47.641597,
                Longitude = 18.697721,
                Elevation = 101
            }
        };

        static readonly List<TrackingEntry> DescendingEntryList = new List<TrackingEntry>
        {
            new TrackingEntry
            {
                Latitude = 47.641597,
                Longitude = 18.697721,
                Elevation = 100
            },
            new TrackingEntry
            {
                Latitude = 47.641597,
                Longitude = 18.697721,
                Elevation = 99
            }
        };

        static readonly List<TrackingEntry> EmptyEntryList = new List<TrackingEntry>
        {
        };

    }
    internal class GetSpeedKmHTestData
    {
        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { "01ValidSingleEntryList", SingleEntryList, 0.0, null }; //expected succes
            yield return new object[] { "02Valid1Km1h2DEntryList", _1Km1Hour2DEntryList, 1.0, null }; //expected succes
            yield return new object[] { "03Valid1Km1h3DEntryList", _1Km1Hour3DEntryList, 1.0, null }; //expected succes
            yield return new object[] { "04InvalidEmptyEntryList", EmptyEntryList, null, typeof(InvalidOperationException) }; //expected fail
            yield return new object[] { "05InvalidNullEntryList", null, null, typeof(NullReferenceException) }; //expected fail
        }

        static readonly List<TrackingEntry> SingleEntryList = new List<TrackingEntry>
        {
            new TrackingEntry
            {
                Latitude = 47.641597,
                Longitude = 18.697721,
                Elevation = 100
            }
        };

        static readonly List<TrackingEntry> _1Km1Hour2DEntryList = new List<TrackingEntry>
        {
            new TrackingEntry
            {
                Latitude = 47.641597,
                Longitude = 18.697721,
                Elevation = 100,
                Time = new TimeOnly(13,0,0,0)
            },
            new TrackingEntry
            {
                Latitude = 47.650597,
                Longitude = 18.697721,
                Elevation = 100,
                Time = new TimeOnly(14,0,0,0)
            }
        };
        static readonly List<TrackingEntry> _1Km1Hour3DEntryList = new List<TrackingEntry>
        {
            new TrackingEntry
            {
                Latitude = 47.641597,
                Longitude = 18.697721,
                Elevation = 100,
                Time = new TimeOnly(13,0,0,0)
            },
            new TrackingEntry
            {
                Latitude = 47.650593,
                Longitude = 18.697721,
                Elevation = 150,
                Time = new TimeOnly(14,0,0,0)
            }
        };

        static readonly List<TrackingEntry> EmptyEntryList = new List<TrackingEntry>
        {
        };
    }
}

