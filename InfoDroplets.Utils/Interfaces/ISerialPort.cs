using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoDroplets.Utils.Interfaces
{
    public interface ISerialPort : IDisposable
    {
        string PortName { get; set; }
        int BaudRate { get; set; }
        bool IsOpen { get; }

        event SerialDataReceivedEventHandler DataReceived;
        void Open();
        void Close();
        void WriteLine(string text);
        string ReadLine();
    }
}
