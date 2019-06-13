namespace pmdbs
{
    partial class ErrorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorForm));
            this.ErrorFormWindowButtonClose = new CustomMetroForms.WindowButton();
            this.ErrorFormMetroPanel = new MetroFramework.Controls.MetroPanel();
            this.ErrorFormCustomLabel = new CustomMetroForms.CustomLabel();
            this.ErrorFormAnimatedButtonOK = new CustomMetroForms.AnimatedButton();
            this.ErrorFormMetroPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ErrorFormWindowButtonClose
            // 
            this.ErrorFormWindowButtonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ErrorFormWindowButtonClose.BackgroundColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.ErrorFormWindowButtonClose.BackgroundColorNormal = System.Drawing.Color.Empty;
            this.ErrorFormWindowButtonClose.ImageHover = ((System.Drawing.Image)(resources.GetObject("ErrorFormWindowButtonClose.ImageHover")));
            this.ErrorFormWindowButtonClose.ImageNormal = ((System.Drawing.Image)(resources.GetObject("ErrorFormWindowButtonClose.ImageNormal")));
            this.ErrorFormWindowButtonClose.Location = new System.Drawing.Point(578, 6);
            this.ErrorFormWindowButtonClose.Name = "ErrorFormWindowButtonClose";
            this.ErrorFormWindowButtonClose.Size = new System.Drawing.Size(60, 50);
            this.ErrorFormWindowButtonClose.TabIndex = 13;
            // 
            // ErrorFormMetroPanel
            // 
            this.ErrorFormMetroPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ErrorFormMetroPanel.Controls.Add(this.ErrorFormCustomLabel);
            this.ErrorFormMetroPanel.Controls.Add(this.ErrorFormAnimatedButtonOK);
            this.ErrorFormMetroPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ErrorFormMetroPanel.HorizontalScrollbarBarColor = true;
            this.ErrorFormMetroPanel.HorizontalScrollbarHighlightOnWheel = false;
            this.ErrorFormMetroPanel.HorizontalScrollbarSize = 10;
            this.ErrorFormMetroPanel.Location = new System.Drawing.Point(20, 60);
            this.ErrorFormMetroPanel.Name = "ErrorFormMetroPanel";
            this.ErrorFormMetroPanel.Size = new System.Drawing.Size(600, 280);
            this.ErrorFormMetroPanel.TabIndex = 14;
            this.ErrorFormMetroPanel.Theme = MetroFramework.MetroThemeStyle.Light;
            this.ErrorFormMetroPanel.VerticalScrollbarBarColor = false;
            this.ErrorFormMetroPanel.VerticalScrollbarHighlightOnWheel = false;
            this.ErrorFormMetroPanel.VerticalScrollbarSize = 10;
            // 
            // ErrorFormCustomLabel
            // 
            this.ErrorFormCustomLabel.Content = "ERROR_MESSAGE";
            this.ErrorFormCustomLabel.Header = "";
            this.ErrorFormCustomLabel.Location = new System.Drawing.Point(3, 3);
            this.ErrorFormCustomLabel.Name = "ErrorFormCustomLabel";
            this.ErrorFormCustomLabel.Size = new System.Drawing.Size(593, 199);
            this.ErrorFormCustomLabel.TabIndex = 3;
            // 
            // ErrorFormAnimatedButtonOK
            // 
            this.ErrorFormAnimatedButtonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.ErrorFormAnimatedButtonOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ErrorFormAnimatedButtonOK.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.ErrorFormAnimatedButtonOK.Depth = 0;
            this.ErrorFormAnimatedButtonOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ErrorFormAnimatedButtonOK.Icon = null;
            this.ErrorFormAnimatedButtonOK.Location = new System.Drawing.Point(201, 211);
            this.ErrorFormAnimatedButtonOK.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.ErrorFormAnimatedButtonOK.MouseState = CustomMetroForms.AnimatedButton.MouseStateBase.HOVER;
            this.ErrorFormAnimatedButtonOK.Name = "ErrorFormAnimatedButtonOK";
            this.ErrorFormAnimatedButtonOK.Primary = false;
            this.ErrorFormAnimatedButtonOK.Size = new System.Drawing.Size(197, 68);
            this.ErrorFormAnimatedButtonOK.TabIndex = 2;
            this.ErrorFormAnimatedButtonOK.Text = "OK";
            this.ErrorFormAnimatedButtonOK.UseVisualStyleBackColor = true;
            this.ErrorFormAnimatedButtonOK.Click += new System.EventHandler(this.ErrorFormAnimatedButtonOK_Click);
            // 
            // ErrorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ErrorFormAnimatedButtonOK;
            this.ClientSize = new System.Drawing.Size(640, 360);
            this.Controls.Add(this.ErrorFormMetroPanel);
            this.Controls.Add(this.ErrorFormWindowButtonClose);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(640, 360);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 360);
            this.Name = "ErrorForm";
            this.Style = MetroFramework.MetroColorStyle.Red;
            this.Text = "ERROR_TYPE";
            this.ErrorFormMetroPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private CustomMetroForms.WindowButton ErrorFormWindowButtonClose;
        private MetroFramework.Controls.MetroPanel ErrorFormMetroPanel;
        private CustomMetroForms.CustomLabel ErrorFormCustomLabel;
        private CustomMetroForms.AnimatedButton ErrorFormAnimatedButtonOK;
    }
}