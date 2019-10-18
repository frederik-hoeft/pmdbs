using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pmdbs
{
    public class GraphDataF : GraphData
    {
        private new float _x = 0f;
        private new float _y = 1f;
        public GraphDataF(float X, string Name)
        {
            _x = X;
            _name = Name;
        }

        public GraphDataF(float X, float Y, string Name)
        {
            _x = X;
            _y = Y;
            _name = Name;
        }

        public new virtual float X
        {
            get { return _x; }
            set { _x = value; }
        }

        public new virtual float Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public new virtual PointF ToPoint()
        {
            return new PointF(_x, _y);
        }
    }
}
