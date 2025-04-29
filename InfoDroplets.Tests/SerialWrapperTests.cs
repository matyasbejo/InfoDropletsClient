using NUnit.Framework;
using Moq;
using InfoDroplets.Utils.SerialCommunication;
using InfoDroplets.Utils.Interfaces;


namespace InfoDroplets.Tests
{
    [TestFixture]
    public class SerialWrapperTests
    {
        private Mock<ISerialPort> _mockedSerialPort;
        private SerialWrapper _wrapper;

        [SetUp]
        public void Setup()
        {
            _mockedSerialPort = new Mock<ISerialPort>();
            _wrapper = new SerialWrapper(_mockedSerialPort.Object);
        }

        [Test]
        public void Valid_WriteLineCallsUnderlyingSerialPort()
        {
            _wrapper.WriteLine("hello");

            _mockedSerialPort.Verify(p => p.WriteLine("hello"), Times.Once);
        }

        [Test]
        public void Valid_ReadLineReturnsCorrectValue()
        {
            _mockedSerialPort.Setup(p => p.ReadLine()).Returns("OK");

            var result = _wrapper.ReadLine();

            Assert.That("OK", Is.EqualTo(result));
        }

        [Test]
        public void Valid_SafeOpenWaitsForRestart()
        {
            _mockedSerialPort.Setup(p => p.IsOpen).Returns(false);
            _mockedSerialPort.SetupSequence(p => p.ReadLine())
                     .Returns("8;12;11:56:30;46.186565;19.223429;7364.000000")
                     .Returns("GNU Receiver started");

            _wrapper.SafeOpen();

            _mockedSerialPort.Verify(p => p.Open(), Times.Once);
            _mockedSerialPort.Verify(p => p.WriteLine("reset"), Times.Exactly(1));
        }

        [Test]
        public void Invalid_SafeOpenThrowsExceptionWhenOpened()
        {
            _mockedSerialPort.Setup(p => p.IsOpen).Returns(true);

            var ex = Assert.Throws<Exception>(() => _wrapper.SafeOpen());
            Assert.That(ex.Message, Is.EqualTo("Port is already open"));
        }

        [Test]
        public void Valid_SafeCloseCallsCloseWhenClosed()
        {
            _mockedSerialPort.Setup(p => p.IsOpen).Returns(true);

            _wrapper.SafeClose();

            _mockedSerialPort.Verify(p => p.Close(), Times.Once);
        }

        [Test]
        public void Invalid_SafeCloseThrowsExceptionWhenClosed()
        {
            _mockedSerialPort.Setup(p => p.IsOpen).Returns(false);

            var ex = Assert.Throws<Exception>(() => _wrapper.SafeClose());

            Assert.That(ex.Message, Is.EqualTo("Port is already closed"));
        }
    }
}
