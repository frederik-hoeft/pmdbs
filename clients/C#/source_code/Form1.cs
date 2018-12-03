using pmdbs.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pmdbs
{
    public partial class Form1 : Form
    {
        private List<ListEntry> entryList = new List<ListEntry>();
        public Form1()
        {
            InitializeComponent();
            WireAllControls2(advancedButton1);
            WireAllControls2(advancedButton2);
            for (int i = 0; i < 10; i++)
            {
                ListEntry listEntry1 = new ListEntry
                {
                    BackColor = Color.White,
                    FavIcon = Image.FromFile(@"C:\Users\Administrator.XEON\Desktop\favicon.ico"),
                    HostName = "GitHub",
                    HostNameFont = new Font("Century Gothic", 14F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0))),
                    HostNameForeColor = SystemColors.ControlText,
                    Name = "listEntry15",
                    Size = new Size(1041, 64),
                    TabIndex = 14,
                    TimeStamp = "2018-12-03 18:05",
                    TimeStampFont = new Font("Century Gothic", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))),
                    TimeStampForeColor = SystemColors.ControlText,
                    UserName = "Th3-Fr3d",
                    UserNameFont = new Font("Century Gothic", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))),
                    UserNameForeColor = SystemColors.ControlText
                };
                flowLayoutPanel1.Controls.Add(listEntry1);
                entryList.Add(listEntry1);
                listEntry1.MouseClick += new MouseEventHandler(this.listEntry_MouseClick);
                WireAllControls(listEntry1);
            }
        }

            

        private void WireAllControls(Control cont)
        {
            foreach (Control ctl in cont.Controls)
            {
                ctl.Click += listEntry_MouseClick;
                if (ctl.HasChildren)
                {
                    WireAllControls(ctl);
                }
            }
        }

        private void WireAllControls2(Control cont)
        {
            foreach (Control ctl in cont.Controls)
            {
                ctl.Click += advancedButton1_Click;
                if (ctl.HasChildren)
                {
                    WireAllControls2(ctl);
                }
            }
        }

        private void listEntry_MouseClick(object sender, EventArgs e)
        {
            var senderObject = (Panel)sender;
            foreach (ListEntry entry in entryList)
            {
                if (senderObject.Parent.Equals((Control)entry))
                {
                    int index = entryList.IndexOf(entry);
                    MessageBox.Show("le" + index.ToString(), "le", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void flowLayoutPanel1_Resize(object sender, EventArgs e)
        {
            foreach (ListEntry entry in flowLayoutPanel1.Controls)
            {
                entry.Width = flowLayoutPanel1.Width - 15;
            }
        }

        private void advancedButton1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("abn","abn",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void listEntry15_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void advancedButton1_Click(object sender, MouseEventArgs e)
        {

        }
    }
}
