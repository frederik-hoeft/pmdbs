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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.WindowIconPanel = new System.Windows.Forms.Panel();
            this.ButtonMinimize = new pmdbs.WindowButton();
            this.ButtonMaximize = new pmdbs.WindowButton();
            this.ButtonClose = new pmdbs.WindowButton();
            this.DashboardPanel = new System.Windows.Forms.Panel();
            this.advancedButton2 = new pmdbs.AdvancedButton();
            this.advancedButton1 = new pmdbs.AdvancedButton();
            this.DataPanelMain = new System.Windows.Forms.Panel();
            this.DataTableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.DataPanelDetails = new System.Windows.Forms.Panel();
            this.DataEditAdvancedImageButton = new pmdbs.AdvancedImageButton();
            this.DataRemoveAdvancedImageButton = new pmdbs.AdvancedImageButton();
            this.dynamicLink1 = new pmdbs.DynamicLink();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.DataLabelDetailsHostname = new System.Windows.Forms.Label();
            this.DataPictureBoxDetailsLogo = new System.Windows.Forms.PictureBox();
            this.DataTableLayoutPanelSubLeft = new System.Windows.Forms.TableLayoutPanel();
            this.DataFlowLayoutPanelList = new System.Windows.Forms.FlowLayoutPanel();
            this.DataPanelFooter = new System.Windows.Forms.Panel();
            this.DataAddAdvancedImageButton = new pmdbs.AdvancedImageButton();
            this.DataLeftAdvancedImageButton = new pmdbs.AdvancedImageButton();
            this.DataRightAdvancedImageButton = new pmdbs.AdvancedImageButton();
            this.WindowHeaderPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.WindowIconPanel.SuspendLayout();
            this.DashboardPanel.SuspendLayout();
            this.DataPanelMain.SuspendLayout();
            this.DataTableLayoutPanelMain.SuspendLayout();
            this.DataPanelDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataPictureBoxDetailsLogo)).BeginInit();
            this.DataTableLayoutPanelSubLeft.SuspendLayout();
            this.DataPanelFooter.SuspendLayout();
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
            this.panel1.Controls.Add(this.pictureBox1);
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
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(2, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(62, 50);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
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
            this.ButtonMinimize.Location = new System.Drawing.Point(3, 3);
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
            this.ButtonMaximize.Location = new System.Drawing.Point(59, 3);
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
            this.ButtonClose.Location = new System.Drawing.Point(115, 3);
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
            this.advancedButton2.ColorNormal = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
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
            this.advancedButton1.ColorNormal = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
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
            this.DataTableLayoutPanelMain.Controls.Add(this.DataPanelDetails, 3, 1);
            this.DataTableLayoutPanelMain.Controls.Add(this.DataTableLayoutPanelSubLeft, 1, 1);
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
            // DataPanelDetails
            // 
            this.DataPanelDetails.BackColor = System.Drawing.Color.White;
            this.DataPanelDetails.Controls.Add(this.DataEditAdvancedImageButton);
            this.DataPanelDetails.Controls.Add(this.DataRemoveAdvancedImageButton);
            this.DataPanelDetails.Controls.Add(this.dynamicLink1);
            this.DataPanelDetails.Controls.Add(this.label7);
            this.DataPanelDetails.Controls.Add(this.label5);
            this.DataPanelDetails.Controls.Add(this.label6);
            this.DataPanelDetails.Controls.Add(this.label3);
            this.DataPanelDetails.Controls.Add(this.label4);
            this.DataPanelDetails.Controls.Add(this.label2);
            this.DataPanelDetails.Controls.Add(this.label1);
            this.DataPanelDetails.Controls.Add(this.DataLabelDetailsHostname);
            this.DataPanelDetails.Controls.Add(this.DataPictureBoxDetailsLogo);
            this.DataPanelDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataPanelDetails.Location = new System.Drawing.Point(1106, 13);
            this.DataPanelDetails.Name = "DataPanelDetails";
            this.DataPanelDetails.Size = new System.Drawing.Size(694, 999);
            this.DataPanelDetails.TabIndex = 3;
            // 
            // DataEditAdvancedImageButton
            // 
            this.DataEditAdvancedImageButton.ImageHover = ((System.Drawing.Image)(resources.GetObject("DataEditAdvancedImageButton.ImageHover")));
            this.DataEditAdvancedImageButton.ImageNormal = ((System.Drawing.Image)(resources.GetObject("DataEditAdvancedImageButton.ImageNormal")));
            this.DataEditAdvancedImageButton.Location = new System.Drawing.Point(535, 919);
            this.DataEditAdvancedImageButton.Name = "DataEditAdvancedImageButton";
            this.DataEditAdvancedImageButton.Size = new System.Drawing.Size(72, 72);
            this.DataEditAdvancedImageButton.TabIndex = 14;
            // 
            // DataRemoveAdvancedImageButton
            // 
            this.DataRemoveAdvancedImageButton.ImageHover = ((System.Drawing.Image)(resources.GetObject("DataRemoveAdvancedImageButton.ImageHover")));
            this.DataRemoveAdvancedImageButton.ImageNormal = ((System.Drawing.Image)(resources.GetObject("DataRemoveAdvancedImageButton.ImageNormal")));
            this.DataRemoveAdvancedImageButton.Location = new System.Drawing.Point(613, 919);
            this.DataRemoveAdvancedImageButton.Name = "DataRemoveAdvancedImageButton";
            this.DataRemoveAdvancedImageButton.Size = new System.Drawing.Size(72, 72);
            this.DataRemoveAdvancedImageButton.TabIndex = 13;
            // 
            // dynamicLink1
            // 
            this.dynamicLink1.ActiveLinkColor = System.Drawing.Color.Red;
            this.dynamicLink1.AutoSize = true;
            this.dynamicLink1.LinkColor = System.Drawing.Color.Black;
            this.dynamicLink1.LinkColorOnMouseHover = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(96)))), ((int)(((byte)(49)))));
            this.dynamicLink1.LinkFont = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dynamicLink1.LinkText = "https://mail.google.com";
            this.dynamicLink1.LinkVisited = false;
            this.dynamicLink1.Location = new System.Drawing.Point(19, 488);
            this.dynamicLink1.Name = "dynamicLink1";
            this.dynamicLink1.Size = new System.Drawing.Size(666, 45);
            this.dynamicLink1.TabIndex = 10;
            this.dynamicLink1.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))));
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(20, 461);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 30);
            this.label7.TabIndex = 9;
            this.label7.Text = "Link:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(20, 370);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(146, 30);
            this.label5.TabIndex = 7;
            this.label5.Text = "Your Email:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(19, 399);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(303, 34);
            this.label6.TabIndex = 6;
            this.label6.Text = "example@gmail.com";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(20, 279);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(190, 30);
            this.label3.TabIndex = 5;
            this.label3.Text = "Your Password:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(19, 308);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(376, 34);
            this.label4.TabIndex = 4;
            this.label4.Text = "cgRr4$).k7tx6qvOs,+6HFz/!";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(20, 188);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(201, 30);
            this.label2.TabIndex = 3;
            this.label2.Text = "Your Username:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(19, 217);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 34);
            this.label1.TabIndex = 2;
            this.label1.Text = "Us3r123";
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
            // DataTableLayoutPanelSubLeft
            // 
            this.DataTableLayoutPanelSubLeft.ColumnCount = 1;
            this.DataTableLayoutPanelSubLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.DataTableLayoutPanelSubLeft.Controls.Add(this.DataFlowLayoutPanelList, 0, 0);
            this.DataTableLayoutPanelSubLeft.Controls.Add(this.DataPanelFooter, 0, 2);
            this.DataTableLayoutPanelSubLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataTableLayoutPanelSubLeft.Location = new System.Drawing.Point(13, 13);
            this.DataTableLayoutPanelSubLeft.Name = "DataTableLayoutPanelSubLeft";
            this.DataTableLayoutPanelSubLeft.RowCount = 3;
            this.DataTableLayoutPanelSubLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.DataTableLayoutPanelSubLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.DataTableLayoutPanelSubLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 84F));
            this.DataTableLayoutPanelSubLeft.Size = new System.Drawing.Size(1077, 999);
            this.DataTableLayoutPanelSubLeft.TabIndex = 4;
            // 
            // DataFlowLayoutPanelList
            // 
            this.DataFlowLayoutPanelList.AutoScroll = true;
            this.DataFlowLayoutPanelList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.DataFlowLayoutPanelList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataFlowLayoutPanelList.Location = new System.Drawing.Point(3, 3);
            this.DataFlowLayoutPanelList.Name = "DataFlowLayoutPanelList";
            this.DataFlowLayoutPanelList.Size = new System.Drawing.Size(1071, 899);
            this.DataFlowLayoutPanelList.TabIndex = 4;
            this.DataFlowLayoutPanelList.MouseEnter += new System.EventHandler(this.flowLayoutPanel1_MouseEnter);
            this.DataFlowLayoutPanelList.Resize += new System.EventHandler(this.flowLayoutPanel1_Resize);
            // 
            // DataPanelFooter
            // 
            this.DataPanelFooter.BackColor = System.Drawing.Color.White;
            this.DataPanelFooter.Controls.Add(this.DataRightAdvancedImageButton);
            this.DataPanelFooter.Controls.Add(this.DataLeftAdvancedImageButton);
            this.DataPanelFooter.Controls.Add(this.DataAddAdvancedImageButton);
            this.DataPanelFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataPanelFooter.Location = new System.Drawing.Point(3, 918);
            this.DataPanelFooter.Name = "DataPanelFooter";
            this.DataPanelFooter.Size = new System.Drawing.Size(1071, 78);
            this.DataPanelFooter.TabIndex = 0;
            // 
            // DataAddAdvancedImageButton
            // 
            this.DataAddAdvancedImageButton.ImageHover = ((System.Drawing.Image)(resources.GetObject("DataAddAdvancedImageButton.ImageHover")));
            this.DataAddAdvancedImageButton.ImageNormal = ((System.Drawing.Image)(resources.GetObject("DataAddAdvancedImageButton.ImageNormal")));
            this.DataAddAdvancedImageButton.Location = new System.Drawing.Point(2, 3);
            this.DataAddAdvancedImageButton.Name = "DataAddAdvancedImageButton";
            this.DataAddAdvancedImageButton.Size = new System.Drawing.Size(72, 72);
            this.DataAddAdvancedImageButton.TabIndex = 15;
            // 
            // DataLeftAdvancedImageButton
            // 
            this.DataLeftAdvancedImageButton.ImageHover = ((System.Drawing.Image)(resources.GetObject("DataLeftAdvancedImageButton.ImageHover")));
            this.DataLeftAdvancedImageButton.ImageNormal = ((System.Drawing.Image)(resources.GetObject("DataLeftAdvancedImageButton.ImageNormal")));
            this.DataLeftAdvancedImageButton.Location = new System.Drawing.Point(454, 3);
            this.DataLeftAdvancedImageButton.Name = "DataLeftAdvancedImageButton";
            this.DataLeftAdvancedImageButton.Size = new System.Drawing.Size(72, 72);
            this.DataLeftAdvancedImageButton.TabIndex = 16;
            // 
            // DataRightAdvancedImageButton
            // 
            this.DataRightAdvancedImageButton.ImageHover = ((System.Drawing.Image)(resources.GetObject("DataRightAdvancedImageButton.ImageHover")));
            this.DataRightAdvancedImageButton.ImageNormal = ((System.Drawing.Image)(resources.GetObject("DataRightAdvancedImageButton.ImageNormal")));
            this.DataRightAdvancedImageButton.Location = new System.Drawing.Point(532, 2);
            this.DataRightAdvancedImageButton.Name = "DataRightAdvancedImageButton";
            this.DataRightAdvancedImageButton.Size = new System.Drawing.Size(72, 72);
            this.DataRightAdvancedImageButton.TabIndex = 17;
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
            this.Name = "Form1";
            this.WindowHeaderPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.WindowIconPanel.ResumeLayout(false);
            this.DashboardPanel.ResumeLayout(false);
            this.DataPanelMain.ResumeLayout(false);
            this.DataTableLayoutPanelMain.ResumeLayout(false);
            this.DataPanelDetails.ResumeLayout(false);
            this.DataPanelDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataPictureBoxDetailsLogo)).EndInit();
            this.DataTableLayoutPanelSubLeft.ResumeLayout(false);
            this.DataPanelFooter.ResumeLayout(false);
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
        private System.Windows.Forms.Panel DataPanelFooter;
        private System.Windows.Forms.Panel WindowIconPanel;
        private WindowButton ButtonMinimize;
        private WindowButton ButtonMaximize;
        private WindowButton ButtonClose;
        private System.Windows.Forms.Label WindowHeaderLabelLogo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label DataLabelDetailsHostname;
        private System.Windows.Forms.PictureBox DataPictureBoxDetailsLogo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private DynamicLink dynamicLink1;
        private AdvancedImageButton DataEditAdvancedImageButton;
        private AdvancedImageButton DataRemoveAdvancedImageButton;
        private AdvancedImageButton DataAddAdvancedImageButton;
        private AdvancedImageButton DataRightAdvancedImageButton;
        private AdvancedImageButton DataLeftAdvancedImageButton;
    }
}

