using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace pmdbs
{
    public partial class OvalPanel : Panel
    {
        public OvalPanel()
        {
            InitializeComponent();
        }

        private void OvalPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Brush brush = new SolidBrush(Color.Firebrick);
            graphics.DrawArc(new Pen(brush), 0, 0, Width, Height, 90, 180);
        }
    }
}
