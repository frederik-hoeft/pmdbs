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
        private int _infoFontSize = 15;
        private Color _foreColorHeader = Color.Orange;
        private Color _foreColorInfo = Color.FromArgb(100, 100, 100);
        private Point _headerLocation = new Point(70, 0);
        private Point _infoLocation = new Point(72, 35);
        private bool _showInfo = true;
        public LunaSmallCard()
        {
            InitializeComponent();
            pictureBox1.BackColor = _foreColorHeader;
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

        }

        private void LunaSmallCard_Click(object sender, EventArgs e)
        {

        }

        private void LunaSmallCard_MouseEnter(object sender, EventArgs e)
        {

        }

        private void LunaSmallCard_MouseLeave(object sender, EventArgs e)
        {

        }
    }
}
