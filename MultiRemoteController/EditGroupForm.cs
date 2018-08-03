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
    public partial class EditGroupForm : Form
    {
        private Group mGroup;

        private string _returnedResult;

        List<string> _items = new List<string>();

        public EditGroupForm()
        {
            InitializeComponent();
            mGroup = MainForm.Instance.CurGroup;
            InitializeInfos();
        }

        private void InitializeInfos()
        {
            textBox1.Text = mGroup.groupName;
            List<string> programs = mGroup.GetPrograms();
            List<string> devices = mGroup.GetDevices();
            for(int i = 0; i < programs.Count; i++)
            {
                string content = programs[i] + "(" + devices[i] + ")";
                _items.Add(content);
            }
            listBox1.DataSource = null;
            listBox1.DataSource = _items;
        }

        /// <summary>
        /// 为程序列表添加成员
        /// </summary>
        private void btn_AddItem_Click(object sender, EventArgs e)
        {
            using (ProgramListForm programListForm = new ProgramListForm())
            {
                DialogResult dr = programListForm.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    if (programListForm.SelectedProgram != "" && programListForm.SelectedProgram != "空")
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

        /// <summary>
        /// 更新分组信息
        /// </summary>
        private void btn_OK_Click(object sender, EventArgs e)
        {
            mGroup.Clear();
            foreach(string content in listBox1.Items)
            {
                string[] tempArray = content.Split(new char[2] { '(', ')' });
                mGroup.AddProgram(tempArray[1], tempArray[0]);
            }
            XMLOperator.UpdateGroup(mGroup);
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
