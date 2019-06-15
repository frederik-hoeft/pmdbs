namespace pmdbs
{
    partial class ConfirmationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfirmationForm));
            this.MetroPanel = new MetroFramework.Controls.MetroPanel();
            this.animatedButtonCancel = new CustomMetroForms.AnimatedButton();
            this.CustomLabel = new CustomMetroForms.CustomLabel();
            this.AnimatedButtonOk = new CustomMetroForms.AnimatedButton();
            this.WindowButtonClose = new CustomMetroForms.WindowButton();
            this.MetroPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MetroPanel
            // 
            this.MetroPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MetroPanel.Controls.Add(this.animatedButtonCancel);
            this.MetroPanel.Controls.Add(this.CustomLabel);
            this.MetroPanel.Controls.Add(this.AnimatedButtonOk);
            this.MetroPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MetroPanel.HorizontalScrollbarBarColor = true;
            this.MetroPanel.HorizontalScrollbarHighlightOnWheel = false;
            this.MetroPanel.HorizontalScrollbarSize = 10;
            this.MetroPanel.Location = new System.Drawing.Point(20, 60);
            this.MetroPanel.Name = "MetroPanel";
            this.MetroPanel.Size = new System.Drawing.Size(600, 280);
            this.MetroPanel.TabIndex = 15;
            this.MetroPanel.Theme = MetroFramework.MetroThemeStyle.Light;
            this.MetroPanel.VerticalScrollbarBarColor = false;
            this.MetroPanel.VerticalScrollbarHighlightOnWheel = false;
            this.MetroPanel.VerticalScrollbarSize = 10;
            // 
            // animatedButtonCancel
            // 
            this.animatedButtonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.animatedButtonCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.animatedButtonCancel.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(96)))), ((int)(((byte)(49)))));
            this.animatedButtonCancel.Depth = 0;
            this.animatedButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.animatedButtonCancel.Icon = null;
            this.animatedButtonCancel.Location = new System.Drawing.Point(402, 211);
            this.animatedButtonCancel.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.animatedButtonCancel.MouseState = CustomMetroForms.AnimatedButton.MouseStateBase.HOVER;
            this.animatedButtonCancel.Name = "animatedButtonCancel";
            this.animatedButtonCancel.Primary = false;
            this.animatedButtonCancel.Size = new System.Drawing.Size(197, 68);
            this.animatedButtonCancel.TabIndex = 4;
            this.animatedButtonCancel.Text = "Cancel";
            this.animatedButtonCancel.UseVisualStyleBackColor = true;
            this.animatedButtonCancel.Click += new System.EventHandler(this.animatedButtonCancel_Click);
            // 
            // CustomLabel
            // 
            this.CustomLabel.Content = "CONFIRM_ACTION";
            this.CustomLabel.Header = "";
            this.CustomLabel.Location = new System.Drawing.Point(3, 3);
            this.CustomLabel.Name = "CustomLabel";
            this.CustomLabel.Size = new System.Drawing.Size(593, 199);
            this.CustomLabel.TabIndex = 3;
            // 
            // AnimatedButtonOk
            // 
            this.AnimatedButtonOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.AnimatedButtonOk.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AnimatedButtonOk.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(96)))), ((int)(((byte)(49)))));
            this.AnimatedButtonOk.Depth = 0;
            this.AnimatedButtonOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.AnimatedButtonOk.Icon = null;
            this.AnimatedButtonOk.Location = new System.Drawing.Point(-1, 211);
            this.AnimatedButtonOk.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.AnimatedButtonOk.MouseState = CustomMetroForms.AnimatedButton.MouseStateBase.HOVER;
            this.AnimatedButtonOk.Name = "AnimatedButtonOk";
            this.AnimatedButtonOk.Primary = false;
            this.AnimatedButtonOk.Size = new System.Drawing.Size(197, 68);
            this.AnimatedButtonOk.TabIndex = 2;
            this.AnimatedButtonOk.Text = "OK";
            this.AnimatedButtonOk.UseVisualStyleBackColor = true;
            this.AnimatedButtonOk.Click += new System.EventHandler(this.AnimatedButtonOk_Click);
            // 
            // WindowButtonClose
            // 
            this.WindowButtonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.WindowButtonClose.BackgroundColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(71)))), ((int)(((byte)(71)))));
            this.WindowButtonClose.BackgroundColorNormal = System.Drawing.Color.Empty;
            this.WindowButtonClose.ImageHover = ((System.Drawing.Image)(resources.GetObject("WindowButtonClose.ImageHover")));
            this.WindowButtonClose.ImageNormal = ((System.Drawing.Image)(resources.GetObject("WindowButtonClose.ImageNormal")));
            this.WindowButtonClose.Location = new System.Drawing.Point(581, 6);
            this.WindowButtonClose.Name = "WindowButtonClose";
            this.WindowButtonClose.Size = new System.Drawing.Size(60, 50);
            this.WindowButtonClose.TabIndex = 14;
            // 
            // ConfirmationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 360);
            this.Controls.Add(this.MetroPanel);
            this.Controls.Add(this.WindowButtonClose);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(640, 360);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 360);
            this.Movable = false;
            this.Name = "ConfirmationForm";
            this.Resizable = false;
            this.Style = MetroFramework.MetroColorStyle.Orange;
            this.Text = "Please Confirm";
            this.MetroPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroPanel MetroPanel;
        private CustomMetroForms.CustomLabel CustomLabel;
        private CustomMetroForms.AnimatedButton AnimatedButtonOk;
        private CustomMetroForms.WindowButton WindowButtonClose;
        private CustomMetroForms.AnimatedButton animatedButtonCancel;
    }
}