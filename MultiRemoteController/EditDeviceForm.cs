using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MultiRemoteController.Bases;
using MultiRemoteController.Utilities;

namespace MultiRemoteController
{
    public partial class EditDeviceForm : Form
    {
        private static Device mDevice;

        public EditDeviceForm()
        {
            InitializeComponent();
            mDevice = MainForm.Instance.CurDevice;
            initComponentInfo();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            // Get all information in textbox
            string deviceName = textBox1.Text;
            string deviceIP = textBox2.Text;
            string deviceUser = textBox3.Text;
            string devicePwd = textBox4.Text;

            if(deviceName != "" && DataValidator.IsIP(deviceIP) && devicePwd != "" && deviceUser != "")
            {
                Device updatingDevice = new Device(deviceName, deviceIP, deviceUser, devicePwd);

                XMLOperator.UpdateDevice(updatingDevice);
            }
            else
            {
                MessageBox.Show("输入信息不合法", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void initComponentInfo()
        {
            textBox1.Text = mDevice.DeviceName;
            textBox2.Text = mDevice.DeviceIP;
            textBox3.Text = mDevice.DeviceUser;
            textBox4.Text = mDevice.DevicePwd;
        }
    }
}
