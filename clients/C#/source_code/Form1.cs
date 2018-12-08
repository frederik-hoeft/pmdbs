using pmdbs.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pmdbs
{
    public partial class Form1 : Form
    {
        private List<ListEntry> entryList = new List<ListEntry>();
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        public Form1()
        {
            InitializeComponent();
            advancedButton1.OnClickEvent += advancedButton1_Click;
            advancedButton2.OnClickEvent += advancedButton2_Click;
            ButtonClose.OnClickEvent += ButtonClose_Click;
            ButtonMaximize.OnClickEvent += ButtonMaximize_Click;
            ButtonMinimize.OnClickEvent += ButtonMinimize_Click;
        }

        private void ListEntry_Click(object sender, EventArgs e)
        {
            ListEntry senderObject = (ListEntry)sender;
            int index = senderObject.id;
            MessageBox.Show("INDEX: " + index.ToString(), "finally...", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void flowLayoutPanel1_Resize(object sender, EventArgs e)
        {
            foreach (ListEntry entry in DataFlowLayoutPanelList.Controls)
            {
                entry.Width = DataFlowLayoutPanelList.Width - 25;
                //entry.RePaint();
            }
            ShowScrollBar(DataFlowLayoutPanelList.Handle, (int)ScrollBarDirection.SB_HORZ, false);
        }

        private void Populate()
        {
            for (int i = 0; i < 7; i++)
            {
                ListEntry listEntry = new ListEntry
                {
                    BackColor = Color.White,
                    // FOR ICON DOWNLOAD: http://icons.duckduckgo.com/ip2/www.bbs-me.org.ico  --> Better quality than google
                    FavIcon = Image.FromFile("favicon.ico"),
                    HostName = "Google",
                    HostNameFont = new Font("Century Gothic", 14F, FontStyle.Bold, GraphicsUnit.Point, 0),
                    HostNameForeColor = SystemColors.ControlText,
                    Name = "listEntry",
                    Size = new Size(1041, 52),
                    TabIndex = 14,
                    TimeStamp = "2018-12-03 18:05",
                    TimeStampFont = new Font("Century Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    TimeStampForeColor = SystemColors.ControlText,
                    UserName = "MyUsername",
                    UserNameFont = new Font("Century Gothic", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    UserNameForeColor = SystemColors.ControlText,
                    ColorNormal = Color.White,
                    ColorHover = Colors.Orange,
                    BackgroundColor = Color.White,
                    id = i
                };
                DataFlowLayoutPanelList.Controls.Add(listEntry);
                entryList.Add(listEntry);
                listEntry.OnClickEvent += ListEntry_Click;
                DataFlowLayoutPanelList.SetFlowBreak(listEntry,true);
            }
            flowLayoutPanel1_Resize(this, null);
            DataPictureBoxDetailsLogo.Image = Image.FromFile("favicon.ico");
        }

        private void advancedButton1_Click(object sender, EventArgs e)
        {
            Populate();
        }

        private void advancedButton2_Click(object sender, EventArgs e)
        {
            
        }

        private void flowLayoutPanel1_MouseEnter(object sender, EventArgs e)
        {
            DataFlowLayoutPanelList.Focus();
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ButtonMaximize_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
            }
            flowLayoutPanel1_Resize(this, null);
        }

        private void ButtonMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void WindowHeaderPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        public enum ScrollInfoMask : uint
        {
            SIF_RANGE = 0x1,
            SIF_PAGE = 0x2,
            SIF_POS = 0x4,
            SIF_DISABLENOSCROLL = 0x8,
            SIF_TRACKPOS = 0x10,
            SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS),
        }

        public enum ScrollBarDirection
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct SCROLLINFO
        {
            public uint cbSize;
            public uint fMask;
            public int nMin;
            public int nMax;
            public uint nPage;
            public int nPos;
            public int nTrackPos;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO lpsi);

        private void WindowHeaderLabelLogo_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
