namespace pmdbs
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.WindowHeaderPanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.WindowHeaderLabelLogo = new System.Windows.Forms.Label();
            this.WindowPictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.WindowIconPanel = new System.Windows.Forms.Panel();
            this.ButtonMinimize = new pmdbs.WindowButton();
            this.ButtonMaximize = new pmdbs.WindowButton();
            this.ButtonClose = new pmdbs.WindowButton();
            this.DashboardPanel = new System.Windows.Forms.Panel();
            this.advancedButton2 = new pmdbs.AdvancedButton();
            this.advancedButton1 = new pmdbs.AdvancedButton();
            this.DataPanelMain = new System.Windows.Forms.Panel();
            this.DataTableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.DataTableLayoutPanelSubLeft = new System.Windows.Forms.TableLayoutPanel();
            this.DataFlowLayoutPanelList = new System.Windows.Forms.FlowLayoutPanel();
            this.DataTableLayoutPanelFooter = new System.Windows.Forms.TableLayoutPanel();
            this.DataPanelFooterRight = new System.Windows.Forms.Panel();
            this.DataPanelDetails = new System.Windows.Forms.Panel();
            this.DataCustomLabelNotes = new pmdbs.CustomLabel();
            this.DataDetailsEntryUsername = new pmdbs.DetailsEntry();
            this.DataDetailsEntryPassword = new pmdbs.DetailsEntry();
            this.DataDetailsEntryEmail = new pmdbs.DetailsEntry();
            this.DataDetailsEntryWebsite = new pmdbs.DetailsEntry();
            this.DataEditAdvancedImageButton = new pmdbs.AdvancedImageButton();
            this.DataRemoveAdvancedImageButton = new pmdbs.AdvancedImageButton();
            this.DataLabelDetailsHostname = new System.Windows.Forms.Label();
            this.DataPictureBoxDetailsLogo = new System.Windows.Forms.PictureBox();
            this.DataRightAdvancedImageButton = new pmdbs.AdvancedImageButton();
            this.DataPanelFooterLeft = new System.Windows.Forms.Panel();
            this.DataAddAdvancedImageButton = new pmdbs.AdvancedImageButton();
            this.DataLeftAdvancedImageButton = new pmdbs.AdvancedImageButton();
            this.DataTableLayoutPanelSubRight = new System.Windows.Forms.TableLayoutPanel();
            this.DataPictureBoxBackgroundTop = new System.Windows.Forms.PictureBox();
            this.DataPictureBoxBackgroundBottom = new System.Windows.Forms.PictureBox();
            this.DataPanelSubRightMain = new System.Windows.Forms.Panel();
            this.DataPanelEdit = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.editField1 = new pmdbs.EditField();
            this.editField2 = new pmdbs.EditField();
            this.editField3 = new pmdbs.EditField();
            this.editField4 = new pmdbs.EditField();
            this.editField5 = new pmdbs.EditField();
            this.WindowHeaderPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WindowPictureBoxIcon)).BeginInit();
            this.WindowIconPanel.SuspendLayout();
            this.DashboardPanel.SuspendLayout();
            this.DataPanelMain.SuspendLayout();
            this.DataTableLayoutPanelMain.SuspendLayout();
            this.DataTableLayoutPanelSubLeft.SuspendLayout();
            this.DataTableLayoutPanelFooter.SuspendLayout();
            this.DataPanelFooterRight.SuspendLayout();
            this.DataPanelDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataPictureBoxDetailsLogo)).BeginInit();
            this.DataPanelFooterLeft.SuspendLayout();
            this.DataTableLayoutPanelSubRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataPictureBoxBackgroundTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataPictureBoxBackgroundBottom)).BeginInit();
            this.DataPanelSubRightMain.SuspendLayout();
            this.DataPanelEdit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // WindowHeaderPanel
            // 
            this.WindowHeaderPanel.BackColor = System.Drawing.Color.Black;
            this.WindowHeaderPanel.Controls.Add(this.panel1);
            this.WindowHeaderPanel.Controls.Add(this.WindowIconPanel);
            this.WindowHeaderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.WindowHeaderPanel.Location = new System.Drawing.Point(0, 0);
            this.WindowHeaderPanel.Name = "WindowHeaderPanel";
            this.WindowHeaderPanel.Size = new System.Drawing.Size(1920, 55);
            this.WindowHeaderPanel.TabIndex = 0;
            this.WindowHeaderPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WindowHeaderPanel_MouseDown);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.WindowHeaderLabelLogo);
            this.panel1.Controls.Add(this.WindowPictureBoxIcon);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(257, 55);
            this.panel1.TabIndex = 9;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            // 
            // WindowHeaderLabelLogo
            // 
            this.WindowHeaderLabelLogo.AutoSize = true;
            this.WindowHeaderLabelLogo.Font = new System.Drawing.Font("Century Gothic", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WindowHeaderLabelLogo.ForeColor = System.Drawing.Color.White;
            this.WindowHeaderLabelLogo.Location = new System.Drawing.Point(65, 3);
            this.WindowHeaderLabelLogo.Name = "WindowHeaderLabelLogo";
            this.WindowHeaderLabelLogo.Size = new System.Drawing.Size(124, 38);
            this.WindowHeaderLabelLogo.TabIndex = 9;
            this.WindowHeaderLabelLogo.Text = "pmdbs";
            this.WindowHeaderLabelLogo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WindowHeaderLabelLogo_MouseDown);
            // 
            // WindowPictureBoxIcon
            // 
            this.WindowPictureBoxIcon.Image = ((System.Drawing.Image)(resources.GetObject("WindowPictureBoxIcon.Image")));
            this.WindowPictureBoxIcon.Location = new System.Drawing.Point(2, 2);
            this.WindowPictureBoxIcon.Name = "WindowPictureBoxIcon";
            this.WindowPictureBoxIcon.Size = new System.Drawing.Size(62, 50);
            this.WindowPictureBoxIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.WindowPictureBoxIcon.TabIndex = 10;
            this.WindowPictureBoxIcon.TabStop = false;
            this.WindowPictureBoxIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            // 
            // WindowIconPanel
            // 
            this.WindowIconPanel.Controls.Add(this.ButtonMinimize);
            this.WindowIconPanel.Controls.Add(this.ButtonMaximize);
            this.WindowIconPanel.Controls.Add(this.ButtonClose);
            this.WindowIconPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.WindowIconPanel.Location = new System.Drawing.Point(1755, 0);
            this.WindowIconPanel.Name = "WindowIconPanel";
            this.WindowIconPanel.Size = new System.Drawing.Size(165, 55);
            this.WindowIconPanel.TabIndex = 8;
            // 
            // ButtonMinimize
            // 
            this.ButtonMinimize.BackgroundColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ButtonMinimize.BackgroundColorNormal = System.Drawing.Color.Empty;
            this.ButtonMinimize.ImageHover = ((System.Drawing.Image)(resources.GetObject("ButtonMinimize.ImageHover")));
            this.ButtonMinimize.ImageNormal = ((System.Drawing.Image)(resources.GetObject("ButtonMinimize.ImageNormal")));
            this.ButtonMinimize.Location = new System.Drawing.Point(3, 2);
            this.ButtonMinimize.Name = "ButtonMinimize";
            this.ButtonMinimize.Size = new System.Drawing.Size(50, 50);
            this.ButtonMinimize.TabIndex = 2;
            // 
            // ButtonMaximize
            // 
            this.ButtonMaximize.BackgroundColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ButtonMaximize.BackgroundColorNormal = System.Drawing.Color.Empty;
            this.ButtonMaximize.ImageHover = ((System.Drawing.Image)(resources.GetObject("ButtonMaximize.ImageHover")));
            this.ButtonMaximize.ImageNormal = ((System.Drawing.Image)(resources.GetObject("ButtonMaximize.ImageNormal")));
            this.ButtonMaximize.Location = new System.Drawing.Point(59, 2);
            this.ButtonMaximize.Name = "ButtonMaximize";
            this.ButtonMaximize.Size = new System.Drawing.Size(50, 50);
            this.ButtonMaximize.TabIndex = 1;
            // 
            // ButtonClose
            // 
            this.ButtonClose.BackgroundColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.ButtonClose.BackgroundColorNormal = System.Drawing.Color.Empty;
            this.ButtonClose.ImageHover = ((System.Drawing.Image)(resources.GetObject("ButtonClose.ImageHover")));
            this.ButtonClose.ImageNormal = ((System.Drawing.Image)(resources.GetObject("ButtonClose.ImageNormal")));
            this.ButtonClose.Location = new System.Drawing.Point(115, 2);
            this.ButtonClose.Name = "ButtonClose";
            this.ButtonClose.Size = new System.Drawing.Size(50, 50);
            this.ButtonClose.TabIndex = 0;
            // 
            // DashboardPanel
            // 
            this.DashboardPanel.BackColor = System.Drawing.Color.White;
            this.DashboardPanel.Controls.Add(this.advancedButton2);
            this.DashboardPanel.Controls.Add(this.advancedButton1);
            this.DashboardPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.DashboardPanel.Location = new System.Drawing.Point(0, 55);
            this.DashboardPanel.Name = "DashboardPanel";
            this.DashboardPanel.Size = new System.Drawing.Size(107, 1025);
            this.DashboardPanel.TabIndex = 1;
            // 
            // advancedButton2
            // 
            this.advancedButton2.ColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(96)))), ((int)(((byte)(49)))));
            this.advancedButton2.ColorNormal = System.Drawing.Color.Black;
            this.advancedButton2.FontHover = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.advancedButton2.FontNormal = new System.Drawing.Font("Century Gothic", 9F);
            this.advancedButton2.ImageHover = ((System.Drawing.Image)(resources.GetObject("advancedButton2.ImageHover")));
            this.advancedButton2.ImageNormal = ((System.Drawing.Image)(resources.GetObject("advancedButton2.ImageNormal")));
            this.advancedButton2.Location = new System.Drawing.Point(7, 150);
            this.advancedButton2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.advancedButton2.Name = "advancedButton2";
            this.advancedButton2.Size = new System.Drawing.Size(94, 123);
            this.advancedButton2.TabIndex = 1;
            this.advancedButton2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.advancedButton2.TextHover = "Settings";
            this.advancedButton2.TextNormal = "Settings";
            // 
            // advancedButton1
            // 
            this.advancedButton1.ColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(96)))), ((int)(((byte)(49)))));
            this.advancedButton1.ColorNormal = System.Drawing.Color.Black;
            this.advancedButton1.FontHover = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.advancedButton1.FontNormal = new System.Drawing.Font("Century Gothic", 9F);
            this.advancedButton1.ImageHover = ((System.Drawing.Image)(resources.GetObject("advancedButton1.ImageHover")));
            this.advancedButton1.ImageNormal = ((System.Drawing.Image)(resources.GetObject("advancedButton1.ImageNormal")));
            this.advancedButton1.Location = new System.Drawing.Point(8, 24);
            this.advancedButton1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.advancedButton1.Name = "advancedButton1";
            this.advancedButton1.Size = new System.Drawing.Size(91, 118);
            this.advancedButton1.TabIndex = 0;
            this.advancedButton1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.advancedButton1.TextHover = "Home";
            this.advancedButton1.TextNormal = "Home";
            // 
            // DataPanelMain
            // 
            this.DataPanelMain.BackColor = System.Drawing.Color.DarkGray;
            this.DataPanelMain.Controls.Add(this.DataTableLayoutPanelMain);
            this.DataPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataPanelMain.Location = new System.Drawing.Point(107, 55);
            this.DataPanelMain.Name = "DataPanelMain";
            this.DataPanelMain.Size = new System.Drawing.Size(1813, 1025);
            this.DataPanelMain.TabIndex = 2;
            // 
            // DataTableLayoutPanelMain
            // 
            this.DataTableLayoutPanelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.DataTableLayoutPanelMain.ColumnCount = 5;
            this.DataTableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.DataTableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.DataTableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.DataTableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 700F));
            this.DataTableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.DataTableLayoutPanelMain.Controls.Add(this.DataTableLayoutPanelSubLeft, 1, 1);
            this.DataTableLayoutPanelMain.Controls.Add(this.DataTableLayoutPanelSubRight, 3, 1);
            this.DataTableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataTableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.DataTableLayoutPanelMain.Name = "DataTableLayoutPanelMain";
            this.DataTableLayoutPanelMain.RowCount = 3;
            this.DataTableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.DataTableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.DataTableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.DataTableLayoutPanelMain.Size = new System.Drawing.Size(1813, 1025);
            this.DataTableLayoutPanelMain.TabIndex = 0;
            // 
            // DataTableLayoutPanelSubLeft
            // 
            this.DataTableLayoutPanelSubLeft.ColumnCount = 1;
            this.DataTableLayoutPanelSubLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.DataTableLayoutPanelSubLeft.Controls.Add(this.DataFlowLayoutPanelList, 0, 0);
            this.DataTableLayoutPanelSubLeft.Controls.Add(this.DataTableLayoutPanelFooter, 0, 2);
            this.DataTableLayoutPanelSubLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataTableLayoutPanelSubLeft.Location = new System.Drawing.Point(13, 13);
            this.DataTableLayoutPanelSubLeft.Name = "DataTableLayoutPanelSubLeft";
            this.DataTableLayoutPanelSubLeft.RowCount = 3;
            this.DataTableLayoutPanelSubLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.DataTableLayoutPanelSubLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.DataTableLayoutPanelSubLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 84F));
            this.DataTableLayoutPanelSubLeft.Size = new System.Drawing.Size(1077, 999);
            this.DataTableLayoutPanelSubLeft.TabIndex = 4;
            // 
            // DataFlowLayoutPanelList
            // 
            this.DataFlowLayoutPanelList.AutoScroll = true;
            this.DataFlowLayoutPanelList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.DataFlowLayoutPanelList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataFlowLayoutPanelList.Location = new System.Drawing.Point(1, 1);
            this.DataFlowLayoutPanelList.Margin = new System.Windows.Forms.Padding(1);
            this.DataFlowLayoutPanelList.Name = "DataFlowLayoutPanelList";
            this.DataFlowLayoutPanelList.Size = new System.Drawing.Size(1075, 893);
            this.DataFlowLayoutPanelList.TabIndex = 4;
            this.DataFlowLayoutPanelList.MouseEnter += new System.EventHandler(this.flowLayoutPanel1_MouseEnter);
            this.DataFlowLayoutPanelList.Resize += new System.EventHandler(this.flowLayoutPanel1_Resize);
            // 
            // DataTableLayoutPanelFooter
            // 
            this.DataTableLayoutPanelFooter.ColumnCount = 2;
            this.DataTableLayoutPanelFooter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.DataTableLayoutPanelFooter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.DataTableLayoutPanelFooter.Controls.Add(this.DataPanelFooterRight, 1, 0);
            this.DataTableLayoutPanelFooter.Controls.Add(this.DataPanelFooterLeft, 0, 0);
            this.DataTableLayoutPanelFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataTableLayoutPanelFooter.Location = new System.Drawing.Point(3, 918);
            this.DataTableLayoutPanelFooter.Name = "DataTableLayoutPanelFooter";
            this.DataTableLayoutPanelFooter.RowCount = 1;
            this.DataTableLayoutPanelFooter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.DataTableLayoutPanelFooter.Size = new System.Drawing.Size(1071, 78);
            this.DataTableLayoutPanelFooter.TabIndex = 5;
            // 
            // DataPanelFooterRight
            // 
            this.DataPanelFooterRight.BackColor = System.Drawing.Color.White;
            this.DataPanelFooterRight.Controls.Add(this.DataPanelDetails);
            this.DataPanelFooterRight.Controls.Add(this.DataRightAdvancedImageButton);
            this.DataPanelFooterRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataPanelFooterRight.Location = new System.Drawing.Point(535, 0);
            this.DataPanelFooterRight.Margin = new System.Windows.Forms.Padding(0);
            this.DataPanelFooterRight.Name = "DataPanelFooterRight";
            this.DataPanelFooterRight.Size = new System.Drawing.Size(536, 78);
            this.DataPanelFooterRight.TabIndex = 1;
            // 
            // DataPanelDetails
            // 
            this.DataPanelDetails.BackColor = System.Drawing.Color.White;
            this.DataPanelDetails.Controls.Add(this.DataCustomLabelNotes);
            this.DataPanelDetails.Controls.Add(this.DataDetailsEntryUsername);
            this.DataPanelDetails.Controls.Add(this.DataDetailsEntryPassword);
            this.DataPanelDetails.Controls.Add(this.DataDetailsEntryEmail);
            this.DataPanelDetails.Controls.Add(this.DataDetailsEntryWebsite);
            this.DataPanelDetails.Controls.Add(this.DataEditAdvancedImageButton);
            this.DataPanelDetails.Controls.Add(this.DataRemoveAdvancedImageButton);
            this.DataPanelDetails.Controls.Add(this.DataLabelDetailsHostname);
            this.DataPanelDetails.Controls.Add(this.DataPictureBoxDetailsLogo);
            this.DataPanelDetails.Location = new System.Drawing.Point(81, 25);
            this.DataPanelDetails.Name = "DataPanelDetails";
            this.DataPanelDetails.Size = new System.Drawing.Size(592, 709);
            this.DataPanelDetails.TabIndex = 3;
            // 
            // DataCustomLabelNotes
            // 
            this.DataCustomLabelNotes.Content = "Some  generic notes";
            this.DataCustomLabelNotes.Header = "Notes:";
            this.DataCustomLabelNotes.Location = new System.Drawing.Point(4, 663);
            this.DataCustomLabelNotes.Name = "DataCustomLabelNotes";
            this.DataCustomLabelNotes.Size = new System.Drawing.Size(680, 327);
            this.DataCustomLabelNotes.TabIndex = 23;
            // 
            // DataDetailsEntryUsername
            // 
            this.DataDetailsEntryUsername.Content = "Us3r123";
            this.DataDetailsEntryUsername.Header = "Your Username:";
            this.DataDetailsEntryUsername.ImageHover = ((System.Drawing.Image)(resources.GetObject("DataDetailsEntryUsername.ImageHover")));
            this.DataDetailsEntryUsername.ImageNormal = ((System.Drawing.Image)(resources.GetObject("DataDetailsEntryUsername.ImageNormal")));
            this.DataDetailsEntryUsername.Location = new System.Drawing.Point(4, 159);
            this.DataDetailsEntryUsername.Name = "DataDetailsEntryUsername";
            this.DataDetailsEntryUsername.RawText = null;
            this.DataDetailsEntryUsername.Size = new System.Drawing.Size(680, 120);
            this.DataDetailsEntryUsername.TabIndex = 22;
            // 
            // DataDetailsEntryPassword
            // 
            this.DataDetailsEntryPassword.Content = "cgRr4$).k7tx6qvOs,+6HFz/!cgRr4$).k7tx6qvOs,+6HFz/!cgRr4$).k7tx6qvOs,+6HFz/!";
            this.DataDetailsEntryPassword.Header = "Your Password:";
            this.DataDetailsEntryPassword.ImageHover = ((System.Drawing.Image)(resources.GetObject("DataDetailsEntryPassword.ImageHover")));
            this.DataDetailsEntryPassword.ImageNormal = ((System.Drawing.Image)(resources.GetObject("DataDetailsEntryPassword.ImageNormal")));
            this.DataDetailsEntryPassword.Location = new System.Drawing.Point(4, 285);
            this.DataDetailsEntryPassword.Name = "DataDetailsEntryPassword";
            this.DataDetailsEntryPassword.RawText = null;
            this.DataDetailsEntryPassword.Size = new System.Drawing.Size(680, 120);
            this.DataDetailsEntryPassword.TabIndex = 21;
            // 
            // DataDetailsEntryEmail
            // 
            this.DataDetailsEntryEmail.Content = "example@gmail.com";
            this.DataDetailsEntryEmail.Header = "Your Email:";
            this.DataDetailsEntryEmail.ImageHover = ((System.Drawing.Image)(resources.GetObject("DataDetailsEntryEmail.ImageHover")));
            this.DataDetailsEntryEmail.ImageNormal = ((System.Drawing.Image)(resources.GetObject("DataDetailsEntryEmail.ImageNormal")));
            this.DataDetailsEntryEmail.Location = new System.Drawing.Point(4, 411);
            this.DataDetailsEntryEmail.Name = "DataDetailsEntryEmail";
            this.DataDetailsEntryEmail.RawText = null;
            this.DataDetailsEntryEmail.Size = new System.Drawing.Size(680, 120);
            this.DataDetailsEntryEmail.TabIndex = 20;
            // 
            // DataDetailsEntryWebsite
            // 
            this.DataDetailsEntryWebsite.Content = "https://mail.google.com";
            this.DataDetailsEntryWebsite.Header = "Website:";
            this.DataDetailsEntryWebsite.ImageHover = ((System.Drawing.Image)(resources.GetObject("DataDetailsEntryWebsite.ImageHover")));
            this.DataDetailsEntryWebsite.ImageNormal = ((System.Drawing.Image)(resources.GetObject("DataDetailsEntryWebsite.ImageNormal")));
            this.DataDetailsEntryWebsite.Location = new System.Drawing.Point(4, 537);
            this.DataDetailsEntryWebsite.Name = "DataDetailsEntryWebsite";
            this.DataDetailsEntryWebsite.RawText = "https://accounts.google.com/signin/v2/identifier?continue=https%3A%2F%2Fmail.goog" +
    "le.com%2Fmail%2F&service=mail&sacu=1&rip=1&flowName=GlifWebSignIn&flowEntry=Serv" +
    "iceLogin";
            this.DataDetailsEntryWebsite.Size = new System.Drawing.Size(680, 120);
            this.DataDetailsEntryWebsite.TabIndex = 19;
            // 
            // DataEditAdvancedImageButton
            // 
            this.DataEditAdvancedImageButton.ImageHover = ((System.Drawing.Image)(resources.GetObject("DataEditAdvancedImageButton.ImageHover")));
            this.DataEditAdvancedImageButton.ImageNormal = ((System.Drawing.Image)(resources.GetObject("DataEditAdvancedImageButton.ImageNormal")));
            this.DataEditAdvancedImageButton.Location = new System.Drawing.Point(534, 47);
            this.DataEditAdvancedImageButton.Name = "DataEditAdvancedImageButton";
            this.DataEditAdvancedImageButton.Size = new System.Drawing.Size(72, 72);
            this.DataEditAdvancedImageButton.TabIndex = 14;
            // 
            // DataRemoveAdvancedImageButton
            // 
            this.DataRemoveAdvancedImageButton.ImageHover = ((System.Drawing.Image)(resources.GetObject("DataRemoveAdvancedImageButton.ImageHover")));
            this.DataRemoveAdvancedImageButton.ImageNormal = ((System.Drawing.Image)(resources.GetObject("DataRemoveAdvancedImageButton.ImageNormal")));
            this.DataRemoveAdvancedImageButton.Location = new System.Drawing.Point(612, 47);
            this.DataRemoveAdvancedImageButton.Name = "DataRemoveAdvancedImageButton";
            this.DataRemoveAdvancedImageButton.Size = new System.Drawing.Size(72, 72);
            this.DataRemoveAdvancedImageButton.TabIndex = 13;
            // 
            // DataLabelDetailsHostname
            // 
            this.DataLabelDetailsHostname.AutoSize = true;
            this.DataLabelDetailsHostname.Font = new System.Drawing.Font("Century Gothic", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DataLabelDetailsHostname.Location = new System.Drawing.Point(95, 68);
            this.DataLabelDetailsHostname.Name = "DataLabelDetailsHostname";
            this.DataLabelDetailsHostname.Size = new System.Drawing.Size(168, 47);
            this.DataLabelDetailsHostname.TabIndex = 1;
            this.DataLabelDetailsHostname.Text = "Google";
            // 
            // DataPictureBoxDetailsLogo
            // 
            this.DataPictureBoxDetailsLogo.Location = new System.Drawing.Point(25, 55);
            this.DataPictureBoxDetailsLogo.Name = "DataPictureBoxDetailsLogo";
            this.DataPictureBoxDetailsLogo.Size = new System.Drawing.Size(64, 64);
            this.DataPictureBoxDetailsLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.DataPictureBoxDetailsLogo.TabIndex = 0;
            this.DataPictureBoxDetailsLogo.TabStop = false;
            // 
            // DataRightAdvancedImageButton
            // 
            this.DataRightAdvancedImageButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.DataRightAdvancedImageButton.ImageHover = ((System.Drawing.Image)(resources.GetObject("DataRightAdvancedImageButton.ImageHover")));
            this.DataRightAdvancedImageButton.ImageNormal = ((System.Drawing.Image)(resources.GetObject("DataRightAdvancedImageButton.ImageNormal")));
            this.DataRightAdvancedImageButton.Location = new System.Drawing.Point(3, 3);
            this.DataRightAdvancedImageButton.Name = "DataRightAdvancedImageButton";
            this.DataRightAdvancedImageButton.Size = new System.Drawing.Size(72, 72);
            this.DataRightAdvancedImageButton.TabIndex = 17;
            // 
            // DataPanelFooterLeft
            // 
            this.DataPanelFooterLeft.BackColor = System.Drawing.Color.White;
            this.DataPanelFooterLeft.Controls.Add(this.DataAddAdvancedImageButton);
            this.DataPanelFooterLeft.Controls.Add(this.DataLeftAdvancedImageButton);
            this.DataPanelFooterLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataPanelFooterLeft.ForeColor = System.Drawing.SystemColors.ControlText;
            this.DataPanelFooterLeft.Location = new System.Drawing.Point(0, 0);
            this.DataPanelFooterLeft.Margin = new System.Windows.Forms.Padding(0);
            this.DataPanelFooterLeft.Name = "DataPanelFooterLeft";
            this.DataPanelFooterLeft.Size = new System.Drawing.Size(535, 78);
            this.DataPanelFooterLeft.TabIndex = 0;
            // 
            // DataAddAdvancedImageButton
            // 
            this.DataAddAdvancedImageButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.DataAddAdvancedImageButton.ImageHover = ((System.Drawing.Image)(resources.GetObject("DataAddAdvancedImageButton.ImageHover")));
            this.DataAddAdvancedImageButton.ImageNormal = ((System.Drawing.Image)(resources.GetObject("DataAddAdvancedImageButton.ImageNormal")));
            this.DataAddAdvancedImageButton.Location = new System.Drawing.Point(3, 3);
            this.DataAddAdvancedImageButton.Name = "DataAddAdvancedImageButton";
            this.DataAddAdvancedImageButton.Size = new System.Drawing.Size(72, 72);
            this.DataAddAdvancedImageButton.TabIndex = 15;
            // 
            // DataLeftAdvancedImageButton
            // 
            this.DataLeftAdvancedImageButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.DataLeftAdvancedImageButton.ImageHover = ((System.Drawing.Image)(resources.GetObject("DataLeftAdvancedImageButton.ImageHover")));
            this.DataLeftAdvancedImageButton.ImageNormal = ((System.Drawing.Image)(resources.GetObject("DataLeftAdvancedImageButton.ImageNormal")));
            this.DataLeftAdvancedImageButton.Location = new System.Drawing.Point(460, 3);
            this.DataLeftAdvancedImageButton.Name = "DataLeftAdvancedImageButton";
            this.DataLeftAdvancedImageButton.Size = new System.Drawing.Size(72, 72);
            this.DataLeftAdvancedImageButton.TabIndex = 16;
            // 
            // DataTableLayoutPanelSubRight
            // 
            this.DataTableLayoutPanelSubRight.BackColor = System.Drawing.Color.White;
            this.DataTableLayoutPanelSubRight.ColumnCount = 1;
            this.DataTableLayoutPanelSubRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.DataTableLayoutPanelSubRight.Controls.Add(this.DataPictureBoxBackgroundTop, 0, 0);
            this.DataTableLayoutPanelSubRight.Controls.Add(this.DataPictureBoxBackgroundBottom, 0, 2);
            this.DataTableLayoutPanelSubRight.Controls.Add(this.DataPanelSubRightMain, 0, 1);
            this.DataTableLayoutPanelSubRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataTableLayoutPanelSubRight.Location = new System.Drawing.Point(1106, 13);
            this.DataTableLayoutPanelSubRight.Name = "DataTableLayoutPanelSubRight";
            this.DataTableLayoutPanelSubRight.RowCount = 3;
            this.DataTableLayoutPanelSubRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.DataTableLayoutPanelSubRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1000F));
            this.DataTableLayoutPanelSubRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.DataTableLayoutPanelSubRight.Size = new System.Drawing.Size(694, 999);
            this.DataTableLayoutPanelSubRight.TabIndex = 5;
            // 
            // DataPictureBoxBackgroundTop
            // 
            this.DataPictureBoxBackgroundTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataPictureBoxBackgroundTop.Image = ((System.Drawing.Image)(resources.GetObject("DataPictureBoxBackgroundTop.Image")));
            this.DataPictureBoxBackgroundTop.Location = new System.Drawing.Point(0, 0);
            this.DataPictureBoxBackgroundTop.Margin = new System.Windows.Forms.Padding(0);
            this.DataPictureBoxBackgroundTop.Name = "DataPictureBoxBackgroundTop";
            this.DataPictureBoxBackgroundTop.Size = new System.Drawing.Size(694, 1);
            this.DataPictureBoxBackgroundTop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.DataPictureBoxBackgroundTop.TabIndex = 4;
            this.DataPictureBoxBackgroundTop.TabStop = false;
            // 
            // DataPictureBoxBackgroundBottom
            // 
            this.DataPictureBoxBackgroundBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataPictureBoxBackgroundBottom.Image = ((System.Drawing.Image)(resources.GetObject("DataPictureBoxBackgroundBottom.Image")));
            this.DataPictureBoxBackgroundBottom.Location = new System.Drawing.Point(0, 1000);
            this.DataPictureBoxBackgroundBottom.Margin = new System.Windows.Forms.Padding(0);
            this.DataPictureBoxBackgroundBottom.Name = "DataPictureBoxBackgroundBottom";
            this.DataPictureBoxBackgroundBottom.Size = new System.Drawing.Size(694, 1);
            this.DataPictureBoxBackgroundBottom.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.DataPictureBoxBackgroundBottom.TabIndex = 5;
            this.DataPictureBoxBackgroundBottom.TabStop = false;
            // 
            // DataPanelSubRightMain
            // 
            this.DataPanelSubRightMain.Controls.Add(this.DataPanelEdit);
            this.DataPanelSubRightMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataPanelSubRightMain.Location = new System.Drawing.Point(0, 0);
            this.DataPanelSubRightMain.Margin = new System.Windows.Forms.Padding(0);
            this.DataPanelSubRightMain.Name = "DataPanelSubRightMain";
            this.DataPanelSubRightMain.Size = new System.Drawing.Size(694, 1000);
            this.DataPanelSubRightMain.TabIndex = 6;
            // 
            // DataPanelEdit
            // 
            this.DataPanelEdit.Controls.Add(this.editField5);
            this.DataPanelEdit.Controls.Add(this.editField4);
            this.DataPanelEdit.Controls.Add(this.editField3);
            this.DataPanelEdit.Controls.Add(this.editField2);
            this.DataPanelEdit.Controls.Add(this.editField1);
            this.DataPanelEdit.Controls.Add(this.pictureBox1);
            this.DataPanelEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataPanelEdit.Location = new System.Drawing.Point(0, 0);
            this.DataPanelEdit.Name = "DataPanelEdit";
            this.DataPanelEdit.Size = new System.Drawing.Size(694, 1000);
            this.DataPanelEdit.TabIndex = 4;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(19, 59);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(64, 64);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 24;
            this.pictureBox1.TabStop = false;
            // 
            // editField1
            // 
            this.editField1.AutoSize = true;
            this.editField1.ColorTextBoxFocus = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(96)))), ((int)(((byte)(49)))));
            this.editField1.ColorTextBoxNormal = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.editField1.FontTextBox = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editField1.FontTitle = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editField1.ImageClearHover = ((System.Drawing.Image)(resources.GetObject("editField1.ImageClearHover")));
            this.editField1.ImageClearNormal = ((System.Drawing.Image)(resources.GetObject("editField1.ImageClearNormal")));
            this.editField1.Location = new System.Drawing.Point(8, 164);
            this.editField1.Name = "editField1";
            this.editField1.Size = new System.Drawing.Size(680, 96);
            this.editField1.TabIndex = 25;
            this.editField1.TextTextBox = "Gmail";
            this.editField1.TextTitle = "Hostname:";
            // 
            // editField2
            // 
            this.editField2.AutoSize = true;
            this.editField2.ColorTextBoxFocus = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(96)))), ((int)(((byte)(49)))));
            this.editField2.ColorTextBoxNormal = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.editField2.FontTextBox = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editField2.FontTitle = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editField2.ImageClearHover = ((System.Drawing.Image)(resources.GetObject("editField2.ImageClearHover")));
            this.editField2.ImageClearNormal = ((System.Drawing.Image)(resources.GetObject("editField2.ImageClearNormal")));
            this.editField2.Location = new System.Drawing.Point(8, 266);
            this.editField2.Name = "editField2";
            this.editField2.Size = new System.Drawing.Size(680, 96);
            this.editField2.TabIndex = 26;
            this.editField2.TextTextBox = "Us3r123";
            this.editField2.TextTitle = "Username:";
            // 
            // editField3
            // 
            this.editField3.AutoSize = true;
            this.editField3.ColorTextBoxFocus = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(96)))), ((int)(((byte)(49)))));
            this.editField3.ColorTextBoxNormal = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.editField3.FontTextBox = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editField3.FontTitle = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editField3.ImageClearHover = ((System.Drawing.Image)(resources.GetObject("editField3.ImageClearHover")));
            this.editField3.ImageClearNormal = ((System.Drawing.Image)(resources.GetObject("editField3.ImageClearNormal")));
            this.editField3.Location = new System.Drawing.Point(8, 368);
            this.editField3.Name = "editField3";
            this.editField3.Size = new System.Drawing.Size(680, 96);
            this.editField3.TabIndex = 27;
            this.editField3.TextTextBox = "cgRr4$).k7tx6qvOs,+6HFz/!cgRr4$).k7tx6qvOs,+6HFz/!cgRr4$).k7tx6qvOs,+6HFz/!";
            this.editField3.TextTitle = "Password:";
            // 
            // editField4
            // 
            this.editField4.AutoSize = true;
            this.editField4.ColorTextBoxFocus = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(96)))), ((int)(((byte)(49)))));
            this.editField4.ColorTextBoxNormal = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.editField4.FontTextBox = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editField4.FontTitle = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editField4.ImageClearHover = ((System.Drawing.Image)(resources.GetObject("editField4.ImageClearHover")));
            this.editField4.ImageClearNormal = ((System.Drawing.Image)(resources.GetObject("editField4.ImageClearNormal")));
            this.editField4.Location = new System.Drawing.Point(8, 470);
            this.editField4.Name = "editField4";
            this.editField4.Size = new System.Drawing.Size(680, 96);
            this.editField4.TabIndex = 28;
            this.editField4.TextTextBox = "example@gmail.com";
            this.editField4.TextTitle = "Email";
            // 
            // editField5
            // 
            this.editField5.AutoSize = true;
            this.editField5.ColorTextBoxFocus = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(96)))), ((int)(((byte)(49)))));
            this.editField5.ColorTextBoxNormal = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.editField5.FontTextBox = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editField5.FontTitle = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editField5.ImageClearHover = ((System.Drawing.Image)(resources.GetObject("editField5.ImageClearHover")));
            this.editField5.ImageClearNormal = ((System.Drawing.Image)(resources.GetObject("editField5.ImageClearNormal")));
            this.editField5.Location = new System.Drawing.Point(8, 572);
            this.editField5.Name = "editField5";
            this.editField5.Size = new System.Drawing.Size(680, 96);
            this.editField5.TabIndex = 29;
            this.editField5.TextTextBox = "https://mail.google.com";
            this.editField5.TextTitle = "Website:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1920, 1080);
            this.ControlBox = false;
            this.Controls.Add(this.DataPanelMain);
            this.Controls.Add(this.DashboardPanel);
            this.Controls.Add(this.WindowHeaderPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1280, 1080);
            this.Name = "Form1";
            this.WindowHeaderPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WindowPictureBoxIcon)).EndInit();
            this.WindowIconPanel.ResumeLayout(false);
            this.DashboardPanel.ResumeLayout(false);
            this.DataPanelMain.ResumeLayout(false);
            this.DataTableLayoutPanelMain.ResumeLayout(false);
            this.DataTableLayoutPanelSubLeft.ResumeLayout(false);
            this.DataTableLayoutPanelFooter.ResumeLayout(false);
            this.DataPanelFooterRight.ResumeLayout(false);
            this.DataPanelDetails.ResumeLayout(false);
            this.DataPanelDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataPictureBoxDetailsLogo)).EndInit();
            this.DataPanelFooterLeft.ResumeLayout(false);
            this.DataTableLayoutPanelSubRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DataPictureBoxBackgroundTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataPictureBoxBackgroundBottom)).EndInit();
            this.DataPanelSubRightMain.ResumeLayout(false);
            this.DataPanelEdit.ResumeLayout(false);
            this.DataPanelEdit.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel WindowHeaderPanel;
        private System.Windows.Forms.Panel DashboardPanel;
        private System.Windows.Forms.Panel DataPanelMain;
        private System.Windows.Forms.TableLayoutPanel DataTableLayoutPanelMain;
        private AdvancedButton advancedButton1;
        private AdvancedButton advancedButton2;
        private System.Windows.Forms.Panel DataPanelDetails;
        private System.Windows.Forms.FlowLayoutPanel DataFlowLayoutPanelList;
        private System.Windows.Forms.TableLayoutPanel DataTableLayoutPanelSubLeft;
        private System.Windows.Forms.Panel WindowIconPanel;
        private WindowButton ButtonMinimize;
        private WindowButton ButtonMaximize;
        private WindowButton ButtonClose;
        private System.Windows.Forms.Label WindowHeaderLabelLogo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox WindowPictureBoxIcon;
        private System.Windows.Forms.Label DataLabelDetailsHostname;
        private System.Windows.Forms.PictureBox DataPictureBoxDetailsLogo;
        private AdvancedImageButton DataEditAdvancedImageButton;
        private AdvancedImageButton DataRemoveAdvancedImageButton;
        private AdvancedImageButton DataAddAdvancedImageButton;
        private AdvancedImageButton DataRightAdvancedImageButton;
        private AdvancedImageButton DataLeftAdvancedImageButton;
        private System.Windows.Forms.TableLayoutPanel DataTableLayoutPanelSubRight;
        private System.Windows.Forms.PictureBox DataPictureBoxBackgroundTop;
        private System.Windows.Forms.PictureBox DataPictureBoxBackgroundBottom;
        private System.Windows.Forms.TableLayoutPanel DataTableLayoutPanelFooter;
        private System.Windows.Forms.Panel DataPanelFooterRight;
        private System.Windows.Forms.Panel DataPanelFooterLeft;
        private DetailsEntry DataDetailsEntryUsername;
        private DetailsEntry DataDetailsEntryPassword;
        private DetailsEntry DataDetailsEntryEmail;
        private DetailsEntry DataDetailsEntryWebsite;
        private CustomLabel DataCustomLabelNotes;
        private System.Windows.Forms.Panel DataPanelSubRightMain;
        private System.Windows.Forms.Panel DataPanelEdit;
        private System.Windows.Forms.PictureBox pictureBox1;
        private EditField editField5;
        private EditField editField4;
        private EditField editField3;
        private EditField editField2;
        private EditField editField1;
    }
}

