using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiRemoteController.Utilities
{
    public class TViewComboBox : ComboBox
    {
        ToolStripControlHost treeViewHost;
        ToolStripDropDown dropDown;

        public TViewComboBox()
        {
            TreeView treeView = new TreeView();
            treeView.BorderStyle = BorderStyle.None;
            treeViewHost = new ToolStripControlHost(treeView);

            dropDown = new ToolStripDropDown();
            dropDown.Items.Add(treeViewHost);
        }

        public TreeView TreeView
        {
            get { return treeViewHost.Control as TreeView; }
        }

        private void ShowDropDown()
        {
            if (dropDown != null)
            {
                treeViewHost.Width = DropDownWidth;
                treeViewHost.Height = DropDownHeight;
                dropDown.Show(this, 0, this.Height);
            }
        }

        private const int WM_USER = 0x0400,
                          WM_REFLECT = WM_USER + 0x1C00,
                          WM_COMMAND = 0x0111,
                          CBN_DROPDOWN = 7;

        public static int HIWORD(int n)
        {
            return (n >> 16) & 0xffff;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (WM_REFLECT + WM_COMMAND))
            {
                if (HIWORD((int)m.WParam) == CBN_DROPDOWN)
                {
                    ShowDropDown();
                    return;
                }
            }

            base.WndProc(ref m);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dropDown != null)
                {
                    dropDown.Dispose();
                    dropDown = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}
