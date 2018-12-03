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
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.DataPanelMain = new System.Windows.Forms.Panel();
            this.DataTableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.DataPanelDetails = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.advancedButton2 = new pmdbs.AdvancedButton();
            this.advancedButton1 = new pmdbs.AdvancedButton();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.DataPanelMain.SuspendLayout();
            this.DataTableLayoutPanelMain.SuspendLayout();
            this.DataPanelDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.button7);
            this.panel1.Controls.Add(this.button6);
            this.panel1.Controls.Add(this.button5);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1920, 63);
            this.panel1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(11, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(53, 54);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(1755, 12);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(47, 45);
            this.button7.TabIndex = 6;
            this.button7.Text = "?";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(1808, 12);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(47, 45);
            this.button6.TabIndex = 5;
            this.button6.Text = "_";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(1861, 12);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(47, 45);
            this.button5.TabIndex = 4;
            this.button5.Text = "X";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.advancedButton2);
            this.panel2.Controls.Add(this.advancedButton1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 63);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(107, 1017);
            this.panel2.TabIndex = 1;
            // 
            // DataPanelMain
            // 
            this.DataPanelMain.BackColor = System.Drawing.Color.DarkGray;
            this.DataPanelMain.Controls.Add(this.DataTableLayoutPanelMain);
            this.DataPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataPanelMain.Location = new System.Drawing.Point(107, 63);
            this.DataPanelMain.Name = "DataPanelMain";
            this.DataPanelMain.Size = new System.Drawing.Size(1813, 1017);
            this.DataPanelMain.TabIndex = 2;
            // 
            // DataTableLayoutPanelMain
            // 
            this.DataTableLayoutPanelMain.BackColor = System.Drawing.Color.Gainsboro;
            this.DataTableLayoutPanelMain.ColumnCount = 5;
            this.DataTableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.DataTableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.DataTableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.DataTableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.DataTableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.DataTableLayoutPanelMain.Controls.Add(this.DataPanelDetails, 3, 1);
            this.DataTableLayoutPanelMain.Controls.Add(this.flowLayoutPanel1, 1, 1);
            this.DataTableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataTableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.DataTableLayoutPanelMain.Name = "DataTableLayoutPanelMain";
            this.DataTableLayoutPanelMain.RowCount = 3;
            this.DataTableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.DataTableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.DataTableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.DataTableLayoutPanelMain.Size = new System.Drawing.Size(1813, 1017);
            this.DataTableLayoutPanelMain.TabIndex = 0;
            // 
            // DataPanelDetails
            // 
            this.DataPanelDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.DataPanelDetails.Controls.Add(this.label4);
            this.DataPanelDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataPanelDetails.Location = new System.Drawing.Point(1093, 23);
            this.DataPanelDetails.Name = "DataPanelDetails";
            this.DataPanelDetails.Size = new System.Drawing.Size(694, 971);
            this.DataPanelDetails.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(23, 372);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(646, 64);
            this.label4.TabIndex = 0;
            this.label4.Text = "THIS SHOWS DETAILS";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(23, 23);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1044, 971);
            this.flowLayoutPanel1.TabIndex = 4;
            this.flowLayoutPanel1.Resize += new System.EventHandler(this.flowLayoutPanel1_Resize);
            // 
            // advancedButton2
            // 
            this.advancedButton2.ColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(96)))), ((int)(((byte)(49)))));
            this.advancedButton2.ColorNormal = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.advancedButton2.FontHover = new System.Drawing.Font("Century Gothic", 10F);
            this.advancedButton2.FontNormal = new System.Drawing.Font("Century Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.advancedButton2.ImageHover = ((System.Drawing.Image)(resources.GetObject("advancedButton2.ImageHover")));
            this.advancedButton2.ImageNormal = ((System.Drawing.Image)(resources.GetObject("advancedButton2.ImageNormal")));
            this.advancedButton2.Location = new System.Drawing.Point(3, 120);
            this.advancedButton2.Name = "advancedButton2";
            this.advancedButton2.Size = new System.Drawing.Size(106, 114);
            this.advancedButton2.TabIndex = 1;
            this.advancedButton2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.advancedButton2.TextHover = "Settings";
            this.advancedButton2.TextNormal = "Settings";
            // 
            // advancedButton1
            // 
            this.advancedButton1.ColorHover = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(96)))), ((int)(((byte)(49)))));
            this.advancedButton1.ColorNormal = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.advancedButton1.FontHover = new System.Drawing.Font("Century Gothic", 10F);
            this.advancedButton1.FontNormal = new System.Drawing.Font("Century Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.advancedButton1.ImageHover = ((System.Drawing.Image)(resources.GetObject("advancedButton1.ImageHover")));
            this.advancedButton1.ImageNormal = ((System.Drawing.Image)(resources.GetObject("advancedButton1.ImageNormal")));
            this.advancedButton1.Location = new System.Drawing.Point(1, 0);
            this.advancedButton1.Name = "advancedButton1";
            this.advancedButton1.Size = new System.Drawing.Size(106, 114);
            this.advancedButton1.TabIndex = 0;
            this.advancedButton1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.advancedButton1.TextHover = "Home";
            this.advancedButton1.TextNormal = "Home";
            this.advancedButton1.Click += new System.EventHandler(this.advancedButton1_Click);
            this.advancedButton1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.advancedButton1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1920, 1080);
            this.Controls.Add(this.DataPanelMain);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.DataPanelMain.ResumeLayout(false);
            this.DataTableLayoutPanelMain.ResumeLayout(false);
            this.DataPanelDetails.ResumeLayout(false);
            this.DataPanelDetails.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Panel DataPanelMain;
        private System.Windows.Forms.TableLayoutPanel DataTableLayoutPanelMain;
        private AdvancedButton advancedButton1;
        private AdvancedButton advancedButton2;
        private System.Windows.Forms.Panel DataPanelDetails;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}

