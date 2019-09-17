using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using pmdbs.Properties;

namespace pmdbs
{
    public partial class ConfirmationForm : MetroForm
    {
        public ConfirmationForm(string text)
        {
            InitializeComponent();
            WinAPI.PreventFlickering(this);
            LabelContent.Text = text;
            WindowButtonClose.OnClickEvent += WindowButtonClose_Click;
            SetDarkTheme(false);
        }
        public ConfirmationForm(string text, bool useDarkTheme)
        {
            InitializeComponent();
            LabelContent.Text = text;
            WindowButtonClose.OnClickEvent += WindowButtonClose_Click;
            SetDarkTheme(useDarkTheme);
        }
        public ConfirmationForm(string text, string header)
        {
            InitializeComponent();
            LabelContent.Text = text;
            this.Text = header;
            WindowButtonClose.OnClickEvent += WindowButtonClose_Click;
            SetDarkTheme(false);
        }
        public ConfirmationForm(string text, string header, bool useDarkTheme)
        {
            InitializeComponent();
            LabelContent.Text = text;
            this.Text = header;
            WindowButtonClose.OnClickEvent += WindowButtonClose_Click;
            SetDarkTheme(useDarkTheme);
        }
        public ConfirmationForm(string text, MessageBoxButtons buttons)
        {
            InitializeComponent();
            switch (buttons)
            {
                case MessageBoxButtons.YesNo:
                    {
                        AnimatedButtonOk.Text = "YES";
                        animatedButtonCancel.Text = "No";
                        break;
                    }
                default: { break; }
            }
            LabelContent.Text = text;
            WindowButtonClose.OnClickEvent += WindowButtonClose_Click;
            SetDarkTheme(false);
        }
        public ConfirmationForm(string text, MessageBoxButtons buttons, bool useDarkTheme)
        {
            InitializeComponent();
            switch (buttons)
            {
                case MessageBoxButtons.YesNo:
                    {
                        AnimatedButtonOk.Text = "YES";
                        animatedButtonCancel.Text = "No";
                        break;
                    }
                default: { break; }
            }
            LabelContent.Text = text;
            WindowButtonClose.OnClickEvent += WindowButtonClose_Click;
            SetDarkTheme(useDarkTheme);
        }

        public ConfirmationForm(string text, string header, MessageBoxButtons buttons)
        {
            InitializeComponent();
            switch (buttons)
            {
                case MessageBoxButtons.YesNo:
                    {
                        AnimatedButtonOk.Text = "YES";
                        animatedButtonCancel.Text = "No";
                        break;
                    }
                default: { break; }
            }
            LabelContent.Text = text;
            this.Text = header;
            WindowButtonClose.OnClickEvent += WindowButtonClose_Click;
            SetDarkTheme(false);
        }
        public ConfirmationForm(string text, string header, MessageBoxButtons buttons, bool useDarkTheme)
        {
            InitializeComponent();
            switch (buttons)
            {
                case MessageBoxButtons.YesNo:
                    {
                        AnimatedButtonOk.Text = "YES";
                        animatedButtonCancel.Text = "No";
                        break;
                    }
                default: { break; }
            }
            LabelContent.Text = text;
            this.Text = header;
            WindowButtonClose.OnClickEvent += WindowButtonClose_Click;
            SetDarkTheme(useDarkTheme);
        }

        private void SetDarkTheme(bool useDarkTheme)
        {
            this.Theme = useDarkTheme ? MetroFramework.MetroThemeStyle.Dark : MetroFramework.MetroThemeStyle.Light;
            LabelContent.ForeColor = useDarkTheme ? Color.White : Color.Black;
            WindowButtonClose.ImageNormal = useDarkTheme ? Resources.close : Resources.closeLight_2;
            WindowButtonClose.BackgroundColorNormal = useDarkTheme ? Color.FromArgb(17, 17, 17) : Color.White;
            this.Invalidate();
        }

        private void animatedButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
            this.Dispose();
        }

        private void AnimatedButtonOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
            this.Dispose();
        }

        private void WindowButtonClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
            this.Dispose();
        }
    }
}
