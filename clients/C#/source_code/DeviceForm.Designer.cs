namespace pmdbs
{
    partial class DeviceForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeviceForm));
            this.panelMain = new System.Windows.Forms.Panel();
            this.lunaAnimatedButtonLogout = new LunaForms.LunaAnimatedButton();
            this.panelNetworkInformation = new System.Windows.Forms.Panel();
            this.labelNetworkInformationHeader = new System.Windows.Forms.Label();
            this.labelIp = new System.Windows.Forms.Label();
            this.labelLastSeenHeader = new System.Windows.Forms.Label();
            this.labelLastSeen = new System.Windows.Forms.Label();
            this.labelIpHeader = new System.Windows.Forms.Label();
            this.panelHardwareInformation = new System.Windows.Forms.Panel();
            this.labelMemory = new System.Windows.Forms.Label();
            this.labelMemoryHeader = new System.Windows.Forms.Label();
            this.labelProcessor = new System.Windows.Forms.Label();
            this.labelProcessorHeader = new System.Windows.Forms.Label();
            this.labelHardwareInformationHeader = new System.Windows.Forms.Label();
            this.panelOsInformation = new System.Windows.Forms.Panel();
            this.labelOsInformationHeader = new System.Windows.Forms.Label();
            this.labelArchitecture = new System.Windows.Forms.Label();
            this.labelArchitectureHeader = new System.Windows.Forms.Label();
            this.labelUsername = new System.Windows.Forms.Label();
            this.labelUsernameHeader = new System.Windows.Forms.Label();
            this.labelServicePack = new System.Windows.Forms.Label();
            this.labelServicePackHeader = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelVersionHeader = new System.Windows.Forms.Label();
            this.labelDeviceName = new System.Windows.Forms.Label();
            this.labelDeviceNameHeader = new System.Windows.Forms.Label();
            this.labelEdition = new System.Windows.Forms.Label();
            this.labelEditionHeader = new System.Windows.Forms.Label();
            this.labelDeviceId = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lunaSmallCardIsOnline = new LunaForms.LunaSmallCard();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.WindowHeaderLabelLogo = new System.Windows.Forms.Label();
            this.windowButtonMinimize = new LunaForms.WindowButton();
            this.windowButtonClose = new LunaForms.WindowButton();
            this.panelMain.SuspendLayout();
            this.panelNetworkInformation.SuspendLayout();
            this.panelHardwareInformation.SuspendLayout();
            this.panelOsInformation.SuspendLayout();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.White;
            this.panelMain.Controls.Add(this.lunaAnimatedButtonLogout);
            this.panelMain.Controls.Add(this.panelNetworkInformation);
            this.panelMain.Controls.Add(this.panelHardwareInformation);
            this.panelMain.Controls.Add(this.panelOsInformation);
            this.panelMain.Controls.Add(this.labelDeviceId);
            this.panelMain.Controls.Add(this.labelTitle);
            this.panelMain.Controls.Add(this.panelHeader);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panelMain.Location = new System.Drawing.Point(20, 60);
            this.panelMain.Margin = new System.Windows.Forms.Padding(0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1160, 944);
            this.panelMain.TabIndex = 0;
            // 
            // lunaAnimatedButtonLogout
            // 
            this.lunaAnimatedButtonLogout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.lunaAnimatedButtonLogout.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(96)))), ((int)(((byte)(49)))));
            this.lunaAnimatedButtonLogout.Depth = 0;
            this.lunaAnimatedButtonLogout.Icon = null;
            this.lunaAnimatedButtonLogout.Location = new System.Drawing.Point(427, 863);
            this.lunaAnimatedButtonLogout.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.lunaAnimatedButtonLogout.MouseState = LunaForms.LunaAnimatedButton.MouseStateBase.HOVER;
            this.lunaAnimatedButtonLogout.Name = "lunaAnimatedButtonLogout";
            this.lunaAnimatedButtonLogout.Primary = false;
            this.lunaAnimatedButtonLogout.Size = new System.Drawing.Size(307, 66);
            this.lunaAnimatedButtonLogout.TabIndex = 31;
            this.lunaAnimatedButtonLogout.Text = "Logout from this device";
            this.lunaAnimatedButtonLogout.Uppercase = false;
            this.lunaAnimatedButtonLogout.UseVisualStyleBackColor = true;
            this.lunaAnimatedButtonLogout.Click += new System.EventHandler(this.lunaAnimatedButtonLogout_Click);
            // 
            // panelNetworkInformation
            // 
            this.panelNetworkInformation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelNetworkInformation.Controls.Add(this.labelNetworkInformationHeader);
            this.panelNetworkInformation.Controls.Add(this.labelIp);
            this.panelNetworkInformation.Controls.Add(this.labelLastSeenHeader);
            this.panelNetworkInformation.Controls.Add(this.labelLastSeen);
            this.panelNetworkInformation.Controls.Add(this.labelIpHeader);
            this.panelNetworkInformation.Location = new System.Drawing.Point(80, 714);
            this.panelNetworkInformation.Name = "panelNetworkInformation";
            this.panelNetworkInformation.Size = new System.Drawing.Size(1000, 140);
            this.panelNetworkInformation.TabIndex = 30;
            // 
            // labelNetworkInformationHeader
            // 
            this.labelNetworkInformationHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelNetworkInformationHeader.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.labelNetworkInformationHeader.Location = new System.Drawing.Point(0, 0);
            this.labelNetworkInformationHeader.Name = "labelNetworkInformationHeader";
            this.labelNetworkInformationHeader.Size = new System.Drawing.Size(998, 60);
            this.labelNetworkInformationHeader.TabIndex = 32;
            this.labelNetworkInformationHeader.Text = "Network Information";
            this.labelNetworkInformationHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelIp
            // 
            this.labelIp.AutoSize = true;
            this.labelIp.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.labelIp.Location = new System.Drawing.Point(651, 92);
            this.labelIp.MaximumSize = new System.Drawing.Size(500, 0);
            this.labelIp.Name = "labelIp";
            this.labelIp.Size = new System.Drawing.Size(173, 32);
            this.labelIp.TabIndex = 25;
            this.labelIp.Text = "192.168.178.21";
            // 
            // labelLastSeenHeader
            // 
            this.labelLastSeenHeader.AutoSize = true;
            this.labelLastSeenHeader.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.labelLastSeenHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelLastSeenHeader.Location = new System.Drawing.Point(35, 60);
            this.labelLastSeenHeader.Name = "labelLastSeenHeader";
            this.labelLastSeenHeader.Size = new System.Drawing.Size(97, 28);
            this.labelLastSeenHeader.TabIndex = 20;
            this.labelLastSeenHeader.Text = "Last Seen:";
            // 
            // labelLastSeen
            // 
            this.labelLastSeen.AutoSize = true;
            this.labelLastSeen.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.labelLastSeen.Location = new System.Drawing.Point(34, 92);
            this.labelLastSeen.MaximumSize = new System.Drawing.Size(500, 0);
            this.labelLastSeen.Name = "labelLastSeen";
            this.labelLastSeen.Size = new System.Drawing.Size(234, 32);
            this.labelLastSeen.TabIndex = 21;
            this.labelLastSeen.Text = "2019-09-13 11:01:35";
            // 
            // labelIpHeader
            // 
            this.labelIpHeader.AutoSize = true;
            this.labelIpHeader.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.labelIpHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelIpHeader.Location = new System.Drawing.Point(652, 60);
            this.labelIpHeader.Name = "labelIpHeader";
            this.labelIpHeader.Size = new System.Drawing.Size(200, 28);
            this.labelIpHeader.TabIndex = 24;
            this.labelIpHeader.Text = "Last Connected From:";
            // 
            // panelHardwareInformation
            // 
            this.panelHardwareInformation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelHardwareInformation.Controls.Add(this.labelMemory);
            this.panelHardwareInformation.Controls.Add(this.labelMemoryHeader);
            this.panelHardwareInformation.Controls.Add(this.labelProcessor);
            this.panelHardwareInformation.Controls.Add(this.labelProcessorHeader);
            this.panelHardwareInformation.Controls.Add(this.labelHardwareInformationHeader);
            this.panelHardwareInformation.Location = new System.Drawing.Point(80, 568);
            this.panelHardwareInformation.Name = "panelHardwareInformation";
            this.panelHardwareInformation.Size = new System.Drawing.Size(1000, 140);
            this.panelHardwareInformation.TabIndex = 29;
            // 
            // labelMemory
            // 
            this.labelMemory.AutoSize = true;
            this.labelMemory.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.labelMemory.Location = new System.Drawing.Point(651, 90);
            this.labelMemory.MaximumSize = new System.Drawing.Size(500, 0);
            this.labelMemory.Name = "labelMemory";
            this.labelMemory.Size = new System.Drawing.Size(179, 32);
            this.labelMemory.TabIndex = 36;
            this.labelMemory.Text = "34308390912 B";
            // 
            // labelMemoryHeader
            // 
            this.labelMemoryHeader.AutoSize = true;
            this.labelMemoryHeader.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.labelMemoryHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelMemoryHeader.Location = new System.Drawing.Point(652, 58);
            this.labelMemoryHeader.Name = "labelMemoryHeader";
            this.labelMemoryHeader.Size = new System.Drawing.Size(202, 28);
            this.labelMemoryHeader.TabIndex = 35;
            this.labelMemoryHeader.Text = "Total Pysical Memory:";
            // 
            // labelProcessor
            // 
            this.labelProcessor.AutoSize = true;
            this.labelProcessor.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.labelProcessor.Location = new System.Drawing.Point(34, 90);
            this.labelProcessor.MaximumSize = new System.Drawing.Size(750, 0);
            this.labelProcessor.Name = "labelProcessor";
            this.labelProcessor.Size = new System.Drawing.Size(482, 32);
            this.labelProcessor.TabIndex = 34;
            this.labelProcessor.Text = "Intel(R) Xeon(R) CPU E3-1230 v3 @ 3.30GHz";
            // 
            // labelProcessorHeader
            // 
            this.labelProcessorHeader.AutoSize = true;
            this.labelProcessorHeader.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.labelProcessorHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelProcessorHeader.Location = new System.Drawing.Point(35, 58);
            this.labelProcessorHeader.Name = "labelProcessorHeader";
            this.labelProcessorHeader.Size = new System.Drawing.Size(100, 28);
            this.labelProcessorHeader.TabIndex = 33;
            this.labelProcessorHeader.Text = "Processor:";
            // 
            // labelHardwareInformationHeader
            // 
            this.labelHardwareInformationHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelHardwareInformationHeader.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.labelHardwareInformationHeader.Location = new System.Drawing.Point(0, 0);
            this.labelHardwareInformationHeader.Name = "labelHardwareInformationHeader";
            this.labelHardwareInformationHeader.Size = new System.Drawing.Size(998, 60);
            this.labelHardwareInformationHeader.TabIndex = 32;
            this.labelHardwareInformationHeader.Text = "Hardware Information";
            this.labelHardwareInformationHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelOsInformation
            // 
            this.panelOsInformation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelOsInformation.Controls.Add(this.labelOsInformationHeader);
            this.panelOsInformation.Controls.Add(this.labelArchitecture);
            this.panelOsInformation.Controls.Add(this.labelArchitectureHeader);
            this.panelOsInformation.Controls.Add(this.labelUsername);
            this.panelOsInformation.Controls.Add(this.labelUsernameHeader);
            this.panelOsInformation.Controls.Add(this.labelServicePack);
            this.panelOsInformation.Controls.Add(this.labelServicePackHeader);
            this.panelOsInformation.Controls.Add(this.labelVersion);
            this.panelOsInformation.Controls.Add(this.labelVersionHeader);
            this.panelOsInformation.Controls.Add(this.labelDeviceName);
            this.panelOsInformation.Controls.Add(this.labelDeviceNameHeader);
            this.panelOsInformation.Controls.Add(this.labelEdition);
            this.panelOsInformation.Controls.Add(this.labelEditionHeader);
            this.panelOsInformation.Location = new System.Drawing.Point(80, 260);
            this.panelOsInformation.Name = "panelOsInformation";
            this.panelOsInformation.Size = new System.Drawing.Size(1000, 302);
            this.panelOsInformation.TabIndex = 28;
            // 
            // labelOsInformationHeader
            // 
            this.labelOsInformationHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelOsInformationHeader.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.labelOsInformationHeader.Location = new System.Drawing.Point(0, 0);
            this.labelOsInformationHeader.Name = "labelOsInformationHeader";
            this.labelOsInformationHeader.Size = new System.Drawing.Size(998, 61);
            this.labelOsInformationHeader.TabIndex = 32;
            this.labelOsInformationHeader.Text = "OS Information";
            this.labelOsInformationHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelArchitecture
            // 
            this.labelArchitecture.AutoSize = true;
            this.labelArchitecture.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.labelArchitecture.Location = new System.Drawing.Point(651, 168);
            this.labelArchitecture.MaximumSize = new System.Drawing.Size(500, 0);
            this.labelArchitecture.Name = "labelArchitecture";
            this.labelArchitecture.Size = new System.Drawing.Size(79, 32);
            this.labelArchitecture.TabIndex = 31;
            this.labelArchitecture.Text = "64-Bit";
            // 
            // labelArchitectureHeader
            // 
            this.labelArchitectureHeader.AutoSize = true;
            this.labelArchitectureHeader.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.labelArchitectureHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelArchitectureHeader.Location = new System.Drawing.Point(652, 136);
            this.labelArchitectureHeader.Name = "labelArchitectureHeader";
            this.labelArchitectureHeader.Size = new System.Drawing.Size(153, 28);
            this.labelArchitectureHeader.TabIndex = 30;
            this.labelArchitectureHeader.Text = "OS Architecture:";
            // 
            // labelUsername
            // 
            this.labelUsername.AutoSize = true;
            this.labelUsername.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.labelUsername.Location = new System.Drawing.Point(651, 244);
            this.labelUsername.MaximumSize = new System.Drawing.Size(500, 0);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.Size = new System.Drawing.Size(230, 32);
            this.labelUsername.TabIndex = 29;
            this.labelUsername.Text = "XEON\\Administrator";
            // 
            // labelUsernameHeader
            // 
            this.labelUsernameHeader.AutoSize = true;
            this.labelUsernameHeader.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.labelUsernameHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelUsernameHeader.Location = new System.Drawing.Point(652, 212);
            this.labelUsernameHeader.Name = "labelUsernameHeader";
            this.labelUsernameHeader.Size = new System.Drawing.Size(103, 28);
            this.labelUsernameHeader.TabIndex = 28;
            this.labelUsernameHeader.Text = "Username:";
            // 
            // labelServicePack
            // 
            this.labelServicePack.AutoSize = true;
            this.labelServicePack.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.labelServicePack.Location = new System.Drawing.Point(651, 93);
            this.labelServicePack.MaximumSize = new System.Drawing.Size(500, 0);
            this.labelServicePack.Name = "labelServicePack";
            this.labelServicePack.Size = new System.Drawing.Size(166, 32);
            this.labelServicePack.TabIndex = 27;
            this.labelServicePack.Text = "Service Pack 1";
            // 
            // labelServicePackHeader
            // 
            this.labelServicePackHeader.AutoSize = true;
            this.labelServicePackHeader.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.labelServicePackHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelServicePackHeader.Location = new System.Drawing.Point(652, 61);
            this.labelServicePackHeader.Name = "labelServicePackHeader";
            this.labelServicePackHeader.Size = new System.Drawing.Size(123, 28);
            this.labelServicePackHeader.TabIndex = 26;
            this.labelServicePackHeader.Text = "Service Pack:";
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.labelVersion.Location = new System.Drawing.Point(34, 168);
            this.labelVersion.MaximumSize = new System.Drawing.Size(500, 0);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(103, 32);
            this.labelVersion.TabIndex = 25;
            this.labelVersion.Text = "6.1.7601";
            // 
            // labelVersionHeader
            // 
            this.labelVersionHeader.AutoSize = true;
            this.labelVersionHeader.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.labelVersionHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelVersionHeader.Location = new System.Drawing.Point(35, 136);
            this.labelVersionHeader.Name = "labelVersionHeader";
            this.labelVersionHeader.Size = new System.Drawing.Size(81, 28);
            this.labelVersionHeader.TabIndex = 24;
            this.labelVersionHeader.Text = "Version:";
            // 
            // labelDeviceName
            // 
            this.labelDeviceName.AutoSize = true;
            this.labelDeviceName.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.labelDeviceName.Location = new System.Drawing.Point(34, 244);
            this.labelDeviceName.MaximumSize = new System.Drawing.Size(500, 0);
            this.labelDeviceName.Name = "labelDeviceName";
            this.labelDeviceName.Size = new System.Drawing.Size(77, 32);
            this.labelDeviceName.TabIndex = 23;
            this.labelDeviceName.Text = "XEON";
            // 
            // labelDeviceNameHeader
            // 
            this.labelDeviceNameHeader.AutoSize = true;
            this.labelDeviceNameHeader.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.labelDeviceNameHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelDeviceNameHeader.Location = new System.Drawing.Point(35, 212);
            this.labelDeviceNameHeader.Name = "labelDeviceNameHeader";
            this.labelDeviceNameHeader.Size = new System.Drawing.Size(131, 28);
            this.labelDeviceNameHeader.TabIndex = 22;
            this.labelDeviceNameHeader.Text = "Device Name:";
            // 
            // labelEdition
            // 
            this.labelEdition.AutoSize = true;
            this.labelEdition.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.labelEdition.Location = new System.Drawing.Point(34, 93);
            this.labelEdition.MaximumSize = new System.Drawing.Size(500, 0);
            this.labelEdition.Name = "labelEdition";
            this.labelEdition.Size = new System.Drawing.Size(143, 32);
            this.labelEdition.TabIndex = 21;
            this.labelEdition.Text = "Professional";
            // 
            // labelEditionHeader
            // 
            this.labelEditionHeader.AutoSize = true;
            this.labelEditionHeader.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.labelEditionHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelEditionHeader.Location = new System.Drawing.Point(35, 61);
            this.labelEditionHeader.Name = "labelEditionHeader";
            this.labelEditionHeader.Size = new System.Drawing.Size(78, 28);
            this.labelEditionHeader.TabIndex = 20;
            this.labelEditionHeader.Text = "Edition:";
            // 
            // labelDeviceId
            // 
            this.labelDeviceId.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelDeviceId.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.labelDeviceId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelDeviceId.Location = new System.Drawing.Point(0, 215);
            this.labelDeviceId.Name = "labelDeviceId";
            this.labelDeviceId.Size = new System.Drawing.Size(1160, 42);
            this.labelDeviceId.TabIndex = 9;
            this.labelDeviceId.Text = "Device ID: O32TCYHJ6asG9uFQ1EqbEQz5Ikg2Fwf0/BqWQWnnhBM35eKHU2SJSVaoOzfsk7o9Iuuzhq" +
    "0LvgX0jwfTXEeiOw==";
            this.labelDeviceId.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelTitle
            // 
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 25F);
            this.labelTitle.Location = new System.Drawing.Point(0, 150);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(1160, 65);
            this.labelTitle.TabIndex = 2;
            this.labelTitle.Text = "Windows 7";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.panelHeader.Controls.Add(this.lunaSmallCardIsOnline);
            this.panelHeader.Controls.Add(this.pictureBoxLogo);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1160, 150);
            this.panelHeader.TabIndex = 1;
            // 
            // lunaSmallCardIsOnline
            // 
            this.lunaSmallCardIsOnline.BackColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.lunaSmallCardIsOnline.BackColorImage = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.lunaSmallCardIsOnline.BackColorNormal = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.lunaSmallCardIsOnline.ForeColorHeader = System.Drawing.Color.White;
            this.lunaSmallCardIsOnline.ForeColorHeaderHover = System.Drawing.Color.White;
            this.lunaSmallCardIsOnline.ForeColorInfo = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lunaSmallCardIsOnline.ForeColorInfoHover = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lunaSmallCardIsOnline.Header = "Online";
            this.lunaSmallCardIsOnline.HeaderLocation = new System.Drawing.Point(70, 10);
            this.lunaSmallCardIsOnline.Image = global::pmdbs.Properties.Resources.confirmed2;
            this.lunaSmallCardIsOnline.ImageLocation = new System.Drawing.Point(1, 1);
            this.lunaSmallCardIsOnline.InfoLocation = new System.Drawing.Point(72, 35);
            this.lunaSmallCardIsOnline.Location = new System.Drawing.Point(954, 87);
            this.lunaSmallCardIsOnline.Margin = new System.Windows.Forms.Padding(0);
            this.lunaSmallCardIsOnline.Name = "lunaSmallCardIsOnline";
            this.lunaSmallCardIsOnline.ShowBorder = false;
            this.lunaSmallCardIsOnline.Size = new System.Drawing.Size(206, 60);
            this.lunaSmallCardIsOnline.TabIndex = 14;
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxLogo.Image")));
            this.pictureBoxLogo.Location = new System.Drawing.Point(518, 3);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(125, 125);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxLogo.TabIndex = 0;
            this.pictureBoxLogo.TabStop = false;
            // 
            // WindowHeaderLabelLogo
            // 
            this.WindowHeaderLabelLogo.AutoSize = true;
            this.WindowHeaderLabelLogo.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.WindowHeaderLabelLogo.ForeColor = System.Drawing.Color.White;
            this.WindowHeaderLabelLogo.Location = new System.Drawing.Point(13, 11);
            this.WindowHeaderLabelLogo.Name = "WindowHeaderLabelLogo";
            this.WindowHeaderLabelLogo.Size = new System.Drawing.Size(283, 45);
            this.WindowHeaderLabelLogo.TabIndex = 15;
            this.WindowHeaderLabelLogo.Text = "pmdbs device info";
            // 
            // windowButtonMinimize
            // 
            this.windowButtonMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.windowButtonMinimize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.windowButtonMinimize.BackgroundColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.windowButtonMinimize.BackgroundColorNormal = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.windowButtonMinimize.ImageHover = ((System.Drawing.Image)(resources.GetObject("windowButtonMinimize.ImageHover")));
            this.windowButtonMinimize.ImageNormal = ((System.Drawing.Image)(resources.GetObject("windowButtonMinimize.ImageNormal")));
            this.windowButtonMinimize.Location = new System.Drawing.Point(1078, 7);
            this.windowButtonMinimize.Name = "windowButtonMinimize";
            this.windowButtonMinimize.Size = new System.Drawing.Size(60, 50);
            this.windowButtonMinimize.TabIndex = 17;
            this.windowButtonMinimize.OnClickEvent += new System.EventHandler(this.windowButtonMinimize_OnClickEvent);
            // 
            // windowButtonClose
            // 
            this.windowButtonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.windowButtonClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.windowButtonClose.BackgroundColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.windowButtonClose.BackgroundColorNormal = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.windowButtonClose.ImageHover = ((System.Drawing.Image)(resources.GetObject("windowButtonClose.ImageHover")));
            this.windowButtonClose.ImageNormal = ((System.Drawing.Image)(resources.GetObject("windowButtonClose.ImageNormal")));
            this.windowButtonClose.Location = new System.Drawing.Point(1138, 7);
            this.windowButtonClose.Name = "windowButtonClose";
            this.windowButtonClose.Size = new System.Drawing.Size(60, 50);
            this.windowButtonClose.TabIndex = 16;
            this.windowButtonClose.OnClickEvent += new System.EventHandler(this.windowButtonClose_OnClickEvent);
            // 
            // DeviceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 1024);
            this.Controls.Add(this.windowButtonMinimize);
            this.Controls.Add(this.windowButtonClose);
            this.Controls.Add(this.WindowHeaderLabelLogo);
            this.Controls.Add(this.panelMain);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeviceForm";
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Black;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.panelMain.ResumeLayout(false);
            this.panelNetworkInformation.ResumeLayout(false);
            this.panelNetworkInformation.PerformLayout();
            this.panelHardwareInformation.ResumeLayout(false);
            this.panelHardwareInformation.PerformLayout();
            this.panelOsInformation.ResumeLayout(false);
            this.panelOsInformation.PerformLayout();
            this.panelHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Label WindowHeaderLabelLogo;
        private LunaForms.WindowButton windowButtonMinimize;
        private LunaForms.WindowButton windowButtonClose;
        private System.Windows.Forms.Panel panelHeader;
        private LunaForms.LunaSmallCard lunaSmallCardIsOnline;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelDeviceId;
        private System.Windows.Forms.Label labelIp;
        private System.Windows.Forms.Label labelIpHeader;
        private System.Windows.Forms.Label labelLastSeen;
        private System.Windows.Forms.Label labelLastSeenHeader;
        private System.Windows.Forms.Panel panelNetworkInformation;
        private System.Windows.Forms.Label labelNetworkInformationHeader;
        private System.Windows.Forms.Panel panelHardwareInformation;
        private System.Windows.Forms.Label labelMemory;
        private System.Windows.Forms.Label labelMemoryHeader;
        private System.Windows.Forms.Label labelProcessor;
        private System.Windows.Forms.Label labelProcessorHeader;
        private System.Windows.Forms.Label labelHardwareInformationHeader;
        private System.Windows.Forms.Panel panelOsInformation;
        private System.Windows.Forms.Label labelOsInformationHeader;
        private System.Windows.Forms.Label labelArchitecture;
        private System.Windows.Forms.Label labelArchitectureHeader;
        private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.Label labelUsernameHeader;
        private System.Windows.Forms.Label labelServicePack;
        private System.Windows.Forms.Label labelServicePackHeader;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelVersionHeader;
        private System.Windows.Forms.Label labelDeviceName;
        private System.Windows.Forms.Label labelDeviceNameHeader;
        private System.Windows.Forms.Label labelEdition;
        private System.Windows.Forms.Label labelEditionHeader;
        private LunaForms.LunaAnimatedButton lunaAnimatedButtonLogout;
    }
}