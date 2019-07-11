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
    public partial class LunaSmallCard : UserControl
    {
        private string _header = "LunaSmallCard";
        private string _info = "Info";
        private Font _font = new Font("Segoe UI", 25, GraphicsUnit.Pixel);
        private int _infoFontSizePx = 15;
        private Color _foreColorHeader = Color.Orange;
        private Color _foreColorInfo = Color.FromArgb(100, 100, 100);
        private Color _backColorHover = Color.FromArgb(220, 220, 220);
        private Color _backColor = Color.White;
        private Color currentColor = Color.White;
        private Color normalColor = Color.White;
        private Point _headerLocation = new Point(70, 0);
        private Point _infoLocation = new Point(72, 35);
        private bool _showInfo = false;
        public event EventHandler OnClickEvent;
        public LunaSmallCard()
        {
            InitializeComponent();
            pictureBox1.BackColor = _foreColorHeader;
            if (!_showInfo)
            {
                _headerLocation = new Point(70, 18);
                Refresh();
            }
        }

        private void LunaSmallCard_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawString(_header, _font, new SolidBrush(_foreColorHeader), _headerLocation);
            if (_showInfo)
            {
                g.DrawString(_info, new Font(_font.FontFamily, _infoFontSize, GraphicsUnit.Pixel), new SolidBrush(_foreColorInfo), _infoLocation);
            }
        }

        private void LunaSmallCard_SizeChanged(object sender, EventArgs e)
        {
            if (Height != 60)
            {
                Height = 60;
            }
        }

        private void LunaSmallCard_Click(object sender, EventArgs e)
        {
            ClickEvent(e);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ClickEvent(e);
        }

        protected virtual void ClickEvent(EventArgs e)
        {
            OnClickEvent?.Invoke(this, e);
        }

        private void LunaSmallCard_MouseEnter(object sender, EventArgs e)
        {

        }

        private void LunaSmallCard_MouseLeave(object sender, EventArgs e)
        {

        }

        private void MouseEnterEvent()
        {
            currentColor = BackColor;
            hasFocus = true;
            astep = Convert.ToInt32(_backColorHover.A - normalColor.A > 0 ? Math.Ceiling((double)(_backColorHover.A - normalColor.A) / (double)steps) : Math.Floor((double)(_backColorHover.A - normalColor.A) / (double)steps));
            rstep = Convert.ToInt32(_backColorHover.R - normalColor.R > 0 ? Math.Ceiling((double)(_backColorHover.R - normalColor.R) / (double)steps) : Math.Floor((double)(_backColorHover.R - normalColor.R) / (double)steps));
            gstep = Convert.ToInt32(_backColorHover.G - normalColor.G > 0 ? Math.Ceiling((double)(_backColorHover.G - normalColor.G) / (double)steps) : Math.Floor((double)(_backColorHover.G - normalColor.G) / (double)steps));
            bstep = Convert.ToInt32(_backColorHover.B - normalColor.B > 0 ? Math.Ceiling((double)(_backColorHover.B - normalColor.B) / (double)steps) : Math.Floor((double)(_backColorHover.B - normalColor.B) / (double)steps));
            if (!timerRunning)
            {
                timerRunning = true;
                AnimationTimer.Start();
            }
        }

        private void MouseLeaveEvent()
        {
            currentColor = BackColor;
            hasFocus = false;
            astep = Convert.ToInt32(normalColor.A - _backColorHover.A > 0 ? Math.Ceiling((double)(normalColor.A - _backColorHover.A) / (double)steps) : Math.Floor((double)(normalColor.A - _backColorHover.A) / (double)steps));
            rstep = Convert.ToInt32(normalColor.R - _backColorHover.R > 0 ? Math.Ceiling((double)(normalColor.R - _backColorHover.R) / (double)steps) : Math.Floor((double)(normalColor.R - _backColorHover.R) / (double)steps));
            gstep = Convert.ToInt32(normalColor.G - _backColorHover.G > 0 ? Math.Ceiling((double)(normalColor.G - _backColorHover.G) / (double)steps) : Math.Floor((double)(normalColor.G - _backColorHover.G) / (double)steps));
            bstep = Convert.ToInt32(normalColor.B - _backColorHover.B > 0 ? Math.Ceiling((double)(normalColor.B - _backColorHover.B) / (double)steps) : Math.Floor((double)(normalColor.B - _backColorHover.B) / (double)steps));
            if (!timerRunning)
            {
                timerRunning = true;
                AnimationTimer.Start();
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            int A, R, G, B;
            if (hasFocus)
            {
                if (HoverColor.A > CurrentColor.A)
                {
                    A = (CurrentColor.A + astep > HoverColor.A ? HoverColor.A : CurrentColor.A + astep);
                }
                else if (HoverColor.A < CurrentColor.A)
                {
                    A = (CurrentColor.A + astep < HoverColor.A ? HoverColor.A : CurrentColor.A + astep);
                }
                else
                {
                    A = HoverColor.A;
                }
                if (HoverColor.R > CurrentColor.R)
                {
                    R = (CurrentColor.R + rstep > HoverColor.R ? HoverColor.R : CurrentColor.R + rstep);
                }
                else if (HoverColor.R < CurrentColor.R)
                {
                    R = (CurrentColor.R + rstep < HoverColor.R ? HoverColor.R : CurrentColor.R + rstep);
                }
                else
                {
                    R = HoverColor.R;
                }
                if (HoverColor.G > CurrentColor.G)
                {
                    G = (CurrentColor.G + gstep > HoverColor.G ? HoverColor.G : CurrentColor.G + gstep);
                }
                else if (HoverColor.G < CurrentColor.G)
                {
                    G = (CurrentColor.G + gstep < HoverColor.G ? HoverColor.G : CurrentColor.G + gstep);
                }
                else
                {
                    G = HoverColor.G;
                }
                if (HoverColor.B > CurrentColor.B)
                {
                    B = (CurrentColor.B + bstep > HoverColor.B ? HoverColor.B : CurrentColor.B + bstep);
                }
                else if (HoverColor.B < CurrentColor.B)
                {
                    B = (CurrentColor.B + bstep < HoverColor.B ? HoverColor.B : CurrentColor.B + bstep);
                }
                else
                {
                    B = HoverColor.B;
                }
            }
            else
            {
                if (NormalColor.A > CurrentColor.A)
                {
                    A = (CurrentColor.A + astep > NormalColor.A ? NormalColor.A : CurrentColor.A + astep);
                }
                else if (NormalColor.A < CurrentColor.A)
                {
                    A = (CurrentColor.A + astep < NormalColor.A ? NormalColor.A : CurrentColor.A + astep);
                }
                else
                {
                    A = NormalColor.A;
                }
                if (NormalColor.R > CurrentColor.R)
                {
                    R = CurrentColor.R + rstep > NormalColor.R ? NormalColor.R : CurrentColor.R + rstep;
                }
                else if (NormalColor.R < CurrentColor.R)
                {
                    R = CurrentColor.R + rstep < NormalColor.R ? NormalColor.R : CurrentColor.R + rstep;
                }
                else
                {
                    R = NormalColor.R;
                }
                if (NormalColor.G > CurrentColor.G)
                {
                    G = CurrentColor.G + gstep > NormalColor.G ? NormalColor.G : CurrentColor.G + gstep;
                }
                else if (NormalColor.G < CurrentColor.G)
                {
                    G = CurrentColor.G + gstep < NormalColor.G ? NormalColor.G : CurrentColor.G + gstep;
                }
                else
                {
                    G = NormalColor.G;
                }
                if (NormalColor.B > CurrentColor.B)
                {
                    B = CurrentColor.B + bstep > NormalColor.B ? NormalColor.B : CurrentColor.B + bstep;
                }
                else if (NormalColor.B < CurrentColor.B)
                {
                    B = CurrentColor.B + bstep < NormalColor.B ? NormalColor.B : CurrentColor.B + bstep;
                }
                else
                {
                    B = NormalColor.B;
                }
            }
            CurrentColor = Color.FromArgb(R, G, B);
            BackColor = CurrentColor;
            if (hasFocus)
            {
                if (CurrentColor.Equals(HoverColor))
                {
                    AnimationTimer.Stop();
                    timerRunning = false;
                }
            }
            else
            {
                if (CurrentColor.Equals(NormalColor))
                {
                    AnimationTimer.Stop();
                    timerRunning = false;
                }
            }
            this.Refresh();
        }
    }
}
