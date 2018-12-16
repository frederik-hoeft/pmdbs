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
using MetroFramework;

namespace pmdbs
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        private List<ListEntry> entryList = new List<ListEntry>();
        
        public Form1()
        {
            InitializeComponent();
            advancedButton1.OnClickEvent += advancedButton1_Click;
            advancedButton2.OnClickEvent += advancedButton2_Click;
            DataAddAdvancedImageButton.OnClickEvent += DataAddAdvancedImageButton_Click;
            DataDetailsRemoveAdvancedImageButton.OnClickEvent += DataRemoveAdvancedImageButton_Click;
            DataDetailsEditAdvancedImageButton.OnClickEvent += DataEditAdvancedImageButton_Click;
            DataLeftAdvancedImageButton.OnClickEvent += DataLeftAdvancedImageButton_Click;
            DataRightAdvancedImageButton.OnClickEvent += DataRightAdvancedImageButton_Click;
            DataDetailsEntryEmail.OnClickEvent += DataDetailsEntryEmail_Click;
            DataDetailsEntryUsername.OnClickEvent += DataDetailsEntryUsername_Click;
            DataDetailsEntryPassword.OnClickEvent += DataDetailsEntryPassword_Click;
            DataDetailsEntryWebsite.OnClickEvent += DataDetailsEntryWebsite_Click;
            DataEditSaveAdvancedImageButton.OnClickEvent += DataEditSave_Click;
            DataEditCancelAdvancedImageButton.OnClickEvent += DataEditCancel_Click;
        }
        #region FUNCTIONALITY_METHODS
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

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
                DataTableLayoutPanelMain.ColumnStyles[0].Width = 10;
                DataTableLayoutPanelMain.ColumnStyles[2].Width = 10;
                DataTableLayoutPanelMain.ColumnStyles[4].Width = 10;
                DataTableLayoutPanelMain.RowStyles[0].Height = 10;
                DataTableLayoutPanelMain.RowStyles[2].Height = 10;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                DataTableLayoutPanelMain.ColumnStyles[0].Width = 20;
                DataTableLayoutPanelMain.ColumnStyles[2].Width = 20;
                DataTableLayoutPanelMain.ColumnStyles[4].Width = 20;
                DataTableLayoutPanelMain.RowStyles[0].Height = 20;
                DataTableLayoutPanelMain.RowStyles[2].Height = 20;

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

        private void flowLayoutPanel1_Resize(object sender, EventArgs e)
        {
            foreach (ListEntry entry in DataFlowLayoutPanelList.Controls)
            {
                entry.Width = DataFlowLayoutPanelList.Width - 25;
            }
            ShowScrollBar(DataFlowLayoutPanelList.Handle, (int)ScrollBarDirection.SB_HORZ, false);
        }

        private void DashboardPanel_Paint(object sender, PaintEventArgs e)
        {
            Bitmap B = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(B);
            G.Clear(Color.Transparent);
            G.DrawLine(new Pen(new SolidBrush(Color.FromArgb(17, 17, 17))), new Point(0, 0), new Point(Width, 0));
            e.Graphics.DrawImage((Image)(B.Clone()), 0, 0);
            G.Dispose();
            B.Dispose();
        }
        #endregion
        private void ListEntry_Click(object sender, EventArgs e)
        {
            DataPanelDetails.BringToFront();
            ListEntry senderObject = (ListEntry)sender;
            int index = senderObject.id;
            MessageBox.Show("INDEX: " + index.ToString(), "finally...", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    ColorHover = Colors.LightGray,
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

        private void DataAddAdvancedImageButton_Click(object sender, EventArgs e)
        {

        }

        private void DataEditSave_Click(object sender, EventArgs e)
        {
            DataPanelDetails.BringToFront();
        }

        private void DataEditCancel_Click(object sender, EventArgs e)
        {
            DataPanelDetails.BringToFront();
        }

        private void DataEditAdvancedImageButton_Click(object sender, EventArgs e)
        {
            DataPanelEdit.BringToFront();
        }

        private void DataRemoveAdvancedImageButton_Click(object sender, EventArgs e)
        {

        }

        private void DataLeftAdvancedImageButton_Click(object sender, EventArgs e)
        {

        }

        private void DataRightAdvancedImageButton_Click(object sender, EventArgs e)
        {

        }

        private void DataDetailsEntryUsername_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(DataDetailsEntryUsername.Content);
        }

        private void DataDetailsEntryPassword_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(DataDetailsEntryPassword.Content);
        }

        private void DataDetailsEntryEmail_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(DataDetailsEntryEmail.Content);
        }

        private void DataDetailsEntryWebsite_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(DataDetailsEntryWebsite.RawText);
        }
    }
}
