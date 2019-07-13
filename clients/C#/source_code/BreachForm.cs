using pmdbs.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pmdbs
{
    public partial class BreachForm : MetroFramework.Forms.MetroForm
    {
        public BreachForm()
        {
            InitializeComponent();
            Point pt = new Point(this.flowLayoutPanel1.AutoScrollPosition.X, this.flowLayoutPanel1.AutoScrollPosition.Y);
            this.metroScrollBar1.Minimum = 0;
            this.metroScrollBar1.Maximum = this.flowLayoutPanel1.DisplayRectangle.Height;
            this.metroScrollBar1.LargeChange = metroScrollBar1.Maximum / metroScrollBar1.Height + this.flowLayoutPanel1.Height;
            this.metroScrollBar1.SmallChange = 15;
            this.metroScrollBar1.Value = Math.Abs(this.flowLayoutPanel1.AutoScrollPosition.Y);
            lunaItemList1.LunaItemClicked += card_click;
        }
        private int a = 0;
        private void label5_Click(object sender, EventArgs e)
        {
            lunaItemList1.Add("Test item", Resources.breach, "Infotest", a.ToString(), a);
            a++;
        }
        private void card_click(object sender, EventArgs e)
        {
            LunaItem item = (LunaItem)sender;
            CustomException.ThrowNew.NotImplementedException("item[" + item.Index.ToString() + "]");
        }

        private void label7_Click(object sender, EventArgs e)
        {
            lunaItemList1.Refresh();
        }

        private void metroScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            flowLayoutPanel1.AutoScrollPosition = new Point(0, metroScrollBar1.Value);
            metroScrollBar1.Invalidate();
            Application.DoEvents();
        }
    }
}
