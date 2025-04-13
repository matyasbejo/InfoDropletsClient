using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoDroplets.Utils.SerialCommunication
{
    public class SerialWrapper : IDisposable, ISerialWrapper
    {
        SerialPort _serialPort;
        public event SerialDataReceivedEventHandler WrapperDataReceived;


        public string SelectedSerialPort { get; set; }
        public int SelectedBaudRate { get; set; }
        public List<string> AvaliableSerialPorts
        {
            get
            {
                return SerialPort.GetPortNames().ToList();
            }
        }

        public SerialWrapper()
        {
            _serialPort = new SerialPort();
            _serialPort.DataReceived += Wrapper_DataReceived;
        }

        private void Wrapper_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            WrapperDataReceived?.Invoke(this, e);
        }

        public void SendCommand(object sender, CommandEventArgs e)
        {
            WriteLine(e.Command);
        }
        public void Reset()
        {
            _serialPort.WriteLine("reset");
        }
        public void SafeOpen()
        {
            if (!_serialPort.IsOpen)
            {
                Open();

                bool restarted = false;
                DateTime timeAtReset = DateTime.Now;

                _serialPort.WriteLine("reset");
                do
                {
                    var input = ReadLine();
                    if (input.Contains("GNU Reciever"))
                        restarted = true;
                    else if (DateTime.Now - timeAtReset > TimeSpan.FromSeconds(10))
                    {
                        _serialPort.WriteLine("reset");
                        timeAtReset = DateTime.Now;
                    }
                }
                while (!restarted);
            }
        }
        public void SetPortName(string PortName)
        {
            _serialPort.PortName = PortName;
        }
        public void SetBaudeRate(int BaudeRate = 9600)
        {
            _serialPort.BaudRate = BaudeRate;
        }

        public string GetPortName()
        {
            return _serialPort.PortName;
        }
        public int GetBaudeRate()
        {
            return _serialPort.BaudRate;
        }
        public void WriteLine(string message)
        {
            _serialPort.WriteLine(message);
            Console.WriteLine($"message sent: {message}");
        }
        public void Open() { _serialPort.Open(); }
        public void SafeClose() 
        {
            if (_serialPort.IsOpen)
                _serialPort.Close();
        }
        public string ReadLine() { return _serialPort.ReadLine(); }

        void IDisposable.Dispose()
        {
            _serialPort.Close();
        }
    }
}
