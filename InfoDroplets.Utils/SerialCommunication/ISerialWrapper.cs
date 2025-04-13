using System.IO.Ports;

namespace InfoDroplets.Utils.SerialCommunication
{
    public interface ISerialWrapper
    {
        List<string> AvaliableSerialPorts { get; }
        int SelectedBaudRate { get; set; }
        string SelectedSerialPort { get; set; }

        event SerialDataReceivedEventHandler WrapperDataReceived;

        void SafeClose();
        int GetBaudeRate();
        string GetPortName();
        void Open();
        string ReadLine();
        void Reset();
        void SafeOpen();
        void SendCommand(object sender, CommandEventArgs e);
        void SetBaudeRate(int BaudeRate = 9600);
        void SetPortName(string PortName);
        void WriteLine(string message);
    }
}