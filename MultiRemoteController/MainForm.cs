using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Web;
using System.Net;
using System.IO;
using System.Threading;

using MultiRemoteController.Bases;
using MultiRemoteController.Utilities;

namespace MultiRemoteController
{
    public partial class MainForm : Form
    {
        private static MainForm instance;

        private List<Device> devices;
        private List<Bases.Program> programs;
        private List<Group> groups;

        private Configs mCurConfig;
        private Device mCurDevice;
        private Bases.Program mCurProgram;
        private Group mCurGroup;

        private const int TIMEOUT = 3000;

        // 当前的配置
        public Configs CurConfig
        {
            get { return mCurConfig; }
        }
        // 当前选择的设备
        public Device CurDevice
        {
            get { return mCurDevice; }
        }
        // 当前选择的程序
        public Bases.Program CurProgram
        {
            get { return mCurProgram; }
        }
        // 当前选择的分组
        public Group CurGroup
        {
            get { return mCurGroup; }
        }

        public static MainForm Instance
        {
            get
            {
                return instance;
            }
        }

        public MainForm()
        {
            instance = this;
            InitializeComponent();
            XMLOperator.Initialize();
            BindTreeView();
            mCurConfig = XMLOperator.GetConfigs();
        }

        /// <summary>
        /// 第一次启动时添加xml地初始数据（弃用）
        /// </summary>
        private void GenerateInfo()
        {

            Configs myConfig = new Configs(true, false, true);
            Device myDevice = new Device("Default Device", "192.168.1.118", "Learun", "116219");
            Bases.Program program1 = new Bases.Program("InternetExporer", "C:\\\\Program Files (x86)\\\\Internet Explorer\\\\iexplore.exe", "http://www.baidu.com", "Default Device");
            Bases.Program program2 = new Bases.Program("Media Player", "C:\\\\Program Files (x86)\\\\Windows Media PLayer\\\\wmplayer.exe", "C:\\\\Users\\\\Learun\\\\Documents\\\\Scrats.mp4", "Default Device");
            
            XMLOperator.WriteConfig(myConfig);
            XMLOperator.WriteDevice(myDevice);
            try
            {
                XMLOperator.WriteProgram(program1);
                XMLOperator.WriteProgram(program2);
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

        /// <summary>
        /// 为TreeView绑定数据源
        /// </summary>
        private void BindTreeView()
        {
            // Cannot Edit
            treeView1.LabelEdit = false;

            // 添加根节点
            TreeNode root = new TreeNode();
            root.Text = "我的程序";
            root.Tag = "Root";
            TreeNode root2 = new TreeNode();
            root2.Text = "我的分组";
            root2.Tag = "GroupRoot";

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
                //Console.WriteLine("The Capacity of programs is "+programs.Capacity);
                for(int j = 0; j < programs.Count; j++)
                {
                    TreeNode pLeaf = new TreeNode();
                    //Console.WriteLine("j is " + j.ToString());
                    Bases.Program curProgram = programs[j];
                    pLeaf.Text = curProgram.Name;
                    pLeaf.Tag = "Program";
                    curBranch.Nodes.Add(pLeaf);
                }
                root.Nodes.Add(curBranch);
                programs.Clear();
            }

            // 添加分组以及分组下程序信息
            // 添加第一级节点（分组节点）
            //      添加第二级节点（程序节点）
            groups = XMLOperator.GetGroups();
            for(int i = 0; i < groups.Count; i++)
            {
                TreeNode curBranch = new TreeNode();
                Group curGroup = groups[i];
                curBranch.Text = curGroup.groupName;
                curBranch.Tag = "Group";
                programs = XMLOperator.GetProgramsFromGroup(curGroup.groupName);
                for(int j = 0; j < programs.Count; j++)
                {
                    TreeNode pLeaf = new TreeNode();
                    Bases.Program curProgram = programs[j];
                    pLeaf.Text = curProgram.Name + "(" + curProgram.BelongDevice + ")";
                    pLeaf.Tag = "GroupItem";
                    curBranch.Nodes.Add(pLeaf);
                }
                root2.Nodes.Add(curBranch);
                programs.Clear();
            }

            treeView1.Nodes.Add(root);
            treeView1.Nodes.Add(root2);
        }

        /// <summary>
        /// 刷新TreeView
        /// </summary>
        private void RefreshTreeView()
        {
            // Clear First, Then Bind
            treeView1.Nodes.Clear();

            BindTreeView();
        }

        /// <summary>
        /// 为树状图中的节点设置点击行为
        /// </summary>
        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            // After right-clicking the node, show the options according to its tag
            if(e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = treeView1.GetNodeAt(e.X, e.Y);

                if (treeView1.SelectedNode != null)
                {
                    switch (treeView1.SelectedNode.Tag)
                    {
                        case "Device":
                            contextMenuDevice.Show(treeView1, e.Location);
                            break;
                        case "Program":
                            contextMenuProgram.Show(treeView1, e.Location);
                            break;
                        case "Root":
                            contextMenuRoot.Show(treeView1, e.Location);
                            break;
                        case "GroupRoot":
                            contextMenuGroupRoot.Show(treeView1, e.Location);
                            break;
                        case "Group":
                            contextMenuGroup.Show(treeView1, e.Location);
                            break;
                        default:
                            break;
                    }
                }
            }

            // After left-clicking the node, show the detailed information according to its tag.
            if(e.Button == MouseButtons.Left)
            {
                treeView1.SelectedNode = treeView1.GetNodeAt(e.X, e.Y);

                if(treeView1.SelectedNode != null)
                {
                    switch (treeView1.SelectedNode.Tag)
                    {
                        case "Device":
                            refreshDevice(treeView1.SelectedNode.Text);
                            break;
                        case "Program":
                            refreshProgram(treeView1.SelectedNode.Parent.Text, treeView1.SelectedNode.Text);
                            break;
                        case "GroupItem":
                            mCurGroup = XMLOperator.GetGroup(treeView1.SelectedNode.Parent.Text);
                            string groupInfo = treeView1.SelectedNode.Text;
                            string[] tempArray = groupInfo.Split(new char[2] { '(', ')' });
                            refreshProgram(tempArray[1], tempArray[0]);
                            break;
                        case "Group":
                            mCurGroup = XMLOperator.GetGroup(treeView1.SelectedNode.Text);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void refreshDevice(string deviceName)
        {
            // Set device groupbox's visibility to true, the other one is false.
            groupBox1.Visible = true;
            groupBox2.Visible = false;

            // Refresh Device's information
            Device tempDevice = XMLOperator.GetDevice(deviceName);
            mCurDevice = tempDevice;
            try
            {
                textBox1.Text = tempDevice.DeviceName;
                textBox2.Text = tempDevice.DeviceIP;
                textBox3.Text = tempDevice.DeviceUser;
                textBox4.Text = tempDevice.DevicePwd;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void refreshProgram(string deviceName, string programName)
        {
            // Set program groupbox's visibility to true, the other one is false.
            groupBox2.Visible = true;
            groupBox1.Visible = false;

            // Refresh Program's information
            Bases.Program tempProgram = XMLOperator.GetProgram(deviceName, programName);
            mCurProgram = tempProgram;
            try
            {
                textBox5.Text = tempProgram.Name;
                textBox6.Text = tempProgram.Path;
                textBox7.Text = tempProgram.Args;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        /// <summary>
        /// 构造将要发送的URL
        /// </summary>
        /// <param name="config">启动配置</param>
        /// <param name="device">程序所属的设备</param>
        /// <param name="program">选中的程序</param>
        /// <returns>URL</returns>
        private string generateURL(Configs config, Device device, Bases.Program program)
        {
            string result = "";
            string host = "http://" + XMLOperator.GetHost() + "/PsExecProject/index.php";
            result = host + "?r=" + device.DeviceIP + "&u=" + device.DeviceUser + "&p=" + device.DevicePwd;
            result += config.IsGUI ? "&i=1" : "&i=0";
            result += config.IsWait ? "&w=1" : "&w=0";
            result += config.IsSystem ? "&s=1" : "&s=0";
            result += "&e=" + HttpUtility.UrlEncode(program.Path) + "&args=" + HttpUtility.UrlEncode(program.Args);

            return result;
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="url">发送的URL</param>
        /// <param name="timeout">超时的时间</param>
        /// <param name="method">请求方法</param>
        /// <returns></returns>
        private static string SendRequest(string url, int timeout, string method)
        {
            string result = "";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.Timeout = timeout;

            Stream stream = null;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                stream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (WebException e)
            {
                result = e.Message + "\n";
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            return result;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        #region 对程序节点的右键点击事件

        /// <summary>
        /// 启动当前选择的程序
        /// </summary>
        private void 启动ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mCurConfig = XMLOperator.GetConfigs();

            string url = generateURL(mCurConfig, mCurDevice, mCurProgram);

            textBox8.Text = SendRequest(url, TIMEOUT, "GET");
            
        }

        /// <summary>
        /// 编辑当前选择的程序
        /// </summary>
        private void 编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditProgramForm editProgramForm = new EditProgramForm();
            DialogResult dr =  editProgramForm.ShowDialog();
            if(dr == DialogResult.OK)
            {
                RefreshTreeView();
            }
            if(dr == DialogResult.Cancel)
            {
                // Do nothing now
            }
        }

        /// <summary>
        /// 删除当前选择的程序
        /// </summary>
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("是否要删除该程序？（无法回退该操作）", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if(dr == DialogResult.Yes)
            {
                XMLOperator.DeleteProgram(mCurDevice.DeviceName, mCurProgram.Name);
                RefreshTreeView();
            }

            if(dr == DialogResult.No)
            {
                // Do nothing
            }
        }

        #endregion

        #region 对设备节点的右键点击事件

        /// <summary>
        /// 新建程序
        /// </summary>
        private void 新建ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            NewProgramForm newProgramForm = new NewProgramForm();
            DialogResult dr = newProgramForm.ShowDialog();
            if(dr == DialogResult.OK)
            {
                RefreshTreeView();
            }
            if(dr == DialogResult.Cancel)
            {
                // Do nothing
            }
        }

        /// <summary>
        /// 编辑当前的设备
        /// </summary>
        private void 编辑ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EditDeviceForm editDeviceForm = new EditDeviceForm();
            DialogResult dr = editDeviceForm.ShowDialog();
            if (dr == DialogResult.OK)
            {
                RefreshTreeView();
            }
            if (dr == DialogResult.Cancel)
            {
                // Do nothing
            }
        }

        /// <summary>
        /// 对目标电脑进行远程桌面连接
        /// </summary>
        private void 连接ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RDPClientForm rdpClient = new RDPClientForm();
            rdpClient.Show();
        }

        /// <summary>
        /// 删除当前设备以及当前设备下的程序
        /// </summary>
        private void 删除ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("是否要删除该设备及其所有程序？（无法回退该操作）", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                XMLOperator.DeleteDevice(mCurDevice.DeviceName);
                RefreshTreeView();
            }

            if (dr == DialogResult.No)
            {
                // Do nothing
            }
        }

        /// <summary>
        /// 仅删除当前设备下的程序
        /// </summary>
        private void 删除所有程序ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("是否要删除该设备下的所有程序？（无法回退该操作）", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                XMLOperator.DeletePrograms(mCurDevice.DeviceName);
                RefreshTreeView();
            }

            if (dr == DialogResult.No)
            {
                // Do nothing
            }
        }

        #endregion

        #region 对根节点的右键点击事件

        /// <summary>
        /// 新建设备
        /// </summary>
        private void 新建设备ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewDeviceForm newDeviceForm = new NewDeviceForm();
            DialogResult dr = newDeviceForm.ShowDialog();
            if(dr == DialogResult.OK)
            {
                RefreshTreeView();
            }
            if(dr == DialogResult.Cancel)
            {
                // Do nothing
            }
        }

        #endregion

        #region 对分组根节点的右键点击事件

        private void 新建分组ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            NewGroupForm newGroupForm = new NewGroupForm();
            DialogResult dr = newGroupForm.ShowDialog();
            if(dr == DialogResult.OK)
            {
                RefreshTreeView();
            }
            if(dr == DialogResult.Cancel)
            {
                // Do nothing
            }
        }

        #endregion

        #region 对分组节点的右键点击事件

        /// <summary>
        /// 启动分组中所有的程序
        /// </summary>
        private void 启动ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            mCurConfig = XMLOperator.GetConfigs();

            List<Bases.Program> launchingPrograms = XMLOperator.GetProgramsFromGroup(mCurGroup.groupName);

            foreach(Bases.Program program in launchingPrograms)
            {
                Device device = XMLOperator.GetDevice(program.BelongDevice);
                string url = generateURL(mCurConfig, device, program);
                textBox8.Text = SendRequest(url, TIMEOUT, "GET");
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// 对分组进行编辑
        /// </summary>
        private void 编辑ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            EditGroupForm editGroupForm = new EditGroupForm();
            DialogResult dr = editGroupForm.ShowDialog();
            if (dr == DialogResult.OK)
            {
                RefreshTreeView();
            }
            if (dr == DialogResult.Cancel)
            {
                // Do nothing
            }
        }

        /// <summary>
        /// 删除分组及其所有程序
        /// </summary>
        private void 删除ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("是否要删除该分组及其所有程序？（无法回退该操作）", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                XMLOperator.DeleteGroup(mCurGroup.groupName);
                RefreshTreeView();
            }

            if (dr == DialogResult.No)
            {
                // Do nothing
            }
        }

        /// <summary>
        /// 删除分组下的所有程序
        /// </summary>
        private void 删除所有程序ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("是否要删除该分组的所有程序？（无法回退该操作）", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                XMLOperator.DeleteProgramsInGroup(mCurGroup.groupName);
                RefreshTreeView();
            }

            if (dr == DialogResult.No)
            {
                // Do nothing
            }
        }

        #endregion

        #region 对设置的点击事件

        private void 远程设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoteConfigForm remoteConfigForm = new RemoteConfigForm();
            DialogResult dr = remoteConfigForm.ShowDialog();
            if(dr == DialogResult.OK)
            {
                mCurConfig = XMLOperator.GetConfigs();
            }
            if(dr == DialogResult.Cancel)
            {
                // Do nothing
            }
        }

        private void 启动设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartupConfigForm startupConfigForm = new StartupConfigForm();
            DialogResult dr = startupConfigForm.ShowDialog();
            if(dr == DialogResult.OK)
            {
                mCurConfig = XMLOperator.GetConfigs();
            }
            if(dr == DialogResult.Cancel)
            {
                // Do nothing
            }
        }


        #endregion

        private void 使用帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GuideForm guideForm = new GuideForm();
            guideForm.Show();
        }
    }
}
