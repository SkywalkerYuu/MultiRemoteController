using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MSTSCLib;

using MultiRemoteController.Bases;
using MultiRemoteController.Utilities;

namespace MultiRemoteController
{
    public partial class RDPClientForm : Form
    {
        private Device mDevice;

        public RDPClientForm()
        {
            InitializeComponent();
            mDevice = MainForm.Instance.CurDevice;
        }

        private void RDPClientForm_Load(object sender, EventArgs e)
        {
            axMsTscAxNotSafeForScripting1.Server = mDevice.DeviceIP;
            axMsTscAxNotSafeForScripting1.UserName = mDevice.DeviceUser;
            IMsTscNonScriptable secured = (IMsTscNonScriptable)axMsTscAxNotSafeForScripting1.GetOcx();
            secured.ClearTextPassword = mDevice.DevicePwd;
            axMsTscAxNotSafeForScripting1.Connect();
        }

        private void RDPClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("是否退出远程连接", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if(dr == DialogResult.Yes)
            {
                try
                {
                    axMsTscAxNotSafeForScripting1.Disconnect();
                }
                catch { }
            }
            if(dr == DialogResult.No)
            {
                // Do nothing
            }
        }
    }
}
