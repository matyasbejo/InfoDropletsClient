using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoDroplets.Utils.SerialCommunication
{
    public delegate void CommandGeneratedEventHandler(object sender, CommandEventArgs e);
    public class CommandEventArgs : EventArgs
    {
        public string Command { get; }

        public CommandEventArgs(string command)
        {
            Command = command;
        }
    }
}
