using pmdbs.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace pmdbs
{
    public partial class ErrorForm : MetroFramework.Forms.MetroForm
    {
        public ErrorForm(string _message, string _type)
        {
            InitializeComponent();
            this.ErrorFormLabelContent.Text = _message;
            this.ErrorFormLabelHeader.Text = string.Empty;
            this.Text = _type;
            this.BackColor = Color.FromArgb(240, 71, 71);
            ErrorFormWindowButtonClose.OnClickEvent += ErrorFormWindowButtonClose_Click;
            SetDarkTheme(false);
        }
        public ErrorForm(string _message, string _type, bool useDarkTheme)
        {
            InitializeComponent();
            this.ErrorFormLabelContent.Text = _message;
            this.ErrorFormLabelHeader.Text = string.Empty;
            this.Text = _type;
            this.BackColor = Color.FromArgb(240, 71, 71);
            ErrorFormWindowButtonClose.OnClickEvent += ErrorFormWindowButtonClose_Click;
            SetDarkTheme(useDarkTheme);
        }
        public ErrorForm(string _message, string _type, string _code)
        {
            InitializeComponent();
            this.ErrorFormLabelContent.Text = _message;
            this.ErrorFormLabelHeader.Text = _code;
            this.Text = _type;
            this.BackColor = Color.FromArgb(240, 71, 71);
            ErrorFormWindowButtonClose.OnClickEvent += ErrorFormWindowButtonClose_Click;
            SetDarkTheme(false);
        }
        public ErrorForm(string _message, string _type, string _code, bool useDarkTheme)
        {
            InitializeComponent();
            this.ErrorFormLabelContent.Text = _message;
            this.ErrorFormLabelHeader.Text = _code;
            this.Text = _type;
            this.BackColor = Color.FromArgb(240, 71, 71);
            ErrorFormWindowButtonClose.OnClickEvent += ErrorFormWindowButtonClose_Click;
            SetDarkTheme(useDarkTheme);
        }
        public ErrorForm(string _message, string _type, string _code, bool useDarkTheme, Image image)
        {
            InitializeComponent();
            this.ErrorFormLabelContent.Text = _message;
            this.ErrorFormLabelHeader.Text = _code;
            this.Text = _type;
            this.BackColor = Color.FromArgb(240, 71, 71);
            ErrorFormWindowButtonClose.OnClickEvent += ErrorFormWindowButtonClose_Click;
            pictureBox1.BringToFront();
            pictureBox1.Image = image;
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            SetDarkTheme(useDarkTheme);
        }

        private void SetDarkTheme(bool useDarkTheme)
        {
            ErrorFormWindowButtonClose.ImageNormal = useDarkTheme ? Resources.close : Resources.closeLight_2;
            ErrorFormWindowButtonClose.BackgroundColorNormal = useDarkTheme ? Color.FromArgb(17, 17, 17) : Color.White;
            this.Theme = useDarkTheme ? MetroFramework.MetroThemeStyle.Dark : MetroFramework.MetroThemeStyle.Light;
            ErrorFormLabelContent.ForeColor = useDarkTheme ? Color.White : Color.Black;
            ErrorFormLabelHeader.ForeColor = useDarkTheme ? Color.White : Color.Black;
            // pictureBox1.BackColor = useDarkTheme ? Color.FromArgb(17, 17, 17) : Color.White;
            pictureBox1.BackColor = Color.FromArgb(17, 17, 17);
        }

        private void ErrorFormAnimatedButtonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void ErrorFormWindowButtonClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void ErrorForm_Shown(object sender, EventArgs e)
        {
            this.Focus();
        }
    }
}
