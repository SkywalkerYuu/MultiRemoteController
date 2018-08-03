using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiRemoteController.Bases
{
    public class Program
    {

        public string Name { get; set; }

        public string Path { get; set; }

        public string Args { get; set; }

        public string BelongDevice { get; set; }

        public Program(string name, string path, string args, string device)
        {
            this.Name = name;
            this.Path = path;
            this.Args = args;
            this.BelongDevice = device;
        }
    }
}
