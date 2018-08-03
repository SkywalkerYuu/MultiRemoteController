using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiRemoteController.Bases
{
    public class Device
    {
        public string DeviceName { get; set; }

        public string DeviceIP { get; set; }

        public string DeviceUser { get; set; }

        public string DevicePwd { get; set; }

        public Device(string name, string ip, string user, string pwd)
        {
            this.DeviceName = name;
            this.DeviceIP = ip;
            this.DeviceUser = user;
            this.DevicePwd = pwd;
        }
    }
}
