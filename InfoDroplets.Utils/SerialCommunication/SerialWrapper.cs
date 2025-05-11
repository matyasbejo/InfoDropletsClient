using InfoDroplets.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace InfoDroplets.Utils.SerialCommunication
{
    public class SerialWrapper : IDisposable, ISerialWrapper
    {
        ISerialPort _serialPort;
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
        public List<int> AvaliableBaudRates { get; private set; }
        public bool IsOpen { get { return _serialPort.IsOpen; } }

        public SerialWrapper(ISerialPort serialPort = null)
        {
            AvaliableBaudRates = new List<int> { 4800, 9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600 };
            _serialPort = serialPort ?? new SystemSerialPort();
            _serialPort.DataReceived += Wrapper_DataReceived;
        }

        void Wrapper_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            WrapperDataReceived?.Invoke(this, e);
        }
        public void SendCommand(object sender, CommandEventArgs e)
        {
            WriteLine(e.Command);
        }
        public void SendResetReceiver()
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
                int retries = 0;

                SendResetReceiver();
                while (!restarted && retries < 3 )
                {
                    var input = ReadLine();
                    if (input.Contains("GNU Receiver"))
                        restarted = true;
                    else if (DateTime.Now - timeAtReset > TimeSpan.FromSeconds(10))
                    {
                        _serialPort.WriteLine("reset");
                        timeAtReset = DateTime.Now;
                        retries++;
                    }
                }

                if (!restarted)
                {
                    SafeClose();
                    throw new Exception("No restart message received from ground unit");
                }
            }
            else
                throw new Exception("Port is already open");
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
        void Open() 
        {
            _serialPort.Open();  
        }
        public void SafeClose() 
        {
            if (_serialPort.IsOpen)
                _serialPort.Close();
            else
                throw new Exception("Port is already closed");
        }
        public string ReadLine() 
        {
            try 
            {
                return _serialPort.ReadLine();
            }
            catch (Exception ex) 
            {
                return "";
            }
        }

        void IDisposable.Dispose()
        {
            _serialPort.Close();
        }
    }
}
