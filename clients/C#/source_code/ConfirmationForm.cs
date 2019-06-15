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

namespace pmdbs
{
    public partial class ConfirmationForm : MetroForm
    {
        public ConfirmationForm(string text)
        {
            InitializeComponent();
            CustomLabel.Content = text;
            WindowButtonClose.OnClickEvent += WindowButtonClose_Click;
        }
        public ConfirmationForm(string text, string header)
        {
            InitializeComponent();
            CustomLabel.Content = text;
            this.Text = header;
            WindowButtonClose.OnClickEvent += WindowButtonClose_Click;
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
            CustomLabel.Content = text;
            WindowButtonClose.OnClickEvent += WindowButtonClose_Click;
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
            CustomLabel.Content = text;
            this.Text = header;
            WindowButtonClose.OnClickEvent += WindowButtonClose_Click;
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
