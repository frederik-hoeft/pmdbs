using pmdbs.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pmdbs
{
    public partial class BreachForm : MetroFramework.Forms.MetroForm
    {
        public BreachForm()
        {
            InitializeComponent();
        }

        public BreachForm(string date, string title, Image image, string[] data, string description, int pwnedAccounts, bool isVerified)
        {
            InitializeComponent();
            lunaItemList1.LunaItemClicked += card_click;
            label3.Text = Regex.Replace(description, @"<[^>]*>", "").Replace("&quot;", "\"");
            pictureBox1.Image = image;
            label6.Text = date;
            label1.Text = title;
            label7.Text = string.Format("{0:#,0}", pwnedAccounts.ToString());
            if (isVerified)
            {
                lunaItem2.Image = Resources.confirmed2;
                lunaItem2.Header = "Verified";
            }
            else
            {
                lunaItem2.Image = Resources.breach;
                lunaItem2.Header = "Unverified";
            }
            for (int i = 0; i < data.Length; i++)
            {
                lunaItemList1.Add(data[i], Resources.breach, i.ToString(), i);
            }
        }

        private int a = 0;
        private void label5_Click(object sender, EventArgs e)
        {
            lunaItemList1.Add("Test item", Resources.breach, a.ToString(), a);
            a++;
        }
        private void card_click(object sender, EventArgs e)
        {
            LunaItem item = (LunaItem)sender;
            CustomException.ThrowNew.NotImplementedException("item[" + item.Index.ToString() + "]");
        }

        private void windowButtonClose_OnClickEvent(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
            this.Dispose();
            // Application.Exit();
        }

        private void windowButtonMinimize_OnClickEvent(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
