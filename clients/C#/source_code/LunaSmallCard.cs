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
    /// <summary>
    /// A small info card containing an image, a header text and an optional info text.
    /// </summary>
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
        private Color _backColorImage = Color.Orange;
        private Color currentColor = Color.White;
        private Point _headerLocation = new Point(70, 0);
        private Point _infoLocation = new Point(72, 35);
        private bool _showInfo = false;
        private int _steps = 20;
        private int step = 0;
        private int _animationInterval = 10;
        private Timer animationTimer = new Timer();
        private bool timerRunning = false;
        private bool hasFocus = false;
        private int astep, rstep, gstep, bstep;

        /// <summary>
        /// Occurs when the component is clicked. Should be used instead of LunaSmallCard.Click
        /// </summary>
        public event EventHandler OnClickEvent;

        /// <summary>
        /// A small info card containing an image, a header text and an optional info text.
        /// </summary>
        public LunaSmallCard()
        {
            InitializeComponent();
            pictureBox1.BackColor = _foreColorHeader;
            animationTimer.Tick += new EventHandler(AnimationTimer_Tick);
            animationTimer.Interval = _animationInterval;
            if (!_showInfo)
            {
                _headerLocation = new Point(70, 18);
                Refresh();
            }
        }

        #region Getters / Setters
        /// <summary>
        /// The main text to be displayed.
        /// </summary>
        [DefaultValue("LunaSmallCard")]
        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }

        /// <summary>
        /// Additional information to be displayed.
        /// </summary>
        [DefaultValue("Info")]
        public string Info
        {
            get { return _info; }
            set { _info = value; }
        }

        /// <summary>
        /// The font top be used to draw both Header and Info.
        /// </summary>
        public override Font Font
        {
            get { return _font; }
            set
            {
                base.Font = value;
                _font = value;
                Refresh();
            }
        }

        /// <summary>
        /// The font size in pixels of the info text.
        /// </summary>
        [DefaultValue(15)]
        public int InfoFontSizePx
        {
            get { return _infoFontSizePx; }
            set { _infoFontSizePx = value; }
        }

        /// <summary>
        /// The foreground color of the header text.
        /// </summary>
        public Color ForeColorHeader
        {
            get { return _foreColorHeader; }
            set
            {
                _foreColorHeader = value;
                Refresh();
            }
        }

        /// <summary>
        /// The foreground color of the info text.
        /// </summary>
        public Color ForeColorInfo
        {
            get { return _foreColorInfo; }
            set
            {
                _foreColorInfo = value;
                Refresh();
            }
        }

        /// <summary>
        /// The background color of the LunaSmallCard.
        /// </summary>
        public override Color BackColor
        {
            get { return _backColor; }
            set
            {
                base.BackColor = value;
                _backColor = value;
            }
        }

        /// <summary>
        /// The background color of the LunaSmallCard when the user hovers over it.
        /// </summary>
        public Color BackColorHover
        {
            get { return _backColorHover; }
            set { _backColorHover = value; }
        }

        /// <summary>
        /// The background color of the PictureBox.
        /// </summary>
        public Color BackColorImage
        {
            get { return _backColorImage; }
            set { _backColorImage = value; }
        }

        /// <summary>
        /// The image shown in the LunaSmallCard.
        /// </summary>
        public Image Image
        {
            get { return pictureBox1.Image; }
            set { pictureBox1.Image = value; }
        }

        /// <summary>
        /// The location of the top left corner of the header text.
        /// </summary>
        public Point HeaderLocation
        {
            get { return _headerLocation; }
            set
            {
                _headerLocation = value;
                Refresh();
            }
        }

        /// <summary>
        /// The location of the top left corner of the info text.
        /// </summary>
        public Point InfoLocation
        {
            get { return _infoLocation; }
            set
            {
                _infoLocation = value;
                Refresh();
            }
        }

        /// <summary>
        /// Controls whether the info text is drawn or not.
        /// </summary>
        [DefaultValue(true)]
        public bool ShowInfo
        {
            get { return _showInfo; }
            set
            {
                _showInfo = value;
                Refresh();
            }
        }

        /// <summary>
        /// Controls how smooth the color transition is from the BackColor to BackColorHover. More steps means smoother transition.
        /// </summary>
        [DefaultValue(20)]
        public int AnimationSteps
        {
            get { return _steps; }
            set { _steps = value; }
        }


        #endregion
        private void LunaSmallCard_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawString(_header, _font, new SolidBrush(_foreColorHeader), _headerLocation);
            if (_showInfo)
            {
                g.DrawString(_info, new Font(_font.FontFamily, _infoFontSizePx, GraphicsUnit.Pixel), new SolidBrush(_foreColorInfo), _infoLocation);
            }
        }

        private void LunaSmallCard_SizeChanged(object sender, EventArgs e)
        {
            if (Height != 60)
            {
                Height = 60;
            }
        }
        #region Click event
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
        #endregion

        #region Hover effects
        private void LunaSmallCard_MouseEnter(object sender, EventArgs e)
        {
            MouseEnterEvent();
        }

        private void LunaSmallCard_MouseLeave(object sender, EventArgs e)
        {
            MouseLeaveEvent();
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            MouseEnterEvent();
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            MouseLeaveEvent();
        }

        private void MouseEnterEvent()
        {
            currentColor = _backColor;
            hasFocus = true;
            astep = Convert.ToInt32(_backColorHover.A - _backColor.A > 0 ? Math.Ceiling((double)(_backColorHover.A - _backColor.A) / (double)_steps) : Math.Floor((double)(_backColorHover.A - _backColor.A) / (double)_steps));
            rstep = Convert.ToInt32(_backColorHover.R - _backColor.R > 0 ? Math.Ceiling((double)(_backColorHover.R - _backColor.R) / (double)_steps) : Math.Floor((double)(_backColorHover.R - _backColor.R) / (double)_steps));
            gstep = Convert.ToInt32(_backColorHover.G - _backColor.G > 0 ? Math.Ceiling((double)(_backColorHover.G - _backColor.G) / (double)_steps) : Math.Floor((double)(_backColorHover.G - _backColor.G) / (double)_steps));
            bstep = Convert.ToInt32(_backColorHover.B - _backColor.B > 0 ? Math.Ceiling((double)(_backColorHover.B - _backColor.B) / (double)_steps) : Math.Floor((double)(_backColorHover.B - _backColor.B) / (double)_steps));
            if (!timerRunning)
            {
                timerRunning = true;
                animationTimer.Start();
            }
        }

        private void MouseLeaveEvent()
        {
            currentColor = _backColor;
            hasFocus = false;
            astep = Convert.ToInt32(_backColor.A - _backColorHover.A > 0 ? Math.Ceiling((double)(_backColor.A - _backColorHover.A) / (double)_steps) : Math.Floor((double)(_backColor.A - _backColorHover.A) / (double)_steps));
            rstep = Convert.ToInt32(_backColor.R - _backColorHover.R > 0 ? Math.Ceiling((double)(_backColor.R - _backColorHover.R) / (double)_steps) : Math.Floor((double)(_backColor.R - _backColorHover.R) / (double)_steps));
            gstep = Convert.ToInt32(_backColor.G - _backColorHover.G > 0 ? Math.Ceiling((double)(_backColor.G - _backColorHover.G) / (double)_steps) : Math.Floor((double)(_backColor.G - _backColorHover.G) / (double)_steps));
            bstep = Convert.ToInt32(_backColor.B - _backColorHover.B > 0 ? Math.Ceiling((double)(_backColor.B - _backColorHover.B) / (double)_steps) : Math.Floor((double)(_backColor.B - _backColorHover.B) / (double)_steps));
            if (!timerRunning)
            {
                timerRunning = true;
                animationTimer.Start();
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            int A, R, G, B;
            if (hasFocus)
            {
                if (_backColorHover.A > currentColor.A)
                {
                    A = (currentColor.A + astep > _backColorHover.A ? _backColorHover.A : currentColor.A + astep);
                }
                else if (_backColorHover.A < currentColor.A)
                {
                    A = (currentColor.A + astep < _backColorHover.A ? _backColorHover.A : currentColor.A + astep);
                }
                else
                {
                    A = _backColorHover.A;
                }
                if (_backColorHover.R > currentColor.R)
                {
                    R = (currentColor.R + rstep > _backColorHover.R ? _backColorHover.R : currentColor.R + rstep);
                }
                else if (_backColorHover.R < currentColor.R)
                {
                    R = (currentColor.R + rstep < _backColorHover.R ? _backColorHover.R : currentColor.R + rstep);
                }
                else
                {
                    R = _backColorHover.R;
                }
                if (_backColorHover.G > currentColor.G)
                {
                    G = (currentColor.G + gstep > _backColorHover.G ? _backColorHover.G : currentColor.G + gstep);
                }
                else if (_backColorHover.G < currentColor.G)
                {
                    G = (currentColor.G + gstep < _backColorHover.G ? _backColorHover.G : currentColor.G + gstep);
                }
                else
                {
                    G = _backColorHover.G;
                }
                if (_backColorHover.B > currentColor.B)
                {
                    B = (currentColor.B + bstep > _backColorHover.B ? _backColorHover.B : currentColor.B + bstep);
                }
                else if (_backColorHover.B < currentColor.B)
                {
                    B = (currentColor.B + bstep < _backColorHover.B ? _backColorHover.B : currentColor.B + bstep);
                }
                else
                {
                    B = _backColorHover.B;
                }
            }
            else
            {
                if (_backColor.A > currentColor.A)
                {
                    A = (currentColor.A + astep > _backColor.A ? _backColor.A : currentColor.A + astep);
                }
                else if (_backColor.A < currentColor.A)
                {
                    A = (currentColor.A + astep < _backColor.A ? _backColor.A : currentColor.A + astep);
                }
                else
                {
                    A = _backColor.A;
                }
                if (_backColor.R > currentColor.R)
                {
                    R = currentColor.R + rstep > _backColor.R ? _backColor.R : currentColor.R + rstep;
                }
                else if (_backColor.R < currentColor.R)
                {
                    R = currentColor.R + rstep < _backColor.R ? _backColor.R : currentColor.R + rstep;
                }
                else
                {
                    R = _backColor.R;
                }
                if (_backColor.G > currentColor.G)
                {
                    G = currentColor.G + gstep > _backColor.G ? _backColor.G : currentColor.G + gstep;
                }
                else if (_backColor.G < currentColor.G)
                {
                    G = currentColor.G + gstep < _backColor.G ? _backColor.G : currentColor.G + gstep;
                }
                else
                {
                    G = _backColor.G;
                }
                if (_backColor.B > currentColor.B)
                {
                    B = currentColor.B + bstep > _backColor.B ? _backColor.B : currentColor.B + bstep;
                }
                else if (_backColor.B < currentColor.B)
                {
                    B = currentColor.B + bstep < _backColor.B ? _backColor.B : currentColor.B + bstep;
                }
                else
                {
                    B = _backColor.B;
                }
            }
            currentColor = Color.FromArgb(R, G, B);
            BackColor = currentColor;
            if (hasFocus)
            {
                if (currentColor.Equals(_backColorHover))
                {
                    animationTimer.Stop();
                    timerRunning = false;
                }
            }
            else
            {
                if (currentColor.Equals(_backColor))
                {
                    animationTimer.Stop();
                    timerRunning = false;
                }
            }
            this.Refresh();
        }
        #endregion
    }
}
