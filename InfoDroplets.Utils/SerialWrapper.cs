using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoDroplets.Utils
{
    public class SerialWrapper : IDisposable
    {
        SerialPort _serialPort;

        public SerialWrapper(SerialPort serialPort)
        {
            _serialPort = serialPort;
            _serialPort.BaudRate = 9600;
        }

        public void SetPortName(string PortName)
        {
            _serialPort.PortName = PortName;
        }

        public void SetBaudeRate(int BaudeRate)
        {
            _serialPort.BaudRate = BaudeRate;
        }

        public void SafeOpen() 
        {
            if (!_serialPort.IsOpen)
            {
                Open();
                _serialPort.WriteLine("reset");
                bool started =false;
                while(!started) 
                {
                    var input = this.ReadLine();
                    if(input.Contains("GNU Reciever"))
                        started = true;
                }
            }
        }
        public void Open(){ _serialPort.Open(); }
        public void Close() { _serialPort.Close(); }
        public string ReadLine() { return _serialPort.ReadLine(); }
        public void Reset()
        {
            _serialPort.WriteLine("reset");
        }

        public void Dispose()
        {
            _serialPort?.Close();
        }
    }
}
