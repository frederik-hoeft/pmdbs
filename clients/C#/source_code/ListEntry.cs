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
    public partial class ListEntry : UserControl
    {
        public ListEntry() 
        {
            InitializeComponent();
        }
        
        public Image FavIcon
        {
            get { return pictureBox1.Image; }
            set { pictureBox1.Image = value; }
        }

        public String HostName
        {
            get { return label1.Text; }
            set { label1.Text = value; }
        }

        public String UserName
        {
            get { return label2.Text; }
            set { label2.Text = value; }
        }
        public String TimeStamp
        {
            get { return label3.Text; }
            set { label3.Text = value; }
        }

        public Font HostNameFont
        {
            get { return label1.Font; }
            set { label1.Font = value; }
        }
        public Font UserNameFont
        {
            get { return label2.Font; }
            set { label2.Font = value; }
        }
        public Font TimeStampFont
        {
            get { return label3.Font; }
            set { label3.Font = value; }
        }

        public Color HostNameForeColor
        {
            get { return label1.ForeColor; }
            set { label1.ForeColor = value; }
        }
        public Color UserNameForeColor
        {
            get { return label1.ForeColor; }
            set { label1.ForeColor = value; }
        }
        public Color TimeStampForeColor
        {
            get { return label1.ForeColor; }
            set { label1.ForeColor = value; }
        }
    }
}
