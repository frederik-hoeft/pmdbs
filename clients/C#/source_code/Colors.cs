using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace pmdbs
{
    public struct Colors
    {

        public static Color Orange
        {
            get { return Color.FromArgb(255, 96, 49); }
        }
        public static Color Gray
        {
            get { return Color.FromArgb(64, 64, 64); }
        }
        public static Color DarkGray
        {
            get { return Color.FromArgb(33, 33, 33); }
        }
        public static Color LightGray
        {
            get { return Color.FromArgb(220, 220, 220); }
        }
    }
}
