using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pmdbs
{
    public partial class EditField : UserControl
    {
        public EditField()
        {
            InitializeComponent();
            advancedImageButton1.OnClickEvent += advancedImageButton1_Click;
            OnResized();
        }

        public Image ImageClearNormal
        {
            get { return advancedImageButton1.ImageNormal; }
            set { advancedImageButton1.ImageNormal = value; }
        }

        public Image ImageClearHover
        {
            get { return advancedImageButton1.ImageHover; }
            set { advancedImageButton1.ImageHover = value; }
        }

        public Font FontTitle
        {
            get { return textBox1.Font; }
            set { textBox1.Font = value; }
        }

        public Font FontTextBox
        {
            get { return advancedTextBox1.Font; }
            set { advancedTextBox1.Font = value; }
        }

        public String TextTitle
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        public String TextTextBox
        {
            get { return advancedTextBox1.TextValue; }
            set { advancedTextBox1.TextValue = value; }
        }

        public Color ColorTextBoxNormal
        {
            get { return advancedTextBox1.ColorNormal; }
            set { advancedTextBox1.ColorNormal = value; }
        }

        public Color ColorTextBoxFocus
        {
            get { return advancedTextBox1.ColorFocus; }
            set { advancedTextBox1.ColorFocus = value; }
        }

        private void advancedTextBox1_SizeChanged(object sender, EventArgs e)
        {
            OnResized();
        }

        private void textBox1_SizeChanged(object sender, EventArgs e)
        {
            OnResized();
        }

        private void OnResized()
        {
            this.Height = advancedTextBox1.Height + textBox1.Height + 30;
            tableLayoutPanel1.ColumnStyles[1].Width = advancedTextBox1.Height;
            advancedImageButton1.Size = new Size(advancedTextBox1.Height, advancedTextBox1.Height);
        }

        private void advancedImageButton1_Click(object sender, EventArgs e)
        {
            advancedTextBox1.TextValue = "";
            OnResized();
        }
    }
}
