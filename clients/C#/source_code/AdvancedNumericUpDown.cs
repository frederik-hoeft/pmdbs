using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace pmdbs
{
    public partial class AdvancedNumericUpDown : UserControl
    {
        private Color NormalColor = Color.FromArgb(255, 255, 255);
        private Color FocusColor = Color.FromArgb(255, 96, 49);
        private Color NormalForeColor = Color.FromArgb(33, 33, 33);
        private Color FocusForeColor = Color.Black;
        private Image NormalIncImage;
        private Image HoverIncImage;
        private Image NormalDecImage;
        private Image HoverDecImage;
        private int LowerBound = 10;
        private int UpperBound = 20;
        public AdvancedNumericUpDown()
        {
            InitializeComponent();
            this.Height = textBox1.Height;
            textBox1.LostFocus += OnFocusLost;
        }

        #region getters/setters
        public String Maximum
        {
            get { return UpperBound.ToString(); }
            set { if (IsDigitsOnly(value)) { UpperBound = Convert.ToInt32(value); }; }
        }

        public String Minimum
        {
            get { return LowerBound.ToString(); }
            set { if (IsDigitsOnly(value)) { LowerBound = Convert.ToInt32(value); }; }
        }
        public HorizontalAlignment TextAlign
        {
            get { return textBox1.TextAlign; }
            set { textBox1.TextAlign = value; }
        }

        public Color ForeColorNormal
        {
            get { return NormalForeColor; }
            set { NormalForeColor = value; textBox1.ForeColor = value; }
        }

        public Color ForeColorFocus
        {
            get { return FocusForeColor; }
            set { FocusForeColor = value; }
        }

        public String TextValue
        {
            get { return textBox1.Text; }
            set
            {
                if (IsDigitsOnly(value))
                {
                    int Value = Convert.ToInt32(value);
                    if (Value < LowerBound)
                    {
                        Value = LowerBound;
                    }
                    else if (Value > UpperBound)
                    {
                        Value = UpperBound;
                    }
                    textBox1.Text = Value.ToString();
                }
            }
        }

        public Color ColorNormal
        {
            get { return NormalColor; }
            set { NormalColor = value; this.Invalidate(); }
        }

        public Color ColorFocus
        {
            get { return FocusColor; }
            set { FocusColor = value; }
        }

        [Browsable(false)]
        public new Font Font
        {
            get;
            set;
        }

        public Font FontStyle
        {
            get { return textBox1.Font; }
            set { textBox1.Font = value; }
        }

        public Image ImageIncreaseNormal
        {
            get { return NormalIncImage; }
            set { NormalIncImage = value; pictureBox2.Image = value; }
        }

        public Image ImageIncreaseHover
        {
            get { return HoverIncImage; }
            set { HoverIncImage = value; }
        }

        public Image ImageDecreaseNormal
        {
            get { return NormalDecImage; }
            set { NormalDecImage = value; pictureBox1.Image = value; }
        }

        public Image ImageDecreaseHover
        {
            get { return HoverDecImage; }
            set { HoverDecImage = value; }
        }
        #endregion

        private void AdvancedNumericUpDown_SizeChanged(object sender, EventArgs e)
        {
            tableLayoutPanel1.ColumnStyles[0].Width = this.Height - 2;
            tableLayoutPanel1.ColumnStyles[2].Width = this.Height - 2;
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.Image = HoverDecImage;
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            pictureBox2.Image = HoverIncImage;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.Image = NormalDecImage;
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            pictureBox2.Image = NormalIncImage;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            int Value = Convert.ToInt32(textBox1.Text);
            Value -= 5;
            if (Value < LowerBound)
            {
                Value = LowerBound;
            }
            textBox1.Text = Value.ToString();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            int Value = Convert.ToInt32(textBox1.Text);
            Value += 5;
            if (Value > UpperBound)
            {
                Value = UpperBound;
            }
            textBox1.Text = Value.ToString();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }



        private void OnFocusLost(object sender, EventArgs e)
        {
            int Value = Convert.ToInt32(textBox1.Text);
            if (Value < LowerBound)
            {
                Value = LowerBound;
            }
            else if (Value > UpperBound)
            {
                Value = UpperBound;
            }
            textBox1.Text = Value.ToString();
        }
    }
}
