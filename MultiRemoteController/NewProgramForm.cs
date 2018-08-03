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
    public partial class NewProgramForm : Form
    {
        private static Device mDevice;

        public NewProgramForm()
        {
            InitializeComponent();
            mDevice = MainForm.Instance.CurDevice;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            // Get all information in textbox
            string belongDevice = textBox1.Text;
            string programName = textBox2.Text;
            string programPath = textBox3.Text;
            string programArgs = textBox4.Text;

            if(programName != "" && DataValidator.IsFilePath(programPath))
            {
                if(!XMLOperator.isProgramExists(belongDevice, programName))
                {
                    Bases.Program tempProgram = new Bases.Program(programName, programPath, programArgs, belongDevice);

                    XMLOperator.WriteProgram(tempProgram);
                }
                else
                {
                    MessageBox.Show("该程序已存在", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
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

        private void NewProgramForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = mDevice.DeviceName;
        }

        private void btn_SelectPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            DialogResult dr = openFileDialog.ShowDialog();
            if(dr == DialogResult.OK)
            {
                textBox3.Text = openFileDialog.FileName;
            }
            if(dr == DialogResult.Cancel)
            {
                // Do nothing
            }
        }
    }
}
