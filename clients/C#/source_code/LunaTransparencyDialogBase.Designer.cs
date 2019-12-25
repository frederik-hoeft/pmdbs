namespace pmdbs
{
    abstract partial class LunaTransparencyDialogBase
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
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                if (container != null)
                {
                    container.Dispose();
                }
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
            this.SuspendLayout();
            // 
            // LunaTransparencyDialogBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "LunaTransparencyDialogBase";
            this.Text = "LunaTransparencyDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LunaTransparencyDialogBase_FormClosing);
            this.Load += new System.EventHandler(this.LunaTransparencyDialogBase_Load);
            this.ResumeLayout(false);

        }

        #endregion
    }
}