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
    public partial class NewDeviceForm : Form
    {
        public NewDeviceForm()
        {
            InitializeComponent();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            // TODO: Confirm cancel operation.
            // If there are information in the textbox, then show a messagebox to ask user to confirm it
            this.Close();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            // Get all information in textbox
            string deviceName = textBox1.Text;
            string deviceIp = textBox2.Text;
            string deviceUser = textBox3.Text;
            string devicePwd = textBox4.Text;

            if(deviceName != "" && DataValidator.IsIP(deviceIp) && deviceUser != "" && devicePwd != "")
            {
                if (!XMLOperator.isDeviceExists(deviceName))
                {
                    Device tempDevice = new Device(deviceName, deviceIp, deviceUser, devicePwd);

                    XMLOperator.WriteDevice(tempDevice);
                }
                else
                {
                    MessageBox.Show("此设备已存在", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("输入信息不合法", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
