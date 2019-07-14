namespace pmdbs
{
    partial class BreachForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BreachForm));
            this.panelMain = new System.Windows.Forms.Panel();
            this.animatedButtonClose = new LunaForms.AnimatedButton();
            this.animatedButtonIgnore = new LunaForms.AnimatedButton();
            this.labelInfo = new System.Windows.Forms.Label();
            this.labelInfoHeader = new System.Windows.Forms.Label();
            this.lunaTextPanelDescription = new pmdbs.LunaTextPanel();
            this.lunaItemListData = new pmdbs.LunaItemList();
            this.labelPwnCount = new System.Windows.Forms.Label();
            this.labelPwnCountHeader = new System.Windows.Forms.Label();
            this.labelDataHeader = new System.Windows.Forms.Label();
            this.labelBreachDate = new System.Windows.Forms.Label();
            this.labelDescriptionHeader = new System.Windows.Forms.Label();
            this.labelBreachDateHeader = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lunaSmallCardIsVerified = new LunaForms.LunaSmallCard();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.WindowHeaderLabelLogo = new System.Windows.Forms.Label();
            this.windowButtonClose = new LunaForms.WindowButton();
            this.windowButtonMinimize = new LunaForms.WindowButton();
            this.panelMain.SuspendLayout();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.White;
            this.panelMain.Controls.Add(this.animatedButtonClose);
            this.panelMain.Controls.Add(this.animatedButtonIgnore);
            this.panelMain.Controls.Add(this.labelInfo);
            this.panelMain.Controls.Add(this.labelInfoHeader);
            this.panelMain.Controls.Add(this.lunaTextPanelDescription);
            this.panelMain.Controls.Add(this.lunaItemListData);
            this.panelMain.Controls.Add(this.labelPwnCount);
            this.panelMain.Controls.Add(this.labelPwnCountHeader);
            this.panelMain.Controls.Add(this.labelDataHeader);
            this.panelMain.Controls.Add(this.labelBreachDate);
            this.panelMain.Controls.Add(this.labelDescriptionHeader);
            this.panelMain.Controls.Add(this.labelBreachDateHeader);
            this.panelMain.Controls.Add(this.labelTitle);
            this.panelMain.Controls.Add(this.panelHeader);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(20, 60);
            this.panelMain.Margin = new System.Windows.Forms.Padding(0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1160, 944);
            this.panelMain.TabIndex = 0;
            // 
            // animatedButtonClose
            // 
            this.animatedButtonClose.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.animatedButtonClose.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(128)))), ((int)(((byte)(235)))));
            this.animatedButtonClose.Depth = 0;
            this.animatedButtonClose.Icon = null;
            this.animatedButtonClose.Location = new System.Drawing.Point(236, 886);
            this.animatedButtonClose.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.animatedButtonClose.MouseState = LunaForms.AnimatedButton.MouseStateBase.HOVER;
            this.animatedButtonClose.Name = "animatedButtonClose";
            this.animatedButtonClose.Primary = false;
            this.animatedButtonClose.Size = new System.Drawing.Size(293, 42);
            this.animatedButtonClose.TabIndex = 19;
            this.animatedButtonClose.Text = "Close";
            this.animatedButtonClose.UseVisualStyleBackColor = true;
            // 
            // animatedButtonIgnore
            // 
            this.animatedButtonIgnore.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.animatedButtonIgnore.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(128)))), ((int)(((byte)(235)))));
            this.animatedButtonIgnore.Depth = 0;
            this.animatedButtonIgnore.Icon = null;
            this.animatedButtonIgnore.Location = new System.Drawing.Point(631, 886);
            this.animatedButtonIgnore.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.animatedButtonIgnore.MouseState = LunaForms.AnimatedButton.MouseStateBase.HOVER;
            this.animatedButtonIgnore.Name = "animatedButtonIgnore";
            this.animatedButtonIgnore.Primary = false;
            this.animatedButtonIgnore.Size = new System.Drawing.Size(293, 42);
            this.animatedButtonIgnore.TabIndex = 18;
            this.animatedButtonIgnore.Text = "Ignore this breach";
            this.animatedButtonIgnore.UseVisualStyleBackColor = true;
            // 
            // labelInfo
            // 
            this.labelInfo.AutoSize = true;
            this.labelInfo.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.labelInfo.Location = new System.Drawing.Point(625, 699);
            this.labelInfo.MaximumSize = new System.Drawing.Size(520, 0);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(507, 160);
            this.labelInfo.TabIndex = 16;
            this.labelInfo.Text = "If you had an account at #HOSTNAME during the time of the breach you should consi" +
    "der changing the password of this and any accounts that use the same or similar " +
    "passwords.";
            // 
            // labelInfoHeader
            // 
            this.labelInfoHeader.AutoSize = true;
            this.labelInfoHeader.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.labelInfoHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelInfoHeader.Location = new System.Drawing.Point(624, 646);
            this.labelInfoHeader.Name = "labelInfoHeader";
            this.labelInfoHeader.Size = new System.Drawing.Size(311, 41);
            this.labelInfoHeader.TabIndex = 15;
            this.labelInfoHeader.Text = "What does this mean?";
            // 
            // lunaTextPanelDescription
            // 
            this.lunaTextPanelDescription.IsScrollable = true;
            this.lunaTextPanelDescription.Location = new System.Drawing.Point(625, 288);
            this.lunaTextPanelDescription.LunaScrollBarBackColorScrollBar = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.lunaTextPanelDescription.LunaScrollBarForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lunaTextPanelDescription.LunaScrollBarForeColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.lunaTextPanelDescription.Margin = new System.Windows.Forms.Padding(0);
            this.lunaTextPanelDescription.Name = "lunaTextPanelDescription";
            this.lunaTextPanelDescription.ScrollBarMargin = 25;
            this.lunaTextPanelDescription.ShowBorderOnScrollBarShown = true;
            this.lunaTextPanelDescription.Size = new System.Drawing.Size(500, 335);
            this.lunaTextPanelDescription.TabIndex = 14;
            // 
            // lunaItemListData
            // 
            this.lunaItemListData.ImageLocation = new System.Drawing.Point(1, 1);
            this.lunaItemListData.IsScrollable = true;
            this.lunaItemListData.Location = new System.Drawing.Point(29, 524);
            this.lunaItemListData.LunaItemBackColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lunaItemListData.LunaItemBackColorNormal = System.Drawing.Color.White;
            this.lunaItemListData.LunaItemFont = new System.Drawing.Font("Segoe UI", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.lunaItemListData.LunaItemForeColorHeader = System.Drawing.Color.Black;
            this.lunaItemListData.LunaItemForeColorHeaderHover = System.Drawing.Color.Black;
            this.lunaItemListData.LunaItemForeColorInfo = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lunaItemListData.LunaItemForeColorInfoHover = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lunaItemListData.LunaItemHeader = "Item";
            this.lunaItemListData.LunaItemHeaderLocation = new System.Drawing.Point(70, 10);
            this.lunaItemListData.LunaItemInfoLocation = new System.Drawing.Point(72, 35);
            this.lunaItemListData.LunaItemShowInfo = false;
            this.lunaItemListData.LunaScrollBarBackColorScrollBar = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.lunaItemListData.LunaScrollBarForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lunaItemListData.LunaScrollBarForeColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.lunaItemListData.Margin = new System.Windows.Forms.Padding(0);
            this.lunaItemListData.Name = "lunaItemListData";
            this.lunaItemListData.ScrollBarMargin = 30;
            this.lunaItemListData.SeperatorVerticalPadding = 6;
            this.lunaItemListData.ShowBorderOnScrollBarShown = true;
            this.lunaItemListData.Size = new System.Drawing.Size(500, 335);
            this.lunaItemListData.TabIndex = 11;
            // 
            // labelPwnCount
            // 
            this.labelPwnCount.AutoSize = true;
            this.labelPwnCount.Font = new System.Drawing.Font("Segoe UI", 20F);
            this.labelPwnCount.Location = new System.Drawing.Point(20, 404);
            this.labelPwnCount.MaximumSize = new System.Drawing.Size(500, 0);
            this.labelPwnCount.Name = "labelPwnCount";
            this.labelPwnCount.Size = new System.Drawing.Size(217, 54);
            this.labelPwnCount.TabIndex = 10;
            this.labelPwnCount.Text = "14.936.670";
            // 
            // labelPwnCountHeader
            // 
            this.labelPwnCountHeader.AutoSize = true;
            this.labelPwnCountHeader.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.labelPwnCountHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelPwnCountHeader.Location = new System.Drawing.Point(22, 351);
            this.labelPwnCountHeader.Name = "labelPwnCountHeader";
            this.labelPwnCountHeader.Size = new System.Drawing.Size(262, 41);
            this.labelPwnCountHeader.TabIndex = 9;
            this.labelPwnCountHeader.Text = "Affected accounts:";
            // 
            // labelDataHeader
            // 
            this.labelDataHeader.AutoSize = true;
            this.labelDataHeader.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.labelDataHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelDataHeader.Location = new System.Drawing.Point(22, 475);
            this.labelDataHeader.Name = "labelDataHeader";
            this.labelDataHeader.Size = new System.Drawing.Size(281, 41);
            this.labelDataHeader.TabIndex = 7;
            this.labelDataHeader.Text = "Compromized data:";
            this.labelDataHeader.Click += new System.EventHandler(this.label5_Click);
            // 
            // labelBreachDate
            // 
            this.labelBreachDate.AutoSize = true;
            this.labelBreachDate.Font = new System.Drawing.Font("Segoe UI", 20F);
            this.labelBreachDate.Location = new System.Drawing.Point(23, 288);
            this.labelBreachDate.MaximumSize = new System.Drawing.Size(500, 0);
            this.labelBreachDate.Name = "labelBreachDate";
            this.labelBreachDate.Size = new System.Drawing.Size(231, 54);
            this.labelBreachDate.TabIndex = 6;
            this.labelBreachDate.Text = "2015-03-01";
            // 
            // labelDescriptionHeader
            // 
            this.labelDescriptionHeader.AutoSize = true;
            this.labelDescriptionHeader.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.labelDescriptionHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelDescriptionHeader.Location = new System.Drawing.Point(624, 237);
            this.labelDescriptionHeader.Name = "labelDescriptionHeader";
            this.labelDescriptionHeader.Size = new System.Drawing.Size(176, 41);
            this.labelDescriptionHeader.TabIndex = 4;
            this.labelDescriptionHeader.Text = "Description:";
            this.labelDescriptionHeader.Click += new System.EventHandler(this.labelDescriptionHeader_Click);
            // 
            // labelBreachDateHeader
            // 
            this.labelBreachDateHeader.AutoSize = true;
            this.labelBreachDateHeader.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.labelBreachDateHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelBreachDateHeader.Location = new System.Drawing.Point(22, 237);
            this.labelBreachDateHeader.Name = "labelBreachDateHeader";
            this.labelBreachDateHeader.Size = new System.Drawing.Size(181, 41);
            this.labelBreachDateHeader.TabIndex = 2;
            this.labelBreachDateHeader.Text = "Breach date:";
            // 
            // labelTitle
            // 
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 25F);
            this.labelTitle.Location = new System.Drawing.Point(0, 150);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(1160, 78);
            this.labelTitle.TabIndex = 1;
            this.labelTitle.Text = "Microsoft";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.panelHeader.Controls.Add(this.lunaSmallCardIsVerified);
            this.panelHeader.Controls.Add(this.pictureBoxLogo);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1160, 150);
            this.panelHeader.TabIndex = 0;
            // 
            // lunaSmallCardIsVerified
            // 
            this.lunaSmallCardIsVerified.BackColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.lunaSmallCardIsVerified.BackColorImage = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.lunaSmallCardIsVerified.BackColorNormal = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.lunaSmallCardIsVerified.ForeColorHeader = System.Drawing.Color.White;
            this.lunaSmallCardIsVerified.ForeColorHeaderHover = System.Drawing.Color.White;
            this.lunaSmallCardIsVerified.ForeColorInfo = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lunaSmallCardIsVerified.ForeColorInfoHover = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.lunaSmallCardIsVerified.Header = "Verified";
            this.lunaSmallCardIsVerified.HeaderLocation = new System.Drawing.Point(70, 10);
            this.lunaSmallCardIsVerified.Image = global::pmdbs.Properties.Resources.confirmed2;
            this.lunaSmallCardIsVerified.ImageLocation = new System.Drawing.Point(1, 1);
            this.lunaSmallCardIsVerified.InfoLocation = new System.Drawing.Point(72, 35);
            this.lunaSmallCardIsVerified.Location = new System.Drawing.Point(936, 90);
            this.lunaSmallCardIsVerified.Margin = new System.Windows.Forms.Padding(0);
            this.lunaSmallCardIsVerified.Name = "lunaSmallCardIsVerified";
            this.lunaSmallCardIsVerified.ShowBorder = false;
            this.lunaSmallCardIsVerified.Size = new System.Drawing.Size(224, 60);
            this.lunaSmallCardIsVerified.TabIndex = 14;
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
            this.WindowHeaderLabelLogo.Size = new System.Drawing.Size(288, 45);
            this.WindowHeaderLabelLogo.TabIndex = 14;
            this.WindowHeaderLabelLogo.Text = "pmdbs breach info";
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
            this.windowButtonClose.TabIndex = 13;
            this.windowButtonClose.OnClickEvent += new System.EventHandler(this.windowButtonClose_OnClickEvent);
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
            this.windowButtonMinimize.TabIndex = 15;
            this.windowButtonMinimize.OnClickEvent += new System.EventHandler(this.windowButtonMinimize_OnClickEvent);
            // 
            // BreachForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 1024);
            this.Controls.Add(this.windowButtonMinimize);
            this.Controls.Add(this.WindowHeaderLabelLogo);
            this.Controls.Add(this.windowButtonClose);
            this.Controls.Add(this.panelMain);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BreachForm";
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Black;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.Load += new System.EventHandler(this.BreachForm_Load);
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            this.panelHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private LunaForms.WindowButton windowButtonClose;
        private System.Windows.Forms.Label WindowHeaderLabelLogo;
        private System.Windows.Forms.Label labelPwnCount;
        private System.Windows.Forms.Label labelPwnCountHeader;
        private System.Windows.Forms.Label labelDataHeader;
        private System.Windows.Forms.Label labelBreachDate;
        private System.Windows.Forms.Label labelDescriptionHeader;
        private System.Windows.Forms.Label labelBreachDateHeader;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private LunaItemList lunaItemListData;
        private LunaForms.WindowButton windowButtonMinimize;
        private LunaTextPanel lunaTextPanelDescription;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Label labelInfoHeader;
        private LunaForms.AnimatedButton animatedButtonClose;
        private LunaForms.AnimatedButton animatedButtonIgnore;
        private LunaForms.LunaSmallCard lunaSmallCardIsVerified;
    }
}