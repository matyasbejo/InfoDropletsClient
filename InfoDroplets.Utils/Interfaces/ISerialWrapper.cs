using InfoDroplets.Utils.SerialCommunication;

namespace InfoDroplets.Utils.Interfaces
{
    public interface ISerialWrapper
    {
        List<string> AvaliableSerialPorts { get; set; }

        void Close();
        void Dispose();
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