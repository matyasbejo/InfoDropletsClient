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
    public class SerialWrapper : IDisposable
    {
        SerialPort _serialPort;

        public List<string> AvaliableSerialPorts { get; set; }

        public SerialWrapper(SerialPort serialPort)
        {
            _serialPort = serialPort;
            _serialPort.BaudRate = 9600;
        }
        
        public SerialWrapper()
        {
            AvaliableSerialPorts = SerialPort.GetPortNames().ToList();
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
        public void SetBaudeRate(int BaudeRate)
        {
            _serialPort.BaudRate = BaudeRate;
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
            _serialPort?.Close();
        }
    }

    public class SerialWrapper2
    {
        public List<string> AvaliableSerialPorts { get; set; }

        public SerialWrapper2()
        {
            //Populate with some data
            AvaliableSerialPorts = new List<string> { "COM1", "COM2", "COM3" };
        }
    }
}
