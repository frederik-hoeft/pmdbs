using pmdbs.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pmdbs
{
    public partial class BreachForm : MetroFramework.Forms.MetroForm
    {
        public BreachForm()
        {
            InitializeComponent();
            WinAPI.PreventFlickering(this);
        }

        public BreachForm(string date, string title, Image image, string[] data, string description, int pwnedAccounts, bool isVerified, string domain)
        {
            InitializeComponent();
            lunaTextPanelDescription.Text = Regex.Replace(description, @"<[^>]*>", "").Replace("&quot;", "\"");
            pictureBoxLogo.Image = image;
            labelBreachDate.Text = date;
            labelTitle.Text = title;
            labelPwnCount.Text = pwnedAccounts.ToString("N0", CultureInfo.InvariantCulture);
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
                int style = WinAPI.GetWindowLong(c.Handle, WinAPI.GWL_EXSTYLE);
                style |= WinAPI.WS_EX_COMPOSITED;
                WinAPI.SetWindowLong(c.Handle, WinAPI.GWL_EXSTYLE, style);
            }
        }

        private void windowButtonClose_OnClickEvent(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void windowButtonMinimize_OnClickEvent(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void BreachForm_Shown(object sender, EventArgs e)
        {
            this.Focus();
            lunaItemListData.Refresh();
            lunaTextPanelDescription.Refresh();
        }

        private void animatedButtonIgnore_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Ignore;
        }

        private void animatedButtonClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
