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
    public partial class StartupConfigForm : Form
    {
        private Configs mConfig;

        public StartupConfigForm()
        {
            InitializeComponent();
            mConfig = MainForm.Instance.CurConfig;
            InitializeInfos();
        }

        private void InitializeInfos()
        {
            checkBox1.Checked = mConfig.IsGUI;
            checkBox2.Checked = mConfig.IsWait;
            checkBox3.Checked = mConfig.IsSystem;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            mConfig.IsGUI = checkBox1.Checked;
            mConfig.IsWait = checkBox2.Checked;
            mConfig.IsSystem = checkBox3.Checked;

            XMLOperator.UpdateConfig(mConfig);
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
