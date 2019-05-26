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
using System.Net;
using System.IO;
using System.Drawing.Imaging;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Diagnostics;
using System.Text.RegularExpressions;

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
        private string NickName;
        private string DataDetailsID;
        private IconData AddIcon = new IconData();
        private int DataPerPage = 25;
        private int CurrentPage = 0;
        private int CurrentContentCount = 0;
        int MaxPages = 1;
        public static void InvokeReload()
        {
            GlobalVarPool.Form1.RefreshUserData(0);
        }
        #endregion

        #region FUNCTIONALITY_METHODS_AND_OTHER_UGLY_CODE

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void DataFlowLayoutPanelList_MouseEnter(object sender, EventArgs e)
        {
            DataFlowLayoutPanelList.Focus();
        }
        internal static class NativeWinAPI
        {
            internal static readonly int GWL_EXSTYLE = -20;
            internal static readonly int WS_EX_COMPOSITED = 0x02000000;

            [DllImport("user32")]
            internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

            [DllImport("user32")]
            internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
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
        
        #endregion
        public Form1()
        {
            InitializeComponent();
            #region LOAD
            foreach (Control c in this.Controls)
            {
                int style = NativeWinAPI.GetWindowLong(c.Handle, NativeWinAPI.GWL_EXSTYLE);
                style |= NativeWinAPI.WS_EX_COMPOSITED;
                NativeWinAPI.SetWindowLong(c.Handle, NativeWinAPI.GWL_EXSTYLE, style);
            }
            GlobalVarPool.Form1 = this;
            InitializeTransparency();
            #region INIT
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            MaxSize = this.MaximumSize;
            MinSize = this.MinimumSize;
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
            GlobalVarPool.loadingSpinner = SettingsAdvancedProgressSpinnerLoading;
            GlobalVarPool.loadingLabel = SettingsLabelLoadingStatus;
            GlobalVarPool.loadingLogo = SettingsPictureBoxLoadingLogo;
            GlobalVarPool.loadingPanel = SettingsPanelLoadingMain;
            GlobalVarPool.settingsPanel = SettingsTableLayoutPanelMain;
            GlobalVarPool.settingsAbort = SettingsAdvancedImageButtonFooterAbort;
            GlobalVarPool.settingsSave = SettingsAdvancedImageButtonFooterSave;
            GlobalVarPool.promptAction = SettingsLabelPromptAction;
            GlobalVarPool.promptEMail = SettingsLabelPromptMailInfo;
            GlobalVarPool.promptMain = SettingsLabelPromptMain;
            GlobalVarPool.promptPanel = SettingsPanelPromptMain;
            GlobalVarPool.SyncButton = DataSyncAdvancedImageButton;
            #endregion
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
            MenuMenuEntryHome.OnClickEvent += MenuMenuEntryHome_Click;
            MenuMenuEntrySettings.OnClickEvent += MenuMenuEntrySettings_Click;
            MenuMenuEntryPasswords.OnClickEvent += MenuMenuEntryPasswords_Click;
            AddPanelAdvancedImageButtonSave.OnClickEvent += AddPanelAdvancedImageButtonSave_Click;
            AddPanelAdvancedImageButtonAbort.OnClickEvent += AddPanelAdvancedImageButtonAbort_Click;
            windowButtonClose.OnClickEvent += WindowButtonClose_Click;
            SettingsAdvancedImageButtonFooterAbort.OnClickEvent += SettingsAdvancedImageButtonFooterAbort_Click;
            SettingsAdvancedImageButtonFooterSave.OnClickEvent += SettingsAdvancedImageButtonFooterSave_Click;
            #endregion
            #endregion
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
            Task<List<string>> Initialize = DataBaseHelper.GetDataAsList("SELECT U_wasOnline, U_name FROM Tbl_user;", 2);
            List<string> Result = await Initialize;
            try
            {
                if (Result[0].Equals("1"))
                {
                    GlobalVarPool.wasOnline = true;
                    try
                    {
                        // GET SERVER SETTINGS
                        Task<List<string>> getSettings = DataBaseHelper.GetDataAsList("SELECT S_server_ip, S_server_port FROM Tbl_settings LIMIT 1;",2);
                        List<string> settingsList = await getSettings;
                        GlobalVarPool.REMOTE_ADDRESS = settingsList[0];
                        GlobalVarPool.REMOTE_PORT = Convert.ToInt32(settingsList[1]);
                        Task<List<string>> getUsername = DataBaseHelper.GetDataAsList("SELECT U_username FROM Tbl_user LIMIT 1;",(int)ColumnCount.SingleColumn);
                        List<string> usernameList = await getUsername;
                        GlobalVarPool.username = usernameList[0];
                    }
                    catch (Exception ex)
                    {
                        CustomException.ThrowNew.GenericException(ex.ToString());
                    }
                    //TODO: Check for Password Changes
                }
                NickName = Result[1];
                LoginPictureBoxOnlineMain.SuspendLayout();
                LoginPictureBoxOfflineMain.BringToFront();
                LoginPictureBoxRegisterMain.SuspendLayout();
                LoginPictureBoxLoadingMain.SuspendLayout();
                if (!NickName.Equals(GlobalVarPool.name))
                {
                    GlobalVarPool.name = NickName;
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
            Thread.Sleep(1500); // SHOW SPLASHSCREEN
            GuiLoaded = true;
        }

        #region Menu

        private void MenuMenuEntryHome_Click(object sender, EventArgs e)
        {
            MenuPanelHomeIndicator.BackColor = Colors.Orange;
            MenuPanelSettingsIndicator.BackColor = Color.White;
            MenuPanelPasswordsIndicator.BackColor = Color.White;
            WindowHeaderLabelTitle.Text = "Dashboard";
        }

        private void MenuMenuEntrySettings_Click(object sender, EventArgs e)
        {
            MenuPanelHomeIndicator.BackColor = Color.White;
            MenuPanelSettingsIndicator.BackColor = Colors.Orange;
            MenuPanelPasswordsIndicator.BackColor = Color.White;
            SettingsTableLayoutPanelMain.BringToFront();
            if (GlobalVarPool.wasOnline)
            {
                SettingsFlowLayoutPanelOnline.BringToFront();
            }
            else
            {
                SettingsFlowLayoutPanelOffline.BringToFront();
            }
            WindowHeaderLabelTitle.Text = "Settings";
        }

        private void MenuMenuEntryPasswords_Click(object sender, EventArgs e)
        {
            MenuPanelHomeIndicator.BackColor = Color.White;
            MenuPanelSettingsIndicator.BackColor = Color.White;
            MenuPanelPasswordsIndicator.BackColor = Colors.Orange;
            DataTableLayoutPanelMain.BringToFront();
            WindowHeaderLabelTitle.Text = "Your Accounts";
        }

        private void MenuSyncPictureBox_Click(object sender, EventArgs e)
        {
            // MenuSyncPictureBox.Image = RotateImage(MenuSyncPictureBox.Image, 72);
            timer1.Interval = 100;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            MenuSyncPictureBox.Image = RotateImage(MenuSyncPictureBox.Image, 72);
        }

        public Image RotateImage(Image img)
        {
            var bmp = new Bitmap(img);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.Clear(Color.White);
                gfx.DrawImage(img, 0, 0, img.Width, img.Height);
            }

            bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
            return bmp;
        }
        #endregion

        #region DataPanel
        private void AddSingleEntry(DataRow newRow)
        {
            if (CurrentContentCount > 24)
            {
                return;
            }
            int ID = Convert.ToInt32(newRow["0"]);
            string strTimeStamp = TimeConverter.UnixTimeStampToDateTime(Convert.ToDouble(newRow["2"].ToString())).ToString("u");
            strTimeStamp = strTimeStamp.Substring(0, strTimeStamp.Length - 1);
            ListEntry listEntry = new ListEntry
            {
                BackColor = Color.White,
                HostName = newRow["3"].ToString().Equals("\x01") ? "-" : newRow["3"].ToString(),
                HostNameFont = new Font("Century Gothic", 14F, FontStyle.Bold, GraphicsUnit.Point, 0),
                HostNameForeColor = SystemColors.ControlText,
                Name = "listEntry",
                Size = new Size(1041, 75),
                TabIndex = 14,
                TimeStamp = strTimeStamp,
                TimeStampFont = new Font("Century Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                TimeStampForeColor = SystemColors.ControlText,
                UserName = newRow["4"].ToString(),
                UserNameFont = new Font("Century Gothic", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                UserNameForeColor = SystemColors.ControlText,
                ColorNormal = Color.White,
                ColorHover = Colors.LightGray,
                BackgroundColor = Color.White,
                id = ID
            };
            byte[] iconBytes = Convert.FromBase64String(newRow["9"].ToString());
            using (MemoryStream ms = new MemoryStream(iconBytes, 0, iconBytes.Length))
            {
                Image icon = Image.FromStream(ms, true);
                listEntry.FavIcon = icon.GetThumbnailImage(350, 350, null, new IntPtr());
                icon.Dispose();
            }
            DataFlowLayoutPanelList.Controls.Add(listEntry);
            entryList.Add(listEntry);
            listEntry.OnClickEvent += ListEntry_Click;
            DataFlowLayoutPanelList.SetFlowBreak(listEntry, true);
            FlowLayoutPanel1_Resize(this, null);
        }

        private void ReloadSingleEntry(DataRow invalidatedRow)
        {
            int ID = Convert.ToInt32(invalidatedRow["0"]);
            ListEntry invalidatedEntry = entryList.Where(element => element.id == ID).First();
            RefreshUserData(CurrentPage);
        }

        private void RefreshUserData(int page)
        {
            if (page < 0)
            {
                page = 0;
            }
            MaxPages = Convert.ToInt32(Math.Floor((double)GlobalVarPool.UserData.Rows.Count / DataPerPage));
            if (GlobalVarPool.UserData.Rows.Count % DataPerPage != 0)
            {
                MaxPages++;
            }
            if (page < MaxPages)
            {
                CurrentPage = page;
            }
            else
            {
                if (CurrentPage == MaxPages - 1)
                {
                    return;
                }
                else
                {
                    CurrentPage = MaxPages - 1 < 0 ? 0 : MaxPages - 1;
                }
            }
            CurrentContentCount = 0;
            //D_id, D_hid, D_datetime, D_host, D_uname, D_password, D_url, D_email, D_notes
            DataFlowLayoutPanelList.SuspendLayout();
            for (int i = 0; i < entryList.Count; i++)
            {
                entryList[i].Hide();
            }
            for (int i = CurrentPage * DataPerPage; ((CurrentPage * DataPerPage) + DataPerPage >= GlobalVarPool.UserData.Rows.Count) ? i < GlobalVarPool.UserData.Rows.Count : i < (CurrentPage * DataPerPage) + DataPerPage; i++)
            {
                int ID = Convert.ToInt32(GlobalVarPool.UserData.Rows[i]["0"]);
                string strTimeStamp = TimeConverter.UnixTimeStampToDateTime(Convert.ToDouble(GlobalVarPool.UserData.Rows[i]["2"].ToString())).ToString("u");
                strTimeStamp = strTimeStamp.Substring(0, strTimeStamp.Length - 1);
                ListEntry entry = entryList[i % DataPerPage];
                byte[] iconBytes = Convert.FromBase64String(GlobalVarPool.UserData.Rows[i]["9"].ToString());
                using (MemoryStream ms = new MemoryStream(iconBytes, 0, iconBytes.Length))
                {
                    Image icon = Image.FromStream(ms, true);
                    entry.FavIcon = icon.GetThumbnailImage(350, 350, null, new IntPtr());
                    icon.Dispose();
                }
                entry.HostName = GlobalVarPool.UserData.Rows[i]["3"].ToString().Equals("\x01") ? "-" : GlobalVarPool.UserData.Rows[i]["3"].ToString();
                entry.TimeStamp = strTimeStamp;
                entry.UserName = GlobalVarPool.UserData.Rows[i]["4"].ToString();
                entry.id = ID;
                entry.Show();
                CurrentContentCount++;
            }
            DataFlowLayoutPanelList.ResumeLayout();
            FlowLayoutPanel1_Resize(this, null);
        }

        private void ListEntry_Click(object sender, EventArgs e)
        {
            //D_id, D_hid, D_datetime, D_host, D_uname, D_password, D_url, D_email, D_notes
            DataPanelDetails.BringToFront();
            ListEntry SenderObject = (ListEntry)sender;
            int index = SenderObject.id;
            DataRow LinkedRow = GlobalVarPool.UserData.AsEnumerable().SingleOrDefault(r => r.Field<String>("0").Equals(index.ToString()));
            UpdateDetailsWindow(LinkedRow);
        }

        private void UpdateDetailsWindow(DataRow LinkedRow)
        {
            byte[] iconBytes = Convert.FromBase64String(LinkedRow["9"].ToString());
            using (MemoryStream ms = new MemoryStream(iconBytes, 0, iconBytes.Length))
            {
                Image icon = Image.FromStream(ms);
                DataPictureBoxDetailsLogo.Image = icon.GetThumbnailImage(350, 350, null, new IntPtr());
                icon.Dispose();
            }
            DataLabelDetailsHostname.Text = LinkedRow["3"].ToString();
            DataDetailsEntryUsername.Content = LinkedRow["4"].ToString().Equals("\x01") ? "-" : LinkedRow["4"].ToString();
            DataDetailsEntryPassword.Content = LinkedRow["5"].ToString().Equals("\x01") ? "-" : LinkedRow["5"].ToString();
            DataDetailsEntryWebsite.Content = LinkedRow["6"].ToString().Equals("\x01") ? "-" : LinkedRow["6"].ToString();
            DataDetailsEntryWebsite.RawText = LinkedRow["6"].ToString().Equals("\x01") ? "-" : LinkedRow["6"].ToString();
            DataDetailsEntryEmail.Content = LinkedRow["7"].ToString().Equals("\x01") ? "-" : LinkedRow["7"].ToString();
            DataDetailsCustomLabelNotes.Content = LinkedRow["8"].ToString().Equals("\x01") ? "-" : LinkedRow["8"].ToString();
            DataDetailsID = LinkedRow["0"].ToString();
        }

        private void DataAddAdvancedImageButton_Click(object sender, EventArgs e)
        {
            DataTableLayoutPanelMain.SuspendLayout();
            AddPanelMain.BringToFront();
            AddPanelMain.ResumeLayout();
        }

        private async void DataEditSave_Click(object sender, EventArgs e)
        {
            DataEditSaveAdvancedImageButton.Enabled = false;
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
                    Values[i] = CryptoHelper.AESEncrypt(Values[i], GlobalVarPool.localAESkey);
                }
            }
            string Query = "UPDATE Tbl_data SET D_datetime = \"" + DateTime + "\"";
            for (int i = 0; i < Columns.Length; i++)
            {
                Query += ", " + Columns[i] + " = \"" + Values[i] + "\"";
            }
            Query += " WHERE D_id = " + DataDetailsID + ";";
            await DataBaseHelper.ModifyData(Query);
            DataRow LinkedRow = GlobalVarPool.UserData.AsEnumerable().SingleOrDefault(r => r.Field<string>("0").Equals(DataDetailsID));
            string oldUrl = LinkedRow["6"].ToString();
            if (!Website.Equals(oldUrl))
            {
                new Thread(async delegate () {
                    string favIcon = "";
                    try
                    {
                        if (string.IsNullOrWhiteSpace(Website))
                        {
                            // EXECUTE CODE IN CATCH
                            throw new Exception();
                        }
                        else
                        {
                            favIcon = WebHelper.GetFavIcons(Website);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message.ToUpper() + "\n" + ex.ToString());
                        favIcon = HelperMethods.GenerateIcon(Hostname);
                    }
                    LinkedRow["9"] = favIcon;
                    string encryptedFavIcon = CryptoHelper.AESEncrypt(favIcon, GlobalVarPool.localAESkey);
                    Query = "UPDATE Tbl_data SET D_icon = \"" + encryptedFavIcon + "\" WHERE D_id = " + DataDetailsID + ";";
                    await DataBaseHelper.ModifyData(Query);
                    Invoke((MethodInvoker)delegate
                    {
                        UpdateDetailsWindow(LinkedRow);
                        ReloadSingleEntry(LinkedRow);
                        DataEditSaveAdvancedImageButton.Enabled = true;
                        DataFlowLayoutPanelEdit.SuspendLayout();
                        DataPanelDetails.BringToFront();
                        DataPanelDetails.ResumeLayout();
                    });
                }).Start();
            }
            for (int i = 3; i < (int)ColumnCount.Tbl_data - 1; i++)
            {
                LinkedRow[i.ToString()] = RawValues[i - 3].Equals("") ? "\x01" : RawValues[i - 3];
            }
            UpdateDetailsWindow(LinkedRow);
            LinkedRow["2"] = DateTime;
            ReloadSingleEntry(LinkedRow);
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
            DataRow LinkedRow = GlobalVarPool.UserData.AsEnumerable().SingleOrDefault(r => r.Field<String>("0").Equals(DataDetailsID));
            DataEditEditFieldHostname.TextTextBox = LinkedRow["3"].ToString();
            DataEditEditFieldUsername.TextTextBox = LinkedRow["4"].ToString().Equals("\x01") ? "" : LinkedRow["4"].ToString();
            DataEditEditFieldPassword.TextTextBox = LinkedRow["5"].ToString().Equals("\x01") ? "" : LinkedRow["5"].ToString();
            DataEditEditFieldWebsite.TextTextBox = LinkedRow["6"].ToString().Equals("\x01") ? "" : LinkedRow["6"].ToString();
            
            DataEditEditFieldEmail.TextTextBox = LinkedRow["7"].ToString().Equals("\x01") ? "" : LinkedRow["7"].ToString();
            DataEditAdvancedRichTextBoxNotes.TextValue = LinkedRow["8"].ToString().Equals("\x01") ? "" : LinkedRow["8"].ToString();
            byte[] iconBytes = Convert.FromBase64String(LinkedRow["9"].ToString());
            using (MemoryStream ms = new MemoryStream(iconBytes, 0, iconBytes.Length))
            {
                Image icon = Image.FromStream(ms);
                DataEditPictureBoxLogo.Image = icon.GetThumbnailImage(350, 350, null, new IntPtr());
                icon.Dispose();
            }
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
            DataRow LinkedRow = GlobalVarPool.UserData.AsEnumerable().SingleOrDefault(r => r.Field<string>("0").Equals(DataDetailsID));
            string hid = LinkedRow["1"].ToString();
            if (!hid.Equals("EMPTY"))
            {
                await DataBaseHelper.ModifyData("INSERT INTO Tbl_delete (DEL_hid) VALUES (\"" + hid + "\");");
            }
            await DataBaseHelper.ModifyData("DELETE FROM Tbl_data WHERE D_id = " + DataDetailsID + ";");
            GlobalVarPool.UserData.Rows.Remove(LinkedRow);
            RefreshUserData(CurrentPage);
            DataPanelDetails.SuspendLayout();
            DataPanelNoSel.BringToFront();
            DataPanelNoSel.ResumeLayout();
        }

        private void DataLeftAdvancedImageButton_Click(object sender, EventArgs e)
        {
            if (CurrentPage != 0)
            {
                RefreshUserData(CurrentPage - 1);
            }
        }

        private void DataRightAdvancedImageButton_Click(object sender, EventArgs e)
        {
            RefreshUserData(CurrentPage + 1);
        }

        private void DataSyncAdvancedImageButton_Click(object sender, EventArgs e)
        {
            if (!GlobalVarPool.wasOnline)
            {
                DataTableLayoutPanelMain.SuspendLayout();
                SettingsTableLayoutPanelMain.BringToFront();
                SettingsFlowLayoutPanelRegister.BringToFront();
                return;
            }
            AutomatedTaskFramework.Tasks.Clear();
            if (!GlobalVarPool.connected)
            {
                AutomatedTaskFramework.Task.Create(SearchCondition.In, "COOKIE_DOES_EXIST|DTACKI", NetworkAdapter.MethodProvider.Connect);
            }
            if (!GlobalVarPool.isUser)
            {
                AutomatedTaskFramework.Task.Create(SearchCondition.In, "ALREADY_LOGGED_IN|LOGIN_SUCCESSFUL", NetworkAdapter.MethodProvider.Login);
            }
            AutomatedTaskFramework.Task.Create(SearchCondition.Contains, "FETCH_SYNC", NetworkAdapter.MethodProvider.Sync);
            AutomatedTaskFramework.Tasks.Execute();
            DataSyncAdvancedImageButton.Enabled = false;
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
            if (!DataDetailsEntryWebsite.RawText.Equals("-"))
            {
                System.Diagnostics.Process.Start(DataDetailsEntryWebsite.RawText);
            }
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
            AddPanelAdvancedImageButtonSave.Enabled = false;
            string Hostname = AddEditFieldHostname.TextTextBox;
            string Username = AddEditFieldUsername.TextTextBox;
            string Password = AddEditFieldPassword.TextTextBox;
            string Email = AddEditFieldEmail.TextTextBox;
            string Website = AddEditFieldWebsite.TextTextBox;
            string Notes = AddPanelNotesAdvancedRichTextBox.TextValue;
            string DateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            
            if (Password.Equals("") || Hostname.Equals(""))
            {
                return;
            }
            new Thread(async delegate() {
                string favIcon = "";
                try
                {
                    if (string.IsNullOrWhiteSpace(Website))
                    {
                        // EXECUTE CODE IN CATCH
                        throw new Exception();
                    }
                    else
                    {
                        favIcon = WebHelper.GetFavIcons(Website);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToUpper() + "\n" + ex.ToString());
                    HelperMethods.GenerateIcon(Hostname);
                }
                string[] Values = new string[]
                {
                    Hostname,
                    Username,
                    Password,
                    Email,
                    Website,
                    Notes,
                    favIcon
                };
                string[] Columns = new string[]
                {
                    "D_host",
                    "D_uname",
                    "D_password",
                    "D_email",
                    "D_url",
                    "D_notes",
                    "D_icon"
                };
                for (int i = 0; i < Values.Length; i++)
                {
                    if (Values[i].Equals(""))
                    {
                        Values[i] = "\x01";
                    }
                    else
                    {
                        Values[i] = CryptoHelper.AESEncrypt(Values[i], GlobalVarPool.localAESkey);
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
                await DataBaseHelper.ModifyData(Query);
                if (GlobalVarPool.UserData == null)
                {
                    GlobalVarPool.UserData = new DataTable();
                    for (int i = 0; i < (int)ColumnCount.Tbl_data; i++)
                    {
                        GlobalVarPool.UserData.Columns.Add(i.ToString(), typeof(string));
                    }
                }
                //D_id, D_hid, D_datetime, D_host, D_uname, D_password, D_url, D_email, D_notes
                Task<List<string>> GetId = DataBaseHelper.GetDataAsList("SELECT D_id FROM Tbl_data ORDER BY D_id DESC LIMIT 1;", (int)ColumnCount.SingleColumn);
                List<string> IdList = await GetId;
                Invoke((MethodInvoker)delegate
                {
                    DataRow NewRow = GlobalVarPool.UserData.Rows.Add();
                    NewRow["0"] = IdList[0];
                    NewRow["1"] = "EMPTY";
                    NewRow["2"] = DateTime;
                    NewRow["3"] = Hostname.Equals("") ? "\x01" : Hostname;
                    NewRow["4"] = Username.Equals("") ? "\x01" : Username;
                    NewRow["5"] = Password.Equals("") ? "\x01" : Password;
                    NewRow["6"] = Website.Equals("") ? "\x01" : Website;
                    NewRow["7"] = Email.Equals("") ? "\x01" : Email;
                    NewRow["8"] = Notes.Equals("") ? "\x01" : Notes;
                    NewRow["9"] = favIcon;
                    RefreshUserData(CurrentPage);
                    AddPanelAdvancedImageButtonSave.Enabled = true;
                    AddPanelMain.SuspendLayout();
                    DataTableLayoutPanelMain.BringToFront();
                    DataTableLayoutPanelMain.ResumeLayout();
                    ResetFields();
                });
            }).Start();
        }

        private void AddPanelAdvancedImageButtonAbort_Click(object sender, EventArgs e)
        {
            AddPanelMain.SuspendLayout();
            DataTableLayoutPanelMain.BringToFront();
            DataTableLayoutPanelMain.ResumeLayout();
            ResetFields();
        }

        private void animatedButton1_Click(object sender, EventArgs e)
        {
            animatedButton1.Enabled = false;
            new Thread(delegate ()
            {
                string base64Img;
                try
                {
                    base64Img = WebHelper.GetFavIcons(AddEditFieldWebsite.TextTextBox);
                }
                catch
                {
                    base64Img = HelperMethods.GenerateIcon(AddEditFieldHostname.TextTextBox);
                }
                byte[] iconBytes = Convert.FromBase64String(base64Img);
                Invoke((MethodInvoker)delegate
                {
                    using (MemoryStream ms = new MemoryStream(iconBytes, 0, iconBytes.Length))
                    {
                        Image icon = Image.FromStream(ms, true);
                        pictureBox1.Image = icon.GetThumbnailImage(350, 350, null, new IntPtr());
                        icon.Dispose();
                    }
                    animatedButton1.Enabled = true;
                });
            }).Start();
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
            CustomException.ThrowNew.GenericException();
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
            Task<string> ScryptTask = Task.Run(() => CryptoHelper.SCryptHash(Stage1PasswordHash, FirstUsage));
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
            GlobalVarPool.localAESkey = CryptoHelper.SHA256Hash(Stage1PasswordHash.Substring(32, 32));
            GlobalVarPool.onlinePassword = CryptoHelper.SHA256Hash(Stage1PasswordHash.Substring(0, 32));
            LoginLoadingLabelDetails.Text = "Decrypting Your Data... 0%";
            Task<DataTable> GetData = DataBaseHelper.GetDataAsDataTable("SELECT D_id, D_hid, D_datetime, D_host, D_uname, D_password, D_url, D_email, D_notes, D_icon FROM Tbl_data;", (int)ColumnCount.Tbl_data);
            GlobalVarPool.UserData = await GetData;
            int Columns = GlobalVarPool.UserData.Columns.Count;
            int RowCounter = 0;
            int Fields = (Columns - 3) * GlobalVarPool.UserData.Rows.Count;
            foreach (DataRow Row in GlobalVarPool.UserData.Rows)
            {
                for (int i = 3; i < Columns; i++)
                {
                    string FieldValue = Row[i].ToString();
                    if (!FieldValue.Equals("\x01"))
                    {
                        string decryptedData = CryptoHelper.AESDecrypt(FieldValue, GlobalVarPool.localAESkey);
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
            LoginLoadingLabelDetails.Text = "Loading User Interface...";
            DataFlowLayoutPanelList.SuspendLayout();
            new Thread(delegate () {
                for (int i = 0; i < DataPerPage; i++)
                {
                    ListEntry listEntry = new ListEntry
                    {
                        BackColor = Color.White,
                        HostNameFont = new Font("Century Gothic", 14F, FontStyle.Bold, GraphicsUnit.Point, 0),
                        HostNameForeColor = SystemColors.ControlText,
                        Name = "listEntry",
                        Size = new Size(1041, 75),
                        TabIndex = 14,
                        TimeStampFont = new Font("Century Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                        TimeStampForeColor = SystemColors.ControlText,
                        UserNameFont = new Font("Century Gothic", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
                        UserNameForeColor = SystemColors.ControlText,
                        ColorNormal = Color.White,
                        ColorHover = Colors.LightGray,
                        BackgroundColor = Color.White
                    };
                    listEntry.Hide();
                    Invoke((MethodInvoker)delegate
                    {
                        DataFlowLayoutPanelList.Controls.Add(listEntry);
                    });
                    entryList.Add(listEntry);
                    listEntry.OnClickEvent += ListEntry_Click;
                    DataFlowLayoutPanelList.SetFlowBreak(listEntry, true);
                }
                Invoke((MethodInvoker)delegate
                {
                    DataFlowLayoutPanelList.ResumeLayout();
                    FlowLayoutPanel1_Resize(this, null);
                    LoginPictureBoxOnlineMain.Dispose();
                    LoginPictureBoxOfflineMain.Dispose();
                    LoginPictureBoxRegisterMain.Dispose();
                    PanelMain.BringToFront();
                    PanelLogin.Dispose();
                    this.MinimumSize = MinSize;
                    this.MaximumSize = MaxSize;
                    this.MaximizeBox = true;
                    this.MinimizeBox = true;
                    RefreshUserData(0);
                });
            }).Start();
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
            await DataBaseHelper.ModifyData("INSERT INTO Tbl_user (U_password, U_wasOnline, U_firstUsage) VALUES (\"" + Stage2PasswordHash + "\", 0, \"" + FirstUsage + "\");");
            MasterPassword = Stage1PasswordHash;
            GlobalVarPool.localAESkey = CryptoHelper.SHA256Hash(Stage1PasswordHash.Substring(32, 32));
            GlobalVarPool.onlinePassword = CryptoHelper.SHA256Hash(Stage1PasswordHash.Substring(0, 32));
            LoginPictureBoxOnlineMain.Dispose();
            LoginPictureBoxOfflineMain.Dispose();
            LoginPictureBoxRegisterMain.Dispose();
            PanelMain.BringToFront();
            PanelLogin.Dispose();
            this.MinimumSize = MinSize;
            this.MaximumSize = MaxSize;
        }

        #endregion

        #region SettingsPanel
        #region SettingsFooter
        private void SettingsAdvancedImageButtonFooterSave_Click(object sender, EventArgs e)
        {

        }

        private void SettingsAdvancedImageButtonFooterAbort_Click(object sender, EventArgs e)
        {

        }
        #endregion
        #region SettingsPrompt
        private void SettingsAnimatedButtonPromptSubmit_Click(object sender, EventArgs e)
        {
            string code = SettingsEditFieldPromptCode.TextTextBox;
            if (code.Equals(string.Empty))
            {
                SettingsLabelPromptCode.ForeColor = Color.Firebrick;
                SettingsLabelPromptCode.Text = "*Enter code (this field is required)";
                return;
            }
            if (!Regex.IsMatch(code, "^[0-9]{6}$"))
            {
                SettingsLabelPromptCode.ForeColor = Color.Firebrick;
                SettingsLabelPromptCode.Text = "*Enter code (6 digits)";
                return;
            }
            if (!GlobalVarPool.connected)
            {
                CustomException.ThrowNew.NetworkException("Not connected!");
                return;
            }
            if (string.IsNullOrEmpty(GlobalVarPool.promptCommand))
            {
                CustomException.ThrowNew.GenericException("User entered code but command has not been set!");
                return;
            }
            List<AutomatedTaskFramework.Task> scheduledTasks = AutomatedTaskFramework.Tasks.GetAll();
            AutomatedTaskFramework.Tasks.Clear();
            switch (GlobalVarPool.promptCommand)
            {
                case "ACTIVATE_ACCOUNT":
                    {
                        AutomatedTaskFramework.Task.Create(SearchCondition.Contains, "ACCOUNT_VERIFIED", () => NetworkAdapter.MethodProvider.ActivateAccount(code));
                        AutomatedTaskFramework.Task.Create(SearchCondition.In, "ALREADY_LOGGED_IN|LOGIN_SUCCESSFUL", NetworkAdapter.MethodProvider.Login);
                        AutomatedTaskFramework.Task.Create(SearchCondition.Contains, "LOGGED_OUT", NetworkAdapter.MethodProvider.Logout);
                        AutomatedTaskFramework.Task.Create(SearchCondition.Match, null, NetworkAdapter.MethodProvider.Disconnect);
                        break;
                    }
                case "CONFIRM_NEW_DEVICE":
                    {
                        AutomatedTaskFramework.Task.Create(SearchCondition.Contains, "LOGIN_SUCCESSFUL", () => NetworkAdapter.MethodProvider.ConfirmNewDevice(code));
                        for (int i = 1; i < scheduledTasks.Count; i++)
                        {
                            AutomatedTaskFramework.Tasks.Add(scheduledTasks[i]);
                        }
                        break;
                    }
            }
            AutomatedTaskFramework.Tasks.Execute();
            SettingsPanelPromptMain.SendToBack();
        }
        #endregion
        #region SettingsLogin
        private async void SettingsAnimatedButtonLoginSubmit_Click(object sender, EventArgs e)
        {
            string ip = SettingsEditFieldLoginIP.TextTextBox;
            string strPort = SettingsEditFieldLoginPort.TextTextBox;
            string username = SettingsEditFieldLoginUsername.TextTextBox;
            bool isIP = false;

            if (string.IsNullOrEmpty(ip))
            {
                CustomException.ThrowNew.FormatException("Please enter the IPv4 address or DNS of the server you'd like to connect to.");
                return;
            }
            if (string.IsNullOrEmpty(strPort))
            {
                CustomException.ThrowNew.FormatException("Please enter the port of the server you'd like to connect to.");
                return;
            }
            if (string.IsNullOrEmpty(username))
            {
                CustomException.ThrowNew.FormatException("Please enter your username.");
                return;
            }
            int port = Convert.ToInt32(strPort);
            if (port < 1 || port > 65536)
            {
                CustomException.ThrowNew.FormatException("Please enter a valid port number.");
                return;
            }
            if (Regex.IsMatch(ip, @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5]).){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$"))
            {
                isIP = true;
                GlobalVarPool.REMOTE_ADDRESS = ip;
            }
            GlobalVarPool.username = username;
            try
            {
                if (!isIP)
                {
                    Task<IPHostEntry> ipTask = Dns.GetHostEntryAsync(ip);
                    IPHostEntry ipAddress = await ipTask;
                    string ipv4String = ipAddress.AddressList.First().MapToIPv4().ToString();
                    GlobalVarPool.REMOTE_ADDRESS = ipv4String;
                }
                GlobalVarPool.REMOTE_PORT = port;
                GlobalVarPool.previousPanel = SettingsFlowLayoutPanelRegister;
                Thread t = new Thread(new ParameterizedThreadStart(HelperMethods.LoadingHelper))
                {
                    IsBackground = true
                };
                t.Start(new List<object> { SettingsFlowLayoutPanelOnline, SettingsLabelLoadingStatus, true, "GlobalVarPool.isUser" });
                if (GlobalVarPool.connected)
                {
                    AutomatedTaskFramework.Task.Create(SearchCondition.In, "ALREADY_LOGGED_IN|LOGIN_SUCCESSFUL", NetworkAdapter.MethodProvider.Login);
                }
                else
                {
                    AutomatedTaskFramework.Task.Create(SearchCondition.In, "COOKIE_DOES_EXIST|DTACKI", NetworkAdapter.MethodProvider.Connect);
                    AutomatedTaskFramework.Task.Create(SearchCondition.In, "ALREADY_LOGGED_IN|LOGIN_SUCCESSFUL", NetworkAdapter.MethodProvider.Login);
                }
                AutomatedTaskFramework.Task.Create(SearchCondition.In, "LOGGED_OUT|NOT_LOGGED_IN", NetworkAdapter.MethodProvider.Logout);
                AutomatedTaskFramework.Task.Create(SearchCondition.Match, null, NetworkAdapter.MethodProvider.Disconnect);
                AutomatedTaskFramework.Tasks.Execute();
            }
            catch (Exception ex)
            {
                CustomException.ThrowNew.GenericException(ex.ToString());
            }
        }

        #endregion
        #region SettingsRegister
        private async void SettingsAnimatedButtonSubmit_Click(object sender, EventArgs e)
        {
            string ip = SettingsEditFieldRegisterIP.TextTextBox;
            string strPort = SettingsEditFieldRegisterPort.TextTextBox;
            string username = SettingsEditFieldRegisterUsername.TextTextBox;
            string email = SettingsEditFieldRegisterEmail.TextTextBox; 
            string nickname = SettingsEditFieldRegisterName.TextTextBox;
            bool isIP = false;
            if (string.IsNullOrEmpty(nickname))
            {
                nickname = "User";
            }
            if (string.IsNullOrEmpty(ip))
            {
                CustomException.ThrowNew.FormatException("Please enter the IPv4 address or DNS of the server you'd like to connect to.");
                return;
            }
            if (string.IsNullOrEmpty(strPort))
            {
                CustomException.ThrowNew.FormatException("Please enter the port of the server you'd like to connect to.");
                return;
            }
            if (string.IsNullOrEmpty(username))
            {
                CustomException.ThrowNew.FormatException("Please enter your username.");
                return;
            }
            if (new string[] { username, email, nickname}.Where(element => new string[] { " ", "\"", "'" }.Any(element.Contains)).Any())
            {
                CustomException.ThrowNew.FormatException("The username, nickname and email address may not contain spaces, single or double quotes.");
                return;
            }
            if (username.Contains("__"))
            {
                CustomException.ThrowNew.FormatException("The username may not contain double underscores.");
            }
            if (string.IsNullOrEmpty(email))
            {
                CustomException.ThrowNew.FormatException("Please enter your email address.");
                return;
            }
            // DAMN REGEX SYNTAX SUCKS ... TODO: REPLACE THIS WITH SOME ACTUALLY READABLE LINQ QUERY
            if (!Regex.IsMatch(email, @"^[^.][0-9a-zA-z\.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9-\.]+\.[a-z]+$"))
            {
                CustomException.ThrowNew.FormatException("Please enter a valid email address.");
                return;
            }
            GlobalVarPool.name = nickname;
            GlobalVarPool.email = email;
            GlobalVarPool.username = username;
            int port = Convert.ToInt32(strPort);
            if (port < 1 || port > 65536)
            {
                CustomException.ThrowNew.FormatException("Please enter a valid port number.");
                return;
            }
            // MORE DISGUSTING REGEXES. THIS ONE DOESN'T EVEN WORK PROPERLY AS IT ALLOWS STUFF LIKE 1.1.1 AS IPv4 ADDRESSES --> TODO: LINQ <3
            if (Regex.IsMatch(ip, @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5]).){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$"))
            {
                isIP = true;
                GlobalVarPool.REMOTE_ADDRESS = ip;
            }
            try
            {
                if (!isIP)
                {
                    Task<IPHostEntry> ipTask = Dns.GetHostEntryAsync(ip);
                    IPHostEntry ipAddress = await ipTask;
                    string ipv4String = ipAddress.AddressList.First().ToString();
                    GlobalVarPool.REMOTE_ADDRESS = ipv4String;
                }
                GlobalVarPool.REMOTE_PORT = port;
                GlobalVarPool.previousPanel = SettingsFlowLayoutPanelRegister;
                Thread t = new Thread(new ParameterizedThreadStart(HelperMethods.LoadingHelper))
                {
                    IsBackground = true
                };
                t.Start(new List<object> { SettingsFlowLayoutPanelOnline, SettingsLabelLoadingStatus, true, "GlobalVarPool.isUser" });
                if (GlobalVarPool.connected)
                {
                    AutomatedTaskFramework.Task.Create(SearchCondition.Contains, "SEND_VERIFICATION_ACTIVATE_ACCOUNT", NetworkAdapter.MethodProvider.Register);
                }
                else
                {
                    AutomatedTaskFramework.Task.Create(SearchCondition.In, "COOKIE_DOES_EXIST|DTACKI", NetworkAdapter.MethodProvider.Connect);
                    AutomatedTaskFramework.Task.Create(SearchCondition.Contains, "SEND_VERIFICATION_ACTIVATE_ACCOUNT", NetworkAdapter.MethodProvider.Register);
                }
                AutomatedTaskFramework.Tasks.Execute();
            }
            catch
            {
                CustomException.ThrowNew.GenericException("Something went wrong.");
            }
        }
        #endregion
        #region SettingsOffline
        private void SettingsAnimatedButtonOfflineLogin_Click(object sender, EventArgs e)
        {
            SettingsFlowLayoutPanelLogin.BringToFront();
        }
        private void SettingsAnimatedButtonOfflineRegister_Click(object sender, EventArgs e)
        {
            SettingsFlowLayoutPanelRegister.BringToFront();
        }
        #endregion

        #region SettingsOnline
        #endregion

        #endregion

        
    }
}
