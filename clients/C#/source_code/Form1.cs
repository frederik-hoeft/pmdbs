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
using System.Threading;

namespace pmdbs
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        #region DECLARATIONS
        private List<ListEntry> entryList = new List<ListEntry>();
        private char[] passwordSpecialCharacters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '_', '-', '$', '%', '&', '/', '(', ')', '=', '?', '{', '[', ']', '}', '\\', '+', '*', '#', ',', '.', '<', '>', '|', '@', '!', '~', ';', ':', '"' };
        private char[] passwordCharacters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
        private string MasterPassword;
        private bool GuiLoaded = false;
        private Size MaxSize;
        private Size MinSize;
        private string LocalAESkey;
        private string GlobalAESkey;
        private string NickName;
        private DataTable UserData;
        private string DataDetailsID;
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

        private void DataFlowLayoutPanelEdit_MouseEnter(object sender, EventArgs e)
        {
            DataFlowLayoutPanelEdit.Focus();
        }

        private void WindowButtonClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
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

        private async void Form1_Load(object sender, EventArgs e)
        {

            if (GuiLoaded)
            {
                return;
            }
            this.Hide();

            ThreadPool.QueueUserWorkItem((x) =>
            {
                using (var splashForm = new SplashForm())
                {
                    splashForm.Show();
                    while (!GuiLoaded)
                        Application.DoEvents();
                    splashForm.Close();
                    this.Invoke((MethodInvoker)delegate {
                        // this code runs on the UI thread!
                        this.Show();
                        this.Activate();
                    });

                }
            });
            Task<List<String>> Initialize = DataBaseHelper.GetDataAsList("SELECT U_wasOnline, U_name FROM Tbl_user;", 2);
            List<String> Result = await Initialize;
            try
            {
                if (Result[0].Equals("1"))
                {
                    //TODO: Check for Password Changes
                }
                NickName = Result[1];
                LoginPictureBoxOnlineMain.SuspendLayout();
                LoginPictureBoxOfflineMain.BringToFront();
                LoginPictureBoxRegisterMain.SuspendLayout();
                LoginPictureBoxLoadingMain.SuspendLayout();
                if (!NickName.Equals("User"))
                {
                    LoginLabelOfflineUsername.Text = NickName;
                }
            }
            catch
            {
                LoginPictureBoxOnlineMain.SuspendLayout();
                LoginPictureBoxOfflineMain.SuspendLayout();
                LoginPictureBoxRegisterMain.BringToFront();
                LoginPictureBoxLoadingMain.SuspendLayout();
            }
            Thread.Sleep(1500); // Emulate hardwork
            GuiLoaded = true;

        }
        #endregion
        public Form1()
        {
            InitializeComponent();
            #region LOAD
            InitializeTransparency();
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            MaxSize = this.MaximumSize;
            MinSize = this.MinimumSize;
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
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
            windowButtonClose.OnClickEvent += WindowButtonClose_Click;
            #endregion
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
            //Populate();
        }
        #endregion

        #region DataPanel

        private void RefreshUserData()
        {
            //D_id, D_hid, D_datetime, D_host, D_uname, D_password, D_url, D_email, D_notes
            DataFlowLayoutPanelList.SuspendLayout();
            for (int i = 0; i < entryList.Count; i++)
            {
                entryList[i].Dispose();
            }
            for (int i = 0; i < UserData.Rows.Count; i++)
            {
                string strTimeStamp = TimeConverter.UnixTimeStampToDateTime(Convert.ToDouble(UserData.Rows[i]["2"].ToString())).ToString("u");
                strTimeStamp = strTimeStamp.Substring(0, strTimeStamp.Length - 1);
                ListEntry listEntry = new ListEntry
                {
                    BackColor = Color.White,
                    // FOR ICON DOWNLOAD: http://icons.duckduckgo.com/ip2/www.bbs-me.org.ico  --> Better quality than google
                    FavIcon = Image.FromFile("favicon.ico"),
                    HostName = UserData.Rows[i]["3"].ToString().Equals("\x01") ? "-" : UserData.Rows[i]["3"].ToString(),
                    HostNameFont = new Font("Century Gothic", 14F, FontStyle.Bold, GraphicsUnit.Point, 0),
                    HostNameForeColor = SystemColors.ControlText,
                    Name = "listEntry",
                    Size = new Size(1041, 75),
                    TabIndex = 14,
                    TimeStamp = strTimeStamp,
                    TimeStampFont = new Font("Century Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    TimeStampForeColor = SystemColors.ControlText,
                    UserName = UserData.Rows[i]["4"].ToString(),
                    UserNameFont = new Font("Century Gothic", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    UserNameForeColor = SystemColors.ControlText,
                    ColorNormal = Color.White,
                    ColorHover = Colors.LightGray,
                    BackgroundColor = Color.White,
                    id = Convert.ToInt32(UserData.Rows[i]["0"])
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
            //D_id, D_hid, D_datetime, D_host, D_uname, D_password, D_url, D_email, D_notes
            DataPanelDetails.BringToFront();
            ListEntry SenderObject = (ListEntry)sender;
            int index = SenderObject.id;
            DataRow LinkedRow = UserData.AsEnumerable().SingleOrDefault(r => r.Field<String>("0").Equals(index.ToString()));
            DataLabelDetailsHostname.Text = LinkedRow["3"].ToString();
            DataDetailsEntryUsername.Content = LinkedRow["4"].ToString().Equals("\x01") ? "-" : LinkedRow["4"].ToString();
            DataDetailsEntryPassword.Content = LinkedRow["5"].ToString().Equals("\x01") ? "-" : LinkedRow["5"].ToString();
            DataDetailsEntryWebsite.Content = LinkedRow["6"].ToString().Equals("\x01") ? "-" : LinkedRow["6"].ToString();
            DataDetailsEntryEmail.Content = LinkedRow["7"].ToString().Equals("\x01") ? "-" : LinkedRow["7"].ToString();
            DataDetailsCustomLabelNotes.Content = LinkedRow["8"].ToString().Equals("\x01") ? "-" : LinkedRow["8"].ToString();
            DataDetailsID = index.ToString();
        }

        private void DataAddAdvancedImageButton_Click(object sender, EventArgs e)
        {
            DataTableLayoutPanelMain.SuspendLayout();
            AddPanelMain.BringToFront();
            AddPanelMain.ResumeLayout();
        }

        private async void DataEditSave_Click(object sender, EventArgs e)
        {
            //D_id, D_hid, D_datetime, D_host, D_uname, D_password, D_url, D_email, D_notes
            string Hostname = DataEditEditFieldHostname.TextTextBox;
            string Username = DataEditEditFieldUsername.TextTextBox;
            string Password = DataEditEditFieldPassword.TextTextBox;
            string Email = DataEditEditFieldEmail.TextTextBox;
            string Website = DataEditEditFieldWebsite.TextTextBox;
            string Notes = DataEditAdvancedRichTextBoxNotes.TextValue;
            string DateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            string[] Values = new string[]
            {
                Hostname,
                Username,
                Password,
                Website,
                Email,
                Notes
            };
            string[] RawValues = new string[Values.Length];
            Array.Copy(Values, RawValues, Values.Length);
            string[] Columns = new string[]
            {
                "D_host",
                "D_uname",
                "D_password",
                "D_url",
                "D_email",
                "D_notes"
            };
            if (Password.Equals("") || Hostname.Equals(""))
            {
                return;
            }
            for (int i = 0; i < Values.Length; i++)
            {
                if (Values[i].Equals(""))
                {
                    Values[i] = "\x01";
                }
                else
                {
                    Values[i] = CryptoHelper.AESEncrypt(Values[i], LocalAESkey);
                }
            }
            string Query = "UPDATE Tbl_data SET D_datetime = \"" + DateTime + "\"";
            for (int i = 0; i < Columns.Length; i++)
            {
                Query += ", " + Columns[i] + " = \"" + Values[i] + "\"";
            }
            Query += " WHERE D_id = " + DataDetailsID + ";";
            Task UpdateData = DataBaseHelper.ModifyData(Query);
            await Task.WhenAll(UpdateData);
            DataRow LinkedRow = UserData.AsEnumerable().SingleOrDefault(r => r.Field<String>("0").Equals(DataDetailsID));
            for (int i = 3; i < (int)ColumnCount.Tbl_data - 1; i++)
            {
                LinkedRow[i.ToString()] = RawValues[i - 3].Equals("") ? "\x01" : RawValues[i - 3];
            }
            LinkedRow["2"] = DateTime;
            RefreshUserData();
            DataFlowLayoutPanelEdit.SuspendLayout();
            DataPanelDetails.BringToFront();
            DataPanelDetails.ResumeLayout();
        }

        private void DataEditCancel_Click(object sender, EventArgs e)
        {
            DataFlowLayoutPanelEdit.SuspendLayout();
            DataPanelDetails.BringToFront();
            DataPanelDetails.ResumeLayout();
        }

        private void DataEditAdvancedImageButton_Click(object sender, EventArgs e)
        {
            DataRow LinkedRow = UserData.AsEnumerable().SingleOrDefault(r => r.Field<String>("0").Equals(DataDetailsID));
            DataEditEditFieldHostname.TextTextBox = LinkedRow["3"].ToString();
            DataEditEditFieldUsername.TextTextBox = LinkedRow["4"].ToString().Equals("\x01") ? "" : LinkedRow["4"].ToString();
            DataEditEditFieldPassword.TextTextBox = LinkedRow["5"].ToString().Equals("\x01") ? "" : LinkedRow["5"].ToString();
            DataEditEditFieldWebsite.TextTextBox = LinkedRow["6"].ToString().Equals("\x01") ? "" : LinkedRow["6"].ToString();
            DataEditEditFieldEmail.TextTextBox = LinkedRow["7"].ToString().Equals("\x01") ? "" : LinkedRow["7"].ToString();
            DataEditAdvancedRichTextBoxNotes.TextValue = LinkedRow["8"].ToString().Equals("\x01") ? "" : LinkedRow["8"].ToString();
            DataPanelDetails.SuspendLayout();
            DataFlowLayoutPanelEdit.BringToFront();
            DataFlowLayoutPanelEdit.ResumeLayout();
        }

        private void DataEditAnimatedButtonGeneratePassword_Click(object sender, EventArgs e)
        {
            char[] characterSet;
            int passwordLength = Convert.ToInt32(DataEditAdvancedNumericUpDown.TextValue);
            bool specialCharactersEnabled = DataEditAdvancedCheckBox.Checked;
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
            DataEditEditFieldPassword.TextTextBox = randomizedPassword;
        }

        private async void DataRemoveAdvancedImageButton_Click(object sender, EventArgs e)
        {
            Task DeleteItem = DataBaseHelper.ModifyData("DELETE FROM Tbl_data WHERE D_id = " + DataDetailsID + ";");
            await Task.WhenAll(DeleteItem);
            DataRow LinkedRow = UserData.AsEnumerable().SingleOrDefault(r => r.Field<String>("0").Equals(DataDetailsID));
            UserData.Rows.Remove(LinkedRow);
            DataPanelDetails.SuspendLayout();
            DataPanelNoSel.BringToFront();
            DataPanelNoSel.ResumeLayout();
            RefreshUserData();
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

        private async void AddPanelAdvancedImageButtonSave_Click(object sender, EventArgs e)
        {
            string Hostname = AddEditFieldHostname.TextTextBox;
            string Username = AddEditFieldUsername.TextTextBox;
            string Password = AddEditFieldPassword.TextTextBox;
            string Email = AddEditFieldEmail.TextTextBox;
            string Website = AddEditFieldWebsite.TextTextBox;
            string Notes = AddPanelNotesAdvancedRichTextBox.TextValue;
            string DateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            string[] Values = new string[]
            {
                Hostname,
                Username,
                Password,
                Email,
                Website,
                Notes
            };
            string[] Columns = new string[]
            {
                "D_host",
                "D_uname",
                "D_password",
                "D_email",
                "D_url",
                "D_notes"
            };
            if (Password.Equals("") || Hostname.Equals(""))
            {
                return;
            }
            for (int i = 0; i < Values.Length; i++)
            {
                if (Values[i].Equals(""))
                {
                    Values[i] = "\x01";
                }
                else
                {
                    Values[i] = CryptoHelper.AESEncrypt(Values[i], LocalAESkey);
                }
            }
            string Query = "INSERT INTO Tbl_data (D_datetime";
            for (int i = 0; i < Columns.Count(); i++)
            {
                Query += ", " + Columns[i];
            }
            Query += ") VALUES (\"" + DateTime + "\"";
            for (int i = 0; i < Values.Count(); i++)
            {
                Query += ", \"" + Values[i] + "\"";
            }
            Query += ");";
            Task InsertData = DataBaseHelper.ModifyData(Query);
            await Task.WhenAll(InsertData);
            if (UserData == null)
            {
                UserData = new DataTable();
                for (int i = 0; i < (int)ColumnCount.Tbl_data; i++)
                {
                    UserData.Columns.Add(i.ToString(), typeof(string));
                }
            }
            //D_id, D_hid, D_datetime, D_host, D_uname, D_password, D_url, D_email, D_notes
            Task<List<String>> GetId = DataBaseHelper.GetDataAsList("SELECT D_id FROM Tbl_data ORDER BY D_id DESC LIMIT 1;", (int)ColumnCount.SingleColumn);
            List<String> IdList = await GetId;
            DataRow NewRow = UserData.Rows.Add();
            NewRow["0"] = IdList[0];
            NewRow["1"] = "EMPTY";
            NewRow["2"] = DateTime;
            NewRow["3"] = Hostname.Equals("") ? "\x01" : Hostname;
            NewRow["4"] = Username.Equals("") ? "\x01" : Username;
            NewRow["5"] = Password.Equals("") ? "\x01" : Password;
            NewRow["6"] = Website.Equals("") ? "\x01" : Website;
            NewRow["7"] = Email.Equals("") ? "\x01" : Email;
            NewRow["8"] = Notes.Equals("") ? "\x01" : Notes;
            RefreshUserData();
            AddPanelMain.SuspendLayout();
            DataTableLayoutPanelMain.BringToFront();
            DataTableLayoutPanelMain.ResumeLayout();
            ResetFields();
        }

        private void AddPanelAdvancedImageButtonAbort_Click(object sender, EventArgs e)
        {
            AddPanelMain.SuspendLayout();
            DataTableLayoutPanelMain.BringToFront();
            DataTableLayoutPanelMain.ResumeLayout();
            ResetFields();
        }


        #endregion

        #region LoginPanel

        private bool LoginButtonDisabled = false;
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
            this.LoginPictureBoxLoadingMain.Image = bmp;
        }
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

        private async void LoginAnimatedButtonOfflineLogin_Click(object sender, EventArgs e)
        {
            if (LoginButtonDisabled)
            {
                return;
            }
            LoginButtonDisabled = true;
            LoginLabelOfflineError.ForeColor = Color.FromArgb(17, 17, 17);
            string Password = LoginEditFieldOfflinePassword.TextTextBox;
            if (Password.Equals(""))
            {
                LoginLabelOfflineError.ForeColor = Color.Firebrick;
                LoginLabelOfflineError.Text = "Please enter a password!";
                LoginButtonDisabled = false;
                return;
            }
            LoginPictureBoxLoadingMain.ResumeLayout();
            LoginPictureBoxLoadingMain.BringToFront();
            LoginPictureBoxOfflineMain.SuspendLayout();
            LoginLoadingLabelDetails.Text = "Loading Saved Hash...";
            Task<List<String>> GetSavedHash = DataBaseHelper.GetDataAsList("SELECT U_password, U_firstUsage FROM Tbl_user ORDER BY U_id ASC LIMIT 1;", 2);
            List<String> TrueHashList = await GetSavedHash;
            string TrueHash = TrueHashList[0];
            string FirstUsage = TrueHashList[1];
            LoginLoadingLabelDetails.Text = "Hashing Password...";
            string Stage1PasswordHash = CryptoHelper.SHA256Hash(Password);
            Task<String> ScryptTask = Task.Run(() => CryptoHelper.SCryptHash(Stage1PasswordHash, FirstUsage));
            string Stage2PasswordHash = await ScryptTask;
            LoginLoadingLabelDetails.Text = "Checking Password...";
            if (!Stage2PasswordHash.Equals(TrueHash))
            {
                LoginLabelOfflineError.ForeColor = Color.Firebrick;
                LoginLabelOfflineError.Text = "Wrong Password!";
                LoginButtonDisabled = false;
                LoginPictureBoxOfflineMain.ResumeLayout();
                LoginPictureBoxOfflineMain.BringToFront();
                LoginPictureBoxLoadingMain.SuspendLayout();
                return;
            }
            MasterPassword = Stage1PasswordHash;
            LocalAESkey = CryptoHelper.SHA256Hash(Stage1PasswordHash.Substring(32, 32));
            GlobalAESkey = CryptoHelper.SHA256Hash(Stage1PasswordHash.Substring(0, 32));
            LoginLoadingLabelDetails.Text = "Decrypting Your Data... 0%";
            Task<DataTable> GetData = DataBaseHelper.GetDataAsDataTable("SELECT D_id, D_hid, D_datetime, D_host, D_uname, D_password, D_url, D_email, D_notes FROM Tbl_data;", (int)ColumnCount.Tbl_data);
            UserData = await GetData;
            int Columns = UserData.Columns.Count;
            int RowCounter = 0;
            int Fields = (Columns - 3) * UserData.Rows.Count;
            foreach (DataRow Row in UserData.Rows)
            {
                for (int i = 3; i < Columns; i++)
                {
                    string FieldValue = Row[i].ToString();
                    if (!FieldValue.Equals("\x01"))
                    {
                        string decryptedData = CryptoHelper.AESDecrypt(FieldValue, LocalAESkey);
                        Row.BeginEdit();
                        Row.SetField(i, decryptedData);
                        Row.EndEdit();
                    }
                    double Percentage = ((((double)RowCounter * ((double)Columns - (double)3)) + (double)i - 3) / (double)Fields) * (double)100;
                    double FinalPercentage = Math.Round(Percentage, 0, MidpointRounding.ToEven);
                    LoginLoadingLabelDetails.Text = "Decrypting Your Data... " + FinalPercentage.ToString() + "%";
                }
                RowCounter++;
            }
            LoginPictureBoxOnlineMain.Dispose();
            LoginPictureBoxOfflineMain.Dispose();
            LoginPictureBoxRegisterMain.Dispose();
            PanelMain.BringToFront();
            PanelLogin.Dispose();
            this.MinimumSize = MinSize;
            this.MaximumSize = MaxSize;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            RefreshUserData();
        }

        private async void LoginAnimatedButtonRegister_Click(object sender, EventArgs e)
        {
            if (LoginButtonDisabled)
            {
                return;
            }
            LoginButtonDisabled = true;
            LoginLabelRegisterError.ForeColor = Color.FromArgb(17, 17, 17);
            string Password1 = LoginEditFieldRegisterPassword.TextTextBox;
            string Password2 = LoginEditFieldRegisterPassword2.TextTextBox;
            if (!Password1.Equals(Password2))
            {
                LoginLabelRegisterError.ForeColor = Color.Firebrick;
                LoginLabelRegisterError.Text = "These passwords don't match!";
                LoginButtonDisabled = false;
                return;
            }
            LoginLabelRegisterError.ForeColor = Color.Firebrick;
            List<object> PasswordStrength = PasswordChecker.chkPass(Password1);
            int score = (int)PasswordStrength[0];
            string meaning = (string)PasswordStrength[1];
            LoginLabelRegisterError.Text = meaning + " (" + score.ToString() + " points)";
            PasswordScore Strength = PasswordAdvisor.CheckStrength(Password1);
            switch (Strength)
            {
                case PasswordScore.Blank:
                    LoginLabelRegisterError.ForeColor = Color.Firebrick;
                    LoginLabelRegisterError.Text = "Password too weak.";
                    LoginButtonDisabled = false;
                    return;
                case PasswordScore.VeryWeak:
                    LoginLabelRegisterError.ForeColor = Color.Firebrick;
                    LoginLabelRegisterError.Text = "Password too weak.";
                    LoginButtonDisabled = false;
                    return;
                case PasswordScore.Weak:
                    LoginLabelRegisterError.ForeColor = Color.Firebrick;
                    LoginLabelRegisterError.Text = "Password too weak.";
                    LoginButtonDisabled = false;
                    return;
            }
            LoginPictureBoxLoadingMain.ResumeLayout();
            LoginPictureBoxLoadingMain.BringToFront();
            LoginPictureBoxRegisterMain.SuspendLayout();
            LoginLoadingLabelDetails.Text = "Hashing Password...";
            string Stage1PasswordHash = CryptoHelper.SHA256Hash(Password1);
            string FirstUsage = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            Task<String> ScryptTask = Task.Run(() => CryptoHelper.SCryptHash(Stage1PasswordHash, FirstUsage));
            string Stage2PasswordHash = await ScryptTask;
            LoginLoadingLabelDetails.Text = "Initializing Database...";
            Task SetupDatabase = DataBaseHelper.ModifyData("INSERT INTO Tbl_user (U_password, U_wasOnline, U_firstUsage) VALUES (\"" + Stage2PasswordHash + "\", 0, \"" + FirstUsage + "\");");
            await Task.WhenAll(SetupDatabase);
            MasterPassword = Stage1PasswordHash;
            LocalAESkey = CryptoHelper.SHA256Hash(Stage1PasswordHash.Substring(32, 32));
            GlobalAESkey = CryptoHelper.SHA256Hash(Stage1PasswordHash.Substring(0, 32));
            LoginPictureBoxOnlineMain.Dispose();
            LoginPictureBoxOfflineMain.Dispose();
            LoginPictureBoxRegisterMain.Dispose();
            PanelMain.BringToFront();
            PanelLogin.Dispose();
            this.MinimumSize = MinSize;
            this.MaximumSize = MaxSize;
        }


        #endregion
    }
}
