using pmdbs.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static pmdbs.Form1;

namespace pmdbs
{
    public partial class BreachForm : MetroFramework.Forms.MetroForm
    {
        public BreachForm()
        {
            InitializeComponent();
        }

        public BreachForm(string date, string title, Image image, string[] data, string description, int pwnedAccounts, bool isVerified, string domain)
        {
            InitializeComponent();
            lunaItemListData.LunaItemClicked += card_click;
            lunaTextPanelDescription.Text = Regex.Replace(description, @"<[^>]*>", "").Replace("&quot;", "\"");
            pictureBoxLogo.Image = image;
            labelBreachDate.Text = date;
            labelTitle.Text = title;
            labelPwnCount.Text = string.Format("{0:#,0}", pwnedAccounts.ToString());
            if (isVerified)
            {
                lunaSmallCardIsVerified.Image = Resources.confirmed2;
                lunaSmallCardIsVerified.Header = "Verified";
            }
            else
            {
                lunaSmallCardIsVerified.Image = Resources.breach;
                lunaSmallCardIsVerified.Header = "Unverified";
            }
            labelInfo.Text = labelInfo.Text.Replace("#HOSTNAME", domain);
            for (int i = 0; i < data.Length; i++)
            {
                lunaItemListData.Add(data[i], Resources.breach, i.ToString(), i);
            }
            // PREVENT FLICKERING
            foreach (Control c in this.Controls)
            {
                int style = NativeWinAPI.GetWindowLong(c.Handle, NativeWinAPI.GWL_EXSTYLE);
                style |= NativeWinAPI.WS_EX_COMPOSITED;
                NativeWinAPI.SetWindowLong(c.Handle, NativeWinAPI.GWL_EXSTYLE, style);
            }
            lunaItemListData.Refresh();
            lunaTextPanelDescription.Refresh();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            lunaItemListData.Refresh();
        }
        private void card_click(object sender, EventArgs e)
        {
            LunaItem item = (LunaItem)sender;
            CustomException.ThrowNew.NotImplementedException("item[" + item.Index.ToString() + "]");
        }

        private void windowButtonClose_OnClickEvent(object sender, EventArgs e)
        {
            // this.DialogResult = DialogResult.Cancel;
            // this.Close();
            // this.Dispose();
            Application.Exit();
        }

        private void windowButtonMinimize_OnClickEvent(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void labelDescriptionHeader_Click(object sender, EventArgs e)
        {
            lunaTextPanelDescription.Refresh();
        }

        private void BreachForm_Load(object sender, EventArgs e)
        {
            lunaItemListData.Refresh();
            lunaTextPanelDescription.Refresh();
        }
    }
}
