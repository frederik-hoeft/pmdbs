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
using System.Drawing.Imaging;

namespace pmdbs
{
   
    public partial class AdvancedTextBox : UserControl
    {
        [DllImport("user32.dll")]
        static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);
        [DllImport("user32.dll")]
        static extern bool ShowCaret(IntPtr hWnd);
        float SizeAnimation = 0;
        float PointAnimation = 0;
        private Color NormalColor = Color.FromArgb(33, 33, 33);
        private Color FocusColor = Color.FromArgb(255, 96, 49);
        private Boolean IsFocused = false;
        public AdvancedTextBox()
        {
            InitializeComponent();
            this.Height = textBox1.Height + 4;
            this.textBox1.GotFocus += OnFocus;
            this.textBox1.LostFocus += OnFocusLost;
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

        public override Font Font
        {
            get { return textBox1.Font; }
            set { textBox1.Font = value; }
        }

        private void AdvancedTextBox_Paint(object sender, PaintEventArgs e)
        {
            Bitmap B = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(B);
            G.Clear(Color.Transparent);
            
            G.DrawLine(new Pen(new SolidBrush(IsFocused ? FocusColor : NormalColor)), new Point(0, Height - 2), new Point(Width, Height - 2));
            if (IsFocused)
            {
                G.FillRectangle(new SolidBrush(FocusColor), PointAnimation, (float)Height - 3, SizeAnimation, 3);
            }

            e.Graphics.DrawImage((Image)(B.Clone()), 0, 0);
            G.Dispose();
            B.Dispose();
        }

        private void textBox1_SizeChanged(object sender, EventArgs e)
        {
            this.Height = textBox1.Height + 4;
        }

        private void OnFocus(object sender, EventArgs e)
        {
            IsFocused = true;
            this.Invalidate();
            Bitmap bmp = new Bitmap(1,1);
            //bmp.SetPixel(0, 0, Color.FromArgb(49, 255, 238));
            bmp.SetPixel(0, 0, ColorExtensions.GetContrast(FocusColor, true));
            bmp = new Bitmap(bmp, 2, textBox1.Height);
            CreateCaret(textBox1.Handle, bmp.GetHbitmap(FocusColor), 2, textBox1.Height);
            ShowCaret(textBox1.Handle);
        }

        private void OnFocusLost(object sender, EventArgs e)
        {
            IsFocused = false;
            this.Invalidate();
        }
    }
}
