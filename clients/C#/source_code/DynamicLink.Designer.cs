namespace pmdbs
{
    partial class DynamicLink
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Link = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // Link
            // 
            this.Link.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Link.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Link.Location = new System.Drawing.Point(0, 0);
            this.Link.Name = "Link";
            this.Link.Size = new System.Drawing.Size(219, 51);
            this.Link.TabIndex = 0;
            this.Link.TabStop = true;
            this.Link.Text = "https://example.com";
            this.Link.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Link_LinkClicked);
            this.Link.MouseEnter += new System.EventHandler(this.Link_MouseEnter);
            this.Link.MouseLeave += new System.EventHandler(this.Link_MouseLeave);
            // 
            // DynamicLink
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Link);
            this.Name = "DynamicLink";
            this.Size = new System.Drawing.Size(219, 51);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.LinkLabel Link;
    }
}
