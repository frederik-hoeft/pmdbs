using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.IO;

namespace pmdbs
{
    public partial class DeviceForm : MetroFramework.Forms.MetroForm
    {
        public DeviceForm()
        {
            InitializeComponent();
            labelDeviceId.Text = "Device ID: " + CryptoHelper.SHA256HashBase64("O32TCYHJ6asG9uFQ1EqbEQz5Ikg2Fwf0/BqWQWnnhBM35eKHU2SJSVaoOzfsk7o9Iuuzhq0LvgX0jwfTXEeiOw==");
            GetDeviceInfoTemp();
        }

        private void GetDeviceInfoTemp()
        {
            
        }

        private void labelTitle_Click(object sender, EventArgs e)
        {
            GetDeviceInfoTemp();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = (Panel)sender;
            Color[] shadow = new Color[3];
            shadow[0] = Color.FromArgb(181, 181, 181);
            shadow[1] = Color.FromArgb(195, 195, 195);
            shadow[2] = Color.FromArgb(211, 211, 211);
            Pen pen = new Pen(shadow[0]);
            using (pen)
            {
                foreach (Panel p in panel.Controls.OfType<Panel>())
                {
                    Point pt = p.Location;
                    pt.Y += p.Height;
                    for (var sp = 0; sp < 3; sp++)
                    {
                        pen.Color = shadow[sp];
                        e.Graphics.DrawLine(pen, pt.X, pt.Y, pt.X + p.Width - 1, pt.Y);
                        pt.Y++;
                    }
                }
            }
        }
    }
}
