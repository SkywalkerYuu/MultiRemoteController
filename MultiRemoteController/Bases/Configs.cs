using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiRemoteController.Bases
{
    public class Configs
    {
        public bool IsGUI { get; set; }

        public bool IsWait { get; set; }

        public bool IsSystem { get; set; }

        public Configs()
        {
            new Configs(true, false, true);
        }

        public Configs(bool isGui, bool isWait, bool isSystem)
        {
            this.IsGUI = isGui;
            this.IsWait = isWait;
            this.IsSystem = isSystem;
        }
    }
}
