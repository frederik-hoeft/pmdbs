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
using System.Drawing.Drawing2D;

namespace pmdbs
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        #region DECLARATIONS
        private List<ListEntry> entryList = new List<ListEntry>();
        private char[] passwordSpecialCharacters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '_', '-', '$', '%', '&', '/', '(', ')', '=', '?', '{', '[', ']', '}', '\\', '+', '*', '#', ',', '.', '<', '>', '|', '@', '!', '~', ';', ':', '"' };
        private char[] passwordCharacters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
        #endregion

        #region FUNCTIONALITY_METHODS_AND_OTHER_UGLY_CODE
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void FlowLayoutPanel1_MouseEnter(object sender, EventArgs e)
        {
            DataFlowLayoutPanelList.Focus();
        }

        private void AddFlowLayoutPanelCenter_MouseEnter(object sender, EventArgs e)
        {
            AddFlowLayoutPanelCenter.Focus();
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

        private void FlowLayoutPanel1_Resize(object sender, EventArgs e)
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

        public Form1()
        {
            InitializeComponent();
            InitializeTransparency();
            LoginPictureBoxOnlineMain.SuspendLayout();
            LoginPictureBoxOfflineMain.SuspendLayout();
            LoginPictureBoxRegisterMain.BringToFront();
            timer1.Start();
            #region ADD_EVENTHANDLERS
            DataAddAdvancedImageButton.OnClickEvent += DataAddAdvancedImageButton_Click;
            DataDetailsRemoveAdvancedImageButton.OnClickEvent += DataRemoveAdvancedImageButton_Click;
            DataDetailsEditAdvancedImageButton.OnClickEvent += DataEditAdvancedImageButton_Click;
            DataLeftAdvancedImageButton.OnClickEvent += DataLeftAdvancedImageButton_Click;
            DataRightAdvancedImageButton.OnClickEvent += DataRightAdvancedImageButton_Click;
            DataSyncAdvancedImageButton.OnClickEvent += DataSyncAdvancedImageButton_Click;
            DataDetailsEntryEmail.OnClickEvent += DataDetailsEntryEmail_Click;
            DataDetailsEntryUsername.OnClickEvent += DataDetailsEntryUsername_Click;
            DataDetailsEntryPassword.OnClickEvent += DataDetailsEntryPassword_Click;
            DataDetailsEntryWebsite.OnClickEvent += DataDetailsEntryWebsite_Click;
            DataEditSaveAdvancedImageButton.OnClickEvent += DataEditSave_Click;
            DataEditCancelAdvancedImageButton.OnClickEvent += DataEditCancel_Click;
            DashboardMenuEntryHome.OnClickEvent += DashboardMenuEntryHome_Click;
            DashboardMenuEntrySettings.OnClickEvent += DashboardMenuEntrySettings_Click;
            DashboardMenuEntryPasswords.OnClickEvent += DashboardMenuEntryPasswords_Click;
            AddPanelAdvancedImageButtonSave.OnClickEvent += AddPanelAdvancedImageButtonSave_Click;
            AddPanelAdvancedImageButtonAbort.OnClickEvent += AddPanelAdvancedImageButtonAbort_Click;
            #endregion
        }
        #region Dashboard

        private void DashboardMenuEntryHome_Click(object sender, EventArgs e)
        {
            DashboardPanelHomeIndicator.BackColor = Colors.Orange;
            DashboardPanelSettingsIndicator.BackColor = Color.White;
            DashboardPanelPasswordsIndicator.BackColor = Color.White;
        }

        private void DashboardMenuEntrySettings_Click(object sender, EventArgs e)
        {
            DashboardPanelHomeIndicator.BackColor = Color.White;
            DashboardPanelSettingsIndicator.BackColor = Colors.Orange;
            DashboardPanelPasswordsIndicator.BackColor = Color.White;
        }

        private void DashboardMenuEntryPasswords_Click(object sender, EventArgs e)
        {
            DashboardPanelHomeIndicator.BackColor = Color.White;
            DashboardPanelSettingsIndicator.BackColor = Color.White;
            DashboardPanelPasswordsIndicator.BackColor = Colors.Orange;
            Populate();
        }
        #endregion

        #region DataPanel
        private void Populate()
        {
            DataFlowLayoutPanelList.SuspendLayout();
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
                DataFlowLayoutPanelList.SetFlowBreak(listEntry, true);
            }
            DataFlowLayoutPanelList.ResumeLayout();
            FlowLayoutPanel1_Resize(this, null);
            DataPictureBoxDetailsLogo.Image = Image.FromFile("favicon.ico");
        }

        private void ListEntry_Click(object sender, EventArgs e)
        {
            DataPanelDetails.BringToFront();
            ListEntry senderObject = (ListEntry)sender;
            int index = senderObject.id;
            MessageBox.Show("INDEX: " + index.ToString(), "finally...", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DataAddAdvancedImageButton_Click(object sender, EventArgs e)
        {
            DataPanelMain.SuspendLayout();
            AddPanelMain.BringToFront();
            AddPanelMain.ResumeLayout();
        }

        private void DataEditSave_Click(object sender, EventArgs e)
        {
            DataPanelEdit.SuspendLayout();
            DataPanelDetails.BringToFront();
            DataPanelDetails.ResumeLayout();
        }

        private void DataEditCancel_Click(object sender, EventArgs e)
        {
            DataPanelEdit.SuspendLayout();
            DataPanelDetails.BringToFront();
            DataPanelDetails.ResumeLayout();
        }

        private void DataEditAdvancedImageButton_Click(object sender, EventArgs e)
        {
            DataPanelDetails.SuspendLayout();
            DataPanelEdit.BringToFront();
            DataPanelEdit.ResumeLayout();
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

        private void DataSyncAdvancedImageButton_Click(object sender, EventArgs e)
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
        #endregion

        #region AddPanel
        private void AddPanelGeneratePasswordAnimatedButtonGenerate_Click(object sender, EventArgs e)
        {
            char[] characterSet;
            int passwordLength = Convert.ToInt32(AddPanelGeneratePasswordeAdvancedNumericUpDown.TextValue);
            bool specialCharactersEnabled = AddPanelGeneratePasswordAdvancedCheckBox.Checked;
            string randomizedPassword = string.Empty;
            if (specialCharactersEnabled)
            {
                characterSet = passwordSpecialCharacters;
            }
            else
            {
                characterSet = passwordCharacters;
            }
            using (System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                // INITIALIZE 4 BYTE BUFFER
                byte[] buffer = new byte[4];

                for (int i = 0; i < passwordLength; i++)
                {
                    // FILL BUFFER.
                    rng.GetBytes(buffer);

                    // CONVERT TO Int32 AND USE ABSOLUTE VALUE.
                    int value = Math.Abs(BitConverter.ToInt32(buffer, 0));
                    // USE MODULO TO MAP 32 BIT INTEGER TO CHARACTER RANGE
                    randomizedPassword += characterSet[value % characterSet.Length];
                }
            }
            AddEditFieldPassword.TextTextBox = randomizedPassword;
        }

        private void ResetFields()
        {
            AddPanelNotesAdvancedRichTextBox.TextValue = "";
            AddEditFieldEmail.TextTextBox = "";
            AddEditFieldHostname.TextTextBox = "";
            AddEditFieldPassword.TextTextBox = "";
            AddEditFieldUsername.TextTextBox = "";
            AddEditFieldWebsite.TextTextBox = "";
        }

        private void AddPanelAdvancedImageButtonSave_Click(object sender, EventArgs e)
        {
            AddPanelMain.SuspendLayout();
            DataPanelMain.BringToFront();
            DataPanelMain.ResumeLayout();
            ResetFields();
        }

        private void AddPanelAdvancedImageButtonAbort_Click(object sender, EventArgs e)
        {
            AddPanelMain.SuspendLayout();
            DataPanelMain.BringToFront();
            DataPanelMain.ResumeLayout();
            ResetFields();
        }

        #endregion

        #region LoginPanel
        private void InitializeTransparency()
        {
            Bitmap bmp = new Bitmap(LoginPictureBoxOnlineMain.Width, LoginPictureBoxOnlineMain.Height);
            using (Graphics graph = Graphics.FromImage(bmp))
            {
                Rectangle ImageSize = new Rectangle(0, 0, LoginPictureBoxOnlineMain.Width, LoginPictureBoxOnlineMain.Height);
                graph.FillRectangle(new SolidBrush(Color.FromArgb(220, 17, 17, 17)), ImageSize);
            }
            this.LoginPictureBoxOnlineMain.Image = bmp;
            this.LoginPictureBoxRegisterMain.Image = bmp;
            this.LoginPictureBoxOfflineMain.Image = bmp;
        }
        #endregion

        private void LoginLabelOnlineRegister_Click(object sender, EventArgs e)
        {
            LoginPictureBoxOnlineMain.SuspendLayout();
            LoginPictureBoxRegisterMain.BringToFront();
            LoginPictureBoxRegisterMain.ResumeLayout();
        }

        private void LoginLabelRegisterSignIn_Click(object sender, EventArgs e)
        {
            LoginPictureBoxRegisterMain.SuspendLayout();
            LoginPictureBoxOnlineMain.BringToFront();
            LoginPictureBoxOnlineMain.ResumeLayout();
        }

        private void LoginAnimatedButtonOnlineLogin_Click(object sender, EventArgs e)
        {

        }

        private void LoginAnimatedButtonOfflineLogin_Click(object sender, EventArgs e)
        {

        }
        
        private void LoginAnimatedButtonRegister_Click(object sender, EventArgs e)
        {

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            Random rng = new Random();
            metroProgressBar3.Step = rng.Next(0, 6);
            metroProgressBar4.Step = rng.Next(0, 6);
            metroProgressBar4.PerformStep();
            metroProgressBar3.PerformStep();
            if (metroProgressBar3.Value >= 100)
            {
                metroProgressBar3.Value = 0;
            }
            if (metroProgressBar4.Value >= 100)
            {
                metroProgressBar4.Value = 0;
            }
        }
    }
}
