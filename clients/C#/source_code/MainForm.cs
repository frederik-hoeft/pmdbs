using pmdbs.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.IO;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using LunaForms;
using Newtonsoft.Json;

namespace pmdbs
{
    /// <summary>
    /// The main UI form of the application.
    /// </summary>
    public partial class MainForm : MetroFramework.Forms.MetroForm
    {
        #region DECLARATIONS
        private List<ListEntry> entryList = new List<ListEntry>();
        
        private readonly char[] passwordSpecialCharacters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '_', '-', '$', '%', '&', '/', '(', ')', '=', '?', '{', '[', ']', '}', '\\', '+', '*', '#', ',', '.', '<', '>', '|', '@', '!', '~', ';', ':', '"' };
        private readonly char[] passwordCharacters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
        private bool GuiLoaded = false;
        private bool IsDefaultIcon = true;
        private bool EditFieldShown = false;
        private Size MaxSize;
        private Size MinSize;
        private string DataDetailsID;
        private int DataPerPage = 25;
        private int CurrentPage = 0;
        private int CurrentContentCount = 0;
        private int MaxPages = 1;
        public static void InvokeReload()
        {
            GlobalVarPool.MainForm.Invoke((MethodInvoker)delegate
            {
                GlobalVarPool.MainForm.ApplyFilter(0);
            });
        }

        public static void InvokeSyncAnimationStop()
        {
            GlobalVarPool.MainForm.Invoke((MethodInvoker)delegate
            {
                GlobalVarPool.MainForm.SyncAnimationStop();
            });
        }

        public static void InvokeRefreshSettings()
        {
            GlobalVarPool.MainForm.Invoke((MethodInvoker)delegate
            {
                GlobalVarPool.MainForm.RefreshSettings();
            });
        }

        public static void InvokeDashboardUpdate()
        {
            GlobalVarPool.MainForm.Invoke((MethodInvoker)delegate
            {
                GlobalVarPool.MainForm.UpdateDashboard();
            });
        }
        #endregion

        #region FUNCTIONALITY_METHODS_AND_OTHER_UGLY_CODE
        private void DataFlowLayoutPanelList_MouseEnter(object sender, EventArgs e)
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

        private void windowButtonMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void WindowHeaderLabelLogo_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                WinAPI.ReleaseCapture();
                WinAPI.SendMessage(Handle, WinAPI.WM_NCLBUTTONDOWN, WinAPI.HT_CAPTION, 0);
            }
        }

        private void FlowLayoutPanel1_Resize(object sender, EventArgs e)
        {
            foreach (ListEntry entry in DataFlowLayoutPanelList.Controls)
            {
                entry.Width = DataFlowLayoutPanelList.Width - 25;
            }
            WinAPI.ShowScrollBar(DataFlowLayoutPanelList.Handle, (int)WinAPI.ScrollBarDirection.SB_HORZ, false);
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
        /// <summary>
        /// The main UI form of the application.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            #region LOAD
            // PREVENT FLICKERING
            WinAPI.PreventFlickering(this);
            GlobalVarPool.MainForm = this;
            InitializeTransparency();
            #region INIT
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            MaxSize = this.MaximumSize;
            MinSize = this.MinimumSize;
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
            GlobalVarPool.settingsPanel = SettingsTableLayoutPanelMain;
            GlobalVarPool.syncButton = DataSyncAdvancedImageButton;
            GlobalVarPool.deviceList = DashboardLunaItemListDevices;
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
            FilterEditFieldSearch.TextBoxTextChanged += FilterEditFieldSearch_TextChanged;
            LoginEditFieldOfflinePassword.EnterKeyPressed += LoginEditFieldOfflinePassword_EnterKeyPressed;
            SyncAnimationTimer.Tick += SyncAnimationTimer_Tick;
            windowButtonMinimize.OnClickEvent += windowButtonMinimize_Click;
            DashboardLunaSmallCardListHotspots.OnCardClicked += DashboardLunaSmallCardListHotspots_CardClicked;
            #endregion

            #endregion
            DashboardLunaItemListDevices.LunaItemClicked += DashboardLunaItemDevice_Click;
            DashboardLunaItemListBreaches.LunaItemClicked += Breach_Clicked;
        }

        private async void MainForm_Load(object sender, EventArgs e)
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
            InitializeLoadingScreen();
            await HelperMethods.LoadSettings();
            //TODO: Check for Password Changes
            if (GlobalVarPool.scryptHash.Equals(string.Empty))
            {
                LoginPictureBoxOnlineMain.SuspendLayout();
                LoginPictureBoxOfflineMain.SuspendLayout();
                LoginPictureBoxRegisterMain.BringToFront();
            }
            else
            {
                LoginPictureBoxOnlineMain.SuspendLayout();
                LoginPictureBoxOfflineMain.BringToFront();
                LoginPictureBoxRegisterMain.SuspendLayout();
                if (!GlobalVarPool.name.Equals("User"))
                {
                    LoginLabelOfflineUsername.Text = GlobalVarPool.name;
                }
            }
            Thread.Sleep(1500); // SHOW SPLASHSCREEN
            HideFilterPanel();
            GuiLoaded = true;
        }

        #region Menu
        Bitmap bmp;
        float angle = 0f;
        bool showSyncAnimation = false;
        private System.Windows.Forms.Timer SyncAnimationTimer = new System.Windows.Forms.Timer();
        private void MenuMenuEntryHome_Click(object sender, EventArgs e)
        {
            MenuPanelHomeIndicator.BackColor = Colors.Orange;
            MenuPanelSettingsIndicator.BackColor = Color.White;
            MenuPanelPasswordsIndicator.BackColor = Color.White;
            WindowHeaderLabelTitle.Text = "Dashboard";
            HideFilterPanel();
            DashboardTableLayoutPanel.BringToFront();
        }

        private void MenuMenuEntrySettings_Click(object sender, EventArgs e)
        {
            MenuPanelHomeIndicator.BackColor = Color.White;
            MenuPanelSettingsIndicator.BackColor = Colors.Orange;
            MenuPanelPasswordsIndicator.BackColor = Color.White;
            SettingsTableLayoutPanelMain.BringToFront();
            HideFilterPanel();
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
            ShowFilterPanel();
            DataTableLayoutPanelMain.BringToFront();
            WindowHeaderLabelTitle.Text = "Passwords";
        }

        private void HideFilterPanel()
        {
            FilterLabelSort.Visible = false;
            FilterAdvancedComboBoxSort.Visible = false;
            FilterEditFieldSearch.Visible = false;
            FilterPanel.Visible = false;
        }

        private void ShowFilterPanel()
        {
            FilterLabelSort.Visible = true;
            FilterAdvancedComboBoxSort.Visible = true;
            FilterEditFieldSearch.Visible = true;
            FilterPanel.Visible = true;
        }

        private void SyncAnimationStart()
        {
            showSyncAnimation = true;
            WindowManager.LoadingScreen.OutputLabel = MenuSyncLabelStatus;
            WindowManager.LoadingScreen.OutputAvailable = true;
            bmp = new Bitmap(Resources.icon_syncing);
            MenuSyncLabelHeader.Text = "Syncing ...";
            MenuSyncLabelHeader.ForeColor = Colors.Orange;
            MenuSyncLabelStatus.ForeColor = Colors.Orange;
            MenuSyncPictureBox.Image = Resources.icon_empty;
            SyncAnimationTimer.Interval = 50;
            SyncAnimationTimer.Start();
        }

        private void SyncAnimationStop()
        {
            showSyncAnimation = false;
            WindowManager.LoadingScreen.OutputAvailable = false;
        }

        private void SyncAnimationTimer_Tick(object sender, EventArgs e)
        {
            angle += 18;
            angle = angle % 360;
            MenuSyncPictureBox.Invalidate();
            if (!showSyncAnimation && angle == 0)
            {
                SyncAnimationTimer.Stop();
                bmp = null;
                MenuSyncPictureBox.Invalidate();
                MenuSyncPictureBox.Image = Resources.Icon_sync;
                WindowManager.LoadingScreen.OutputAvailable = false;
                MenuSyncLabelHeader.ForeColor = Color.FromArgb(100, 100, 100);
                MenuSyncLabelStatus.ForeColor = Color.FromArgb(100, 100, 100);
                MenuSyncLabelHeader.Text = "Sync";
                MenuSyncLabelStatus.Text = "Last Update: " + TimeConverter.UnixTimeStampToDateTime(Convert.ToDouble(TimeConverter.TimeStamp())).ToString("u");
            }
        }

        private void MenuSyncPictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (bmp != null)
            {
                float bw2 = bmp.Width / 2f;
                float bh2 = bmp.Height / 2f;
                e.Graphics.TranslateTransform(bw2, bh2);
                e.Graphics.RotateTransform(angle);
                e.Graphics.TranslateTransform(-bw2, -bh2);
                e.Graphics.DrawImage(bmp, 0, 0);
                e.Graphics.ResetTransform();
            }
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
                HostNameFont = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0),
                HostNameForeColor = SystemColors.ControlText,
                Name = "listEntry",
                Size = new Size(1041, 75),
                TabIndex = 14,
                TimeStamp = strTimeStamp,
                TimeStampFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                TimeStampForeColor = SystemColors.ControlText,
                UserName = newRow["4"].ToString(),
                UserNameFont = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
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

        private void RefreshUserData(int page)
        {
            if (page < 0)
            {
                page = 0;
            }
            DataTable userData = GlobalVarPool.FilteredUserData;
            // GET MAXIMUM PAGES (FLOOR)
            MaxPages = Convert.ToInt32(Math.Floor((double)userData.Rows.Count / DataPerPage));
            if (userData.Rows.Count % DataPerPage != 0)
            {
                // ADD ONE PAGE
                MaxPages++;
            }
            // CHECK IF PAGE IS WITHIN BOUNDARIES
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
            for (int i = CurrentPage * DataPerPage; ((CurrentPage * DataPerPage) + DataPerPage >= userData.Rows.Count) ? i < userData.Rows.Count : i < (CurrentPage * DataPerPage) + DataPerPage; i++)
            {
                int ID = Convert.ToInt32(userData.Rows[i]["0"]);
                string strTimeStamp = TimeConverter.UnixTimeStampToDateTime(Convert.ToDouble(userData.Rows[i]["2"].ToString())).ToString("u");
                strTimeStamp = strTimeStamp.Substring(0, strTimeStamp.Length - 1);
                ListEntry entry = entryList[i % DataPerPage];
                byte[] iconBytes = Convert.FromBase64String(userData.Rows[i]["9"].ToString());
                using (MemoryStream ms = new MemoryStream(iconBytes, 0, iconBytes.Length))
                {
                    Image icon = Image.FromStream(ms, true);
                    entry.FavIcon = icon.GetThumbnailImage(350, 350, null, new IntPtr());
                    icon.Dispose();
                }
                entry.HostName = userData.Rows[i]["3"].ToString().Equals("\x01") ? "-" : userData.Rows[i]["3"].ToString();
                entry.TimeStamp = strTimeStamp;
                entry.UserName = userData.Rows[i]["4"].ToString();
                entry.id = ID;
                entry.Show();
                CurrentContentCount++;
            }
            DataFlowLayoutPanelList.ResumeLayout();
            FlowLayoutPanel1_Resize(this, null);
        }

        private async void ListEntry_Click(object sender, EventArgs e)
        {
            if (EditFieldShown)
            {
                // USE LINQ TO CHECK IF THERE ARE UNSAVED CHANGES
                if (!GlobalVarPool.UserData.AsEnumerable().Where(row => row["0"].ToString().Equals(DataDetailsID)).Where(row => (row["3"].ToString().Equals(DataEditEditFieldHostname.TextTextBox) || (row["3"].ToString().Equals("\x01") && string.IsNullOrEmpty(DataEditEditFieldHostname.TextTextBox))) && (row["4"].ToString().Equals(DataEditEditFieldUsername.TextTextBox) || (row["4"] .ToString().Equals("\x01") && string.IsNullOrEmpty(DataEditEditFieldUsername.TextTextBox))) && (row["5"].ToString().Equals(DataEditEditFieldPassword.TextTextBox) || (row["5"].ToString().Equals("\x01") && string.IsNullOrEmpty(DataEditEditFieldPassword.TextTextBox))) && (row["6"].ToString().Equals(DataEditEditFieldWebsite.TextTextBox) || (row["6"].ToString().Equals("\x01") && string.IsNullOrEmpty(DataEditEditFieldWebsite.TextTextBox))) && (row["7"].ToString().Equals(DataEditEditFieldEmail.TextTextBox) || row["7"].ToString().Equals("\x01") && (string.IsNullOrEmpty(DataEditEditFieldEmail.TextTextBox))) && (row["8"].ToString().Equals(DataEditAdvancedRichTextBoxNotes.TextValue) || (row["8"].ToString().Equals("\x01") && string.IsNullOrEmpty(DataEditAdvancedRichTextBoxNotes.TextValue)))).Any())
                {
                    Task<DialogResult> ShowDialog = WindowManager.Overlay.Spawn(this, new ConfirmationForm("There are unsaved changes. Do you really want to discard them?", MessageBoxButtons.YesNo));
                    DialogResult result = await ShowDialog;
                    if (!result.Equals(DialogResult.OK))
                    {
                        return;
                    }
                }
                EditFieldShown = false;
            }
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
            string score = LinkedRow["10"].ToString();
            int scoreValue = Convert.ToInt32(score);
            Password.Result result = Password.Result.FromScore(scoreValue);
            DataLunaSmallCardDetailsPasswordStrength.Image = result.Icon;
            DataLunaSmallCardDetailsPasswordStrength.Info = "Score: " + result.Score.ToString();
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
            string hostname = DataBaseHelper.Security.SQLInjectionCheck(DataEditEditFieldHostname.TextTextBox);
            string username = DataBaseHelper.Security.SQLInjectionCheck(DataEditEditFieldUsername.TextTextBox);
            string password = DataBaseHelper.Security.SQLInjectionCheck(DataEditEditFieldPassword.TextTextBox);
            string email = DataBaseHelper.Security.SQLInjectionCheck(DataEditEditFieldEmail.TextTextBox);
            string website = DataBaseHelper.Security.SQLInjectionCheck(DataEditEditFieldWebsite.TextTextBox);
            string notes = DataBaseHelper.Security.SQLInjectionCheck(DataEditAdvancedRichTextBoxNotes.TextValue);
            string dateTime = TimeConverter.TimeStamp();
            if (password.Equals("") || hostname.Equals(""))
            {
                CustomException.ThrowNew.GenericException("Please enter at least a hostname and a password to save this account!");
                DataEditSaveAdvancedImageButton.Enabled = true;
                return;
            }
            Task<Password.Result> GetResult = Password.Security.CheckAsync(password);
            Password.Result result = await GetResult;
            switch (result.IsCompromised)
            {
                case 1:
                    {
                        _ = WindowManager.Overlay.Spawn(this, new ErrorForm("This password is COMMONLY USED and has been leaked " + result.TimesSeen.ToString() + " times previously. ", "Security Warning", "Common password detected!", false));
                        DataEditSaveAdvancedImageButton.Enabled = true;
                        return;
                    }
                default:
                    {
                        break;
                    }
            }
            string score = result.Score.ToString();
            string[] values = new string[]
            {
                hostname,
                username,
                password,
                website,
                email,
                notes,
                score
            };
            string[] rawValues = new string[values.Length];
            Array.Copy(values, rawValues, values.Length);
            string[] Columns = new string[]
            {
                "D_host",
                "D_uname",
                "D_password",
                "D_url",
                "D_email",
                "D_notes",
                "D_score"
            };
            
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Equals(""))
                {
                    values[i] = "\x01";
                }
                else
                {
                    values[i] = CryptoHelper.AESEncrypt(values[i], GlobalVarPool.localAESkey);
                }
            }
            string Query = "UPDATE Tbl_data SET D_datetime = \"" + dateTime + "\"";
            for (int i = 0; i < Columns.Length; i++)
            {
                Query += ", " + Columns[i] + " = \"" + values[i] + "\"";
            }
            Query += " WHERE D_id = " + DataDetailsID + ";";
            await DataBaseHelper.ModifyData(Query);
            DataRow LinkedRow = GlobalVarPool.UserData.AsEnumerable().SingleOrDefault(r => r.Field<string>("0").Equals(DataDetailsID));
            string oldUrl = LinkedRow["6"].ToString();
            string oldHostname = LinkedRow["3"].ToString();
            if (!website.Equals(oldUrl))
            {
                new Thread(async delegate () {
                    string favIcon = "";
                    try
                    {
                        if (string.IsNullOrWhiteSpace(website))
                        {
                            if (hostname[0].Equals(oldHostname[0]) || oldUrl.Equals("\x01"))
                            {
                                return;
                            }
                            favIcon = IconExtractor.Generate(hostname);
                        }
                        else
                        {
                            IconExtractor extractor = IconExtractor.Load(website);
                            await extractor.Extract();
                            extractor.ApplyFilter(48);
                            if (extractor.IconsAvailable)
                            {
                                IconExtractor.Icon icon = extractor.GetBestIcon();
                                favIcon = icon.ToBase64String();
                            }
                            else
                            {
                                favIcon = IconExtractor.Generate(hostname);
                            }
                        }
                    }
                    catch (UriFormatException)
                    {
                        favIcon = IconExtractor.Generate(hostname);
                    }
                    catch (Exception ex)
                    {
                        CustomException.ThrowNew.NetworkException(ex.ToString());
                        return;
                    }
                    LinkedRow["9"] = favIcon;
                    string encryptedFavIcon = CryptoHelper.AESEncrypt(favIcon, GlobalVarPool.localAESkey);
                    Query = DataBaseHelper.Security.SQLInjectionCheckQuery(new string[] { "UPDATE Tbl_data SET D_icon = \"", encryptedFavIcon, "\" WHERE D_id = ", DataDetailsID, ";" });
                    await DataBaseHelper.ModifyData(Query);
                    Invoke((MethodInvoker)delegate
                    {
                        UpdateDetailsWindow(LinkedRow);
                        ApplyFilter(CurrentPage);
                    });
                }).Start();
            }
            for (int i = 3; i < (int)ColumnCount.Tbl_data - 2; i++)
            {
                LinkedRow[i.ToString()] = rawValues[i - 3].Equals("") ? "\x01" : rawValues[i - 3];
            }
            LinkedRow["10"] = score;
            UpdateDetailsWindow(LinkedRow);
            ApplyFilter(CurrentPage);
            DataFlowLayoutPanelEdit.SuspendLayout();
            DataPanelDetails.BringToFront();
            DataPanelDetails.ResumeLayout();
            DataEditSaveAdvancedImageButton.Enabled = true;
            EditFieldShown = false;
        }

        private async void DataEditCancel_Click(object sender, EventArgs e)
        {
            if (HasUnsafedChanges())
            {
                Task<DialogResult> ShowDialog = WindowManager.Overlay.Spawn(this, new ConfirmationForm("There are unsaved changes. Do you really want to discard them?", MessageBoxButtons.YesNo));
                DialogResult result = await ShowDialog;
                if (!result.Equals(DialogResult.OK))
                {
                    return;
                }
            }
            DataFlowLayoutPanelEdit.SuspendLayout();
            DataPanelDetails.BringToFront();
            DataPanelDetails.ResumeLayout();
            EditFieldShown = false;
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
            EditFieldShown = true;
        }

        private void DataEditEditFieldPassword_TextBoxTextChanged(object sender, EventArgs e)
        {
            string password = DataEditEditFieldPassword.TextTextBox;
            UpdatePasswordStrength(password, DataEditLabelPasswordStrength, DataEditPasswordStrengthIndicator);
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
            Task<DialogResult> ShowDialog = WindowManager.Overlay.Spawn(this, new ConfirmationForm("Do you really want to delete the selected account?", MessageBoxButtons.YesNo));
            DialogResult result = await ShowDialog;
            if (!result.Equals(DialogResult.OK))
            {
                return;
            }
            DataRow LinkedRow = GlobalVarPool.UserData.AsEnumerable().SingleOrDefault(r => r.Field<string>("0").Equals(DataDetailsID));
            string hid = LinkedRow["1"].ToString();
            if (!hid.Equals("EMPTY"))
            {
                await DataBaseHelper.ModifyData(DataBaseHelper.Security.SQLInjectionCheckQuery(new string[] { "INSERT INTO Tbl_delete (DEL_hid) VALUES (\"", hid, "\");" }));
            }
            await DataBaseHelper.ModifyData(DataBaseHelper.Security.SQLInjectionCheckQuery(new string[] { "DELETE FROM Tbl_data WHERE D_id = ", DataDetailsID, ";" }));
            GlobalVarPool.UserData.Rows.Remove(LinkedRow);
            ApplyFilter(CurrentPage);
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
            SyncAnimationStart();
            AutomatedTaskFramework.Tasks.Clear();
            if (!GlobalVarPool.connected)
            {
                AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "DEVICE_AUTHORIZED", NetworkAdapter.MethodProvider.Connect);
            }
            if (!GlobalVarPool.isUser)
            {
                AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.In, "ALREADY_LOGGED_IN|LOGIN_SUCCESSFUL", NetworkAdapter.MethodProvider.Login);
            }
            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "FETCH_SYNC", NetworkAdapter.MethodProvider.Sync);
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

        private void DataFilterResultsAnimatedButtonReset_Click(object sender, EventArgs e)
        {
            FilterEditFieldSearch.TextTextBox = string.Empty;
            FilterEditFieldSearch.ShowDefaultValue();
            DataPanelNoSel.BringToFront();
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
            AddPictureBoxCheckIconIcon.Image = Resources._default;
            IsDefaultIcon = true;
        }

        private async void AddPanelAdvancedImageButtonSave_Click(object sender, EventArgs e)
        {
            AddPanelAdvancedImageButtonSave.Enabled = false;
            string hostname = DataBaseHelper.Security.SQLInjectionCheck(AddEditFieldHostname.TextTextBox);
            string username = DataBaseHelper.Security.SQLInjectionCheck(AddEditFieldUsername.TextTextBox);
            string password = DataBaseHelper.Security.SQLInjectionCheck(AddEditFieldPassword.TextTextBox);
            string email = DataBaseHelper.Security.SQLInjectionCheck(AddEditFieldEmail.TextTextBox);
            string website = DataBaseHelper.Security.SQLInjectionCheck(AddEditFieldWebsite.TextTextBox);
            string notes = DataBaseHelper.Security.SQLInjectionCheck(AddPanelNotesAdvancedRichTextBox.TextValue);
            string dateTime = TimeConverter.TimeStamp();
            
            if (password.Equals("") || hostname.Equals(""))
            {
                CustomException.ThrowNew.GenericException("Please enter at least a hostname and a password to save this account!");
                AddPanelAdvancedImageButtonSave.Enabled = true;
                return;
            }
            Task<Password.Result> GetResult = Password.Security.CheckAsync(password);
            Password.Result result = await GetResult;
            switch (result.IsCompromised)
            {
                case 1:
                {
                        _ = WindowManager.Overlay.Spawn(this, new ErrorForm("This password is COMMONLY USED and has been leaked " + result.TimesSeen.ToString() + " times previously. ", "Security Warning", "Common password detected!", false));
                    AddPanelAdvancedImageButtonSave.Enabled = true;
                    return;
                }
                default:
                {
                    break;
                }
            }
            string score = result.Score.ToString();
            new Thread(async delegate() {
                string favIcon = string.Empty;
                if (!IsDefaultIcon)
                {
                    using (Bitmap iconBmp = new Bitmap(AddPictureBoxCheckIconIcon.Image))
                    using (MemoryStream ms = new MemoryStream())
                    {
                        iconBmp.Save(ms, ImageFormat.Png);
                        favIcon = Convert.ToBase64String(ms.ToArray());
                    }
                }
                else
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(website))
                        {
                            favIcon = IconExtractor.Generate(hostname);
                        }
                        else
                        {
                            IconExtractor extractor = IconExtractor.Load(website);
                            await extractor.Extract();
                            extractor.ApplyFilter(48);
                            if (extractor.IconsAvailable)
                            {
                                IconExtractor.Icon icon = extractor.GetBestIcon();
                                favIcon = icon.ToBase64String();
                            }
                            else
                            {
                                favIcon = IconExtractor.Generate(hostname);
                            }
                        }
                    }
                    catch (UriFormatException)
                    {
                        favIcon = IconExtractor.Generate(hostname);
                    }
                    catch(Exception ex)
                    {
                        CustomException.ThrowNew.NetworkException(ex.ToString());
                        return;
                    }
                }
                string[] Values = new string[]
                {
                    hostname,
                    username,
                    password,
                    email,
                    website,
                    notes,
                    favIcon,
                    score
                };
                string[] Columns = new string[]
                {
                    "D_host",
                    "D_uname",
                    "D_password",
                    "D_email",
                    "D_url",
                    "D_notes",
                    "D_icon",
                    "D_score"
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
                Query += ") VALUES (\"" + dateTime + "\"";
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
                    NewRow["2"] = dateTime;
                    NewRow["3"] = hostname.Equals("") ? "\x01" : hostname;
                    NewRow["4"] = username.Equals("") ? "\x01" : username;
                    NewRow["5"] = password.Equals("") ? "\x01" : password;
                    NewRow["6"] = website.Equals("") ? "\x01" : website;
                    NewRow["7"] = email.Equals("") ? "\x01" : email;
                    NewRow["8"] = notes.Equals("") ? "\x01" : notes;
                    NewRow["9"] = favIcon;
                    NewRow["10"] = score;
                    ApplyFilter(CurrentPage);
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

        private void AddPanelAnimatedButtonCheckIcon_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(AddEditFieldHostname.TextTextBox))
            {
                CustomException.ThrowNew.GenericException("Please enter a valid Hostname first!");
                return;
            }
            AddPanelAnimatedButtonCheckIcon.Enabled = false;
            new Thread(async delegate ()
            {
                string favIcon = string.Empty;
                string url = AddEditFieldWebsite.TextTextBox;
                string hostname = AddEditFieldHostname.TextTextBox;
                try
                {
                    IconExtractor extractor = IconExtractor.Load(url);
                    await extractor.Extract();
                    extractor.ApplyFilter(48);
                    if (extractor.IconsAvailable)
                    {
                        IconExtractor.Icon icon = extractor.GetBestIcon();
                        favIcon = icon.ToBase64String();
                    }
                    else
                    {
                        favIcon = IconExtractor.Generate(hostname);
                    }
                }
                catch (UriFormatException)
                {
                    favIcon = IconExtractor.Generate(hostname);
                }
                catch (Exception ex)
                {
                    CustomException.ThrowNew.NetworkException(ex.ToString());
                    return;
                }
                byte[] iconBytes = Convert.FromBase64String(favIcon);
                Invoke((MethodInvoker)delegate
                {
                    using (MemoryStream ms = new MemoryStream(iconBytes, 0, iconBytes.Length))
                    {
                        Image icon = Image.FromStream(ms, true);
                        AddPictureBoxCheckIconIcon.Image = icon.GetThumbnailImage(350, 350, null, new IntPtr());
                        icon.Dispose();
                    }
                    IsDefaultIcon = false;
                    AddPanelAnimatedButtonCheckIcon.Enabled = true;
                });
            }).Start();
        }

        private void AddEditFieldPassword_TextBoxTextChanged(object sender, EventArgs e)
        {
            string password = AddEditFieldPassword.TextTextBox;
            UpdatePasswordStrength(password, AddLabelPasswordStrengthIndicator, AddPasswordStrengthIndicator);
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
                graph.FillRectangle(new SolidBrush(Color.FromArgb(200, 240, 240, 240)), ImageSize);
            }
            LoginPictureBoxOnlineMain.Image = bmp;
            LoginPictureBoxRegisterMain.Image = bmp;
            LoginPictureBoxOfflineMain.Image = bmp;
            LoadingPictureBoxOverlay.Image = bmp;
            LoginPictureBoxOnlineSettings.Image = bmp;
            bmp = new Bitmap(PictureBoxOverlay.Width, PictureBoxOverlay.Height);
            using (Graphics graph = Graphics.FromImage(bmp))
            {
                Rectangle ImageSize = new Rectangle(0, 0, LoginPictureBoxOnlineMain.Width, LoginPictureBoxOnlineMain.Height);
                graph.FillRectangle(new SolidBrush(Color.FromArgb(100, 17, 17, 17)), ImageSize);
            }
            PictureBoxOverlay.Image = bmp;
        }

        private void LoginLabelOnlineBack_Click(object sender, EventArgs e)
        {
            LoginPictureBoxOnlineMain.SuspendLayout();
            LoginPictureBoxOnlineSettings.BringToFront();
            LoginPictureBoxOnlineSettings.ResumeLayout();
        }

        private void LoginLabelRegisterSignIn_Click(object sender, EventArgs e)
        {
            LoginPictureBoxRegisterMain.SuspendLayout();
            LoginPictureBoxOnlineSettings.BringToFront();
            LoginPictureBoxOnlineSettings.ResumeLayout();
        }

        private void LoginAnimatedButtonOnlineLogin_Click(object sender, EventArgs e)
        {
            string username = LoginEditFieldOnlineUsername.TextTextBox;
            string password = LoginEditFieldOnlinePassword.TextTextBox;
            if (!username.IsValidUsername())
            {
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                LoginLabelOfflineError.Text = "Please enter a password!";
                LoginLabelOfflineError.Visible = true;
                LoginButtonDisabled = false;
                return;
            }
            WindowManager.LoadingScreen.Show(LoadingLabelStatus);
            WindowManager.LoadingScreen.SetStatus("Hashing Password...");
            LoginPictureBoxOnlineMain.SuspendLayout();
            
            
            GlobalVarPool.plainMasterPassword = password;
            string stage1PasswordHash = CryptoHelper.SHA256Hash(password);
            GlobalVarPool.localAESkey = CryptoHelper.SHA256Hash(stage1PasswordHash.Substring(32, 32));
            GlobalVarPool.onlinePassword = CryptoHelper.SHA256Hash(stage1PasswordHash.Substring(0, 32));
            Action OnTaskFailed = new Action(delegate 
            {
                GlobalVarPool.MainForm.Invoke((MethodInvoker)delegate 
                {
                    LoginPictureBoxOnlineSettings.ResumeLayout();
                    WindowManager.LoadingScreen.Hide();
                });
            });
            Action OnLoginFailed = new Action(delegate
            {
                GlobalVarPool.MainForm.Invoke((MethodInvoker)delegate
                {
                    LoginPictureBoxOnlineMain.ResumeLayout();
                    WindowManager.LoadingScreen.Hide();
                });
            });
            Action InitializeDatabase = new Action(async delegate
            {
                WindowManager.LoadingScreen.InvokeSetStatus("Hashing Password...");
                string firstUsage = TimeConverter.TimeStamp();
                Task<string> ScryptTask = Task.Run(() => CryptoHelper.ScryptHash(stage1PasswordHash, firstUsage));
                string stage2PasswordHash = await ScryptTask;
                WindowManager.LoadingScreen.InvokeSetStatus("Initializing Database...");
                await DataBaseHelper.ModifyData(DataBaseHelper.Security.SQLInjectionCheckQuery(new string[] { "UPDATE Tbl_user SET U_password = \"", stage2PasswordHash, "\", U_wasOnline = 1, U_firstUsage = \"", firstUsage, "\", U_username = \"", GlobalVarPool.username, "\", U_cookie = \"", GlobalVarPool.cookie, "\", U_datetime = 0;" }));
                for (int i = GlobalVarPool.UserData.Columns.Count; i < 11; i++)
                {
                    GlobalVarPool.UserData.Columns.Add(i.ToString(), typeof(string));
                }
                GlobalVarPool.isLocalDatabaseInitialized = true;
            });
            Func<bool> databaseInitializedWhen = new Func<bool>(delegate 
            {
                return GlobalVarPool.isLocalDatabaseInitialized;
            });
            Func<bool> dataListInitializedWhen = new Func<bool>(delegate
            {
                return GlobalVarPool.uiInitialized;
            });
            Func<bool> syncFinishedWhen = new Func<bool>(delegate
            {
                if (AutomatedTaskFramework.Tasks.InteractiveSubTaskFinished)
                {
                    AutomatedTaskFramework.Tasks.InteractiveSubTaskFinished = false;
                    return true;
                }
                return false;
            });
            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "DEVICE_AUTHORIZED", NetworkAdapter.MethodProvider.Connect, OnTaskFailed);
            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.In, "ALREADY_LOGGED_IN|LOGIN_SUCCESSFUL", NetworkAdapter.MethodProvider.Login, OnLoginFailed);
            AutomatedTaskFramework.Task.Create(TaskType.Interactive, InitializeDatabase, databaseInitializedWhen, OnTaskFailed);
            AutomatedTaskFramework.Task.Create(TaskType.Interactive, InitializeDataList, dataListInitializedWhen, OnTaskFailed);
            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.In, "AD_OUTDATED|AD_UPTODATE", NetworkAdapter.MethodProvider.GetAccountDetails, OnTaskFailed);
            AutomatedTaskFramework.Task.Create(TaskType.Interactive, NetworkAdapter.MethodProvider.Sync, syncFinishedWhen, OnTaskFailed);
            AutomatedTaskFramework.Task.Create(TaskType.FireAndForget, FinalizeLogin);
            AutomatedTaskFramework.Tasks.Execute();
        }

        private void LoginEditFieldOfflinePassword_EnterKeyPressed(object sender, EventArgs e)
        {
            LoginAnimatedButtonOfflineLogin_Click(sender, e);
        }

        private void LoginEditFieldOfflinePassword_TextBoxTextChanged(object sender, EventArgs e)
        {
            LoginLabelOfflineError.Visible = false;
        }

        private async void LoginAnimatedButtonOfflineLogin_Click(object sender, EventArgs e)
        {
            if (LoginButtonDisabled)
            {
                return;
            }
            LoginButtonDisabled = true;
            string password = LoginEditFieldOfflinePassword.TextTextBox;
            if (string.IsNullOrEmpty(password))
            {
                LoginLabelOfflineError.Text = "Please enter a password!";
                LoginLabelOfflineError.Visible = true;
                LoginButtonDisabled = false;
                return;
            }
            WindowManager.LoadingScreen.Show(LoadingLabelStatus);
            LoginPictureBoxOfflineMain.SuspendLayout();
            WindowManager.LoadingScreen.SetStatus("Hashing Password...");
            string stage1PasswordHash = CryptoHelper.SHA256Hash(password);
            Task<string> ScryptTask = Task.Run(() => CryptoHelper.ScryptHash(stage1PasswordHash, GlobalVarPool.firstUsage));
            string stage2PasswordHash = await ScryptTask;
            WindowManager.LoadingScreen.SetStatus("Checking Password...");
            if (!stage2PasswordHash.Equals(GlobalVarPool.scryptHash))
            {
                LoginLabelOfflineError.Text = "Wrong Password!";
                LoginLabelOfflineError.Visible = true;
                LoginButtonDisabled = false;
                LoginPictureBoxOfflineMain.ResumeLayout();
                WindowManager.LoadingScreen.Hide();
                return;
            }
            GlobalVarPool.localAESkey = CryptoHelper.SHA256Hash(stage1PasswordHash.Substring(32, 32));
            GlobalVarPool.onlinePassword = CryptoHelper.SHA256Hash(stage1PasswordHash.Substring(0, 32));
            WindowManager.LoadingScreen.SetStatus("Decrypting Your Data... 0%");
            Task<DataTable> GetData = DataBaseHelper.GetDataAsDataTable("SELECT D_id, D_hid, D_datetime, D_host, D_uname, D_password, D_url, D_email, D_notes, D_icon, D_score FROM Tbl_data;", (int)ColumnCount.Tbl_data);
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
                    WindowManager.LoadingScreen.SetStatus("Decrypting Your Data... " + FinalPercentage.ToString() + "%");
                }
                RowCounter++;
            }
            WindowManager.LoadingScreen.SetStatus("Loading User Interface...");
            DataFlowLayoutPanelList.SuspendLayout();
            new Thread(InitializeUI).Start();
        }

        private async void LoginAnimatedButtonRegister_Click(object sender, EventArgs e)
        {
            if (LoginButtonDisabled)
            {
                return;
            }
            LoginButtonDisabled = true;
            string Password1 = LoginEditFieldRegisterPassword.TextTextBox;
            string Password2 = LoginEditFieldRegisterPassword2.TextTextBox;
            if (!Password1.Equals(Password2))
            {
                LoginLabelRegisterError.Visible = true;
                LoginLabelRegisterError.Text = "These passwords don't match!";
                LoginButtonDisabled = false;
                return;
            }
            Password.Result offlineResult = Password.Security.SimpleCheck(Password1);
            if (offlineResult.Score < 110)
            {
                _ = WindowManager.Overlay.Spawn(this, new ErrorForm("It should be at least \"Okay\".\n\nTry adding more numbers, special characters or even unicode characters to increase the password strength.", "Security Exception", "Your password is too weak!", false));
                LoginLabelRegisterError.Visible = true;
                LoginLabelRegisterError.Text = "Password too weak.";
                LoginButtonDisabled = false;
                return;
            }
            WindowManager.LoadingScreen.Show(LoadingLabelStatus);
            LoginPictureBoxRegisterMain.SuspendLayout();
            WindowManager.LoadingScreen.SetStatus("Checking Password Strength...");
            Task<Password.Result> GetResult = Password.Security.OnlineCheckAsync(Password1);
            Password.Result result = await GetResult;
            LoginLabelRegisterError.ForeColor = Color.Firebrick;
            switch (result.IsCompromised)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        _ = WindowManager.Overlay.Spawn(this, new ErrorForm("This password is COMMONLY USED and has been leaked " + result.TimesSeen.ToString() + " times previously. ", "Security Warning", "Common password detected!", false));
                        LoginPictureBoxRegisterMain.ResumeLayout();
                        LoginButtonDisabled = false;
                        WindowManager.LoadingScreen.Hide();
                        return;
                    }
                default:
                    {
                        // CONNECTION ERROR
                        bool actionIsConfirmed = WindowManager.Overlay.Spawn(this, new ConfirmationForm("Failed to establish a connection to the following online service:\nPassword Leak Checker.\nIt could not be validated that your password is strong and has not been leaked previously.\n\nDo you want to continue anyway?", "Security Warning", MessageBoxButtons.YesNo, false)).Equals(DialogResult.OK);
                        if (!actionIsConfirmed)
                        {
                            LoginPictureBoxRegisterMain.ResumeLayout();
                            LoginButtonDisabled = false;
                            WindowManager.LoadingScreen.Hide();
                            return;
                        }
                        break;
                    }
            }
            WindowManager.LoadingScreen.SetStatus("Hashing Password...");
            string Stage1PasswordHash = CryptoHelper.SHA256Hash(Password1);
            string firstUsage = TimeConverter.TimeStamp();
            Task<string> ScryptTask = Task.Run(() => CryptoHelper.ScryptHash(Stage1PasswordHash, firstUsage));
            string stage2PasswordHash = await ScryptTask;
            WindowManager.LoadingScreen.SetStatus("Initializing Database...");
            await DataBaseHelper.ModifyData(DataBaseHelper.Security.SQLInjectionCheckQuery(new string[] { "UPDATE Tbl_user SET U_password = \"", stage2PasswordHash, "\", U_wasOnline = 1, U_firstUsage = \"", firstUsage, "\";" }));
            GlobalVarPool.isLocalDatabaseInitialized = true;
            GlobalVarPool.localAESkey = CryptoHelper.SHA256Hash(Stage1PasswordHash.Substring(32, 32));
            GlobalVarPool.onlinePassword = CryptoHelper.SHA256Hash(Stage1PasswordHash.Substring(0, 32));
            for (int i = GlobalVarPool.UserData.Columns.Count; i < 11; i++)
            {
                GlobalVarPool.UserData.Columns.Add(i.ToString(), typeof(string));
            }
            new Thread(InitializeUI).Start();
        }
        private void LoginEditFieldRegisterPassword2_TextBoxTextChanged(object sender, EventArgs e)
        {
            if (LoginLabelRegisterError.Visible)
            {
                LoginLabelRegisterError.Visible = false;
            }
        }
        private void LoginEditFieldRegisterPassword_TextBoxTextChanged(object sender, EventArgs e)
        {
            if (LoginLabelRegisterError.Visible)
            {
                LoginLabelRegisterError.Visible = false;
            }
            string password = LoginEditFieldRegisterPassword.TextTextBox;
            if (string.IsNullOrEmpty(password))
            {
                LoginLabelRegisterPasswordStrengthIndicator.Text = "...";
                LoginLabelRegisterPasswordStrengthIndicator.ForeColor = Color.White;
                LoginPasswordStrengthIndicatorRegister.SetIndex(0);
                return;
            }
            Password.Result result = Password.Security.SimpleCheck(password);
            int strength = Array.IndexOf(new string[] { "F", "D-", "D", "D+", "C-", "C", "C+", "B-", "B", "B+", "A-", "A" }, result.Grade);
            LoginPasswordStrengthIndicatorRegister.SetIndex(strength);
            LoginLabelRegisterPasswordStrengthIndicator.ForeColor = LoginPasswordStrengthIndicatorRegister.Colors[strength];
            LoginLabelRegisterPasswordStrengthIndicator.Text = result.Complexity;
        }

        #endregion

        #region SettingsPanel
        #region SettingsLogin
        private async void SettingsAnimatedButtonLoginSubmit_Click(object sender, EventArgs e)
        {
            string ip = SettingsEditFieldLoginIP.TextTextBox;
            string strPort = SettingsEditFieldLoginPort.TextTextBox;
            string username = SettingsEditFieldLoginUsername.TextTextBox;
            string password = SettingsEditFieldLoginPassword.TextTextBox;

            if (new bool[] { await ip.IsValidIp(), strPort.IsValidPort(), username.IsValidUsername() }.Contains(false))
            {
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                CustomException.ThrowNew.FormatException("Please enter your password.");
                return;
            }
            string stage1PasswordHash = CryptoHelper.SHA256Hash(password);
            GlobalVarPool.onlinePassword = CryptoHelper.SHA256Hash(stage1PasswordHash.Substring(0, 32));
            GlobalVarPool.plainMasterPassword = password;


            WindowManager.LoadingScreen.Show(LoadingLabelStatus);
            WindowManager.LoadingScreen.LoadingType = LoadingType.LOGIN;

            AutomatedTaskFramework.Tasks.Clear();
            Action onTaskFailed = new Action(delegate
            {
                WindowManager.LoadingScreen.InvokeHide();
            });
            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "DEVICE_AUTHORIZED", NetworkAdapter.MethodProvider.Connect, onTaskFailed);
            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.In, "ALREADY_LOGGED_IN|LOGIN_SUCCESSFUL", NetworkAdapter.MethodProvider.Login, onTaskFailed);

            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.In, "AD_OUTDATED|AD_UPTODATE", NetworkAdapter.MethodProvider.GetAccountDetails, onTaskFailed);

            Func<bool> syncFinishedWhen = new Func<bool>(delegate
            {
                if (AutomatedTaskFramework.Tasks.InteractiveSubTaskFinished)
                {
                    AutomatedTaskFramework.Tasks.InteractiveSubTaskFinished = false;
                    return true;
                }
                return false;
            });
            AutomatedTaskFramework.Task.Create(TaskType.Interactive, NetworkAdapter.MethodProvider.Sync, syncFinishedWhen, onTaskFailed);

            Action finalizeLogin = new Action(delegate
            {
                SettingsFlowLayoutPanelOnline.BringToFront();
            });
            AutomatedTaskFramework.Task.Create(TaskType.FireAndForget, finalizeLogin);
            AutomatedTaskFramework.Tasks.Execute();
            await DataBaseHelper.ModifyData(DataBaseHelper.Security.SQLInjectionCheckQuery(new string[] { "UPDATE Tbl_settings SET S_server_ip = \"", GlobalVarPool.REMOTE_ADDRESS, "\", S_server_port = \"", GlobalVarPool.REMOTE_PORT.ToString(), "\";" }));
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
            if (string.IsNullOrEmpty(nickname))
            {
                nickname = "User";
            }
            if (new string[] { " ", "\"", "'" }.Any(nickname.Contains))
            {
                CustomException.ThrowNew.FormatException("The nickname may not contain spaces, single or double quotes.");
                return;
            }
            if (new bool[] { await ip.IsValidIp(), strPort.IsValidPort(), username.IsValidUsername(), email.IsValidEmail() }.Contains(false))
            {
                return;
            }
            GlobalVarPool.name = nickname;

            WindowManager.LoadingScreen.LoadingType = LoadingType.REGISTER;
            WindowManager.LoadingScreen.Show(LoadingLabelStatus);

            AutomatedTaskFramework.Tasks.Clear();
            Action onTaskFailed = new Action(delegate
            {
                WindowManager.LoadingScreen.InvokeHide();
            });
            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "DEVICE_AUTHORIZED", NetworkAdapter.MethodProvider.Connect, onTaskFailed);
            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "SEND_VERIFICATION_ACTIVATE_ACCOUNT", NetworkAdapter.MethodProvider.Register, onTaskFailed);
            AutomatedTaskFramework.Tasks.Execute();
            await DataBaseHelper.ModifyData(DataBaseHelper.Security.SQLInjectionCheckQuery(new string[] { "UPDATE Tbl_settings SET S_server_ip = \"", GlobalVarPool.REMOTE_ADDRESS, "\", S_server_port = \"", GlobalVarPool.REMOTE_PORT.ToString(), "\";" }));
        }
        #endregion
        #region SettingsOffline
        private void SettingsAnimatedButtonLogin_Click(object sender, EventArgs e)
        {
            SettingsFlowLayoutPanelLogin.BringToFront();
        }
        private void SettingsAnimatedButtonRegister_Click(object sender, EventArgs e)
        {
            SettingsFlowLayoutPanelRegister.BringToFront();
        }
        private async void SettingsAnimatedButtonChangePasswordSubmit_Click(object sender, EventArgs e)
        {
            // TODO: CHECK PASSWORD STRENGTH
            string password = SettingsEditFieldOfflineNewPassword.TextTextBox;
            string password2 = SettingsEditFieldOfflineNewPasswordConfirm.TextTextBox;
            if (!password.Equals(password2))
            {
                CustomException.ThrowNew.GenericException("These passwords don't match.");
                return;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                CustomException.ThrowNew.GenericException("Please enter a new master password.");
                return;
            }
            WindowManager.LoadingScreen.LoadingType = LoadingType.DEFAULT;
            WindowManager.LoadingScreen.Show(LoadingLabelStatus);
            await HelperMethods.ChangeMasterPassword(password, true);
            SettingsFlowLayoutPanelOffline.BringToFront();
            WindowManager.LoadingScreen.Hide();
        }

        private void SettingsAnimatedButtonOfflineChangeNameSubmit_Click(object sender, EventArgs e)
        {
            string name = GetNameOrDefault(SettingsEditFieldOfflineName.TextTextBox);
            ChangeLocalName(name);
        }

        #endregion
        #region SettingsOnline
        private void SettingsAnimatedButtonOnlinePasswordChangeSubmit_Click(object sender, EventArgs e)
        {
            // TODO: CHECK PASSWORD STRENGTH
            string password = SettingsEditFieldOnlinePasswordChangeNew.TextTextBox;
            string password2 = SettingsEditFieldOnlinePasswordChangeConfirm.TextTextBox;
            if (!password.Equals(password2))
            {
                CustomException.ThrowNew.GenericException("These passwords don't match.");
                return;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                CustomException.ThrowNew.GenericException("Please enter a new master password.");
                return;
            }
            WindowManager.LoadingScreen.LoadingType = LoadingType.PASSWORD_CHANGE;
            GlobalVarPool.plainMasterPassword = password;
            SettingsEditFieldOnlinePasswordChangeNew.TextTextBox = string.Empty;
            SettingsEditFieldOnlinePasswordChangeConfirm.TextTextBox = string.Empty;
            WindowManager.LoadingScreen.Show(LoadingLabelStatus);
            // TODO: Server fallback on failure --> change to old password
            AutomatedTaskFramework.Tasks.Clear();
            Action onTaskFailed = new Action(delegate
            {
                WindowManager.LoadingScreen.InvokeHide();
            });
            if (!GlobalVarPool.connected)
            {
                AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "DEVICE_AUTHORIZED", NetworkAdapter.MethodProvider.Connect, onTaskFailed);
            }
            if (!GlobalVarPool.isUser)
            {
                AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.In, "ALREADY_LOGGED_IN|LOGIN_SUCCESSFUL", NetworkAdapter.MethodProvider.Login, onTaskFailed);
            }
            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "!TASK_TERMINATED_BY_PROMPT!", NetworkAdapter.MethodProvider.InitPasswordChange, onTaskFailed);
            Action changeLocalPassword = new Action(async delegate
            {
                using (Task<List<string>> GetHids = DataBaseHelper.GetDataAsList("SELECT D_hid FROM Tbl_data;", 1))
                {
                    List<string> hids = await GetHids;
                    for (int i = 0; i < hids.Count; i++)
                    {
                        await DataBaseHelper.ModifyData(DataBaseHelper.Security.SQLInjectionCheckQuery(new string[] { "INSERT INTO Tbl_delete (DEL_hid) VALUES (\"", hids[i], "\");" }));
                    }
                }
                await HelperMethods.ChangeMasterPassword(GlobalVarPool.plainMasterPassword, false);
                AutomatedTaskFramework.Tasks.InteractiveSubTaskFinished = true;
            });
            Func<bool> localPasswordChangedWhen = new Func<bool>(delegate 
            {
                if (AutomatedTaskFramework.Tasks.InteractiveSubTaskFinished)
                {
                    AutomatedTaskFramework.Tasks.InteractiveSubTaskFinished = false;
                    return true;
                }
                return false;
            });
            AutomatedTaskFramework.Task.Create(TaskType.Interactive, changeLocalPassword, localPasswordChangedWhen, onTaskFailed);
            Func<bool> syncFinishedWhen = new Func<bool>(delegate
            {
                if (AutomatedTaskFramework.Tasks.InteractiveSubTaskFinished)
                {
                    AutomatedTaskFramework.Tasks.InteractiveSubTaskFinished = false;
                    return true;
                }
                return false;
            });
            AutomatedTaskFramework.Task.Create(TaskType.Interactive, NetworkAdapter.MethodProvider.Sync, syncFinishedWhen, onTaskFailed);
            Action finalizePasswordChange = new Action(delegate
            {
                WindowManager.LoadingScreen.InvokeHide();
            });
            AutomatedTaskFramework.Task.Create(TaskType.FireAndForget, finalizePasswordChange);
            AutomatedTaskFramework.Tasks.Execute();
        }
        private void SettingsAnimatedButtonOnlineChangeName_Click(object sender, EventArgs e)
        {
            string name = GetNameOrDefault(SettingsEditFieldOnlineChangeName.TextTextBox);
            ChangeLocalName(name);
            WindowManager.LoadingScreen.LoadingType = LoadingType.DEFAULT;
            WindowManager.LoadingScreen.Show(LoadingLabelStatus);
            Action onTaskFailed = new Action(delegate
            {
                WindowManager.LoadingScreen.InvokeHide();
            });
            AutomatedTaskFramework.Tasks.Clear();
            if (!GlobalVarPool.connected)
            {
                AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "DEVICE_AUTHORIZED", NetworkAdapter.MethodProvider.Connect, onTaskFailed);
            }
            if (!GlobalVarPool.isUser)
            {
                AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.In, "ALREADY_LOGGED_IN|LOGIN_SUCCESSFUL", NetworkAdapter.MethodProvider.Login, onTaskFailed);
            }
            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "NAME_CHANGED", () => NetworkAdapter.MethodProvider.ChangeName(name), onTaskFailed);
            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.In, "LOGGED_OUT|NOT_LOGGED_IN", NetworkAdapter.MethodProvider.Logout, onTaskFailed);
            AutomatedTaskFramework.Task.Create(TaskType.FireAndForget, NetworkAdapter.MethodProvider.Disconnect);
            // TODO: Create notification "name successfully changed"
            AutomatedTaskFramework.Tasks.Execute();
        }
        #endregion
        private async void ChangeLocalName(string name)
        {
            await DataBaseHelper.ModifyData(DataBaseHelper.Security.SQLInjectionCheckQuery(new string[] { "UPDATE Tbl_user SET U_name = \"", name, "\";" }));
            GlobalVarPool.name = name;
            RefreshSettings();
        }
        private string GetNameOrDefault(string name)
        {
            return string.IsNullOrWhiteSpace(name)? "User" : name;
        }
        private void RefreshSettings()
        {
            if (GlobalVarPool.wasOnline)
            {
                SettingsEditFieldOnlineChangeName.TextTextBox = GlobalVarPool.name;
                SettingsEditFieldRegisterIP.TextTextBox = GlobalVarPool.REMOTE_ADDRESS;
                SettingsEditFieldRegisterPort.TextTextBox = GlobalVarPool.REMOTE_PORT.ToString();
                SettingsEditFieldLoginIP.TextTextBox = GlobalVarPool.REMOTE_ADDRESS;
                SettingsEditFieldLoginPort.TextTextBox = GlobalVarPool.REMOTE_PORT.ToString();
            }
            else
            {
                SettingsEditFieldOfflineName.TextTextBox = GlobalVarPool.name;
            }
        }
        #endregion

        #region FilterPanel
        private string previousTextBoxContent = string.Empty;
        private int previousIndex = 2;
        private bool indexForceChange = false;
        private bool textForceChange = false;

        private void InitFilterPanel()
        {
            FilterAdvancedComboBoxSort.SelectedIndex = 2;
        }
        private async void FilterEditFieldSearch_TextChanged(object sender, EventArgs e)
        {
            if (textForceChange)
            {
                textForceChange = false;
                return;
            }
            if (EditFieldShown)
            {
                if (HasUnsafedChanges())
                {
                    Task<DialogResult> ShowDialog = WindowManager.Overlay.Spawn(this, new ConfirmationForm("There are unsaved changes. Do you really want to discard them?", MessageBoxButtons.YesNo));
                    DialogResult result = await ShowDialog;
                    if (!result.Equals(DialogResult.OK))
                    {
                        textForceChange = true;
                        FilterEditFieldSearch.TextTextBox = previousTextBoxContent;
                        return;
                    }
                }
                EditFieldShown = false;
            }
            string textBoxContent = FilterEditFieldSearch.TextTextBox;
            if (!textBoxContent.Equals(previousTextBoxContent))
            {
                previousTextBoxContent = textBoxContent;
                // Do things
                ApplyFilter(0);
            }
        }
        private async void FilterAdvancedComboBoxSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (indexForceChange)
            {
                indexForceChange = false;
                return;
            }
            if (EditFieldShown)
            {
                // USE LINQ TO CHECK IF THERE ARE UNSAVED CHANGES
                if (HasUnsafedChanges())
                {
                    Task<DialogResult> ShowDialog = WindowManager.Overlay.Spawn(this, new ConfirmationForm("There are unsaved changes. Do you really want to discard them?", MessageBoxButtons.YesNo));
                    DialogResult result = await ShowDialog;
                    if (!result.Equals(DialogResult.OK))
                    {
                        indexForceChange = true;
                        FilterAdvancedComboBoxSort.SelectedIndex = previousIndex;
                        return;
                    }
                }
                EditFieldShown = false;
            }
            int selectedIndex = FilterAdvancedComboBoxSort.SelectedIndex;
            if (GlobalVarPool.UserData.Rows.Count != 0 && selectedIndex != previousIndex)
            {
                previousIndex = selectedIndex;
                ApplyFilter(0);
            }
        }
        private void ApplyFilter(int page)
        {
            GlobalVarPool.FilteredUserData = GlobalVarPool.UserData.Copy();
            string searchTerm = FilterEditFieldSearch.TextTextBox;
            if (!string.IsNullOrWhiteSpace(searchTerm) && !searchTerm.Equals(FilterEditFieldSearch.DefaultValue))
            {
                EnumerableRowCollection<DataRow> searchResults = GlobalVarPool.FilteredUserData.AsEnumerable().Where(row => row["3"].ToString().StartsWith(searchTerm));
                if (searchResults.Any())
                {
                    DataFilterResultsLabelSearchTerm.Text = searchTerm;
                    DataFilterResultsLabelSortTerm.Text = FilterAdvancedComboBoxSort.Items[FilterAdvancedComboBoxSort.SelectedIndex].ToString();
                    DataFilterResultsPanel.BringToFront();
                    GlobalVarPool.FilteredUserData = searchResults.CopyToDataTable();
                }
                else
                {
                    DataFilterLabelSearchTerm.Text = searchTerm;
                    DataFilterPanelNotFound.BringToFront();
                    GlobalVarPool.FilteredUserData = new DataTable();
                }
            }
            else
            {
                DataPanelNoSel.BringToFront();
            }
            if (GlobalVarPool.FilteredUserData.Rows.Count > 0)
            {
                switch (FilterAdvancedComboBoxSort.SelectedIndex)
                {
                    case 0:
                        {
                            GlobalVarPool.FilteredUserData = GlobalVarPool.FilteredUserData.AsEnumerable().OrderBy(row => row["3"]).CopyToDataTable();
                            break;
                        }
                    case 1:
                        {
                            GlobalVarPool.FilteredUserData = GlobalVarPool.FilteredUserData.AsEnumerable().OrderByDescending(row => row["3"]).CopyToDataTable();
                            break;
                        }
                    case 2:
                        {
                            GlobalVarPool.FilteredUserData = GlobalVarPool.FilteredUserData.AsEnumerable().OrderBy(row => row["2"]).CopyToDataTable();
                            break;
                        }
                    case 3:
                        {
                            GlobalVarPool.FilteredUserData = GlobalVarPool.FilteredUserData.AsEnumerable().OrderByDescending(row => row["2"]).CopyToDataTable();
                            break;
                        }
                    default:
                        {
                            GlobalVarPool.FilteredUserData = GlobalVarPool.FilteredUserData.AsEnumerable().OrderBy(row => row["2"]).CopyToDataTable();
                            break;
                        }
                }
            }
            RefreshUserData(page);
        }


        #endregion

        #region Dashboard

        private void DashboardLunaItemDevice_Click(object sender, EventArgs e)
        {
            LunaItem item = (LunaItem)sender;
            _ = WindowManager.Overlay.Spawn(this, new DeviceForm(item.Id));
        }
        private List<Breaches.Breach> breaches;
        private void label16_Click(object sender, EventArgs e)
        {
            UpdateDashboard();
        }

        private void UpdateDashboard()
        {
            UpdateBreaches();
            UpdateStats();
            UpdatePasswordScore();
            UpdateHotspots();
            UpdateDevices();
        }

        private async void UpdateStats()
        {
            await DataBaseHelper.ModifyData("VACUUM");
            Task<string> GetAccountCount = DataBaseHelper.GetSingleOrDefault("SELECT COUNT(1) FROM Tbl_data;");
            string accountCount = await GetAccountCount;
            FileInfo fileInfo = new FileInfo(@"Resources\localdata_windows.db");
            string fileSize = Convert.ToDouble(fileInfo.Length - 36864).ToHumanReadableFileSize(1);
            DashboardLabelAccountNumber.Text = accountCount;
            if (accountCount.Equals("1"))
            {
                DashboardLabelAccountsTotal.Text = "account total";
            }
            else
            {
                DashboardLabelAccountsTotal.Text = "accounts total";
            }
            DashboardLabelDiskSpaceValue.Text = fileSize;
        }

        private void UpdateDevices()
        {
            if (!GlobalVarPool.wasOnline)
            {
                SettingsTableLayoutPanelMain.BringToFront();
                SettingsFlowLayoutPanelRegister.BringToFront();
                return;
            }
            AutomatedTaskFramework.Tasks.Clear();
            if (!GlobalVarPool.connected)
            {
                AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "DEVICE_AUTHORIZED", NetworkAdapter.MethodProvider.Connect);
            }
            if (!GlobalVarPool.isUser)
            {
                AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.In, "ALREADY_LOGGED_IN|LOGIN_SUCCESSFUL", NetworkAdapter.MethodProvider.Login);
            }
            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "DTADEVdata", NetworkAdapter.MethodProvider.GetDevices);
            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.In, "LOGGED_OUT|NOT_LOGGED_IN", NetworkAdapter.MethodProvider.Logout);
            AutomatedTaskFramework.Task.Create(TaskType.FireAndForget, NetworkAdapter.MethodProvider.Disconnect);
            AutomatedTaskFramework.Tasks.Execute();
        }

        private async void DashboardLunaAnimatedButtonLogoutAll_Click(object sender, EventArgs e)
        {
            Task<DialogResult> ShowDialog = WindowManager.Overlay.Spawn(this, new ConfirmationForm("This will log you out from all devices. You'll have to confirm each new device with an email again.", MessageBoxButtons.OKCancel));
            DialogResult result = await ShowDialog;
            if (result != DialogResult.OK)
            {
                return;
            }
            DashboardLunaAnimatedButtonLogoutAll.Enabled = false;
            AutomatedTaskFramework.Tasks.Clear();
            if (!GlobalVarPool.connected)
            {
                AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "DEVICE_AUTHORIZED", NetworkAdapter.MethodProvider.Connect);
            }
            if (!GlobalVarPool.isUser)
            {
                AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.In, "ALREADY_LOGGED_IN|LOGIN_SUCCESSFUL", NetworkAdapter.MethodProvider.Login);
            }
            List<OSInfo.Device> devices = new List<OSInfo.Device>();
            for (int i = 0; i < DashboardLunaItemListDevices.Items.Count; i++)
            {
                devices.Add(JsonConvert.DeserializeObject<OSInfo.Device>(DashboardLunaItemListDevices.Items[i].Id));
            }
            if (devices.Count == 0)
            {
                AutomatedTaskFramework.Tasks.Clear();
                DashboardLunaAnimatedButtonLogoutAll.Enabled = true;
                return;
            }
            for (int i = 0; i < devices.Count; i++)
            {
                string cookie = devices[i].DeviceId;
                if (!cookie.Equals(GlobalVarPool.cookie))
                {
                    AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "UNLINK_SUCCESSFUL", () => NetworkAdapter.MethodProvider.RemoveDevice(cookie));
                }
            }
            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.Contains, "DTADEVdata%eq", NetworkAdapter.MethodProvider.GetDevices);
            AutomatedTaskFramework.Task.Create(TaskType.NetworkTask, SearchCondition.In, "LOGGED_OUT|NOT_LOGGED_IN", NetworkAdapter.MethodProvider.Logout);
            AutomatedTaskFramework.Task.Create(TaskType.Interactive, NetworkAdapter.MethodProvider.Disconnect, new Func<bool>(delegate
            {
                return !PDTPClient.ThreadRunning;
            }));
            AutomatedTaskFramework.Task.Create(TaskType.FireAndForget, new Action(delegate ()
            {
                DashboardLunaAnimatedButtonLogoutAll.Invoke((MethodInvoker)delegate
                {
                    DashboardLunaAnimatedButtonLogoutAll.Enabled = true;
                });
            }));
            DashboardLunaItemListDevices.RemoveAll();
            AutomatedTaskFramework.Tasks.Execute();
        }

        private void UpdateHotspots()
        {
            DashboardLunaSmallCardListHotspots.RemoveAll();
            List<Password.SimplifiedResult> simplifiedResults = new List<Password.SimplifiedResult>();
            DataRowCollection rows = GlobalVarPool.UserData.Rows;
            for (int i = 0; i < rows.Count; i++)
            {
                DataRow row = rows[i];
                simplifiedResults.Add(Password.SimplifiedResult.Create(row["0"].ToString(), Convert.ToInt32(row["10"].ToString()), row["3"].ToString()));
            }
            simplifiedResults.Sort((x, y) => x.Score.CompareTo(y.Score));
            for (int i = 0; i < simplifiedResults.Count; i++)
            {
                if (i > 2)
                {
                    break;
                }
                Password.Result result = simplifiedResults[i].ToResult();

                if (result.Score <= Password.Results.Okay)
                {
                    if (i == 0)
                    {
                        DashboardPanelHotspotsZero.SendToBack();
                        DashboardLunaSmallCardListHotspots.BringToFront();
                    }
                    DashboardLunaSmallCardListHotspots.Add(simplifiedResults[i].Host, result.Icon, "Score: " + result.Score, simplifiedResults[i].Id, -1);
                }
                else
                {
                    if (i == 0)
                    {
                        DashboardLunaSmallCardListHotspots.SendToBack();
                        DashboardPanelHotspotsZero.BringToFront();
                    }
                    break;
                }
            }
        }

        private void DashboardLunaSmallCardListHotspots_CardClicked(object sender, EventArgs e)
        {
            LunaSmallCardItem hotspot = (LunaSmallCardItem)sender;
            string id = hotspot.Id;
            DataRow LinkedRow = GlobalVarPool.UserData.AsEnumerable().SingleOrDefault(r => r.Field<string>("0").Equals(id));
            UpdateDetailsWindow(LinkedRow);
            DataPanelDetails.BringToFront();
            MenuMenuEntryPasswords_Click(null, null);
        }

        private async void UpdatePasswordScore()
        {
            int accountCount = GlobalVarPool.UserData.Rows.Count;
            int totalScore = 0;
            for (int i = 0; i < accountCount; i++)
            {
                totalScore += Convert.ToInt32(GlobalVarPool.UserData.Rows[i]["10"].ToString());
            }
            double avgScore = Math.Round(totalScore / (double)accountCount, 2, MidpointRounding.AwayFromZero);
            Task<string> GetOldAverageScore = DataBaseHelper.GetSingleOrDefault("SELECT M_avgScoreOld FROM Tbl_meta LIMIT 1;");
            string avgScoreOldString = await GetOldAverageScore;
            double avgScoreOld = Convert.ToDouble(avgScoreOldString);
            double deltaScore = avgScore - avgScoreOld;
            await DataBaseHelper.ModifyData("UPDATE Tbl_meta SET M_avgScoreOld = \"" + avgScore.ToString() + "\";");
            DashboardLabelPasswordStrength.Text = avgScore.ToString();
            DashboardLabelPasswordStrengthChange.Text = Math.Round(deltaScore, 2, MidpointRounding.AwayFromZero).ToString();
            Password.Result result = Password.Result.FromScore((int)Math.Round(avgScore, 0, MidpointRounding.AwayFromZero));
            DashboardLunaSmallCardPasswordStrength.Image = result.Icon;
            DashboardLunaSmallCardPasswordStrength.Header = result.Complexity;
            if (deltaScore < 0)
            {
                DashboardLabelPasswordStrengthChange.ForeColor = Colors.Red;
                DashboardLunaTrianglePasswordStrength.ForeColor = Colors.Red;
                DashboardLunaTrianglePasswordStrength.TriangleDirection = LunaTriangle.Direction.Down;
            }
            else
            {
                DashboardLabelPasswordStrengthChange.ForeColor = Colors.Green;
                DashboardLunaTrianglePasswordStrength.ForeColor = Colors.Green;
                DashboardLunaTrianglePasswordStrength.TriangleDirection = LunaTriangle.Direction.Top;
            }
        }

        private async void UpdateBreaches()
        {
            DashboardLunaItemListBreaches.RemoveAll();
            List<string> domains = new List<string>();
            int rowCount = GlobalVarPool.UserData.Rows.Count;
            for (int i = 0; i < rowCount; i++)
            {
                string domain = HttpHelper.GetDomain(GlobalVarPool.UserData.Rows[i]["6"].ToString());
                if (domain != null)
                {
                    domains.Add(domain);
                }
            }
            try
            {
                Task<List<Breaches.Breach>> GetBreaches = Breaches.FetchAllAsync();
                breaches = await GetBreaches;
            }
            catch (Exception ex)
            {
                CustomException.ThrowNew.NetworkException(ex.ToString());
                return;
            }
            if (breaches == null)
            {
                // TODO: EMBED ERROR MESSAGE INTO UI
                CustomException.ThrowNew.NetworkException("Connection failure.");
                return;
            }
            Task<List<string>> GetIgnoredBreaches = DataBaseHelper.GetDataAsList("SELECT B_hash FROM Tbl_breaches;", (int)ColumnCount.SingleColumn);
            List<string> ignoredBreaches = await GetIgnoredBreaches;
            int index = 0;
            for (int i = 0; i < breaches.Count; i++)
            {
                Breaches.Breach breach = breaches[i];
                if (domains.Contains(breach.Domain))
                {
                    string hash = CryptoHelper.SHA256HashBase64(breach.Name + breach.BreachDate);
                    if (!ignoredBreaches.Contains(hash))
                    {
                        DashboardLunaItemListBreaches.Add(breach.Title, Resources.exclamation_mark, breach.BreachDate, hash, i, index);
                        index++;
                    }
                }
            }
        }

        private async void Breach_Clicked(object sender, EventArgs e)
        {
            LunaItem item = (LunaItem)sender;
            Breaches.Breach breach = breaches[item.Index];
            Task<IconExtractor.Icon> GetIcon = IconExtractor.DownloadImage(breach.LogoPath);
            IconExtractor.Icon icon = await GetIcon;
            Task<DialogResult> ShowDialog = WindowManager.Overlay.Spawn(this, new BreachForm(breach.BreachDate, breach.Title, icon.Image ?? Resources.breach, breach.DataClasses, breach.Description, breach.PwnCount, breach.IsVerified, breach.Domain));
            DialogResult result = await ShowDialog;
            if (result.Equals(DialogResult.Ignore))
            {
                await DataBaseHelper.ModifyData("INSERT INTO Tbl_breaches (B_hash) VALUES (\"" + item.Id + "\");");
                DashboardLunaItemListBreaches.RemoveAt(item.Index2);
            }
        }
        #endregion
        #region PRIVATE METHODS
        private void UpdatePasswordStrength(string password, Label passwordStrengthlabel, PasswordStrengthIndicator passwordStrengthIndicator)
        {
            if (string.IsNullOrEmpty(password))
            {
                passwordStrengthlabel.Text = "Too short";
                passwordStrengthIndicator.SetIndex(0);
                return;
            }
            Password.Result result = Password.Security.SimpleCheck(password);
            int strength = Array.IndexOf(new string[] { "F", "D-", "D", "D+", "C-", "C", "C+", "B-", "B", "B+", "A-", "A" }, result.Grade);
            passwordStrengthIndicator.SetIndex(strength);
            passwordStrengthlabel.Text = result.Complexity;
        }

        private void InitializeDataList()
        {
            for (int i = 0; i < DataPerPage; i++)
            {
                ListEntry listEntry = new ListEntry
                {
                    BackColor = Color.White,
                    HostNameFont = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0),
                    HostNameForeColor = SystemColors.ControlText,
                    Name = "listEntry",
                    Size = new Size(1041, 75),
                    TabIndex = 14,
                    TimeStampFont = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0),
                    TimeStampForeColor = SystemColors.ControlText,
                    UserNameFont = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0),
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
                InitFilterPanel();
            });
            GlobalVarPool.uiInitialized = true;
        }

        private void FinalizeLogin()
        {
            Invoke((MethodInvoker)delegate
            {
                DataFlowLayoutPanelList.ResumeLayout();
                FlowLayoutPanel1_Resize(this, null);
                LoginPictureBoxOnlineMain.Dispose();
                LoginPictureBoxOfflineMain.Dispose();
                LoginPictureBoxRegisterMain.Dispose();
                LoginPictureBoxOnlineSettings.Dispose();
                PanelMain.BringToFront();
                WindowManager.LoadingScreen.Hide();
                PanelLogin.Dispose();
                HelperMethods.CollectGarbage();
            });
        }

        private void InitializeUI()
        {
            InitializeDataList();
            FinalizeLogin();
            Invoke((MethodInvoker)delegate
            {
                ApplyFilter(0);
            });
        }

        private void InitializeLoadingScreen()
        {
            // TODO: Add AbortButton (currently null)
            WindowManager.LoadingScreen.ControlCollection loadingScreenControls = new WindowManager.LoadingScreen.ControlCollection(PanelLoading, LoadingLunaProgressSpinnerFading, null, this);
            WindowManager.LoadingScreen.Initialize(loadingScreenControls);
        }

        private bool HasUnsafedChanges()
        {
            // USE LINQ TO CHECK IF THERE ARE UNSAVED CHANGES
            return !GlobalVarPool.UserData.AsEnumerable().Where(row => row["0"].ToString().Equals(DataDetailsID)).Where(row => (row["3"].ToString().Equals(DataEditEditFieldHostname.TextTextBox) || (row["3"].ToString().Equals("\x01") && string.IsNullOrEmpty(DataEditEditFieldHostname.TextTextBox))) && (row["4"].ToString().Equals(DataEditEditFieldUsername.TextTextBox) || (row["4"].ToString().Equals("\x01") && string.IsNullOrEmpty(DataEditEditFieldUsername.TextTextBox))) && (row["5"].ToString().Equals(DataEditEditFieldPassword.TextTextBox) || (row["5"].ToString().Equals("\x01") && string.IsNullOrEmpty(DataEditEditFieldPassword.TextTextBox))) && (row["6"].ToString().Equals(DataEditEditFieldWebsite.TextTextBox) || (row["6"].ToString().Equals("\x01") && string.IsNullOrEmpty(DataEditEditFieldWebsite.TextTextBox))) && (row["7"].ToString().Equals(DataEditEditFieldEmail.TextTextBox) || row["7"].ToString().Equals("\x01") && (string.IsNullOrEmpty(DataEditEditFieldEmail.TextTextBox))) && (row["8"].ToString().Equals(DataEditAdvancedRichTextBoxNotes.TextValue) || (row["8"].ToString().Equals("\x01") && string.IsNullOrEmpty(DataEditAdvancedRichTextBoxNotes.TextValue)))).Any();
        }
        #endregion

        private void LoginLabelOnlineSettingsRegister_Click(object sender, EventArgs e)
        {
            LoginPictureBoxOnlineSettings.SuspendLayout();
            LoginPictureBoxRegisterMain.BringToFront();
            LoginPictureBoxRegisterMain.ResumeLayout();
        }

        private async void LoginLunaAnimatedButtonNext_Click(object sender, EventArgs e)
        {
            LoginLunaAnimatedButtonNext.Enabled = false;
            string ip = LoginEditFieldOnlineSettingsIp.TextTextBox;
            string strPort = LoginEditFieldOnlineSettingsPort.TextTextBox;
            if (new bool[] { await ip.IsValidIp(), strPort.IsValidPort() }.Contains(false))
            {
                LoginLunaAnimatedButtonNext.Enabled = true;
                return;
            }
            LoginLunaAnimatedButtonNext.Enabled = true;
            LoginPictureBoxOnlineMain.BringToFront();
        }
    }
}
