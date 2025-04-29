using CommunityToolkit.Mvvm.Messaging;
using InfoDroplets.Logic;
using InfoDroplets.Models;
using InfoDroplets.Repository;
using Moq;
using NUnit.Framework;

namespace InfoDroplets.Tests
{
    [TestFixture]
    public class TrackingEntryLogicTests
    {
        TrackingEntryLogic logic;
        Mock<IRepository<TrackingEntry>> MockTrackingEntryRepository;

        [SetUp]
        public void Init()
        {
            var input = new List<TrackingEntry>()
            {
                new TrackingEntry(1,1,5,47.1,16.1,100,new TimeOnly(13,0,0)),
                new TrackingEntry(2,1,5,47.1001,16.1002,100.3,new TimeOnly(13,0,10)),
                new TrackingEntry(3,1,4,47.1004,16.1006,100.43,new TimeOnly(13,0,50))
            }.AsQueryable();

            MockTrackingEntryRepository = new Mock<IRepository<TrackingEntry>>();
            MockTrackingEntryRepository.Setup(s => s.ReadAll()).Returns(input);
            MockTrackingEntryRepository.Setup(s => s.Read(1)).Returns(input.First(s => s.Id == 1));
            MockTrackingEntryRepository.Setup(s => s.Read(2)).Returns(input.First(s => s.Id == 2));
            MockTrackingEntryRepository.Setup(s => s.Read(3)).Returns(input.First(s => s.Id == 3));
            logic = new TrackingEntryLogic(MockTrackingEntryRepository.Object);
        }

        [Test]
        public void ReadExistingTeTest()
        {
            TrackingEntry te = logic.Read(2);

            Assert.That(te.Equals(logic.Read(2)));
        }

        [Test]
        public void ReadFakeTeTest()
        {
            Assert.Throws<ArgumentException>(() => { logic.Read(99); });
        }

        [Test]
        public void CreateNewValidTeTest()
        {

            string inputstring = "1;12;11:56:25;46.186901;19.222485;7383.200195";
            TrackingEntry newTe = new TrackingEntry(inputstring);

            logic.Create(inputstring);

            MockTrackingEntryRepository.Verify(r => r.Create(It.Is<TrackingEntry>(t => t.Latitude == 46.186901 && t.Longitude == 19.222485)), Times.Once());
        }

        [Test]
        public void CreateNewInvalidTeTest()
        {

            string inputstring = "1;12;11:56:25;46.186901;19.222kuki485;7383.200195";

            Assert.Throws<ArgumentException>(() => { logic.Create(inputstring); });
        }
        
        [Test]
        public void CreateNewInvalidPartialTeTest()
        {

            string inputstring = "1;12;11:56:25;46.186901;7383.200195";

            Assert.Throws<ArgumentException>(() => { logic.Create(inputstring); });
        }
        
        [Test]
        public void UpdateTeTest()
        {
            TrackingEntry te = logic.Read(1);
            te.Elevation = 600.69;
            logic.Update(te);

            MockTrackingEntryRepository.Verify(r => r.Update(te), Times.Once);
        }
    }
}
