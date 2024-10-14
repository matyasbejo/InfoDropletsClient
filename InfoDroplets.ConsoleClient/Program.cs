using System;
using InfoDroplets.Logic;
using InfoDroplets.Models;
using InfoDroplets.Repository;
using InfoDroplets.Utils.SerialCommunication;

namespace InfoDroplets.ConsoleClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ClientDbContext ctx = new ClientDbContext();

            DropletRepository dropletRepository = new DropletRepository(ctx);
            TrackingEntryRepository trackingEntryRepository = new TrackingEntryRepository(ctx);

            DropletLogic dropletLogic = new DropletLogic(dropletRepository);
            TrackingEntryLogic trackingEntryLogic = new TrackingEntryLogic(trackingEntryRepository);

            SerialWrapper sw = new SerialWrapper(new System.IO.Ports.SerialPort());

            dropletLogic.CommandGenerated += sw.SendCommand;

            var a = SerialWrapper.GetPortNames();
            sw.SetPortName("COM4");
            sw.SafeOpen();

            dropletLogic.CommandGenerated += sw.SendCommand;

            while (true)
            {
                var line = sw.ReadLine();
                try
                {
                    trackingEntryLogic.Create(line);
                    Console.WriteLine($"Added: {line}");
                }
                catch (Exception e){ Console.WriteLine(e.Message); }
            }
        }
    }
}
