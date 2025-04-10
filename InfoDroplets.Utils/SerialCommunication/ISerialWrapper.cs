namespace InfoDroplets.Utils.SerialCommunication
{
    public interface ISerialWrapper
    {
        List<string> AvaliableSerialPorts { get; }

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