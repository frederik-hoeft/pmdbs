using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;

namespace pmdbs
{
    public partial class DynamicLink : UserControl
    {
        private Color HoverColor;
        private Color NormalColor;
        private Boolean Disabled = true;

        public DynamicLink()
        {
            InitializeComponent();
        }

        public Color LinkColorOnMouseHover
        {
            get { return HoverColor; }
            set { HoverColor = value; }
        }

        public Color LinkColor
        {
            get { return NormalColor; }
            set { NormalColor = value; Link.ForeColor = value; Link.LinkColor = value; }
        }

        public Font LinkFont
        {
            get { return Link.Font; }
            set { Link.Font = value; }
        }

        public String LinkText
        {
            get { return Link.Text; }
            set
            {
                try
                {
                    Uri url = new Uri(value);
                    string domain = url.Host;
                    Link.Text = "https://" + domain;
                    Disabled = false;
                }
                catch
                {
                    Link.Text = "INVALID URL";
                    Disabled = true;
                }
            }
        }

        public Color ActiveLinkColor
        {
            get { return Link.ActiveLinkColor; }
            set { Link.ActiveLinkColor = value; }
        }

        public Color VisitedLinkColor
        {
            get { return Link.VisitedLinkColor; }
            set { Link.VisitedLinkColor = value; }
        }

        public Boolean LinkVisited
        {
            get { return Link.LinkVisited; }
            set { Link.LinkVisited = value; }
        }

        private void Link_MouseEnter(object sender, EventArgs e)
        {
            Link.LinkColor = HoverColor;
        }

        private void Link_MouseLeave(object sender, EventArgs e)
        {
            Link.LinkColor = NormalColor;
        }

        private void Link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!Disabled)
            {
                System.Diagnostics.Process.Start(Link.Text);
                Link.LinkVisited = true;
            }
        }
    }
}
