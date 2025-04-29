using InfoDroplets.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoDroplets.Utils.SerialCommunication
{
    public class SystemSerialPort : ISerialPort
    {
        private readonly SerialPort _serialPort;

        public SystemSerialPort()
        {
            _serialPort = new SerialPort();
        }

        public string PortName
        {
            get => _serialPort.PortName;
            set => _serialPort.PortName = value;
        }

        public int BaudRate
        {
            get => _serialPort.BaudRate;
            set => _serialPort.BaudRate = value;
        }

        public bool IsOpen => _serialPort.IsOpen;

        public event SerialDataReceivedEventHandler DataReceived
        {
            add => _serialPort.DataReceived += value;
            remove => _serialPort.DataReceived -= value;
        }

        public void Open() => _serialPort.Open();
        public void Close() => _serialPort.Close();
        public void WriteLine(string text) => _serialPort.WriteLine(text);
        public string ReadLine() => _serialPort.ReadLine();
        public void Dispose() => _serialPort.Dispose();
    }
}
