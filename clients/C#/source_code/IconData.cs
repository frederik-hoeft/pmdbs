using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pmdbs
{
    class IconData
    {
        private Image image;
        private String path;
        public Image Image
        {
            get { return image; }
            set { image = value; }
        }

        public string Path
        {
            get { return path; }
            set { path = value; }
        }
    }
}
