using CommunityToolkit.Mvvm.Messaging;
using InfoDroplets.Logic;
using InfoDroplets.Models;
using InfoDroplets.Repository;
using InfoDroplets.Utils.Enums;
using Moq;
using NUnit.Framework;

namespace InfoDroplets.Tests
{
    [TestFixture]
    public class DropletLogicCRUDTests
    {
        DropletLogic logic;
        Mock<IRepository<Droplet>> MockDropletRepository;

        [SetUp]
        public void Init()
        {
            var input = new List<Droplet>()
            {
                new Droplet(0),
                new Droplet(1),
                new Droplet(2)
            }.AsQueryable();

            MockDropletRepository = new Mock<IRepository<Droplet>>();
            MockDropletRepository.Setup(s => s.ReadAll()).Returns(input);
            MockDropletRepository.Setup(s => s.Read(0)).Returns(input.First(s => s.Id == 0));
            MockDropletRepository.Setup(s => s.Read(1)).Returns(input.First(s => s.Id == 1));
            MockDropletRepository.Setup(s => s.Read(2)).Returns(input.First(s => s.Id == 2));
            logic = new DropletLogic(MockDropletRepository.Object);
        }

        [Test]
        public void ReadExistingDropletTest()
        {
            Droplet droplet = logic.Read(2);

            Assert.That(droplet.Equals(logic.Read(2)));
        }

        [Test]
        public void ReadFakeDropletTest()
        {
            Assert.Throws<ArgumentException>(() => { logic.Read(99); });
        }

        [Test]
        public void CreateNewValidDropletTest()
        {

            Droplet newDroplet = new Droplet(6);

            logic.Create(newDroplet);

            MockDropletRepository.Verify(r => r.Create(newDroplet), Times.Once());
        }

        [Test]
        public void CreateNewInvalidExistingDropletTest()
        {

            Droplet newDroplet = new Droplet(1);

            Assert.Throws<InvalidOperationException>(() => { logic.Create(newDroplet); });
        }

        [Test]
        public void UpdateDropletTest()
        {
            Droplet droplet = logic.Read(1);
            droplet.ElevationTrend = DropletElevationTrend.Rising;
            logic.Update(droplet);

            MockDropletRepository.Verify(r => r.Update(droplet), Times.Once);
        }

        [Test]
        public void UpdateDropletStatusTestRuns()
        {
            string inputstring1 = "1;12;11:56:25;46.186901;7383.200195;111";
            string inputstring2 = "1;12;12:56:25;46.186922;7410.200195;122";
            TrackingEntry te1 = new TrackingEntry(inputstring1);
            TrackingEntry te2 = new TrackingEntry(inputstring2);

            Droplet d1 = logic.Read(1);
            d1.Measurements = new List<TrackingEntry>();
            d1.Measurements.Add(te1);
            d1.Measurements.Add(te2);

            logic.Update(d1);
            logic.UpdateDropletStatus(1);

            MockDropletRepository.Verify(r => r.Update(d1), Times.Exactly(2));
        }
        
        [Test]
        public void UpdateDropletStatusTestValues()
        {
            string inputstring1 = "1;12;11:56:25;46.186901;7383.200195;111";
            string inputstring2 = "1;12;12:56:25;46.186922;7410.200195;122";
            TrackingEntry te1 = new TrackingEntry(inputstring1);
            TrackingEntry te2 = new TrackingEntry(inputstring2);

            Droplet d1 = logic.Read(1);
            d1.Measurements = new List<TrackingEntry>();
            d1.Measurements.Add(te1);
            d1.Measurements.Add(te2);

            logic.Update(d1);
            logic.UpdateDropletStatus(1);
            d1 = logic.Read(1);

            Assert.That(d1.DistanceFromGNU2D.HasValue);
            Assert.That(d1.DistanceFromGNU3D.HasValue);
            Assert.That(d1.LastData != null);
            Assert.That(d1.LastUpdated.HasValue);
        }

        [Test]
        public void DropletHasNoExtraDataBeforeUpdate()
        {
            Droplet d1 = logic.Read(1);
            Assert.That(!d1.DistanceFromGNU2D.HasValue);
            Assert.That(!d1.DistanceFromGNU3D.HasValue);
            Assert.That(d1.LastData == null);
            Assert.That(!d1.LastUpdated.HasValue);
        }
    }
}
