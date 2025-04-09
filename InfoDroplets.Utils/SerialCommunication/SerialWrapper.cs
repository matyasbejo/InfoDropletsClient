using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoDroplets.Utils.Interfaces;

namespace InfoDroplets.Utils.SerialCommunication
{
    public class SerialWrapper : ISerialWrapper, IDisposable
    {
        SerialPort _serialPort;

        public List<string> AvaliableSerialPorts { get; set; }

        public SerialWrapper()
        {
            _serialPort = new SerialPort();
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
        public void Close() { _serialPort.Close(); }
        public string ReadLine() { return _serialPort.ReadLine(); }
        public static string[] GetPortNames() { return SerialPort.GetPortNames(); }

        public void Dispose()
        {
            _serialPort.Close();
        }
    }
}
