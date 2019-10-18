using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pmdbs
{
    public class GraphData
    {
        private protected int _x = 0;
        private protected int _y = 1;
        private protected string _name = "GraphData";
        private protected Color _color = Color.Navy;
        public GraphData()
        {

        }
        public GraphData(int X, string Name)
        {
            _x = X;
            _name = Name;
        }

        public GraphData(int X, int Y, string Name)
        {
            _x = X;
            _y = Y;
            _name = Name;
        }

        public virtual int X
        {
            get { return _x; }
            set { _x = value; }
        }

        public virtual int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public virtual Point ToPoint()
        {
            return new Point(_x, _y);
        }
    }
}
