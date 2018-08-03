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
    public partial class NewGroupForm : Form
    {
        private string _returnedResult;

        List<string> _items = new List<string>();

        public NewGroupForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 为程序列表添加成员
        /// </summary>
        private void btn_AddItem_Click(object sender, EventArgs e)
        {
            using(ProgramListForm programListForm = new ProgramListForm())
            {
                DialogResult dr = programListForm.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    if(programListForm.SelectedProgram != "" && programListForm.SelectedProgram != "空")
                    {
                        _returnedResult = programListForm.SelectedProgram;
                        _items.Add(_returnedResult);

                        listBox1.DataSource = null;
                        listBox1.DataSource = _items;
                    }
                }
                if (dr == DialogResult.Cancel)
                {
                    // Do Nothing
                }
            }
        }

        /// <summary>
        /// 移除程序列表中的成员
        /// </summary>
        private void btn_RemoveItem_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBox1.SelectedIndex;
            try
            {
                _items.RemoveAt(selectedIndex);
            }
            catch { }

            listBox1.DataSource = null;
            listBox1.DataSource = _items;

        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != "")
            {
                if (!XMLOperator.isGroupExists(textBox1.Text))
                {
                    Group tempGroup = new Group(textBox1.Text);
                    foreach (string content in listBox1.Items)
                    {
                        string[] tempArray = content.Split(new char[2] { '(', ')' });
                        tempGroup.AddProgram(tempArray[1], tempArray[0]);
                    }
                    XMLOperator.WriteGroup(tempGroup);
                }
                else
                {
                    MessageBox.Show("该分组已存在", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
    }
}
