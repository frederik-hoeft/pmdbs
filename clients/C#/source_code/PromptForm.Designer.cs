namespace pmdbs
{
    partial class PromptForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PromptForm));
            this.WindowHeaderLabelLogo = new System.Windows.Forms.Label();
            this.WindowButtonMinimize = new LunaForms.WindowButton();
            this.WindowButtonClose = new LunaForms.WindowButton();
            this.PanelMain = new System.Windows.Forms.Panel();
            this.LinkLabelResendCode = new System.Windows.Forms.LinkLabel();
            this.LabelAction = new System.Windows.Forms.Label();
            this.LabelMailInfo = new System.Windows.Forms.Label();
            this.AnimatedButtonSubmit = new LunaForms.LunaAnimatedButton();
            this.PanelCenter = new System.Windows.Forms.Panel();
            this.LabelCode = new System.Windows.Forms.Label();
            this.EditFieldCode = new LunaForms.EditField();
            this.LabelPrefix = new System.Windows.Forms.Label();
            this.LabelTitle = new System.Windows.Forms.Label();
            this.PanelHeader = new System.Windows.Forms.Panel();
            this.PictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.PanelMain.SuspendLayout();
            this.PanelCenter.SuspendLayout();
            this.PanelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // WindowHeaderLabelLogo
            // 
            this.WindowHeaderLabelLogo.AutoSize = true;
            this.WindowHeaderLabelLogo.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.WindowHeaderLabelLogo.ForeColor = System.Drawing.Color.White;
            this.WindowHeaderLabelLogo.Location = new System.Drawing.Point(13, 11);
            this.WindowHeaderLabelLogo.Name = "WindowHeaderLabelLogo";
            this.WindowHeaderLabelLogo.Size = new System.Drawing.Size(327, 45);
            this.WindowHeaderLabelLogo.TabIndex = 18;
            this.WindowHeaderLabelLogo.Text = "pmdbs security check";
            // 
            // WindowButtonMinimize
            // 
            this.WindowButtonMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.WindowButtonMinimize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.WindowButtonMinimize.BackgroundColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.WindowButtonMinimize.BackgroundColorNormal = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.WindowButtonMinimize.ImageHover = ((System.Drawing.Image)(resources.GetObject("WindowButtonMinimize.ImageHover")));
            this.WindowButtonMinimize.ImageNormal = ((System.Drawing.Image)(resources.GetObject("WindowButtonMinimize.ImageNormal")));
            this.WindowButtonMinimize.Location = new System.Drawing.Point(1078, 7);
            this.WindowButtonMinimize.Name = "WindowButtonMinimize";
            this.WindowButtonMinimize.Size = new System.Drawing.Size(60, 50);
            this.WindowButtonMinimize.TabIndex = 20;
            this.WindowButtonMinimize.OnClickEvent += new System.EventHandler(this.WindowButtonMinimize_OnClickEvent);
            // 
            // WindowButtonClose
            // 
            this.WindowButtonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.WindowButtonClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.WindowButtonClose.BackgroundColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.WindowButtonClose.BackgroundColorNormal = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.WindowButtonClose.ImageHover = ((System.Drawing.Image)(resources.GetObject("WindowButtonClose.ImageHover")));
            this.WindowButtonClose.ImageNormal = ((System.Drawing.Image)(resources.GetObject("WindowButtonClose.ImageNormal")));
            this.WindowButtonClose.Location = new System.Drawing.Point(1138, 7);
            this.WindowButtonClose.Name = "WindowButtonClose";
            this.WindowButtonClose.Size = new System.Drawing.Size(60, 50);
            this.WindowButtonClose.TabIndex = 19;
            this.WindowButtonClose.OnClickEvent += new System.EventHandler(this.WindowButtonClose_OnClickEvent);
            // 
            // PanelMain
            // 
            this.PanelMain.BackColor = System.Drawing.Color.White;
            this.PanelMain.Controls.Add(this.LinkLabelResendCode);
            this.PanelMain.Controls.Add(this.LabelAction);
            this.PanelMain.Controls.Add(this.LabelMailInfo);
            this.PanelMain.Controls.Add(this.AnimatedButtonSubmit);
            this.PanelMain.Controls.Add(this.PanelCenter);
            this.PanelMain.Controls.Add(this.LabelTitle);
            this.PanelMain.Controls.Add(this.PanelHeader);
            this.PanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelMain.Location = new System.Drawing.Point(20, 60);
            this.PanelMain.Name = "PanelMain";
            this.PanelMain.Size = new System.Drawing.Size(1160, 944);
            this.PanelMain.TabIndex = 21;
            // 
            // LinkLabelResendCode
            // 
            this.LinkLabelResendCode.ActiveLinkColor = System.Drawing.Color.Blue;
            this.LinkLabelResendCode.DisabledLinkColor = System.Drawing.Color.Blue;
            this.LinkLabelResendCode.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.LinkLabelResendCode.Location = new System.Drawing.Point(3, 642);
            this.LinkLabelResendCode.Name = "LinkLabelResendCode";
            this.LinkLabelResendCode.Size = new System.Drawing.Size(1157, 69);
            this.LinkLabelResendCode.TabIndex = 28;
            this.LinkLabelResendCode.TabStop = true;
            this.LinkLabelResendCode.Text = "Resend code";
            this.LinkLabelResendCode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LinkLabelResendCode.VisitedLinkColor = System.Drawing.Color.Blue;
            this.LinkLabelResendCode.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelResendCode_LinkClicked);
            // 
            // LabelAction
            // 
            this.LabelAction.AutoSize = true;
            this.LabelAction.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.LabelAction.Location = new System.Drawing.Point(147, 378);
            this.LabelAction.Name = "LabelAction";
            this.LabelAction.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LabelAction.Size = new System.Drawing.Size(562, 32);
            this.LabelAction.TabIndex = 27;
            this.LabelAction.Text = "Looks like you\'re trying to login from a new device.";
            // 
            // LabelMailInfo
            // 
            this.LabelMailInfo.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.LabelMailInfo.Location = new System.Drawing.Point(147, 416);
            this.LabelMailInfo.Name = "LabelMailInfo";
            this.LabelMailInfo.Size = new System.Drawing.Size(870, 70);
            this.LabelMailInfo.TabIndex = 25;
            this.LabelMailInfo.Text = "An email containing a verification code has been sent to example@gmail.com.";
            // 
            // AnimatedButtonSubmit
            // 
            this.AnimatedButtonSubmit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AnimatedButtonSubmit.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(96)))), ((int)(((byte)(49)))));
            this.AnimatedButtonSubmit.Depth = 0;
            this.AnimatedButtonSubmit.Icon = null;
            this.AnimatedButtonSubmit.Location = new System.Drawing.Point(394, 792);
            this.AnimatedButtonSubmit.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.AnimatedButtonSubmit.MouseState = LunaForms.LunaAnimatedButton.MouseStateBase.HOVER;
            this.AnimatedButtonSubmit.Name = "AnimatedButtonSubmit";
            this.AnimatedButtonSubmit.Primary = false;
            this.AnimatedButtonSubmit.Size = new System.Drawing.Size(376, 80);
            this.AnimatedButtonSubmit.TabIndex = 24;
            this.AnimatedButtonSubmit.Text = "Confirm";
            this.AnimatedButtonSubmit.Uppercase = false;
            this.AnimatedButtonSubmit.UseVisualStyleBackColor = true;
            this.AnimatedButtonSubmit.Click += new System.EventHandler(this.AnimatedButtonSubmit_Click);
            // 
            // PanelCenter
            // 
            this.PanelCenter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PanelCenter.Controls.Add(this.LabelCode);
            this.PanelCenter.Controls.Add(this.EditFieldCode);
            this.PanelCenter.Controls.Add(this.LabelPrefix);
            this.PanelCenter.Location = new System.Drawing.Point(368, 509);
            this.PanelCenter.Name = "PanelCenter";
            this.PanelCenter.Size = new System.Drawing.Size(429, 130);
            this.PanelCenter.TabIndex = 23;
            // 
            // LabelCode
            // 
            this.LabelCode.AutoSize = true;
            this.LabelCode.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.LabelCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.LabelCode.Location = new System.Drawing.Point(12, 16);
            this.LabelCode.Name = "LabelCode";
            this.LabelCode.Size = new System.Drawing.Size(105, 28);
            this.LabelCode.TabIndex = 17;
            this.LabelCode.Text = "Enter code";
            // 
            // EditFieldCode
            // 
            this.EditFieldCode.AutoSize = true;
            this.EditFieldCode.BackColor = System.Drawing.Color.White;
            this.EditFieldCode.BackGroundColor = System.Drawing.Color.White;
            this.EditFieldCode.ColorTextBoxFocus = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(96)))), ((int)(((byte)(49)))));
            this.EditFieldCode.ColorTextBoxNormal = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.EditFieldCode.ColorTitle = System.Drawing.SystemColors.WindowText;
            this.EditFieldCode.DefaultValue = "Enter code...";
            this.EditFieldCode.FontTextBox = new System.Drawing.Font("Segoe UI", 20F);
            this.EditFieldCode.FontTitle = new System.Drawing.Font("Segoe UI", 8F);
            this.EditFieldCode.ForeColorTextBoxFocus = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(96)))), ((int)(((byte)(49)))));
            this.EditFieldCode.ForeColorTextBoxNormal = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.EditFieldCode.ImageClearHover = ((System.Drawing.Image)(resources.GetObject("EditFieldCode.ImageClearHover")));
            this.EditFieldCode.ImageClearNormal = ((System.Drawing.Image)(resources.GetObject("EditFieldCode.ImageClearNormal")));
            this.EditFieldCode.Location = new System.Drawing.Point(97, 8);
            this.EditFieldCode.Name = "EditFieldCode";
            this.EditFieldCode.Size = new System.Drawing.Size(323, 110);
            this.EditFieldCode.TabIndex = 15;
            this.EditFieldCode.TextTextBox = "";
            this.EditFieldCode.TextTitle = "";
            this.EditFieldCode.UseColoredCaret = true;
            this.EditFieldCode.UseDefaultValue = false;
            this.EditFieldCode.UseSystemPasswordChar = false;
            this.EditFieldCode.EnterKeyPressed += new System.EventHandler(this.EditFieldCode_EnterKeyPressed);
            // 
            // LabelPrefix
            // 
            this.LabelPrefix.AutoSize = true;
            this.LabelPrefix.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelPrefix.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(96)))), ((int)(((byte)(49)))));
            this.LabelPrefix.Location = new System.Drawing.Point(7, 58);
            this.LabelPrefix.Name = "LabelPrefix";
            this.LabelPrefix.Size = new System.Drawing.Size(97, 54);
            this.LabelPrefix.TabIndex = 16;
            this.LabelPrefix.Text = "PM-";
            // 
            // LabelTitle
            // 
            this.LabelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.LabelTitle.Font = new System.Drawing.Font("Segoe UI", 25F);
            this.LabelTitle.Location = new System.Drawing.Point(0, 215);
            this.LabelTitle.Name = "LabelTitle";
            this.LabelTitle.Size = new System.Drawing.Size(1160, 90);
            this.LabelTitle.TabIndex = 4;
            this.LabelTitle.Text = "Confirm new device";
            this.LabelTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // PanelHeader
            // 
            this.PanelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.PanelHeader.Controls.Add(this.PictureBoxLogo);
            this.PanelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelHeader.Location = new System.Drawing.Point(0, 0);
            this.PanelHeader.Name = "PanelHeader";
            this.PanelHeader.Size = new System.Drawing.Size(1160, 215);
            this.PanelHeader.TabIndex = 3;
            // 
            // PictureBoxLogo
            // 
            this.PictureBoxLogo.Image = ((System.Drawing.Image)(resources.GetObject("PictureBoxLogo.Image")));
            this.PictureBoxLogo.Location = new System.Drawing.Point(495, 19);
            this.PictureBoxLogo.Name = "PictureBoxLogo";
            this.PictureBoxLogo.Size = new System.Drawing.Size(175, 175);
            this.PictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureBoxLogo.TabIndex = 0;
            this.PictureBoxLogo.TabStop = false;
            // 
            // PromptForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 1024);
            this.Controls.Add(this.PanelMain);
            this.Controls.Add(this.WindowButtonMinimize);
            this.Controls.Add(this.WindowButtonClose);
            this.Controls.Add(this.WindowHeaderLabelLogo);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PromptForm";
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Black;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.PanelMain.ResumeLayout(false);
            this.PanelMain.PerformLayout();
            this.PanelCenter.ResumeLayout(false);
            this.PanelCenter.PerformLayout();
            this.PanelHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LunaForms.WindowButton WindowButtonMinimize;
        private LunaForms.WindowButton WindowButtonClose;
        private System.Windows.Forms.Label WindowHeaderLabelLogo;
        private System.Windows.Forms.Panel PanelMain;
        private System.Windows.Forms.Label LabelTitle;
        private System.Windows.Forms.Panel PanelHeader;
        private System.Windows.Forms.PictureBox PictureBoxLogo;
        private System.Windows.Forms.LinkLabel LinkLabelResendCode;
        private System.Windows.Forms.Label LabelAction;
        private System.Windows.Forms.Label LabelMailInfo;
        private LunaForms.LunaAnimatedButton AnimatedButtonSubmit;
        private System.Windows.Forms.Panel PanelCenter;
        private System.Windows.Forms.Label LabelCode;
        private LunaForms.EditField EditFieldCode;
        private System.Windows.Forms.Label LabelPrefix;
    }
}