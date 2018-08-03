using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

using MultiRemoteController.Bases;

namespace MultiRemoteController.Utilities
{
    /// <summary>
    /// XML的数据操作中心
    /// e.g. XML结构
    /// <root>
    /// <config>
    ///     <IsGui></IsGui>
    ///     <IsWait></IsWait>
    ///     <IsSystem></IsSystem>
    ///     <host></host>
    /// </config>
    /// <device>
    ///     <name></name>
    ///     <user></user>
    ///     <pwd></pwd>
    ///     <ip></ip>
    ///     <program>
    ///         <name></name>
    ///         <path></path>
    ///         <args></args>
    ///         <belong></belong>
    ///     </program>
    ///     <program>...</program>
    /// </device>
    /// <device>...</device>
    /// <group>
    ///     <name></name>
    ///     <groupItem>
    ///         <name></name>
    ///         <belong></belong>
    ///     </groupItem>
    ///     <groupItem>...</groupItem>
    /// </group>
    /// <group>...</group>
    /// </root>
    /// </summary>
    class XMLOperator
    {
        private const string xmlpath = "DataInfo.xml";

        private static XmlDocument xmlDoc;

        private static XmlNode mTargetDeviceNode = null;

        private static XmlNode mTargetProgramNode = null;

        private static XmlNode mTargetGroupNode = null;

        /// <summary>
        /// 完成简单的初始化操作
        /// </summary>
        public static void Initialize()
        {
            xmlDoc = new XmlDocument();
            if (!File.Exists(xmlpath))
            {
                try
                {
                    XmlDeclaration declaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                    XmlNode root = xmlDoc.CreateElement("root");
                    xmlDoc.AppendChild(root);
                    xmlDoc.Save(xmlpath);
                    WriteConfig(new Configs(true, false, true));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                try
                {
                    xmlDoc.Load(xmlpath);
                    xmlDoc.Save(xmlpath);
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        #region 获取XML中的信息

        /// <summary>
        /// 获取启动配置信息
        /// </summary>
        /// <returns></returns>
        public static Configs GetConfigs()
        {
            xmlDoc.Load(xmlpath);
            XmlNode configNode = xmlDoc.SelectSingleNode("root").SelectSingleNode("config");
            if(configNode == null)
            {
                throw new Exception("No config in the xml");
            }
            string isGui = configNode.SelectSingleNode("IsGui").InnerText;
            string isWait = configNode.SelectSingleNode("IsWait").InnerText;
            string isSystem = configNode.SelectSingleNode("IsSystem").InnerText;
            return new Configs(Convert.ToBoolean(isGui), Convert.ToBoolean(isWait), Convert.ToBoolean(isSystem));
        }

        /// <summary>
        /// 获取远程服务器的主机名
        /// </summary>
        /// <returns></returns>
        public static string GetHost()
        {
            xmlDoc.Load(xmlpath);
            XmlNode configNode = xmlDoc.SelectSingleNode("root").SelectSingleNode("config");
            if (configNode == null)
            {
                throw new Exception("No config in the xml");
            }
            return configNode.SelectSingleNode("host").InnerText;
        }

        /// <summary>
        /// 根据设备名获取设备信息
        /// </summary>
        /// <param name="deviceName">需要查询的设备名</param>
        /// <returns></returns>
        public static Device GetDevice(string deviceName)
        {
            xmlDoc.Load(xmlpath);

            if (!isDeviceExists(deviceName))
            {
                throw new Exception("No such device in the xml");
            }

            string name = mTargetDeviceNode.SelectSingleNode("name").InnerText;
            string user = mTargetDeviceNode.SelectSingleNode("user").InnerText;
            string pwd = mTargetDeviceNode.SelectSingleNode("pwd").InnerText;
            string ip = mTargetDeviceNode.SelectSingleNode("ip").InnerText;

            return new Device(name, ip, user, pwd);
        }

        /// <summary>
        /// 获取所有设备信息
        /// </summary>
        /// <returns></returns>
        public static List<Device> GetDevices()
        {
            xmlDoc.Load(xmlpath);

            XmlNode targetNode = xmlDoc.SelectSingleNode("root");

            XmlNodeList searchList = targetNode.SelectNodes("device");

            if(searchList == null)
            {
                throw new Exception("No devices in xml!");
            }

            List<Device> devices = new List<Device>();
            foreach(XmlNode node in searchList)
            {
                string name = node.SelectSingleNode("name").InnerText;
                string user = node.SelectSingleNode("user").InnerText;
                string pwd = node.SelectSingleNode("pwd").InnerText;
                string ip = node.SelectSingleNode("ip").InnerText;
                devices.Add(new Device(name, user, pwd, ip));
            }

            return devices;
        }

        /// <summary>
        /// 获取特定设备的特定程序
        /// </summary>
        /// <param name="deviceName">设备名</param>
        /// <param name="programName">程序名</param>
        /// <returns></returns>
        public static Bases.Program GetProgram(string deviceName, string programName)
        {
            xmlDoc.Load(xmlpath);
            
            if(!isProgramExists(deviceName, programName))
            {
                throw new Exception("No such program in the xml");
            }

            string name = mTargetProgramNode.SelectSingleNode("name").InnerText;
            string path = mTargetProgramNode.SelectSingleNode("path").InnerText;
            string args = mTargetProgramNode.SelectSingleNode("args").InnerText;
            string belong = mTargetProgramNode.ParentNode.SelectSingleNode("name").InnerText;

            return new Bases.Program(name, path, args, belong);
        }

        /// <summary>
        /// 获取特定设备下的所有程序信息
        /// </summary>
        /// <param name="deviceName">查询的设备名</param>
        /// <returns></returns>
        public static List<Bases.Program> GetPrograms(string deviceName)
        {
            xmlDoc.Load(xmlpath);

            if (!isDeviceExists(deviceName))
            {
                throw new Exception("No such device in the xml!");
            }

            if (mTargetDeviceNode == null)
            {
                throw new Exception("Cannot Find Device!");
            }

            List<Bases.Program> programs = new List<Bases.Program>();
            XmlNodeList programNodes = mTargetDeviceNode.SelectNodes("program");
            foreach(XmlNode node in programNodes)
            {
                string name = node.SelectSingleNode("name").InnerText;
                string path = node.SelectSingleNode("path").InnerText;
                string args = node.SelectSingleNode("args").InnerText;
                string belong = node.ParentNode.SelectSingleNode("name").InnerText;
                programs.Add(new Bases.Program(name, path, args, belong));
            }

            return programs;
        }

        /// <summary>
        /// 获取所有的分组信息
        /// </summary>
        /// <returns></returns>
        public static List<Group> GetGroups()
        {
            xmlDoc.Load(xmlpath);

            XmlNodeList list = xmlDoc.SelectSingleNode("root").SelectNodes("group");

            if(list == null)
            {
                throw new Exception("No groups in xml!");
            }

            List<Group> groups = new List<Group>();
            foreach(XmlNode node in list)
            {
                Group tempGroup = new Group(node.SelectSingleNode("name").InnerText);
                XmlNodeList programs = node.SelectNodes("program");
                foreach(XmlNode program in programs)
                {
                    tempGroup.AddProgram(program.SelectSingleNode("belong").InnerText, program.SelectSingleNode("name").InnerText);
                }
                groups.Add(tempGroup);
            }

            return groups;
        }

        /// <summary>
        /// 根据分组名获取分组信息
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static Group GetGroup(string groupName) {
            xmlDoc.Load(xmlpath);

            if (!isGroupExists(groupName))
            {
                return null;
            }

            Group group = new Group(groupName);

            XmlNodeList itemList = mTargetGroupNode.SelectNodes("groupItem");
            foreach(XmlNode item in itemList)
            {
                string name = item.SelectSingleNode("name").InnerText;
                string belong = item.SelectSingleNode("belong").InnerText;
                group.AddProgram(belong, name);
            }

            return group;
        }

        /// <summary>
        /// 获取特定分组下的所有程序信息
        /// </summary>
        /// <returns></returns>
        public static List<Bases.Program> GetProgramsFromGroup(string groupName)
        {
            xmlDoc.Load(xmlpath);

            if (!isGroupExists(groupName))
            {
                return null;
            }

            XmlNodeList list = mTargetGroupNode.SelectNodes("groupItem");
            if(list == null)
            {
                return null;
            }

            List<Bases.Program> programList = new List<Bases.Program>();

            foreach(XmlNode item in list)
            {
                string name = item.SelectSingleNode("name").InnerText;
                string belong = item.SelectSingleNode("belong").InnerText;
                Bases.Program program = GetProgram(belong, name);
                programList.Add(program);
            }

            return programList;
        }

        /// <summary>
        /// 获取特定分组下的特定程序信息
        /// </summary>
        /// <param name="groupName">分组名</param>
        /// <param name="deviceName">设备名</param>
        /// <param name="programName">程序名</param>
        /// <returns></returns>
        public static Bases.Program GetProgram(string groupName, string deviceName, string programName)
        {
            xmlDoc.Load(xmlpath);

            if (!isGroupExists(groupName))
            {
                return null;
            }

            XmlNodeList list = mTargetGroupNode.SelectNodes("groupItem");
            if (list == null)
            {
                return null;
            }

            foreach(XmlNode item in list)
            {
                string name = item.SelectSingleNode("name").InnerText;
                string belong = item.SelectSingleNode("belong").InnerText;

                if((name == programName) && (belong == deviceName))
                {
                    return GetProgram(belong, name);
                }
            }

            return null;
        }

        #endregion

        #region 向XML中添加信息（注：再写入配置信息就多此一举了，一般不对其进行public调用）

        /// <summary>
        /// 写入配置信息
        /// </summary>
        /// <param name="config">配置对象</param>
        public static void WriteConfig(Configs config)
        {
            xmlDoc.Load(xmlpath);

            if(xmlDoc.SelectSingleNode("root").SelectSingleNode("config") != null)
            {
                xmlDoc.Save(xmlpath);
                return;
            }

            // Create config.IsGUI element
            XmlElement ele0 = xmlDoc.CreateElement("IsGui");
            XmlText text0 = xmlDoc.CreateTextNode(config.IsGUI.ToString());

            // Create config.IsWait element
            XmlElement ele1 = xmlDoc.CreateElement("IsWait");
            XmlText text1 = xmlDoc.CreateTextNode(config.IsWait.ToString());

            // Create config.IsSystem element
            XmlElement ele2 = xmlDoc.CreateElement("IsSystem");
            XmlText text2 = xmlDoc.CreateTextNode(config.IsSystem.ToString());

            // Create config.host element
            XmlElement ele3 = xmlDoc.CreateElement("host");
            XmlText text3 = xmlDoc.CreateTextNode("192.168.1.114:8081");

            // Create the config node
            XmlNode configNode = xmlDoc.CreateNode(XmlNodeType.Element, "config", "");
            // Add elements to this node
            configNode.AppendChild(ele0);
            configNode.LastChild.AppendChild(text0);
            configNode.AppendChild(ele1);
            configNode.LastChild.AppendChild(text1);
            configNode.AppendChild(ele2);
            configNode.LastChild.AppendChild(text2);
            configNode.AppendChild(ele3);
            configNode.LastChild.AppendChild(text3);

            XmlElement root = xmlDoc.DocumentElement;
            root.AppendChild(configNode);

            xmlDoc.Save(xmlpath);
        }

        /// <summary>
        /// 写入设备信息
        /// </summary>
        /// <param name="device">设备对象</param>
        public static void WriteDevice(Device device)
        {
            xmlDoc.Load(xmlpath);
            if (isDeviceExists(device.DeviceName))
            {
                return;
            }

            // Create device.DeviceName element
            XmlElement ele0 = xmlDoc.CreateElement("name");
            XmlText text0 = xmlDoc.CreateTextNode(device.DeviceName);

            // Create device.DeviceUser element
            XmlElement ele1 = xmlDoc.CreateElement("user");
            XmlText text1 = xmlDoc.CreateTextNode(device.DeviceUser);

            // Create device.DevicePwd element
            XmlElement ele2 = xmlDoc.CreateElement("pwd");
            XmlText text2 = xmlDoc.CreateTextNode(device.DevicePwd);

            XmlElement ele3 = xmlDoc.CreateElement("ip");
            XmlText text3 = xmlDoc.CreateTextNode(device.DeviceIP);

            // Create the device node
            XmlNode deviceNode = xmlDoc.CreateNode(XmlNodeType.Element, "device", "");
            // Add elements to this node
            deviceNode.AppendChild(ele0);
            deviceNode.LastChild.AppendChild(text0);
            deviceNode.AppendChild(ele1);
            deviceNode.LastChild.AppendChild(text1);
            deviceNode.AppendChild(ele2);
            deviceNode.LastChild.AppendChild(text2);
            deviceNode.AppendChild(ele3);
            deviceNode.LastChild.AppendChild(text3);

            XmlElement root = xmlDoc.DocumentElement;
            root.AppendChild(deviceNode);

            xmlDoc.Save(xmlpath);

        }

        /// <summary>
        /// 写入程序信息
        /// </summary>
        /// <param name="program">程序对象</param>
        public static void WriteProgram(Bases.Program program)
        {
            xmlDoc.Load(xmlpath);

            string deviceName = program.BelongDevice;

            if (!isDeviceExists(deviceName))
            {
                return;
            }

            // Create device.DeviceName element
            XmlElement ele0 = xmlDoc.CreateElement("name");
            XmlText text0 = xmlDoc.CreateTextNode(program.Name.ToString());

            // Create device.DeviceUser element
            XmlElement ele1 = xmlDoc.CreateElement("path");
            XmlText text1 = xmlDoc.CreateTextNode(program.Path.ToString());

            // Create device.DevicePwd element
            XmlElement ele2 = xmlDoc.CreateElement("args");
            XmlText text2 = xmlDoc.CreateTextNode(program.Args.ToString());

            XmlElement ele3 = xmlDoc.CreateElement("belong");
            XmlText text3 = xmlDoc.CreateTextNode(program.BelongDevice.ToString());

            // Create the program Node
            XmlNode programNode = xmlDoc.CreateNode(XmlNodeType.Element, "program", "");
            // Add elements to this node
            programNode.AppendChild(ele0);
            programNode.LastChild.AppendChild(text0);
            programNode.AppendChild(ele1);
            programNode.LastChild.AppendChild(text1);
            programNode.AppendChild(ele2);
            programNode.LastChild.AppendChild(text2);
            programNode.AppendChild(ele3);
            programNode.LastChild.AppendChild(text3);

            // Append the programNode to targetNode
            mTargetDeviceNode.AppendChild(programNode);

            xmlDoc.Save(xmlpath);
        }

        /// <summary>
        /// 写入分组信息
        /// </summary>
        /// <param name="group">分组对象</param>
        public static void WriteGroup(Group group)
        {
            xmlDoc.Load(xmlpath);

            // The group must be unique
            if (isGroupExists(group.groupName))
            {
                return;
            }

            // Create the group node
            XmlNode groupNode = xmlDoc.CreateNode(XmlNodeType.Element, "group", "");

            // Create Group.groupName element
            XmlElement ele0 = xmlDoc.CreateElement("name");
            XmlText text0 = xmlDoc.CreateTextNode(group.groupName);

            groupNode.AppendChild(ele0);
            groupNode.LastChild.AppendChild(text0);

            List<string> programs = group.GetPrograms();
            List<string> devices = group.GetDevices();

            for(int i = 0; i < group.GetPrograms().Count; i++)
            {
                XmlElement ele1 = xmlDoc.CreateElement("name");
                XmlText text1 = xmlDoc.CreateTextNode(programs[i]);

                XmlElement ele2 = xmlDoc.CreateElement("belong");
                XmlText text2 = xmlDoc.CreateTextNode(devices[i]);

                XmlNode groupItemNode = xmlDoc.CreateNode(XmlNodeType.Element, "groupItem", "");

                groupItemNode.AppendChild(ele1);
                groupItemNode.LastChild.AppendChild(text1);
                groupItemNode.AppendChild(ele2);
                groupItemNode.LastChild.AppendChild(text2);

                groupNode.AppendChild(groupItemNode);
            }

            XmlElement root = xmlDoc.DocumentElement;
            root.AppendChild(groupNode);

            xmlDoc.Save(xmlpath);
        }

        /// <summary>
        /// 向分组写入程序信息
        /// </summary>
        /// <param name="groupName">分组名</param>
        /// <param name="programName">程序名</param>
        public static void WriteProgramToGroup(string groupName, string deviceName, string programName)
        {
            xmlDoc.Load(xmlpath);

            if (!isGroupExists(groupName))
            {
                return;
            }

            XmlElement ele0 = xmlDoc.CreateElement("name");
            XmlText text0 = xmlDoc.CreateTextNode(programName);
            XmlElement ele1 = xmlDoc.CreateElement("belong");
            XmlText text1 = xmlDoc.CreateTextNode(deviceName);

            XmlNode groupItemNode = xmlDoc.CreateNode(XmlNodeType.Element, "groupItem", "");

            groupItemNode.AppendChild(ele0);
            groupItemNode.LastChild.AppendChild(text0);
            groupItemNode.AppendChild(ele1);
            groupItemNode.LastChild.AppendChild(text1);

            mTargetGroupNode.AppendChild(groupItemNode);

            xmlDoc.Save(xmlpath);
        }

        #endregion

        #region 删除XML中的信息

        /// <summary>
        /// 删除特定设备（同时会删除设备下的所有程序）
        /// </summary>
        /// <param name="deviceName">设备名</param>
        public static void DeleteDevice(string deviceName)
        {
            xmlDoc.Load(xmlpath);

            if (isDeviceExists(deviceName))
            {
                mTargetDeviceNode.ParentNode.RemoveChild(mTargetDeviceNode);
            }

            xmlDoc.Save(xmlpath);
        }

        /// <summary>
        /// 删除特定设备的特定程序
        /// </summary>
        /// <param name="deviceName">设备名</param>
        /// <param name="programName">程序名</param>
        public static void DeleteProgram(string deviceName, string programName)
        {
            xmlDoc.Load(xmlpath);

            if(isProgramExists(deviceName, programName))
            {
                //RemoveAll()无法删除节点自身
                //mTargetProgramNode.RemoveAll();
                mTargetProgramNode.ParentNode.RemoveChild(mTargetProgramNode);
            }

            xmlDoc.Save(xmlpath);
        }

        /// <summary>
        /// 删除特定设备的所有程序
        /// </summary>
        /// <param name="deviceName">设备名</param>
        public static void DeletePrograms(string deviceName)
        {
            xmlDoc.Load(xmlpath);

            if (isDeviceExists(deviceName))
            {
                XmlNodeList programNodes = mTargetDeviceNode.SelectNodes("program");
                foreach (XmlNode node in programNodes)
                {
                    node.ParentNode.RemoveChild(node);
                }
            }

            xmlDoc.Save(xmlpath);
        }

        /// <summary>
        /// 删除特定分组（同时会删除分组下的所有程序）
        /// </summary>
        /// <param name="groupName">分组名</param>
        public static void DeleteGroup(string groupName)
        {
            xmlDoc.Load(xmlpath);

            if (isGroupExists(groupName))
            {
                mTargetGroupNode.ParentNode.RemoveChild(mTargetGroupNode);
            }

            xmlDoc.Save(xmlpath);
        }

        /// <summary>
        /// 删除特定分组的所有程序
        /// </summary>
        /// <param name="groupName">分组名</param>
        public static void DeleteProgramsInGroup(string groupName)
        {
            xmlDoc.Load(xmlpath);

            if (isGroupExists(groupName))
            {
                XmlNodeList itemNodes = mTargetGroupNode.SelectNodes("groupItem");
                foreach(XmlNode node in itemNodes)
                {
                    node.ParentNode.RemoveChild(node);
                }
            }

            xmlDoc.Save(xmlpath);
        }

        /// <summary>
        /// 删除特定分组下的特定程序
        /// </summary>
        /// <param name="groupName">分组名</param>
        /// <param name="programName">程序名</param>
        public static void DeleteProgramInGroup(string groupName, string deviceName, string programName)
        {
            // FIXME: 可能不会被用到，而是直接用更新方法来解决
        }

        #endregion

        #region 更新XML中的信息

        /// <summary>
        /// 更新启动配置信息
        /// </summary>
        /// <param name="config">配置对象</param>
        public static void UpdateConfig(Configs config)
        {
            xmlDoc.Load(xmlpath);

            XmlNode node = xmlDoc.SelectSingleNode("root").SelectSingleNode("config");

            if(node != null)
            {
                node.SelectSingleNode("IsGui").InnerText = config.IsGUI.ToString();
                node.SelectSingleNode("IsWait").InnerText = config.IsWait.ToString();
                node.SelectSingleNode("IsSystem").InnerText = config.IsSystem.ToString();
            }

            xmlDoc.Save(xmlpath);

        }

        /// <summary>
        /// 更新远程服务器主机信息
        /// </summary>
        /// <param name="host"></param>
        public static void UpdateHost(string host)
        {
            xmlDoc.Load(xmlpath);

            XmlNode configNode = xmlDoc.SelectSingleNode("root").SelectSingleNode("config");
            if (configNode == null)
            {
                throw new Exception("No config in the xml");
            }
            configNode.SelectSingleNode("host").InnerText = host;

            xmlDoc.Save(xmlpath);
        }

        /// <summary>
        /// 更新设备信息
        /// </summary>
        /// <param name="device">设备对象</param>
        public static void UpdateDevice(Device device)
        {
            xmlDoc.Load(xmlpath);

            if (isDeviceExists(device.DeviceName))
            {
                mTargetDeviceNode.SelectSingleNode("name").InnerText = device.DeviceName;
                mTargetDeviceNode.SelectSingleNode("user").InnerText = device.DeviceUser;
                mTargetDeviceNode.SelectSingleNode("pwd").InnerText = device.DevicePwd;
                mTargetDeviceNode.SelectSingleNode("ip").InnerText = device.DeviceIP;
            }

            xmlDoc.Save(xmlpath);
        }

        /// <summary>
        /// 更新程序信息
        /// </summary>
        /// <param name="deviceName">设备名</param>
        /// <param name="program">程序对象</param>
        public static void UpdateProgram(string deviceName, Bases.Program program)
        {
            xmlDoc.Load(xmlpath);

            if (isProgramExists(deviceName, program.Name))
            {
                mTargetProgramNode.SelectSingleNode("name").InnerText = program.Name;
                mTargetProgramNode.SelectSingleNode("path").InnerText = program.Path;
                mTargetProgramNode.SelectSingleNode("args").InnerText = program.Args;
            }

            xmlDoc.Save(xmlpath);
        }

        /// <summary>
        /// 更新分组信息
        /// </summary>
        /// <param name="group">分组对象</param>
        public static void UpdateGroup(Group group)
        {
            xmlDoc.Load(xmlpath);

            string groupName = group.groupName;
            if (isGroupExists(groupName))
            {
                // 更新分组名
                mTargetGroupNode.SelectSingleNode("node").InnerText = groupName;
                // 删除分组下的程序名并重新填充（否则要做的事情实在就太繁琐了）
                DeleteProgramsInGroup(groupName);
                List<string> devices = group.GetDevices();
                List<string> programs = group.GetPrograms();
                for(int i = 0; i < devices.Count; i++)
                {
                    WriteProgramToGroup(groupName, devices[i], programs[i]);
                }
            }

            xmlDoc.Save(xmlpath);
        }

        #endregion

        /// <summary>
        /// 判断特定设备是否存在
        /// </summary>
        /// <param name="deviceName">设备名</param>
        /// <returns></returns>
        public static bool isDeviceExists(string deviceName)
        {
            xmlDoc.Load(xmlpath);
            XmlNodeList list = xmlDoc.SelectSingleNode("root").SelectNodes("device");
            if (list == null)
            {
                return false;
            }
            foreach (XmlNode node in list)
            {
                if (node.SelectSingleNode("name").InnerText == deviceName)
                {

                    mTargetDeviceNode = node;
                    return true;
                }
                else
                {
                    continue;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断特定程序是否存在
        /// </summary>
        /// <param name="deviceName">设备名</param>
        /// <param name="programName">程序名</param>
        /// <returns></returns>
        public static bool isProgramExists(string deviceName, string programName)
        {
            if (!isDeviceExists(deviceName))
            {
                return false;
            }else
            {
                XmlNodeList programList = mTargetDeviceNode.SelectNodes("program");
                foreach(XmlNode node in programList)
                {
                    if(node.SelectSingleNode("name").InnerText == programName)
                    {
                        mTargetProgramNode = node;
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 判断特定分组是否存在
        /// </summary>
        /// <param name="groupName">分组名</param>
        /// <returns></returns>
        public static bool isGroupExists(string groupName)
        {
            xmlDoc.Load(xmlpath);
            XmlNodeList list = xmlDoc.SelectSingleNode("root").SelectNodes("group");
            if(list == null)
            {
                return false;
            }
            foreach (XmlNode node in list)
            {
                if (node.SelectSingleNode("name").InnerText == groupName)
                {

                    mTargetGroupNode = node;
                    return true;
                }
                else
                {
                    continue;
                }
            }
            return false;
        }
    }
}
