using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiRemoteController.Bases
{
    public class Group
    {
        public string groupName { get; set; }

        private List<string> programs;

        private List<string> belongDevices;
        
        public Group(string name)
        {
            this.groupName = name;
            this.programs = new List<string>();
            this.belongDevices = new List<string>();
        }

        public void AddProgram(string deviceName,string programName)
        {
            belongDevices.Add(deviceName);
            programs.Add(programName);
        }

        public List<string> GetDevices()
        {
            return belongDevices;
        }

        public List<string> GetPrograms()
        {
            return programs;
        }

        public void Clear()
        {
            programs.Clear();
            belongDevices.Clear();
        }
    }
}
