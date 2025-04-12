using InfoDroplets.Logic;
using InfoDroplets.Models;
using InfoDroplets.Repository;
using InfoDroplets.Utils.SerialCommunication;
using System.IO.Ports;

namespace InfoDroplets.ConsoleClient
{
    internal class Program
    {
        static ISerialWrapper sw {  get; set; }
        static ITrackingEntryLogic trackingEntryLogic { get; set; }
        static IDropletLogic dropletLogic { get; set; }
        static void Main(string[] args)
        {
            ClientDbContext ctx = new ClientDbContext();

            IRepository<Droplet> dropletRepository = new DropletRepository(ctx);
            IRepository<TrackingEntry> trackingEntryRepository = new TrackingEntryRepository(ctx);

            dropletLogic = new DropletLogic(null, dropletRepository);
            trackingEntryLogic = new TrackingEntryLogic(null, trackingEntryRepository);

            sw = new SerialWrapper();

            var a = sw.AvaliableSerialPorts;

            sw.SetPortName("COM5");
            sw.SafeOpen();

            dropletLogic.CommandGenerated += sw.SendCommand;
            sw.WrapperDataReceived += OnDataReceived;

            while (true) { }
        }

        static void OnDataReceived(object sender, EventArgs e)
        {
            var line = sw.ReadLine();
            try
            {
                try
                {
                    trackingEntryLogic.Create(line);
                }
                catch (NullReferenceException ex)
                {
                    var newDropletId = int.Parse(line.Trim().Split(';')[0]);
                    dropletLogic.Create(new Droplet(newDropletId));
                    Console.WriteLine($"Droplet {newDropletId} added.");
                }
                dropletLogic.UpdateDropletStatus(8, new GpsPos(47.500429, 19.084596, 100));
                Console.WriteLine($"Added: {line}");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
    }
}
