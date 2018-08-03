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
    public partial class RemoteConfigForm : Form
    {
        public RemoteConfigForm()
        {
            InitializeComponent();
            InitializeInfos();
        }

        private void InitializeInfos()
        {
            String[] tempArray = XMLOperator.GetHost().Split(':');
            textBox1.Text = tempArray[0];
            textBox2.Text = tempArray[1];
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            string ip = textBox1.Text.Trim();
            string port = textBox2.Text.Trim();
            try
            {
                if (DataValidator.IsIP(ip) && DataValidator.IsValidPort(Convert.ToInt32(port)))
                {
                    string result = textBox1.Text.Trim() + ":" + textBox2.Text.Trim();
                    XMLOperator.UpdateHost(result);
                }
                else
                {
                    MessageBox.Show("参数不合法", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }catch(OverflowException ex)
            {
                MessageBox.Show("参数不合法", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }catch(FormatException ex)
            {

            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
