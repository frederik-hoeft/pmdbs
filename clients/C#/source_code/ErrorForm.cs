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
    public partial class ErrorForm : MetroFramework.Forms.MetroForm
    {
        public ErrorForm(string _message, string _type)
        {
            InitializeComponent();
            this.ErrorFormCustomLabel.Content = _message;
            this.Text = _type;
            this.BackColor = Color.FromArgb(240, 71, 71);
            ErrorFormWindowButtonClose.OnClickEvent += ErrorFormWindowButtonClose_Click;
        }
        public ErrorForm(string _message, string _type, string _code)
        {
            InitializeComponent();
            this.ErrorFormCustomLabel.Content = _message;
            this.ErrorFormCustomLabel.Header = _code;
            this.Text = _type;
            this.BackColor = Color.FromArgb(240, 71, 71);
            ErrorFormWindowButtonClose.OnClickEvent += ErrorFormWindowButtonClose_Click;
        }

        private void ErrorFormAnimatedButtonOK_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void ErrorFormWindowButtonClose_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
