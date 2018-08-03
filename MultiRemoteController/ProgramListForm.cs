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
    public partial class ProgramListForm : Form
    {
        private List<Device> devices;
        private List<Bases.Program> programs;

        public string SelectedProgram
        {
            get {
                if (treeCombo.Text != "" && treeCombo.Text != "空")
                {
                    string[] tempArray = treeCombo.Text.Split(new char[2] { '(', ')' });
                    if (XMLOperator.isProgramExists(tempArray[1], tempArray[0]))
                    {
                        return treeCombo.Text;
                    }
                    else
                    {
                        MessageBox.Show("程序名不可用", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return "";
                    }
                }
                else
                {
                    return "";
                };
            }
        }

        public ProgramListForm()
        {
            InitializeComponent();
            BindTreeView();
        }

        /// <summary>
        /// 为程序列表绑定数据
        /// </summary>
        private void BindTreeView()
        {
            treeCombo.TreeView.LabelEdit = false;

            TreeNode root = new TreeNode();
            root.Tag = "Root";
            root.Text = "我的程序";

            // 添加设备以及相关程序信息
            // 添加第一级节点（设备节点）
            //      添加第二级节点（程序节点）
            // 注意Count和Capacity的不同
            devices = XMLOperator.GetDevices();
            for (int i = 0; i < devices.Count; i++)
            {
                TreeNode curBranch = new TreeNode();
                Device curDevice = devices[i];
                curBranch.Text = curDevice.DeviceName;
                curBranch.Tag = "Device";
                programs = XMLOperator.GetPrograms(curDevice.DeviceName);
                for (int j = 0; j < programs.Count; j++)
                {
                    TreeNode pLeaf = new TreeNode();
                    Bases.Program curProgram = programs[j];
                    pLeaf.Text = curProgram.Name;
                    pLeaf.Tag = "Program";
                    curBranch.Nodes.Add(pLeaf);
                }
                root.Nodes.Add(curBranch);
                programs.Clear();
            }

            treeCombo.TreeView.Nodes.Add(root);
            this.Controls.Add(treeCombo);
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void treeCombo_DoubleClick(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && (e.Clicks == 2))
            {
                treeCombo.TreeView.SelectedNode = treeCombo.TreeView.GetNodeAt(e.X, e.Y);

                if (treeCombo.TreeView.SelectedNode != null)
                {
                    switch (treeCombo.TreeView.SelectedNode.Tag)
                    {
                        case "Program":
                            treeCombo.Text = treeCombo.TreeView.SelectedNode.Text + "(" + treeCombo.TreeView.SelectedNode.Parent.Text + ")";
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
